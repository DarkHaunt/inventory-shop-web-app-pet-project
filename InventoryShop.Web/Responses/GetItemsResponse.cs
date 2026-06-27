namespace InventoryShop.Web.DTO;

public sealed record GetItemsResponse
{
   public List<ItemDTO> Items { get; set; }
}