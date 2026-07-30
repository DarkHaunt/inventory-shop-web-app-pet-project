using InventoryShop.Domain.Enums;

namespace InventoryShop.Application.DTO;

public sealed record EnrichedItemDetails(
   Guid Id,
   ItemType Type,
   string? Description,
   StatsDetails StatsModifiers,
   bool IsEquipped,
   bool IsOnSale,
   string? OwnerName,
   string? CreatorName
);