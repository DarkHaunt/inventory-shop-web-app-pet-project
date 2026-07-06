using AutoMapper;
using CSharpFunctionalExtensions;
using InventoryShop.Application.Commands;
using InventoryShop.Application.DTO;
using InventoryShop.Application.Interfaces;
using InventoryShop.Application.Services;
using InventoryShop.Application.Shared;
using InventoryShop.Domain.Entities;
using InventoryShop.Domain.Errors;
using InventoryShop.Domain.Shared.Errors;
using InventoryShop.Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace InventoryShop.Application.UseCases.Slots;

public sealed class CreateShopSlotUseCase(
   IGuidProvider guidProvider,
   ITransactionManager transactionManager,
   IPlayersRepository playersRepository,
   IShopSlotsRepository slotsRepository,
   IItemsRepository itemsRepository,
   IMapper mapper,
   ILogger logger,
   EnrichedSlotDetailsFactory slotDetailsFactory)
{
   public async Task<Result<EnrichedShopSlotDetails, Error>> ExecuteAsync(CreateShopSlotCommand command, CancellationToken ct)
   {
      var beginTransactionResult = await transactionManager.BeginTransactionAsync(ct);

      if (beginTransactionResult.IsFailure)
         return beginTransactionResult.Error;

      (_, var isFailure, ItemEntity? itemToSell, Error? error) = command.SellerId == null 
         ? await PrepareItemForSellAsSystem(command.ItemToSellId, ct)
         : await PrepareItemForSellAsPlayer((Guid)command.SellerId, command.ItemToSellId, ct);

      if(isFailure)
         return error;

      var slot = ShopSlotEntity.Create
      (
         guidProvider.CreateNew(),
         itemToSell.Id,
         mapper.Map<Wallet>(command.Price),
         mapper.Map<LevelProgress>(command.LevelRequired),
         command.SellerId
      );

      await slotsRepository.AddSlotAsync(slot, ct);
      
      var slotDto = await slotDetailsFactory.CreateAsync(slot, ct);

      if (slotDto.IsFailure)
         return slotDto.Error;
      
      var commit = await transactionManager.CommitTransactionAsync(ct);
      return commit.IsFailure
         ? Result.Failure<EnrichedShopSlotDetails, Error>(commit.Error)
         : Result.Success<EnrichedShopSlotDetails, Error>(slotDto.Value);
   }

   private async Task<Result<ItemEntity, Error>> PrepareItemForSellAsSystem(Guid itemToSellId, CancellationToken ct)
   {
      ItemEntity? itemToSell = await itemsRepository.GetItemByIdAsync(itemToSellId, ct);

      if (itemToSell is null)
      {
         logger.LogError("Can't find item {ID}", itemToSellId);
         return ItemsErrors.ItemWithIdNotFoundError(itemToSellId);
      }

      if (itemToSell.IsSystemOwned == false)
      {
         logger.LogError("System does not own item with id {ItemId}", itemToSellId);
         return ShopSlotsErrors.SystemTriesSellNotOwnedItem(itemToSellId);
      }
      
      return itemToSell;
   }

   private async Task<Result<ItemEntity, Error>> PrepareItemForSellAsPlayer(Guid sellerId, Guid itemToSellId, CancellationToken ct)
   {
      PlayerEntity? seller = await playersRepository.GetPlayerById(sellerId, ct);

      if (seller is null)
      {
         logger.LogError("Can't find player with {ID}", sellerId);
         return PlayerErrors.PlayerWithIdNotFoundError(sellerId);
      }

      ItemEntity? itemToSell = await itemsRepository.GetItemByIdAsync(itemToSellId, ct);

      if (itemToSell is null)
      {
         logger.LogError("Can't find item {ID}", itemToSellId);
         return ItemsErrors.ItemWithIdNotFoundError(itemToSellId);
      }

      if (itemToSell.IsOwnedBy(sellerId) == false)
      {
         logger.LogError("Player with id {ID} does not own item with id {ItemId}", sellerId, itemToSellId);
         return ShopSlotsErrors.PlayerTriesSellNotOwnedItem(sellerId, itemToSellId);
      }

      if (itemToSell.IsEquipped)
         itemToSell.Unequip();

      itemToSell.TransferOwnershipTo(null);

      await itemsRepository.UpdateItemEquipStatus(itemToSellId, isEquipped: false, ct);
      await itemsRepository.UpdateItemOwnership(itemToSellId, ownerId: null, ct);
      
      return itemToSell;
   }
}