namespace InventoryShop.Domain.ValueObjects;

public sealed record OrderData(ItemInOrderSnapshot ItemSnapshot, Wallet Price, uint RequiredLevel);