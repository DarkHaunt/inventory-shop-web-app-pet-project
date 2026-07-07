using CSharpFunctionalExtensions;
using InventoryShop.Application.Interfaces;
using InventoryShop.Application.Shared;
using InventoryShop.Domain.Entities;
using InventoryShop.Domain.Shared.Errors;
using InventoryShop.Domain.Shared.Exceptions;

namespace InventoryShop.Application.UseCases.Slots;

public sealed class DeleteShopSlotUseCase(
   ITransactionManager transactionManager,
   IItemsRepository itemsRepository,
   IShopSlotsRepository shopSlotsRepository)
{
   public async Task<UnitResult<Error>> ExecuteAsync(Guid id, CancellationToken ct)
   {
      var beginTransactionResult = await transactionManager.BeginTransactionAsync(ct);

      if (beginTransactionResult.IsFailure)
         return beginTransactionResult;
      
      ItemEntity? itemInSlot = await itemsRepository.GetItemByIdAsync(id, ct);

      if (itemInSlot is null)
         throw new DataIntegrityException($"Item in slot {id} not found");
      
      itemInSlot.SetIsOnSale(false);
      await itemsRepository.UpdateItemSaleStatus(itemInSlot.Id, false, ct);
      
      await shopSlotsRepository.DeleteSlotAsync(id, ct);
      return await transactionManager.CommitTransactionAsync(ct);
   }
}