using CSharpFunctionalExtensions;
using InventoryShop.Application.Commands;
using InventoryShop.Application.DTO;
using InventoryShop.Application.Interfaces;
using InventoryShop.Application.Shared;
using InventoryShop.Application.UseCases.Orders;
using InventoryShop.Domain.Entities;
using InventoryShop.Domain.Errors;
using InventoryShop.Domain.Shared.Errors;
using InventoryShop.Domain.Shared.Exceptions;
using InventoryShop.Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace InventoryShop.Application.UseCases.Gameplay;

public sealed class ShopPurchaseUseCase(
   ITransactionManager transactionManager,
   IPlayersRepository playersRepository,
   IShopSlotsRepository slotsRepository,
   CreateShopOrderUseCase createShopOrderUseCase,
   IItemsRepository itemsRepository,
   ILogger<ShopPurchaseUseCase> logger)
{
   public async Task<Result<EnrichedShopOrderDetails, Error>> ExecuteAsync(Guid buyerId, Guid slotToExecute, DateTime orderDate, CancellationToken ct)
   {
      (_, var isFailure, (PlayerEntity buyer, ShopSlotEntity slot), Error error) = await ValidatePreconditionsAsync(buyerId, slotToExecute, ct);

      if(isFailure)
         return error;
      
      var beginTransactionResult = await transactionManager.BeginTransactionAsync(ct);

      if (beginTransactionResult.IsFailure)
         return beginTransactionResult.Error;
      
      var playerWalletWithdraw = await WithdrawGoldForSaleAsync(buyer, slot.Price, ct);
      
      if(playerWalletWithdraw.IsFailure)
         return playerWalletWithdraw.Error;

      ItemSnapshot itemSnap = await TransferItemOwnershipAndSnapshot(buyerId, slot, ct);
      await slotsRepository.DeleteSlotAsync(slotToExecute, ct);
      var orderCreation = await CreateOrderAsync(buyer.Id, slot, itemSnap, orderDate, ct);
      
      if(orderCreation.IsFailure)
         return orderCreation.Error;
      
      var commitTransactionResult = await transactionManager.CommitTransactionAsync(ct);

      if (commitTransactionResult.IsFailure)
         return commitTransactionResult.Error;
         
      return Result.Success<EnrichedShopOrderDetails, Error>(orderCreation.Value);
   }

   private async Task<ItemSnapshot> TransferItemOwnershipAndSnapshot(Guid newOwner, ShopSlotEntity slotWithItem, CancellationToken ct)
   {
      ItemEntity? item = await itemsRepository.GetItemByIdAsync(slotWithItem.SellItemId, ct);
      
      if(item is null)
         throw new DataIntegrityException($"Slot {slotWithItem.Id} references item {slotWithItem.SellItemId} which does not exist");
      
      item.TransferOwnershipTo(newOwner);
      item.SetIsOnSale(false);
      
      await itemsRepository.UpdateItemOwnership(slotWithItem.SellItemId, newOwner, ct);
      await itemsRepository.UpdateItemSaleStatus(slotWithItem.SellItemId, false, ct);
      
      return item.Snapshot();
   }

   private async Task<UnitResult<Error>> WithdrawGoldForSaleAsync(PlayerEntity buyer, Wallet slotPrice, CancellationToken ct)
   {
      if (buyer.Wallet.GoldAmount < slotPrice.GoldAmount)
      {
         logger.LogError("Player {PlayerName} doesn't have enough gold", buyer.Nickname);
         return PlayerErrors.NotEnoughGoldError(buyer.Id);
      }
      
      buyer.Withdraw(slotPrice);
      
      await playersRepository.UpdatePlayerWalletAsync(buyer.Id, buyer.Wallet, ct);
      return UnitResult.Success<Error>();
   }

   private async Task<Result<(PlayerEntity buyer, ShopSlotEntity slot), Error>> ValidatePreconditionsAsync(Guid buyerId, Guid slotToExecute, CancellationToken ct)
   {
      PlayerEntity? buyer = await playersRepository.GetPlayerById(buyerId, ct);
      
      if (buyer is null)
      {
         logger.LogError("Can't find player with {ID}", buyerId);
         return PlayerErrors.PlayerWithIdNotFoundError(buyerId);
      }
      
      ShopSlotEntity? slot = await slotsRepository.GetSlotById(slotToExecute, ct);
      
      if (slot is null)
      {
         logger.LogError("Can't find slot with {ID}", slotToExecute);
         return ShopSlotsErrors.ShopSlotWithIdNotFoundError(slotToExecute);
      }

      if (slot.SellerId == buyerId)
      {
         logger.LogError("Player {PlayerName} tries to buy his own slot", buyer.Nickname);
         return ShopSlotsErrors.PlayerCannotBuyHisOwnSlotError();
      }
      
      return Result.Success<(PlayerEntity buyer, ShopSlotEntity slot), Error>((buyer, slot));
   }

   private async Task<Result<EnrichedShopOrderDetails, Error>> CreateOrderAsync(Guid buyerId, ShopSlotEntity slot, ItemSnapshot itemSnap, DateTime orderDate, CancellationToken ct)
   {
      var data = new OrderData(itemSnap, slot.Price, slot.RequiredLevel.Level);
      var command = new CreateShopOrderCommand(buyerId, slot.SellerId, data, orderDate);
      var detailsCreation = await createShopOrderUseCase.ExecuteAsync(command, ct);

      if(detailsCreation.IsFailure)
         return detailsCreation.Error;
      
      return Result.Success<EnrichedShopOrderDetails, Error>(detailsCreation.Value);
   }
}