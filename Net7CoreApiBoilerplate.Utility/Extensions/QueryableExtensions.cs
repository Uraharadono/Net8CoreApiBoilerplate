using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Net7CoreApiBoilerplate.Utility.Extensions
{
    public static class QueryableExtensions
    {
        // It is no longer possible to use this kind of extension method on IQueriable in Ef Core 3.0
        // More info: https://stackoverflow.com/questions/59494591/net-core-3-invalidoperationexception-on-orderby-with-dynamic-field-name
        // And here: https://github.com/dotnet/efcore/issues/19091
        public static IQueryable<T> Sort<T>(this IQueryable<T> query,
            string sortField, bool isAscending)
        {
            if (isAscending)
                return query.OrderBy(s => s.GetType()
                    .GetProperty(sortField));
            return query.OrderByDescending(s => s.GetType()
                .GetProperty(sortField));
        }

        // Taken from: https://github.com/dotnet/efcore/issues/19087#issuecomment-561834607
        public static IOrderedQueryable<TSource> OrderBy<TSource>(this IQueryable<TSource> query, string propertyName)
        {
            var entityType = typeof(TSource);

            // Create x=>x.PropName
            var propertyInfo = entityType.GetProperty(propertyName);
            if (propertyInfo.DeclaringType != entityType)
            {
                propertyInfo = propertyInfo.DeclaringType.GetProperty(propertyName);
            }

            // If we try to order by a property that does not exist in the object return the list
            if (propertyInfo == null)
            {
                return (IOrderedQueryable<TSource>)query;
            }

            var arg = Expression.Parameter(entityType, "x");
            var property = Expression.MakeMemberAccess(arg, propertyInfo);
            var selector = Expression.Lambda(property, new ParameterExpression[] { arg });

            // Get System.Linq.Queryable.OrderBy() method.
            var method = typeof(Queryable).GetMethods()
                .Where(m => m.Name == "OrderBy" && m.IsGenericMethodDefinition)
                .Where(m => m.GetParameters().ToList().Count == 2) // ensure selecting the right overload
                .Single();

            //The linq's OrderBy<TSource, TKey> has two generic types, which provided here
            MethodInfo genericMethod = method.MakeGenericMethod(entityType, propertyInfo.PropertyType);

            /* Call query.OrderBy(selector), with query and selector: x=> x.PropName
              Note that we pass the selector as Expression to the method and we don't compile it.
              By doing so EF can extract "order by" columns and generate SQL for it. */
            return (IOrderedQueryable<TSource>)genericMethod.Invoke(genericMethod, new object[] { query, selector });
        }


        // EfCore issues:
        // Well, this is not supported for now in EF Core. As seen here it is waiting approval: https://github.com/dotnet/efcore/issues/12088
        // Worth Read with list of EF core issues: https://github.com/dotnet/efcore/issues/17068 and https://github.com/dotnet/efcore/issues/17068#issuecomment-586464350
        // Taken from: https://stackoverflow.com/a/40573585/4267429
        public static IQueryable<TSource> DistinctBy<TSource, TKey>(this IQueryable<TSource> source, Expression<Func<TSource, TKey>> keySelector)
        {
            return source.GroupBy(keySelector).Select(x => x.FirstOrDefault());
            // return source.GroupBy(keySelector).Select(x => x.First());
        }
    }
}
