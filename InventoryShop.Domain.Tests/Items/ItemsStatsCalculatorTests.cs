using InventoryShop.Domain.Services;
using InventoryShop.Domain.ValueObjects;
using Xunit;

public sealed class ItemsStatsCalculatorTests
{
   [Fact]
   internal void Calculate_ShouldReturnSummedStats()
   {
      var calculator = new ItemsStatsCalculator();
      var stats = new[] { Stats.Create(1, 1, 1), Stats.Create(1, -1, -2) };
      
      var result = calculator.Calculate(stats);
      
      Assert.Equal(2, result.Agility);
      Assert.Equal(0, result.Strength);
      Assert.Equal(-1, result.Intelligence);
   }
}