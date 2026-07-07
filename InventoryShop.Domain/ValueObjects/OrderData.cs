namespace InventoryShop.Domain.ValueObjects;

public sealed record OrderData(
   ItemSnapshot ItemSnapshot,
   Wallet Price,
   uint RequiredLevel
);