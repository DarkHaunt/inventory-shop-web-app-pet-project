using InventoryShop.Domain.ValueObjects;

namespace InventoryShop.Domain.Entities.Shop;

public class ShopSlotEntity
{
   public const int MaxPrice = 100_000;

   public Guid Id { get; private set; }
   public Guid ItemId { get; private set; }
   public Wallet Price { get; private set; }
   public Level RequiredLevel { get; private set; }

   private ShopSlotEntity() { }

   public static ShopSlotEntity Create(Guid id, Guid itemId, Wallet price, Level requiredLevel)
   {
      if (price.GoldAmount > MaxPrice)
         throw new ArgumentOutOfRangeException(nameof(price), $"Price cannot exceed {MaxPrice}");

      return new ShopSlotEntity
      {
         Id = id,
         ItemId = itemId,
         Price = price,
         RequiredLevel = requiredLevel
      };
   }
}