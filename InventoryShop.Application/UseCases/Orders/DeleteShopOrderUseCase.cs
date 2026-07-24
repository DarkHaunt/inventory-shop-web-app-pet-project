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
      await shopOrdersRepository.DeleteOrderAsync(orderId, ct);
      return await transactionManager.SaveChangesAsync(ct);
   }
}