using System.Linq.Expressions;

namespace InventoryShop.Domain.Shared.Specifications;

internal class ParameterReplacer(ParameterExpression parameter) : ExpressionVisitor
{
   protected override Expression VisitParameter(ParameterExpression node) {
      if (parameter.Type != node.Type) {
         return node;
      }

      return base.VisitParameter(parameter);
   }
}