using System;
using System.Linq.Expressions;

namespace ArticleService.Extensions
{
    public static class LinqExtensions
    {
        public static Expression<Func<T, bool>> OrEx<T>(this Expression<Func<T, bool>> expr)
        {
            return Expression.Lambda<Func<T, bool>>(Expression.Or(expr.Body, expr.Parameters[0]));
        }
    }
}
