using FluentAssertions;
using InventoryShop.Domain.Exceptions;
using InventoryShop.Domain.ValueObjects;
using Xunit;

public sealed class LevelProgressTests
{
   [Fact]
   internal void CreateInitial_ShouldStartAtLevel0()
   {
      var progress = LevelProgress.CreateInitial();

      progress.Level.Should().Be(0);
      progress.Experience.Should().Be(0);
   }
   
   [Fact]
   internal void Create_WithInvalidLevel_ReturnsDomainException()
   {
      var act = () => LevelProgress.Create(LevelProgress.MaxLevel + 1, 0);
      
      act.Should().Throw<ViolatedLevelPolicyException>();
   }
   
   [Fact]
   internal void Create_WithMaxLvlAndExtraExperience_ReturnsMaxLevelWithoutExperience()
   {
      var progress = LevelProgress.Create(LevelProgress.MaxLevel, 1);

      progress.Level.Should().Be(LevelProgress.MaxLevel);
      progress.ExperienceForNextLevel.Should().Be(0);
      progress.Experience.Should().Be(0);
   }
   
   [Fact]
   internal void AddExperience_BelowThreshold_ShouldNotLevelUp()
   {
      var progress = LevelProgress.CreateInitial();

      progress = progress.AddExperience(9);

      progress.Level.Should().Be(0);
      progress.Experience.Should().Be(9);
   }
   
   [Fact]
   internal void AddExperience_ShouldCarryRemainingExperience()
   {
      var progress = LevelProgress.CreateInitial();

      progress = progress.AddExperience(15);

      progress.Level.Should().Be(1);
      progress.Experience.Should().Be(5);
   }
   
   [Fact]
   internal void AddExperienceAboveMax_ShouldSetLevelOnMax()
   {
      var progress = LevelProgress.CreateInitial(); 

      progress = progress.AddExperience(1000);

      progress.Level.Should().Be(LevelProgress.MaxLevel);
      progress.Experience.Should().Be(0);
      progress.ExperienceForNextLevel.Should().Be(0);
   }
}