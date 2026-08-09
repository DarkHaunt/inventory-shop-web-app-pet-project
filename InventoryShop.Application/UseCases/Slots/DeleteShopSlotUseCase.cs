using CSharpFunctionalExtensions;
using InventoryShop.Application.Commands;
using InventoryShop.Application.Interfaces;
using InventoryShop.Application.Shared;
using InventoryShop.Domain.Entities;
using InventoryShop.Domain.Errors;
using InventoryShop.Domain.Shared.Errors;
using InventoryShop.Domain.Shared.Exceptions;

namespace InventoryShop.Application.UseCases.Slots;

public sealed class DeleteShopSlotUseCase(
   ITransactionManager transactionManager,
   IItemsRepository itemsRepository,
   IShopSlotsRepository shopSlotsRepository)
{
   public async Task<UnitResult<Error>> ExecuteAsync(DeleteShopSlotCommand command, CancellationToken ct)
   {
      if (command.IsAdmin == false)
      {
         if (await shopSlotsRepository.IsSlotOwnedByPlayerAsync(command.SlotOwnerId, command.SlotId, ct) == false)
            return ShopSlotsErrors.SlotNotOwnedByPlayerError(command.SlotOwnerId, command.SlotId);
      }
      
      ShopSlotEntity? slot = await shopSlotsRepository.GetSlotById(command.SlotId, ct);

      if (slot is null)
         return ShopSlotsErrors.ShopSlotWithIdNotFoundError(command.SlotId);
      
      ItemEntity? itemInSlot = await itemsRepository.GetItemByIdAsync(slot.SellItemId, ct);

      if (itemInSlot is null)
         throw new DataIntegrityException($"Item in slot {command.SlotId} not found");
      
      var beginTransactionResult = await transactionManager.BeginTransactionAsync(ct);

      if (beginTransactionResult.IsFailure)
         return beginTransactionResult;

      itemInSlot.SetIsOnSale(false);
      await itemsRepository.UpdateItemSaleStatus(itemInSlot.Id, false, ct);
      
      await shopSlotsRepository.DeleteSlotAsync(command.SlotId, ct);
      return await transactionManager.CommitTransactionAsync(ct);
   }
}