using InventoryShop.Domain.Entities;
using InventoryShop.Domain.Enums;
using InventoryShop.Domain.ValueObjects;

namespace InventoryShop.Tests.Common;

public static class ItemEntityFixture
{
   public static ItemEntity Create(
      Guid? ownerId = null,
      Guid? creatorId = null,
      bool isEquipped = false,
      bool isOnSale = false,
      ItemType type = ItemType.Sword)
   {
      var item = ItemEntity.Create(
         Guid.NewGuid(), type, "Test item", Stats.Create(1, 1, 1), creatorId, ownerId);

      if (isEquipped)
         item.Equip(); // требует ownerId != null, иначе бросит исключение

      if (isOnSale)
         item.SetIsOnSale(true);

      return item;
   }
}