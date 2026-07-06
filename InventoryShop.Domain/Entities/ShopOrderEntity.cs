using InventoryShop.Domain.Exceptions;
using InventoryShop.Domain.ValueObjects;

namespace InventoryShop.Domain.Entities;

public sealed class ShopOrderEntity
{
   public Guid Id { get; private set; }
   public Guid BuyerId { get; private set; }
   public Guid? SellerId { get; private set; }
   public DateTime CompletedAtUtc { get; private set; }
   public OrderData OrderData { get; private set; }

   private ShopOrderEntity() { }

   public static ShopOrderEntity Create(Guid id, Guid buyerId, Guid? sellerId, OrderData orderData, DateTime dateOfCompletion)
   {
      if(sellerId != null && sellerId == buyerId)
         throw new ViolatedShopOrderPolicyException("Seller and buyer cannot be the same person");
      
      if(dateOfCompletion > DateTime.UtcNow)
         throw new ViolatedShopOrderPolicyException("Date of completion cannot be in the future");
      
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