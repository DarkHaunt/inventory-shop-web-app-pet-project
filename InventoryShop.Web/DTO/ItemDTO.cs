using InventoryShop.Domain.Enums;

namespace InventoryShop.Web.DTO;

public sealed record ItemDTO
{
   public Guid Id { get; set; }
   public ItemType Type { get; set; }
   public string? Description { get; set; }
   public required StatsDTO StatsModifiers { get; set; }
   public bool IsEquipped { get; set; }
   public string? OwnerName { get; set; }
   public string? CreatorName { get; set; }
}