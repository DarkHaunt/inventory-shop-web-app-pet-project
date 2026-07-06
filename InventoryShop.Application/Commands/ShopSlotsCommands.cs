using InventoryShop.Application.DTO;

namespace InventoryShop.Application.Commands;

public sealed record CreateShopSlotCommand(Guid? SellerId, Guid ItemToSellId, WalletDetails Price, LevelProgressDetails LevelRequired);

public sealed record ModifyShopSlotCommand(Guid Id, WalletDetails? NewPrice, LevelProgressDetails? NewLevelRequired);

