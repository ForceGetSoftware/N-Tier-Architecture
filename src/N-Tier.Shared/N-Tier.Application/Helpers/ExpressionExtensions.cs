using System.Linq.Expressions;

namespace N_Tier.Shared.N_Tier.Application.Helpers
{
    public static class ExpressionExtensions
    {
        public static Expression<Func<T, bool>> AndAlso<T>(this Expression<Func<T, bool>> expr1, Expression<Func<T, bool>> expr2)
        {
            var parameter = Expression.Parameter(typeof(T), "x");

            var left = ReplaceParameter(expr1.Body, expr1.Parameters[0], parameter);
            var right = ReplaceParameter(expr2.Body, expr2.Parameters[0], parameter);

            var combined = Expression.AndAlso(left, right);

            return Expression.Lambda<Func<T, bool>>(combined, parameter);
        }

        private static Expression ReplaceParameter(Expression body, ParameterExpression oldParam, ParameterExpression newParam)
        {
            return new ParameterReplacer(oldParam, newParam).Visit(body);
        }

        private class ParameterReplacer(ParameterExpression oldParam, ParameterExpression newParam) : ExpressionVisitor
        {
            private readonly ParameterExpression _oldParam = oldParam;
            private readonly ParameterExpression _newParam = newParam;

            protected override Expression VisitParameter(ParameterExpression node)
            {
                return node == _oldParam ? _newParam : base.VisitParameter(node);
            }
        }
    }

}
