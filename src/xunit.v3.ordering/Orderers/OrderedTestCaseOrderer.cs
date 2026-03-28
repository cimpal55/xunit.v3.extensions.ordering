using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xunit.Sdk;
using Xunit.v3;

namespace Xunit.v3.Extensions.Ordering;

/// <summary>
/// Orders test cases within a class by <see cref="OrderAttribute"/> value.
/// Tests without the attribute receive a default order of 0.
/// Ties are broken by method name for deterministic results.
/// </summary>
public sealed class OrderedTestCaseOrderer : ITestCaseOrderer
{
    /// <inheritdoc/>
    public IReadOnlyCollection<TTestCase> OrderTestCases<TTestCase>(
        IReadOnlyCollection<TTestCase> testCases)
        where TTestCase : notnull, ITestCase
    {
        return testCases
            .OrderBy(tc => GetOrder(tc))
            .ThenBy(tc => tc.TestCaseDisplayName, StringComparer.Ordinal)
            .ToArray();
    }

    private static int GetOrder<TTestCase>(TTestCase testCase)
        where TTestCase : ITestCase
    {
        if (testCase is not IXunitTestCase xunitTestCase)
            return 0;

        var method = xunitTestCase.TestMethod?.Method;
        if (method is null)
            return 0;

        var attribute = method.GetCustomAttribute<OrderAttribute>();
        return attribute?.Value ?? 0;
    }
}