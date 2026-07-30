namespace InventoryShop.Web.DTO;

public sealed record LevelProgressDTO(
   uint Level,
   uint Experience,
   uint ExperienceForNextLevel
);