using InventoryShop.Domain.Specifications;
using InventoryShop.Tests.Common;
using Xunit;

public class SlotCreatedBySpecificationTests
{
   [Fact]
   internal void ToExpression_MatchesSlotsWithGivenSellerId()
   {
      var sellerId = Guid.NewGuid();
      var matchingSlot = ShopSlotEntityFixture.CreateSlot(sellerId: sellerId);
      var notMatchingSlot = ShopSlotEntityFixture.CreateSlot(sellerId: Guid.NewGuid());
      var spec = new SlotCreatedBySpecification(sellerId);
      
      SpecificationTestHelper.AssertMatches(spec, matchingSlot);
      SpecificationTestHelper.AssertNotMatch(spec, notMatchingSlot);
   }

   [Fact]
   internal void ToExpression_WithNullSellerId_MatchesOnlySystemCreatedSlots()
   {
      var systemSlot = ShopSlotEntityFixture.CreateSlot(sellerId: null);
      var spec = new SlotCreatedBySpecification(null);

      SpecificationTestHelper.AssertMatches(spec, systemSlot);
   }
}