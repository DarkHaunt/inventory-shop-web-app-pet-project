using InventoryShop.Domain.Enums;

namespace InventoryShop.Domain.ValueObjects;

public sealed record ItemInOrderSnapshot(Guid Id, ItemType Type, string? Description, Stats StatsModifiers);