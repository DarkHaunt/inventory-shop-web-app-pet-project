using InventoryShop.Domain.Enums;
using InventoryShop.Domain.ValueObjects;

namespace InventoryShop.Application.DTO;

public sealed record ItemDetails
{
   public Guid Id { get; private set; }
   public ItemType Type { get; private set; }
   public string? Description { get; private set; }
   public Stats StatsModifiers { get; private set; }
   public bool IsEquipped { get; private set; }
   public string OwnerName { get; private set; }
   public string CreatorName { get; private init; }
}