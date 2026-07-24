namespace InventoryShop.Web.Requests;

public sealed record EquipItemByPlayerRequest(Guid ItemId, bool IsEquipped);
