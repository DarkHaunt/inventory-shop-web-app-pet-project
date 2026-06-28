using CSharpFunctionalExtensions;
using InventoryShop.Application.Interfaces;
using InventoryShop.Application.Shared;
using InventoryShop.Domain.Errors;
using InventoryShop.Domain.Shared.Errors;
using Microsoft.Extensions.Logging;

namespace InventoryShop.Application.UseCases.Orders;

public sealed class DeleteShopOrderUseCase(ITransactionManager transactionManager, IShopOrdersRepository shopOrdersRepository, ILogger logger)
{
   public async Task<UnitResult<Error>> ExecuteAsync(Guid id, CancellationToken ct)
   {
      var beginTransactionResult = await transactionManager.BeginTransactionAsync(ct);
      
      if (beginTransactionResult.IsFailure)
         return beginTransactionResult;
      
      try
      {
         await shopOrdersRepository.DeleteOrderAsync(id, ct);
      }
      catch (OperationCanceledException e)
      {
         logger.LogError(e, "Deletion of order {ID} was cancelled", id);
         return GenericErrors.OperationCanceledError();
      }
      catch (Exception e)
      {
         logger.LogError(e, "Failed to delete order {ID}, reason: {Error}", id, e.Message);
         return OrdersErrors.DeletionFailed(id);
      }

      return await transactionManager.CommitTransactionAsync(ct);
   }
}