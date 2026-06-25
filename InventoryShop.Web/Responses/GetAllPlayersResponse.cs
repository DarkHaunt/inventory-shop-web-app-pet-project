namespace InventoryShop.Web.DTO;

public sealed record GetAllPlayersResponse
{
   public List<PlayerDTO> Players { get; set; }
}