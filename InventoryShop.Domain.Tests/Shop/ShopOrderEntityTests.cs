using FluentAssertions;
using InventoryShop.Domain.Entities;
using InventoryShop.Domain.Exceptions;
using InventoryShop.Domain.ValueObjects;
using InventoryShop.Tests.Common;
using Xunit;

public sealed class ShopOrderEntityTests
{
   [Fact]
   internal void Create_WithSameSellerAndBuyer_ReturnsViolationException()
   {
      var sellerAndBuyerId = Guid.NewGuid();
      var itemSnap = ItemEntityFixture.Create().Snapshot();
      var ord = new OrderData(itemSnap, Wallet.CreateInitial(), 1);
      
      var act = () => ShopOrderEntity.Create(Guid.NewGuid(), sellerAndBuyerId, sellerAndBuyerId, ord, DateTime.UtcNow);
      
      act.Should().Throw<ViolatedShopOrderPolicyException>();
   }
}