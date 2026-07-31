using FluentAssertions;
using InventoryShop.Domain.Services;
using InventoryShop.Domain.ValueObjects;
using Moq;
using Xunit;

namespace InventoryShop.Domain.Tests;

public sealed class MinigameRewardCalculatorTests
{
   private MinigameRewardCalculator CreateValid()
   {
      var mock = new Mock<ISimpleRandomPrimitiveProvider>();
      mock.Setup(x => x.GetRandomDouble()).Returns(1);
      
      return new MinigameRewardCalculator(mock.Object);
   }
   
   [Fact]
   internal void CalculateReward_ReturnsCorrectValues()
   {
      MinigameRewardCalculator minigameRewardCalculator = CreateValid();
      
      var stats = new Stats[] {new(1,1,1), new(1,2,3)};
      (LevelProgress newLevel, Wallet reward) = minigameRewardCalculator.CalculateReward(LevelProgress.Create(2, 0), stats);
      
      newLevel.Level.Should().Be(2);
      newLevel.Experience.Should().Be(3);
      reward.GoldAmount.Should().Be(236);
   }
}