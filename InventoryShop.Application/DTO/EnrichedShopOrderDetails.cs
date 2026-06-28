namespace InventoryShop.Application.DTO;

public sealed record EnrichedShopOrderDetails
{
   public Guid Id { get; set; }
   public required string BuyerName { get; set; }
   public string? SellerName { get; set; }
   public DateTime CompletedAtUtc { get; set; }
   public required OrderDataDetails OrderData { get; set; }
}