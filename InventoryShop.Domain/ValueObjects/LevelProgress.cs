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
      Level >= MaxLevel 
         ? 0 
         : (uint)Math.Round(BaseExperience * Math.Pow(ExperienceMultiplier, Level));

   private LevelProgress(uint level, uint experience)
   {
      if (level > MaxLevel)
         throw new ViolatedLevelPolicyException($"Level must be lower than max {MaxLevel}");

      if (level == MaxLevel && experience > 0)
         experience = 0;
      
      Level = level;
      Experience = experience;
   }

   public LevelProgress AddExperience(uint addExperience) =>
      Create(Level, Experience + addExperience);

   private LevelProgress Upgrade()
   {
      uint experience = Experience - ExperienceForNextLevel;
      return new(Level + 1, experience);
   }

   public static LevelProgress Create(uint level, uint experience)
   {
      LevelProgress l = new(level, experience);

      while (l.Level < MaxLevel && l.Experience >= l.ExperienceForNextLevel)
         l = l.Upgrade();

      return l.Level >= MaxLevel ? CreateMax() : l;
   }

   public static LevelProgress CreateInitial() => 
      new(0, 0);

   public static LevelProgress CreateMax() => 
      new(MaxLevel, 0);
}