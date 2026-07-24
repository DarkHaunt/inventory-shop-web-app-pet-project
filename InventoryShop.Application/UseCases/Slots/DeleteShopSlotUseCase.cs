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
      
      var beginTransactionResult = await transactionManager.BeginTransactionAsync(ct);

      if (beginTransactionResult.IsFailure)
         return beginTransactionResult;
      
      ItemEntity? itemInSlot = await itemsRepository.GetItemByIdAsync(command.SlotId, ct);

      if (itemInSlot is null)
         throw new DataIntegrityException($"Item in slot {command.SlotId} not found");
      
      itemInSlot.SetIsOnSale(false);
      await itemsRepository.UpdateItemSaleStatus(itemInSlot.Id, false, ct);
      
      await shopSlotsRepository.DeleteSlotAsync(command.SlotId, ct);
      return await transactionManager.CommitTransactionAsync(ct);
   }
}