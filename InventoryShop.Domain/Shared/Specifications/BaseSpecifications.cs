using System.Linq.Expressions;

namespace InventoryShop.Domain.Shared.Specifications;

public abstract class Specification<T>
{
   public abstract Expression<Func<T , bool>> ToExpression();
 
   public bool IsSatisfiedBy(T entity)
   {
      var predicate = ToExpression().Compile();
      return predicate(entity);
   }
   
   public Specification<T> And(Specification<T> other) =>
      new AndSpecification<T>(this, other);
   
   public Specification<T> Or(Specification<T> other) =>
      new OrSpecification<T>(this, other);
   
   public Specification<T> Not(Specification<T> other) =>
      new NotSpecification<T>(this, other);
   
   public Specification<T> OrNot(Specification<T> other) =>
      new OrNotSpecification<T>(this, other);
}

public class AndSpecification<T>(Specification<T> left, Specification<T> right) : Specification<T>
{
   public override Expression<Func<T , bool>> ToExpression()
   {
      var leftExpression = left.ToExpression();
      var rightExpression = right.ToExpression();

      ParameterExpression paramExpr = Expression.Parameter(typeof(T));
      BinaryExpression exprBody = Expression.AndAlso(leftExpression.Body, rightExpression.Body);
      exprBody = (BinaryExpression)new ParameterReplacer(paramExpr).Visit(exprBody);
      var finalExpr = Expression.Lambda<Func<T, bool>>(exprBody, paramExpr);
      
      return Expression.Lambda<Func<T , bool>>(
         finalExpr, leftExpression.Parameters.Single());
   }
}

public class OrSpecification<T>(Specification<T> left, Specification<T> right) : Specification<T>
{
   public override Expression<Func<T , bool>> ToExpression()
   {
      var leftExpression = left.ToExpression();
      var rightExpression = right.ToExpression();
 
      ParameterExpression paramExpr = Expression.Parameter(typeof(T));
      BinaryExpression exprBody = Expression.Or(leftExpression.Body, rightExpression.Body);
      exprBody = (BinaryExpression)new ParameterReplacer(paramExpr).Visit(exprBody);
      var finalExpr = Expression.Lambda<Func<T, bool>>(exprBody, paramExpr);

      return Expression.Lambda<Func<T , bool>>(
         finalExpr, leftExpression.Parameters.Single());
   }
}

public class NotSpecification<T>(Specification<T> left, Specification<T> right) : Specification<T>
{
   public override Expression<Func<T , bool>> ToExpression()
   {
      var leftExpression = left.ToExpression();
      var rightExpression = right.ToExpression();
      
      ParameterExpression paramExpr = Expression.Parameter(typeof(T));
      BinaryExpression exprBody = Expression.NotEqual(leftExpression.Body, rightExpression.Body);
      exprBody = (BinaryExpression)new ParameterReplacer(paramExpr).Visit(exprBody);
      var finalExpr = Expression.Lambda<Func<T, bool>>(exprBody, paramExpr);

      return Expression.Lambda<Func<T , bool>>(
         finalExpr, leftExpression.Parameters.Single());
   }
}

public class OrNotSpecification<T>(Specification<T> left, Specification<T> right) : Specification<T>
{
   public override Expression<Func<T , bool>> ToExpression()
   {
      var leftExpression = left.ToExpression();
      var rightExpression = right.ToExpression();

      ParameterExpression paramExpr = Expression.Parameter(typeof(T));
      BinaryExpression exprBody = Expression.OrElse(leftExpression.Body, rightExpression.Body);
      exprBody = (BinaryExpression)new ParameterReplacer(paramExpr).Visit(exprBody);
      var finalExpr = Expression.Lambda<Func<T, bool>>(exprBody, paramExpr);

      return Expression.Lambda<Func<T , bool>>(
         finalExpr, leftExpression.Parameters.Single());
   }
}



