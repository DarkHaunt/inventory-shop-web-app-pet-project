using InventoryShop.Domain.Enums;

namespace InventoryShop.Application.DTO;

public sealed record EnrichedItemDetails
{
   public Guid Id { get; set; }
   public ItemType Type { get; set; }
   public string? Description { get; set; }
   public required StatsDetails StatsModifiers { get; set; }
   public bool IsEquipped { get; set; }
   public bool IsOnSale { get; set; }
   public string? OwnerName { get; set; }
   public string? CreatorName { get; set; }
}