using CSharpFunctionalExtensions;
using InventoryShop.Application.Interfaces;
using InventoryShop.Application.Shared;
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

      await shopOrdersRepository.DeleteOrderAsync(id, ct);
      return await transactionManager.CommitTransactionAsync(ct);
   }
}