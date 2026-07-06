using System.Linq.Expressions;
using InventoryShop.Domain.Entities;
using InventoryShop.Domain.Shared.Specifications;

namespace InventoryShop.Domain.Specifications;

public sealed class OrdersCompletedByPlayerSpecification(Guid buyerId) : Specification<ShopOrderEntity>
{
   public override Expression<Func<ShopOrderEntity, bool>> ToExpression() =>
      x => x.BuyerId == buyerId;
}

public sealed class OrdersCreatedByPlayerSpecification(Guid sellerId) : Specification<ShopOrderEntity>
{
   public override Expression<Func<ShopOrderEntity, bool>> ToExpression() =>
      x => x.SellerId == sellerId;
}