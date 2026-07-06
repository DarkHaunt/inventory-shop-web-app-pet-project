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
      
      if(level == MaxLevel && experience > 0)
         throw new ViolatedLevelPolicyException($"Experience cannot be greater than 0 when level is max");
      
      Level = level;
      Experience = experience;
   }

   public LevelProgress AddExperience(uint addExperience) =>
      Create(Level, Experience + addExperience);

   private LevelProgress Upgrade() =>
      CreateRaw(Level + 1, Experience - ExperienceForNextLevel);

   public static LevelProgress Create(uint level, uint experience)
   {
      LevelProgress l = CreateRaw(level, experience);

      while (l.Level < MaxLevel && l.Experience >= l.ExperienceForNextLevel)
         l = l.Upgrade();

      return l.Level >= MaxLevel ? CreateMax() : l;
   }

   public static LevelProgress CreateInitial() => 
      CreateRaw(0, 0);

   public static LevelProgress CreateMax() => 
      CreateRaw(MaxLevel, 0);

   private static LevelProgress CreateRaw(uint level, uint experience) =>
      new(level, experience);
}