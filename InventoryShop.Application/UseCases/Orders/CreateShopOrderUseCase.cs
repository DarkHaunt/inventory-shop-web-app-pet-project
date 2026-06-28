using CSharpFunctionalExtensions;
using InventoryShop.Application.DTO;
using InventoryShop.Domain.Entities.Shop;
using InventoryShop.Domain.Shared.Errors;

namespace InventoryShop.Application.UseCases.Orders;

public sealed class CreateShopOrderUseCase
{
   public async Task<Result<EnrichedShopOrderDetails, Error>> CreateAsync(ShopOrderEntity order, CancellationToken ct)
   {
      
   }
}