using InventoryShop.Domain.Entities;
using InventoryShop.Domain.Shared.Specifications;

namespace InventoryShop.Application.Interfaces;

public interface IShopOrdersRepository
{
   Task<ShopOrderEntity?> GetOrderByIdAsync(Guid id, CancellationToken ct);
   Task<List<ShopOrderEntity>> GetAllOrdersAsync(CancellationToken ct);
   Task<List<ShopOrderEntity>> GetOrdersSpecifiedAsync(Specification<ShopOrderEntity> specification, CancellationToken ct);
   
   Task AddOrderAsync(ShopOrderEntity order, CancellationToken ct);
   Task DeleteOrderAsync(Guid id, CancellationToken ct);
}