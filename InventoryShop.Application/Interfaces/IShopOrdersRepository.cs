using InventoryShop.Domain.Entities.Shop;

namespace InventoryShop.Application.Interfaces;

public interface IShopOrdersRepository
{
   Task<ShopOrderEntity?> GetOrderById(Guid id, CancellationToken ct);
   Task<List<ShopOrderEntity>> GetAllOrdersAsync(CancellationToken ct);
   Task<List<ShopOrderEntity>> GetAllOrdersCompletedByAsync(Guid buyerId, CancellationToken ct);
   Task<List<ShopOrderEntity>> GetAllOrdersCreatedByAsync(Guid sellerId, CancellationToken ct);
   
   Task AddOrderAsync(ShopOrderEntity order, CancellationToken ct);
   Task DeleteOrderAsync(Guid id, CancellationToken ct);
}