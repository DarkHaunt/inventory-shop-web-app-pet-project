namespace InventoryShop.Application.DTO;

public record EnrichedShopSlotDetails
{
   public Guid Id { get; init; }
   public string? SellerName { get; init; }
   public EnrichedItemDetails SellItem { get; init; }
   public WalletDetails Price { get; init; }
   public uint RequiredLevel { get; init; }
}