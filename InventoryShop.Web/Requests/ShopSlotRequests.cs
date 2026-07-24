using InventoryShop.Web.DTO;

namespace InventoryShop.Web.Requests;

public sealed record CreateShopSlotRequest(Guid? SellerId, Guid ItemToSellId, WalletDTO Price, LevelProgressDTO LevelRequired);

public sealed record ModifyShopSlotRequest(Guid? ModifierId, Guid SlotId, WalletDTO? NewPrice, LevelProgressDTO? NewLevelRequired);

public sealed record DeleteShopSlotRequest(Guid SlotId, Guid? SlotOwnerId);