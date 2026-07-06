namespace InventoryShop.Application.DTO;

public sealed record EnrichedPlayerDetails
{
   public Guid Id { get; set; }
   public string Nickname { get; set; }
   public WalletDetails Wallet { get; set; }
   public LevelProgressDetails LevelProgress { get; set; }
   public StatsDetails Stats { get; set; }
   public List<EnrichedItemDetails> Items { get; set; }
}