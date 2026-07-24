using InventoryShop.Web.DTO;

namespace InventoryShop.Web.Requests;

public sealed record CreateShopSlotRequest(Guid ItemToSellId, WalletDTO Price, LevelProgressDTO LevelRequired);

public sealed record ModifyShopSlotRequest(Guid SlotId, WalletDTO? NewPrice, LevelProgressDTO? NewLevelRequired);
