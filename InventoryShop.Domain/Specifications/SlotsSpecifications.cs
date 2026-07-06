using System.Linq.Expressions;
using InventoryShop.Domain.Entities;
using InventoryShop.Domain.Shared.Specifications;

namespace InventoryShop.Domain.Specifications;

public sealed class SlotCreatedBySpecification(Guid sellerId) : Specification<ShopSlotEntity>
{
    public override Expression<Func<ShopSlotEntity, bool>> ToExpression() =>
        s => s.SellerId == sellerId;
}