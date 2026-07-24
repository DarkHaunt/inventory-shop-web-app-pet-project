using CSharpFunctionalExtensions;
using InventoryShop.Application.Interfaces;
using InventoryShop.Application.Shared;
using InventoryShop.Domain.Errors;
using InventoryShop.Domain.Shared.Errors;

namespace InventoryShop.Application.UseCases.Items;

public sealed class DeleteItemUseCase(ITransactionManager transactionManager, IItemsRepository itemsRepository)
{
   public async Task<UnitResult<Error>> ExecuteAsync(bool isAdmin, Guid itemId, Guid? ownerId, CancellationToken ct)
   {
      if (isAdmin == false)
      {
         if (await itemsRepository.IsItemOwnedByPlayerAsync(itemId, ownerId, ct) == false)
            return ItemsErrors.PlayerDoesNotOwnItem(itemId, ownerId);
      }
      
      var beginTransactionResult = await transactionManager.BeginTransactionAsync(ct);

      if (beginTransactionResult.IsFailure)
         return beginTransactionResult;

      await itemsRepository.DeleteItemAsync(itemId, ct);
      return await transactionManager.CommitTransactionAsync(ct);
   }
}