using InventoryShop.Domain.ValueObjects;

namespace InventoryShop.Domain.Entities.Shop;

public class ShopSlotEntity
{
   public const int MaxPrice = 100_000;

   public Guid Id { get; private set; }
   public Guid SellItemId { get; private set; }
   public Guid? SellerId { get; private set; }
   public Wallet Price { get; private set; }
   public LevelProgress RequiredLevelProgress { get; private set; }

   private ShopSlotEntity() { }

   public static ShopSlotEntity Create(Guid id, Guid sellItemId, Wallet price, LevelProgress requiredLevelProgress, Guid? sellerId)
   {
      if (price.GoldAmount > MaxPrice)
         throw new ArgumentOutOfRangeException(nameof(price), $"Price cannot exceed {MaxPrice}");

      return new ShopSlotEntity
      {
         Id = id,
         SellItemId = sellItemId,
         Price = price,
         RequiredLevelProgress = requiredLevelProgress,
         SellerId = sellerId
      };
   }
}