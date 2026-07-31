using FluentAssertions;
using InventoryShop.Domain.Entities;
using InventoryShop.Domain.Exceptions;
using InventoryShop.Domain.ValueObjects;
using Moq;
using Xunit;

namespace InventoryShop.Domain.Tests;

public sealed class ShopOrderEntityTests
{
   [Fact]
   internal void Create_WithSameSellerAndBuyer_ReturnsViolationException()
   {
      var sellerAndBuyerId = Guid.NewGuid();
      var ord = new Mock<OrderData>();
      
      var act = () => ShopOrderEntity.Create(Guid.NewGuid(), sellerAndBuyerId, sellerAndBuyerId, ord.Object, DateTime.UtcNow);
      
      act.Should().Throw<ViolatedShopOrderPolicyException>();
   }
}