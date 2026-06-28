namespace InventoryShop.Web.DTO;

public sealed record PlayerDTO(
   Guid Id,
   string Nickname,
   WalletDTO Wallet,
   StatsDTO Stats,
   LevelProgressDTO LevelProgress,
   List<ItemDTO> Items
);