using InventoryShop.Web.DTO;

namespace InventoryShop.Web.Requests;

public sealed record ModifyShopSlotRequest(Guid Id, WalletDTO? NewPrice, LevelProgressDTO? NewLevelRequired);