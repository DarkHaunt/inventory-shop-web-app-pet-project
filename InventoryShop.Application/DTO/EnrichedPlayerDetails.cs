namespace InventoryShop.Application.DTO;

public sealed record EnrichedPlayerDetails(
   Guid Id,
   string Nickname,
   string PasswordHashed,
   DateTime CreatedAt,
   WalletDetails Wallet,
   LevelProgressDetails LevelProgress,
   StatsDetails Stats,
   List<EnrichedItemDetails> Items
);