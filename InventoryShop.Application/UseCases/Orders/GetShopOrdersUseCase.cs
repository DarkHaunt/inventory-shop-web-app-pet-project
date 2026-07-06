using CSharpFunctionalExtensions;
using InventoryShop.Application.DTO;
using InventoryShop.Application.Interfaces;
using InventoryShop.Application.Services;
using InventoryShop.Domain.Entities;
using InventoryShop.Domain.Errors;
using InventoryShop.Domain.Shared.Errors;
using InventoryShop.Domain.Specifications;
using Microsoft.Extensions.Logging;

namespace InventoryShop.Application.UseCases.Orders;

public sealed class GetShopOrdersUseCase(IShopOrdersRepository shopOrdersRepository, EnrichedOrderDetailsFactory orderDetailsFactory, ILogger logger)
{
   public async Task<Result<EnrichedShopOrderDetails, Error>> GetOrderById(Guid id, CancellationToken ct)
   {
      ShopOrderEntity? order = await shopOrdersRepository.GetOrderByIdAsync(id, ct);

      if (order is null)
      {
         logger.LogError("Can't find order with {ID}", id);
         return ShopOrdersErrors.OrderWithIdNotFoundError(id);
      }

      return Result.Success<EnrichedShopOrderDetails, Error>(await orderDetailsFactory.CreateAsync(order, ct));
   }

   public async Task<Result<List<EnrichedShopOrderDetails>, Error>> GetAllOrdersAsync(CancellationToken ct)
   {
      var orders = await shopOrdersRepository.GetAllOrdersAsync(ct);
      return await orderDetailsFactory.CreateManyAsync(orders, ct);
   }

   public async Task<Result<List<EnrichedShopOrderDetails>, Error>> GetAllOrdersCompletedByPlayerAsync(Guid ordersCompletorId, CancellationToken ct)
   {
      var s = new OrdersCompletedByPlayerSpecification(ordersCompletorId);
      var orders = await shopOrdersRepository.GetOrdersSpecifiedAsync(s, ct);
      return await orderDetailsFactory.CreateManyAsync(orders, ct);
   }

   public async Task<Result<List<EnrichedShopOrderDetails>, Error>> GetAllOrdersCreatedByPlayerAsync(Guid ordersCreatorId, CancellationToken ct)
   {
      var s = new OrdersCreatedByPlayerSpecification(ordersCreatorId);
      var orders = await shopOrdersRepository.GetOrdersSpecifiedAsync(s, ct);
      return await orderDetailsFactory.CreateManyAsync(orders, ct);
   }
}