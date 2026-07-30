using InventoryShop.Domain.Entities;
using InventoryShop.Domain.Enums;
using InventoryShop.Domain.ValueObjects;

namespace InventoryShop.Infrastructure.Caching;

internal sealed record PlayerCacheEntry(
   Guid Id,
   string Nickname,
   string PasswordHashed,
   string Role,
   DateTime CreatedAt,
   Wallet Wallet,
   uint Level,
   uint Experience)
{
   public static PlayerCacheEntry FromEntity(PlayerEntity player) => new(
      player.Id,
      player.Nickname,
      player.PasswordHashed,
      player.Role,
      player.CreatedAt,
      player.Wallet,
      player.LevelProgress.Level,
      player.LevelProgress.Experience);

   public PlayerEntity ToEntity() =>
      PlayerEntity.Create(
         Id,
         Nickname,
         Role,
         PasswordHashed,
         CreatedAt,
         Wallet,
         LevelProgress.Create(Level, Experience));
}

internal sealed record ShopSlotCacheEntry(
   Guid Id,
   Guid SellItemId,
   Guid? SellerId,
   Wallet Price,
   uint RequiredLevel,
   uint RequiredExperience)
{
   public static ShopSlotCacheEntry FromEntity(ShopSlotEntity slot) => new(
      slot.Id,
      slot.SellItemId,
      slot.SellerId,
      slot.Price,
      slot.RequiredLevel.Level,
      slot.RequiredLevel.Experience);

   public ShopSlotEntity ToEntity() =>
      ShopSlotEntity.Create(
         Id,
         SellItemId,
         Price,
         LevelProgress.Create(RequiredLevel, RequiredExperience),
         SellerId);
}

internal sealed record ShopOrderCacheEntry(
   Guid Id,
   Guid BuyerId,
   Guid? SellerId,
   DateTime CompletedAtUtc,
   OrderData OrderData)
{
   public static ShopOrderCacheEntry FromEntity(ShopOrderEntity order) => new(
      order.Id,
      order.BuyerId,
      order.SellerId,
      order.CompletedAtUtc,
      order.OrderData);

   public ShopOrderEntity ToEntity() =>
      ShopOrderEntity.Create(
         Id,
         BuyerId,
         SellerId,
         OrderData,
         CompletedAtUtc);
}

internal sealed record ItemCacheEntry(
   Guid Id,
   ItemType Type,
   string Description,
   Stats StatsModifiers,
   bool IsEquipped,
   bool IsOnSale,
   Guid? OwnerId,
   Guid? CreatorId)
{
   public static ItemCacheEntry FromEntity(ItemEntity item) => new(
      item.Id, item.Type, item.Description, item.StatsModifiers,
      item.IsEquipped, item.IsOnSale, item.OwnerId, item.CreatorId);

   public ItemEntity ToEntity() =>
      ItemEntity.Restore(Id, Type, Description, StatsModifiers, IsEquipped, IsOnSale, OwnerId, CreatorId);
}