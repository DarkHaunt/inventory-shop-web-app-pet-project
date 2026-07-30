namespace InventoryShop.Application.DTO;

public sealed record LevelProgressDetails(
   uint Level,
   uint Experience,
   uint ExperienceForNextLevel
);