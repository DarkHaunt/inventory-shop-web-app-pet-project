using InventoryShop.Domain.ValueObjects;

namespace InventoryShop.Domain.Services;

public sealed class MinigameRewardCalculator(LevelCalculator levelCalculator, SimpleRandomPrimitiveProvider rng)
{
   private const double AdditionalPercentFor_Strength = 0.075d;
   private const double AdditionalPercentFor_Agility = 0.06d;
   private const double AdditionalPercentFor_Intelligence = 0.085d;
   
   private static readonly Wallet BaseReward = new(100);
   
   public (LevelProgress newLevel, Wallet reward) CalculateReward(LevelProgress oldLevelProgress, params Stats[] statsForCalculation)
   {
      var statsMultiplier = 1.0d;
      
      foreach (Stats stats in statsForCalculation)
         statsMultiplier += CalculateAdditionalPercentFrom(stats);
      
      return (CalculateNewLevel(oldLevelProgress, statsMultiplier), CalculateNewWallet(statsMultiplier));
   }
   
   private LevelProgress CalculateNewLevel(LevelProgress oldLevelProgress, double statsMultiplier)
   {
      var experienceRaw = double.Lerp(1.0d, 2d, rng.GetRandomDouble());
      var experience = (uint)Math.Round(experienceRaw * statsMultiplier);
      
      return levelCalculator.CalculateNewLevelProgress(oldLevelProgress, experience);
   }
   
   private Wallet CalculateNewWallet(double statsMultiplier)
   {
      var goldMultiplier = double.Lerp(1.0d, 1.4d, rng.GetRandomDouble());
      return BaseReward.Multiply(goldMultiplier * statsMultiplier);
   }

   private static double CalculateAdditionalPercentFrom(Stats stats)
   {
      return Math.Max(stats.Agility * AdditionalPercentFor_Agility, 0) +
             Math.Max(stats.Strength * AdditionalPercentFor_Strength, 0) +
             Math.Max(stats.Intelligence * AdditionalPercentFor_Intelligence, 0);
   }
}