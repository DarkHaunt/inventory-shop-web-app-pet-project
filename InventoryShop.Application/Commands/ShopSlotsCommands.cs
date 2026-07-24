using InventoryShop.Application.DTO;

namespace InventoryShop.Application.Commands;

public sealed record CreateShopSlotCommand(Guid? SellerId, Guid ItemToSellId, WalletDetails Price, LevelProgressDetails LevelRequired);

public sealed record ModifyShopSlotCommand(bool isAdmin, Guid? SlotOwnerId, Guid Id, WalletDetails? NewPrice, LevelProgressDetails? NewLevelRequired);

public sealed record DeleteShopSlotCommand(bool IsAdmin, Guid? SlotOwnerId, Guid SlotId);


