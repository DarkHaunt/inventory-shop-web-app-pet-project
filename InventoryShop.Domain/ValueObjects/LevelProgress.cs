using InventoryShop.Domain.Exceptions;

namespace InventoryShop.Domain.ValueObjects;

public sealed record LevelProgress
{
   public const uint MaxLevel = 5; 
   private const uint BaseExperience = 10;
   private const double ExperienceMultiplier = 1.5d;
   
   public uint Level { get; }
   public uint Experience { get; }
   
   public uint ExperienceForNextLevel => 
      (uint)Math.Round(BaseExperience * Math.Pow(ExperienceMultiplier, Level));

   private LevelProgress(uint level, uint experience)
   {
      if (level > MaxLevel)
         throw new ViolatedLevelPolicyException($"Level must be lower than max {MaxLevel}");
      
      Level = level;
      Experience = experience;
   }

   public static LevelProgress CreateInitial() => 
      Create(0, 0);

   public static LevelProgress Create(uint level, uint experience) =>
      new(level, experience);
}