namespace InventoryShop.Domain.ValueObjects;

public sealed record OrderData(Guid ItemId, Wallet Price, LevelProgress RequiredLevelProgress);