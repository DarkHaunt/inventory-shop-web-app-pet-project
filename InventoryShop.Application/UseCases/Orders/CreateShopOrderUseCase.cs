using CSharpFunctionalExtensions;
using InventoryShop.Application.Interfaces;
using InventoryShop.Application.Shared;
using InventoryShop.Domain.Entities.Shop;
using InventoryShop.Domain.Errors;
using InventoryShop.Domain.Shared.Errors;
using InventoryShop.Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace InventoryShop.Application.UseCases.Orders;


// TODO: Use this in controller of shop
public sealed class CreateShopOrderUseCase(
   ITransactionManager transactionManager,
   IShopOrdersRepository ordersRepository,
   IGuidProvider guidProvider,
   ILogger logger)
{
   public async Task<UnitResult<Error>> ExecuteAsync(Guid buyerId, Guid? sellerId, OrderData orderData, DateTime dateOfCompletion, CancellationToken ct)
   {
      var beginTransactionResult = await transactionManager.BeginTransactionAsync(ct);
      
      if (beginTransactionResult.IsFailure)
         return beginTransactionResult;
      
      Guid orderId = guidProvider.CreateNew();
      
      try
      { 
         ShopOrderEntity order = CreateOrderEntity(orderId, buyerId, sellerId, orderData, dateOfCompletion);
         await ordersRepository.AddOrderAsync(order, ct);
      }
      catch (OperationCanceledException e)
      {
         logger.LogError(e, "Creation of order {Nickname} was cancelled", orderId);
         return GenericErrors.OperationCanceledError();
      }
      catch (Exception e)
      {
         logger.LogError(e, "Failed to create order {Nickname}, reason: {Error}", orderId, e.Message);
         return OrdersErrors.CreationFailed(orderId);
      }

      return await transactionManager.CommitTransactionAsync(ct);
   }

   private ShopOrderEntity CreateOrderEntity(Guid orderId, Guid buyerId, Guid? sellerId, OrderData orderData, DateTime dateOfCompletion) =>
      ShopOrderEntity.Create(orderId, buyerId, sellerId, orderData, dateOfCompletion);
}