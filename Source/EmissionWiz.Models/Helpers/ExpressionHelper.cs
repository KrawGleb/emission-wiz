using System;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace EmissionWiz.Models.Helpers;

public static class ExpressionHelper
{
    public static string GetFieldName<T, T1>(Expression<Func<T, T1>> fieldExpression)
    {
        if (ReferenceEquals(fieldExpression, null))
            throw new ArgumentNullException(nameof(fieldExpression));

        var memberExpression = fieldExpression.Body as MemberExpression;
        if (ReferenceEquals(memberExpression, null))
        {
            var unaryExpression = fieldExpression.Body as UnaryExpression;
            if (ReferenceEquals(unaryExpression, null))
                throw new InvalidProgramException("fieldExpression is invalid expression");

            memberExpression = unaryExpression.Operand as MemberExpression;
            if (ReferenceEquals(memberExpression, null))
                throw new InvalidProgramException("fieldExpression is invalid expression");
        }

        return memberExpression.Member.Name;
    }

    public static string GetPropertyDisplayName<T, T1>(Expression<Func<T, T1>> fieldExpression)
    {
        var propertyName = GetFieldName(fieldExpression);

        var propertyInfo = typeof(T).GetProperty(propertyName);
        if (propertyInfo == null)
            throw new InvalidProgramException($"Can't find property {propertyName} in type {typeof(T).FullName}");

        var displayNameAttr = propertyInfo.GetCustomAttribute<DisplayNameAttribute>();
        return displayNameAttr == null ? propertyName : displayNameAttr.DisplayName;
    }

    public static IQueryable<IGrouping<TProperty, TEntity>> GroupByField<TEntity, TProperty>(
        this IQueryable<TEntity> source,
        string fieldName)
        where TEntity : class
    {
        var parameter = Expression.Parameter(typeof(TEntity), "x");
        var propertyAccess = Expression.Property(parameter, fieldName);
        var lambda = Expression.Lambda<Func<TEntity, TProperty>>(propertyAccess, parameter);

        var result = source.GroupBy(lambda);
        return result;
    }
}