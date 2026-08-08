using FluentAssertions;
using InventoryShop.Domain.Shared.Specifications;

internal static class SpecificationTestHelper
{
   internal static void AssertMatches<T>(Specification<T> spec, T entity)
   {
      var predicate = spec.ToExpression().Compile();
      predicate(entity).Should().BeTrue();
   }
   
   internal static void AssertNotMatch<T>(Specification<T> spec, T entity)
   {
      var predicate = spec.ToExpression().Compile();
      predicate(entity).Should().BeFalse();
   }
}