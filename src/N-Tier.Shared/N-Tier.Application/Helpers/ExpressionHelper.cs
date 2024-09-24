using System.Linq.Expressions;

namespace N_Tier.Application.Helpers;

public class ExpressionHelper
{
    public static Expression<Func<T1, bool>> ConvertFilter<T1, T2>(Expression<Func<T2, bool>> subFilter, string propertyName)
    {
        // Create a parameter for the new lambda expression
        ParameterExpression parameter = Expression.Parameter(typeof(T1), subFilter.Parameters[0].Name);

        // Access the DbObject property
        Expression property = Expression.Property(parameter, propertyName);

        // Replace the parameter in subFilter with property
        var body = new ParameterReplacer(subFilter.Parameters[0], property).Visit(subFilter.Body);

        // Create the new lambda expression
        return Expression.Lambda<Func<T1, bool>>(body, parameter);
    }

    public class ParameterReplacer : ExpressionVisitor
    {
        private readonly ParameterExpression _oldParameter;
        private readonly Expression _newExpression;

        public ParameterReplacer(ParameterExpression oldParameter, Expression newExpression)
        {
            _oldParameter = oldParameter;
            _newExpression = newExpression;
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            return node == _oldParameter ? _newExpression : base.VisitParameter(node);
        }
    }
}
