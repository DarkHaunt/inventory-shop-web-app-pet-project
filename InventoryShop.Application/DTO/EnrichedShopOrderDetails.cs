namespace InventoryShop.Application.DTO;

public sealed record EnrichedShopOrderDetails(
   Guid Id,
   string BuyerName,
   string? SellerName,
   DateTime CompletedAtUtc,
   OrderDataDetails OrderData
);