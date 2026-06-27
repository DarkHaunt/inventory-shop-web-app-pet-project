using CSharpFunctionalExtensions;
using InventoryShop.Application.Interfaces;
using InventoryShop.Application.Shared;
using InventoryShop.Domain.Errors;
using InventoryShop.Domain.Shared.Errors;
using Microsoft.Extensions.Logging;

namespace InventoryShop.Application.UseCases.Items;

public sealed class DeleteItemUseCase(ITransactionManager transactionManager, IItemsRepository itemsRepository, ILogger logger)
{
   public async Task<UnitResult<Error>> ExecuteAsync(Guid id, CancellationToken ct)
   {
      var beginTransactionResult = await transactionManager.BeginTransactionAsync(ct);
      
      if (beginTransactionResult.IsFailure)
         return beginTransactionResult;
      
      try
      {
         await itemsRepository.DeleteItemAsync(id, ct);
      }
      catch (OperationCanceledException e)
      {
         logger.LogError(e, "Deletion of item {ID} was cancelled", id);
         return GenericErrors.OperationCanceledError();
      }
      catch (Exception e)
      {
         logger.LogError(e, "Failed to delete item {ID}, reason: {Error}", id, e.Message);
         return ItemsErrors.DeletionFailed(id);
      }

      return await transactionManager.CommitTransactionAsync(ct);
   }
}