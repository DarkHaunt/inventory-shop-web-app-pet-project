using FluentAssertions;
using InventoryShop.Domain.ValueObjects;
using Xunit;

public sealed class StatsTests
{
   [Fact]
   internal void CreateInitial_ShouldCreateEmptyStats()
   {
      var stats = Stats.CreateInitial();

      stats.Agility.Should().Be(0);
      stats.Strength.Should().Be(0);
      stats.Intelligence.Should().Be(0);
   }

   [Fact]
   internal void Add_ShouldAddAllStats()
   {
      var stats = Stats.Create(1, 1, 1);

      var result = stats.Add(Stats.Create(1, -1, -2));

      result.Agility.Should().Be(2);
      result.Strength.Should().Be(0);
      result.Intelligence.Should().Be(-1);
   }

   [Fact]
   internal void Multiply_ShouldMultiplyAllStats()
   {
      var stats = Stats.Create(1, 1, 1);

      var result = stats.Multiply(2d);

      result.Agility.Should().Be(2);
      result.Strength.Should().Be(2);
      result.Intelligence.Should().Be(2);
   }

   [Fact]
   internal void Multiply_ShouldRoundResult()
   {
      var stats = Stats.Create(1, 1, 1);

      var result = stats.Multiply(1.5d);

      result.Agility.Should().Be(2);
      result.Strength.Should().Be(2);
      result.Intelligence.Should().Be(2);
   }

   [Fact]
   internal void Multiply_ByZero_ShouldReturnEmptyStats()
   {
      var stats = Stats.Create(1, 1, 1);

      var result = stats.Multiply(0d);

      result.Agility.Should().Be(0);
      result.Strength.Should().Be(0);
      result.Intelligence.Should().Be(0);
   }
}