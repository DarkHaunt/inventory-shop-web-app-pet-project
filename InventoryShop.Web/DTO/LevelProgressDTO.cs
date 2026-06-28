namespace InventoryShop.Web.DTO;

public sealed record LevelProgressDTO(
   uint CurrentLevel,
   uint CurrentExperience,
   uint NextLevelExperience
);