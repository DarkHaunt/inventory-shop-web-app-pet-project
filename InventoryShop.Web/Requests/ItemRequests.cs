namespace InventoryShop.Web.Requests;

public sealed record CreateItemByPlayerRequest(Guid CreatorId);

public sealed record EquipItemByPlayerRequest(Guid ItemToEquipId, Guid EquipperId, bool IsEquipped);

public sealed record DeletePlayerItemRequest(Guid ItemId, Guid? OwnerId);