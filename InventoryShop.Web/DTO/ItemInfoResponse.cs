using InventoryShop.Domain.Enums;

namespace InventoryShop.Web.DTO;

public sealed record ItemInfoResponse
{
   public Guid Id { get; private set; }
   public ItemType Type { get; private set; }
   public string? Description { get; private set; }
   public StatsDTO StatsModifiers { get; private set; }
   public bool IsEquipped { get; private set; }
   public string OwnerName { get; private set; }
   public string CreatorName { get; private init; }
}