namespace InventoryShop.Web.Requests;

public sealed record PlayMinigameRequest(Guid PlayerId);

public sealed record ExecutePurchaseRequest(Guid BuyerId, Guid SlotToExecute);
