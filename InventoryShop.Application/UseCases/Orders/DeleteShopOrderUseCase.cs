using CSharpFunctionalExtensions;
using InventoryShop.Application.Interfaces;
using InventoryShop.Application.Shared;
using InventoryShop.Domain.Shared.Errors;

namespace InventoryShop.Application.UseCases.Orders;

public sealed class DeleteShopOrderUseCase(
   ITransactionManager transactionManager, 
   IShopOrdersRepository shopOrdersRepository)
{
   public async Task<UnitResult<Error>> ExecuteAsync(Guid orderId, CancellationToken ct)
   {
      var beginTransactionResult = await transactionManager.BeginTransactionAsync(ct);

      if (beginTransactionResult.IsFailure)
         return beginTransactionResult;

      await shopOrdersRepository.DeleteOrderAsync(orderId, ct);
      return await transactionManager.CommitTransactionAsync(ct);
   }
}