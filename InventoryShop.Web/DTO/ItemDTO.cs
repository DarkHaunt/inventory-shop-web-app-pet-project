using InventoryShop.Domain.Enums;

namespace InventoryShop.Web.DTO;

public sealed record ItemDTO(
   Guid Id,
   ItemType Type,
   string? Description,
   StatsDTO StatsModifiers,
   bool IsEquipped,
   string? OwnerName,
   string? CreatorName
);