using InventoryShop.Application.DTO;
using InventoryShop.Domain.Entities;
using InventoryShop.Domain.Enums;
using InventoryShop.Domain.ValueObjects;

namespace InventoryShop.Tests.Common;

public static class ShopSlotEntityFixture
{
   public static ShopSlotEntity CreateSlot(
      Guid? id = null,
      Guid? sellerId = null,
      Guid sellItemId = default)
   {
      return ShopSlotEntity.Create(
         id: id ?? Guid.NewGuid(),
         sellerId: sellerId,
         price: Wallet.CreateInitial(),
         sellItemId: sellItemId,
         requiredLevelProgress: LevelProgress.CreateInitial()
      );
   }

   public static EnrichedShopSlotDetails CreateDetails(Guid id)
   {
      return new EnrichedShopSlotDetails
      (
         id, 
         "seller_name",
         CreateValidItemForSlot(),
         new WalletDetails(100),
         1
      );
   }
   
   private static EnrichedItemDetails CreateValidItemForSlot()
   {
      return new EnrichedItemDetails
      (
         Id: Guid.NewGuid(),
         Type: ItemType.Sword,
         Description: null,
         StatsModifiers: new StatsDetails(1,1,1),
         IsEquipped: false,
         IsOnSale: true,
         OwnerName: "seller_name",
         CreatorName: "creator_name"
      );
   }
}