using InventoryShop.Application.DTO;

namespace InventoryShop.Application.Commands;

public sealed record CreateShopSlotCommand(bool IsAdmin, Guid? SellerId, Guid ItemToSellId, WalletDetails Price, LevelProgressDetails LevelRequired);

public sealed record ModifyShopSlotCommand(bool IsAdmin, Guid? SlotOwnerId, Guid SlotId, WalletDetails? NewPrice, LevelProgressDetails? NewLevelRequired);

public sealed record DeleteShopSlotCommand(bool IsAdmin, Guid? SlotOwnerId, Guid SlotId);
