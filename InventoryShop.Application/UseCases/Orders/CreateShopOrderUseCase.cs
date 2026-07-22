using CSharpFunctionalExtensions;
using InventoryShop.Application.Commands;
using InventoryShop.Application.DTO;
using InventoryShop.Application.Interfaces;
using InventoryShop.Application.Services;
using InventoryShop.Application.Shared;
using InventoryShop.Domain.Entities;
using InventoryShop.Domain.Shared.Errors;

namespace InventoryShop.Application.UseCases.Orders;

// TODO: Use this in controller of shop
public sealed class CreateShopOrderUseCase(
   ITransactionManager transactionManager,
   IShopOrdersRepository ordersRepository,
   EnrichedOrderDetailsFactory enrichedOrderDetailsFactory,
   IGuidProvider guidProvider)
{
   public async Task<Result<EnrichedShopOrderDetails, Error>> ExecuteAsync(CreateShopOrderCommand command, CancellationToken ct)
   {
      var beginTransactionResult = await transactionManager.BeginTransactionAsync(ct);

      if (beginTransactionResult.IsFailure)
         return beginTransactionResult.Error;

      var order = ShopOrderEntity.Create
      (
         guidProvider.CreateNew(),
         command.BuyerId,
         command.SellerId,
         command.OrderData,
         command.DateOfCompletion
      );

      await ordersRepository.AddOrderAsync(order, ct);
      
      EnrichedShopOrderDetails enrichedOrderDetails = await enrichedOrderDetailsFactory.CreateAsync(order, ct);
      var commit = await transactionManager.CommitTransactionAsync(ct);
      
      return commit.IsFailure 
         ? commit.Error 
         : enrichedOrderDetails;
   }
}