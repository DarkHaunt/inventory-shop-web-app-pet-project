namespace InventoryShop.Web.DTO;

public record ShopOrderDTO(
   Guid Id,
   string BuyerName,
   string? SellerName,
   DateTime CompletedAtUtc,
   OrderDataDTO OrderData
);

public record OrderDataDTO(
   ItemDTO ItemId,
   WalletDTO Price,
   uint RequiredLevel
);