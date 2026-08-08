using InventoryShop.Domain.Specifications;
using InventoryShop.Tests.Common;
using Xunit;

public class OrdersCompletedByPlayerSpecificationTests
{
   [Fact]
   internal void ToExpression_WithMatchingBuyerId_ReturnsTrue()
   {
      var buyerId = Guid.NewGuid();
      var order = ShopOrderEntityFixture.Create(buyerId: buyerId);

      SpecificationTestHelper.AssertMatches(
         new OrdersCompletedByPlayerSpecification(buyerId), order);
   }

   [Fact]
   internal void ToExpression_WithDifferentBuyerId_ReturnsFalse()
   {
      var order = ShopOrderEntityFixture.Create(buyerId: Guid.NewGuid());

      SpecificationTestHelper.AssertNotMatch(
         new OrdersCompletedByPlayerSpecification(Guid.NewGuid()), order);
   }
}

public class OrdersCreatedByPlayerSpecificationTests
{
   [Fact]
   internal void ToExpression_WithMatchingSellerId_ReturnsTrue()
   {
      var sellerId = Guid.NewGuid();
      var order = ShopOrderEntityFixture.Create(sellerId: sellerId);

      SpecificationTestHelper.AssertMatches(
         new OrdersCreatedByPlayerSpecification(sellerId), order);
   }

   [Fact]
   internal void ToExpression_WithDifferentSellerId_ReturnsFalse()
   {
      var order = ShopOrderEntityFixture.Create(sellerId: Guid.NewGuid());

      SpecificationTestHelper.AssertNotMatch(
         new OrdersCreatedByPlayerSpecification(Guid.NewGuid()), order);
   }

   [Fact]
   internal void ToExpression_WithNullSellerId_ReturnsFalse()
   {
      var order = ShopOrderEntityFixture.Create(sellerId: null);

      SpecificationTestHelper.AssertNotMatch(
         new OrdersCreatedByPlayerSpecification(Guid.NewGuid()), order);
   }
}