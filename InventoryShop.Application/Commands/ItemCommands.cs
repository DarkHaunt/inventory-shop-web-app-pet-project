namespace InventoryShop.Application.Commands;

public sealed record DeleteItemCommand(bool IsAdmin, Guid ItemId, Guid OwnerId);