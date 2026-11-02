using System;
using System.Collections.Generic;
using System.Linq;

namespace Core.Reports;

public static class EnumerableExtensions
{
    public static IEnumerable<T> TopBy<T, TKey, TThenByKey>(
        this IEnumerable<T> source,
        Func<T, TKey> keySelector,
        Func<T, TThenByKey> thenBySelector,
        int count,
        IComparer<TKey>? comparer = null,
        IComparer<TThenByKey>? thenByComparer = null)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(keySelector);
        ArgumentNullException.ThrowIfNull(thenBySelector);

        return source.OrderByDescending(keySelector, comparer)
                     .ThenBy(thenBySelector, thenByComparer)
                     .Take(Math.Max(0, count));
    }
}