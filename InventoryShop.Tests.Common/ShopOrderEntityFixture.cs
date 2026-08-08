using InventoryShop.Domain.Entities;
using InventoryShop.Domain.ValueObjects;

namespace InventoryShop.Tests.Common;

public static class ShopOrderEntityFixture
{
   public static ShopOrderEntity Create(
      Guid? buyerId = null,
      Guid? sellerId = null)
   {
      ItemSnapshot itemSnapshot = ItemEntityFixture
         .Create()
         .Snapshot();
      
      return ShopOrderEntity.Create(
         Guid.NewGuid(),
         buyerId ?? Guid.NewGuid(),
         sellerId,
         new OrderData(itemSnapshot, Wallet.CreateInitial(), 1),
         DateTime.UtcNow);
   }
}