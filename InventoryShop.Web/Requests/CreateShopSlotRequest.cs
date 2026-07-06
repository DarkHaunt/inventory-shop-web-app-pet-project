using InventoryShop.Web.DTO;

namespace InventoryShop.Web.Requests;

public sealed record CreateShopSlotRequest(Guid? SellerId, Guid ItemToSellId, WalletDTO Price, LevelProgressDTO LevelRequired);