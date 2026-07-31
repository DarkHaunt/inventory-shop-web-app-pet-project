using FluentAssertions;
using InventoryShop.Domain.Entities;
using InventoryShop.Domain.Exceptions;
using InventoryShop.Domain.ValueObjects;
using Xunit;

namespace InventoryShop.Domain.Tests;

public sealed class ShopSlotEntityTests
{
   [Fact]
   internal void Create_WithPriceAboveMax_ReturnsViolationException()
   {
      var act = () => ShopSlotEntity.Create(Guid.NewGuid(), Guid.NewGuid(), Wallet.Create(100_001), LevelProgress.CreateInitial(), null);
      
      act.Should().Throw<ViolatedShopSlotPolicyException>();
   }
}