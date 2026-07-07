namespace InventoryShop.Web.Requests;

public sealed record EquipItemByPlayerRequest(Guid ItemToEquipId, Guid EquipperId, bool IsEquipped);
