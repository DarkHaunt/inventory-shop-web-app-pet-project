namespace InventoryShop.Web.Requests;

public sealed record LoginPlayerRequest(string Nickname, string Password);

public sealed record RegisterNewPlayerRequest(string Nickname, string Password);