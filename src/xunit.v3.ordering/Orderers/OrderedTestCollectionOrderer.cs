using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xunit.Sdk;
using Xunit.v3;

namespace Xunit.v3.Extensions.Ordering;

/// <summary>
/// Orders test collections by <see cref="OrderAttribute"/> on their collection definition class.
/// Collections without the attribute receive a default order of 0.
/// Ties are broken by display name for deterministic results.
/// </summary>
public sealed class OrderedTestCollectionOrderer : ITestCollectionOrderer
{
    public IReadOnlyCollection<TTestCollection> OrderTestCollections<TTestCollection>(
        IReadOnlyCollection<TTestCollection> testCollections)
        where TTestCollection : notnull, ITestCollection
    {
        return testCollections
            .OrderBy(tc => GetOrder(tc))
            .ThenBy(tc => tc.TestCollectionDisplayName, StringComparer.Ordinal)
            .ToArray();
    }

    private static int GetOrder<TTestCollection>(TTestCollection testCollection)
        where TTestCollection : ITestCollection
    {
        if (testCollection is not IXunitTestCollection xunitCollection)
            return 0;

        var definitionType = xunitCollection.CollectionDefinition;
        if (definitionType is null)
            return 0;

        var attribute = definitionType.GetCustomAttribute<OrderAttribute>();
        return attribute?.Value ?? 0;
    }
}