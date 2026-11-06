using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;
using Bk.APG.Business.Models;
using Microsoft.EntityFrameworkCore;

namespace Bk.APG.Infrastructure.DataSource.Extensions;

internal static class CollationExtensions
{
    private static readonly MethodInfo CollateMethod = typeof(RelationalDbFunctionsExtensions)
        .GetMethods(BindingFlags.Public | BindingFlags.Static)
        .Single(m => m.Name == nameof(RelationalDbFunctionsExtensions.Collate) && m.GetParameters().Length == 3)
        .MakeGenericMethod(typeof(string));

    public static string ToIcuCollation(this CultureInfo cultureInfo) => cultureInfo.TwoLetterISOLanguageName switch
    {
        Language.German => "de-x-icu",
        Language.French => "fr-x-icu",
        Language.Italian => "it-x-icu",
        _ => "und-x-icu"
    };

    public static Expression<Func<T, string>> WithCollation<T>(this Expression<Func<T, string>> property, string collation)
    {
        ArgumentNullException.ThrowIfNull(property);
        ArgumentNullException.ThrowIfNull(collation);

        var body = Expression.Call(
            CollateMethod,
            Expression.Constant(EF.Functions, typeof(DbFunctions)),
            property.Body,
            Expression.Constant(collation));

        return Expression.Lambda<Func<T, string>>(body, property.Parameters);
    }
}
