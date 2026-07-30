namespace InventoryShop.Application.DTO;

public record EnrichedShopSlotDetails(
   Guid Id,
   string? SellerName,
   EnrichedItemDetails SellItem,
   WalletDetails Price,
   uint RequiredLevel
);