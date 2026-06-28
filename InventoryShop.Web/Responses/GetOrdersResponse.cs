namespace InventoryShop.Web.DTO;

public sealed record GetOrdersResponse(List<ShopOrderDTO> Orders);