namespace InventoryShop.Web.DTO;

public sealed record ShopSlotDTO(
   Guid Id,
   string? SellerName,
   ItemDTO SellItem,
   WalletDTO Wallet,
   uint RequiredLevel
);