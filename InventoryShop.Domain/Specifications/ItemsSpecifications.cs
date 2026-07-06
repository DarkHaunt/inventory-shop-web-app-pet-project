using System.Linq.Expressions;
using InventoryShop.Domain.Entities;
using InventoryShop.Domain.Shared.Specifications;

namespace InventoryShop.Domain.Specifications;

public sealed class ItemsOwnedByPlayerSpecification(Guid ownerId) : Specification<ItemEntity>
{
   public override Expression<Func<ItemEntity, bool>> ToExpression() =>
      x => x.OwnerId == ownerId;
}

public sealed class ItemsCreatedByPlayerSpecification(Guid creatorId) : Specification<ItemEntity>
{
   public override Expression<Func<ItemEntity, bool>> ToExpression() =>
      x => x.CreatorId == creatorId;
}

public sealed class ItemsEquippedByPlayerSpecification(Guid ownerId) : Specification<ItemEntity>
{
   public override Expression<Func<ItemEntity, bool>> ToExpression() =>
      x => x.OwnerId == ownerId && x.IsEquipped;
}