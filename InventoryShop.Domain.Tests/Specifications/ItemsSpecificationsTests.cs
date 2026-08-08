using InventoryShop.Domain.Specifications;
using InventoryShop.Tests.Common;
using Xunit;

public class ItemsOwnedByPlayerSpecificationTests
{
   [Fact]
   internal void ToExpression_WithMatchingOwnerId_ReturnsTrue()
   {
      var ownerId = Guid.NewGuid();
      var item = ItemEntityFixture.Create(ownerId: ownerId);

      SpecificationTestHelper.AssertMatches(
         new ItemsOwnedByPlayerSpecification(ownerId), item);
   }

   [Fact]
   internal void ToExpression_WithDifferentOwnerId_ReturnsFalse()
   {
      var item = ItemEntityFixture.Create(ownerId: Guid.NewGuid());

      SpecificationTestHelper.AssertNotMatch(
         new ItemsOwnedByPlayerSpecification(Guid.NewGuid()), item);
   }
}

public class ItemsCreatedByPlayerSpecificationTests
{
   [Fact]
   internal void ToExpression_WithMatchingCreatorId_ReturnsTrue()
   {
      var creatorId = Guid.NewGuid();
      var item = ItemEntityFixture.Create(creatorId: creatorId);

      SpecificationTestHelper.AssertMatches(
         new ItemsCreatedByPlayerSpecification(creatorId), item);
   }

   [Fact]
   internal void ToExpression_WithDifferentCreatorId_ReturnsFalse()
   {
      var item = ItemEntityFixture.Create(creatorId: Guid.NewGuid());

      SpecificationTestHelper.AssertNotMatch(
         new ItemsCreatedByPlayerSpecification(Guid.NewGuid()), item);
   }
}

public class ItemsEquippedByPlayerSpecificationTests
{
   [Fact]
   internal void ToExpression_WithMatchingOwnerAndEquipped_ReturnsTrue()
   {
      var ownerId = Guid.NewGuid();
      var item = ItemEntityFixture.Create(ownerId: ownerId, isEquipped: true);

      SpecificationTestHelper.AssertMatches(
         new ItemsEquippedByPlayerSpecification(ownerId), item);
   }

   [Fact]
   internal void ToExpression_WithMatchingOwnerButNotEquipped_ReturnsFalse()
   {
      var ownerId = Guid.NewGuid();
      var item = ItemEntityFixture.Create(ownerId: ownerId, isEquipped: false);

      SpecificationTestHelper.AssertNotMatch(
         new ItemsEquippedByPlayerSpecification(ownerId), item);
   }

   [Fact]
   internal void ToExpression_WithEquippedButDifferentOwner_ReturnsFalse()
   {
      var item = ItemEntityFixture.Create(ownerId: Guid.NewGuid(), isEquipped: true);

      SpecificationTestHelper.AssertNotMatch(
         new ItemsEquippedByPlayerSpecification(Guid.NewGuid()), item);
   }
}

public class ItemsOnSaleByPlayerSpecificationTests
{
   [Fact]
   internal void ToExpression_WithMatchingOwnerAndOnSale_ReturnsTrue()
   {
      var ownerId = Guid.NewGuid();
      var item = ItemEntityFixture.Create(ownerId: ownerId, isOnSale: true);

      SpecificationTestHelper.AssertMatches(
         new ItemsOnSaleByPlayerSpecification(ownerId), item);
   }

   [Fact]
   internal void ToExpression_WithMatchingOwnerButNotOnSale_ReturnsFalse()
   {
      var ownerId = Guid.NewGuid();
      var item = ItemEntityFixture.Create(ownerId: ownerId, isOnSale: false);

      SpecificationTestHelper.AssertNotMatch(
         new ItemsOnSaleByPlayerSpecification(ownerId), item);
   }
}