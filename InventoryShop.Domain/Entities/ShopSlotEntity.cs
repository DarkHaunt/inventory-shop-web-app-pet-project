using InventoryShop.Domain.Exceptions;
using InventoryShop.Domain.ValueObjects;

namespace InventoryShop.Domain.Entities;

public sealed class ShopSlotEntity
{
   public const int MaxPrice = 100_000;

   public Guid Id { get; private set; }
   public Guid SellItemId { get; private set; }
   public Guid? SellerId { get; private set; }
   public Wallet Price { get; private set; }
   public LevelProgress RequiredLevel { get; private set; }

   private ShopSlotEntity() { }

   public static ShopSlotEntity Create(Guid id, Guid sellItemId, Wallet price, LevelProgress requiredLevelProgress, Guid? sellerId)
   {
      if (price.GoldAmount > MaxPrice)
         throw new ViolatedShopSlotPolicyException($"Price cannot exceed {MaxPrice}");

      return new ShopSlotEntity
      {
         Id = id,
         SellItemId = sellItemId,
         Price = price,
         RequiredLevel = requiredLevelProgress,
         SellerId = sellerId
      };
   }
}