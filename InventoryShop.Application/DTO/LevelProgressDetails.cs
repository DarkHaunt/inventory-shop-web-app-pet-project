namespace InventoryShop.Application.DTO;

public sealed record LevelProgressDetails
{
   public uint Level { get; set; }
   public uint Experience { get; set; }
   public uint ExperienceForNextLevel { get; set; }
}