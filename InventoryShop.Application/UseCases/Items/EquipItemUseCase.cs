using CSharpFunctionalExtensions;
using InventoryShop.Application.Interfaces;
using InventoryShop.Application.Shared;
using InventoryShop.Domain.Entities;
using InventoryShop.Domain.Errors;
using InventoryShop.Domain.Shared.Errors;
using Microsoft.Extensions.Logging;

namespace InventoryShop.Application.UseCases.Items;

public sealed class EquipItemUseCase(
   ITransactionManager transactionManager,
   IItemsRepository itemsRepository, 
   ILogger<EquipItemUseCase> logger)
{
   public async Task<UnitResult<Error>> ExecuteAsync(Guid itemId, Guid playerId, bool isEquipped, CancellationToken ct)
   {
      var beginTransactionResult = await transactionManager.BeginTransactionAsync(ct);

      if (beginTransactionResult.IsFailure)
         return beginTransactionResult.Error;
      
      ItemEntity? item = await itemsRepository.GetItemByIdAsync(itemId, ct);

      if (item is null)
      {
         logger.LogError("Can't find item {ID}", itemId);
         return ItemsErrors.ItemWithIdNotFoundError(itemId);
      }

      if (item.OwnerId != playerId)
      {
         logger.LogError("Player with id {ID} does not own item with id {ItemId}", playerId, itemId);
         return ItemsErrors.PlayerTriesEquipNotOwnedItem(playerId, itemId);
      }

      if (item.IsOnSale)
      {
         logger.LogError("Player with id {ID} tries to equip item with id {ItemId} that is on sale", playerId, itemId);
         return ItemsErrors.PlayerTriesEquipOnSaleItem(playerId, itemId);
      }
      
      if(item.IsEquipped == isEquipped)
         return UnitResult.Success<Error>();

      if(isEquipped)
         item.Equip();
      else
         item.Unequip();
      
      await itemsRepository.UpdateItemEquipStatus(itemId, isEquipped, ct);
      return await transactionManager.CommitTransactionAsync(ct);
   }
}