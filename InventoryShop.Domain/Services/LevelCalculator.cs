using InventoryShop.Domain.ValueObjects;

namespace InventoryShop.Domain.Services;

public sealed class LevelCalculator
{
   public LevelProgress CalculateNewLevelProgress(LevelProgress oldLevel, uint experience)
   {
      var experienceForNextLevel = oldLevel.ExperienceForNextLevel;
      var newExperience = experience + oldLevel.Experience;
      
      if (newExperience >= experienceForNextLevel)
         return LevelProgress.Create(oldLevel.Level + 1, newExperience - experienceForNextLevel);
      
      return LevelProgress.Create(oldLevel.Level, newExperience);
   }
}