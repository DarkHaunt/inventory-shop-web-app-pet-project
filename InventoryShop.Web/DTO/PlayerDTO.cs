namespace InventoryShop.Web.DTO;

public sealed record PlayerDTO
{
   public Guid Id { get; set; }
   public string Nickname { get; set; }
   public WalletDTO Wallet { get; set; }
   public StatsDTO Stats { get; set; }
   public LevelProgressDTO LevelProgress { get; set; }
   public List<ItemDTO> Items { get; set; }
}