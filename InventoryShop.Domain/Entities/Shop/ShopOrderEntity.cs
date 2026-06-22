using InventoryShop.Domain.ValueObjects;

namespace InventoryShop.Domain.Entities.Shop;

public sealed class ShopOrderEntity
{
   public Guid Id { get; private set; }
   public Guid BuyerId { get; private set; }
   public Guid SellerId { get; private set; }
   public OrderData OrderData { get; private set; }
   public DateTime CompletedAtUtc { get; private set; }

   private ShopOrderEntity() { }

   public static ShopOrderEntity Create(Guid id, Guid buyerId, Guid sellerId, OrderData orderData, DateTime dateOfCompletion)
   {
      return new ShopOrderEntity
      {
         Id = id,
         BuyerId = buyerId,
         SellerId = sellerId,
         OrderData = orderData,
         CompletedAtUtc = dateOfCompletion
      };
   }
}