using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Xunit.Sdk;
using Xunit.v3;

namespace Xunit.v3.Extensions.Ordering;

/// <summary>
/// A collection runner that orders test classes within a collection
/// by <see cref="OrderAttribute"/> value before executing them.
/// Classes without the attribute receive a default order of 0.
/// Ties are broken by class name for deterministic results.
/// </summary>
public class OrderedTestCollectionRunner :
    XunitTestCollectionRunnerBase<XunitTestCollectionRunnerContext, IXunitTestCollection, IXunitTestClass, IXunitTestCase>
{
    protected OrderedTestCollectionRunner() { }

    public static OrderedTestCollectionRunner Instance { get; } = new();

    protected override ValueTask<RunSummary> RunTestClass(
        XunitTestCollectionRunnerContext ctxt,
        IXunitTestClass? testClass,
        IReadOnlyCollection<IXunitTestCase> testCases)
    {
        if (testClass is null)
            return new(XunitRunnerHelper.FailTestCases(
                ctxt.MessageBus,
                ctxt.CancellationTokenSource,
                testCases,
                "Test class is null"));

        return XunitTestClassRunner.Instance.Run(
            testClass,
            testCases,
            ctxt.ExplicitOption,
            ctxt.MessageBus,
            ctxt.TestCaseOrderer,
            ctxt.Aggregator.Clone(),
            ctxt.CancellationTokenSource,
            ctxt.CollectionFixtureMappings);
    }

    /// <summary>
    /// Overrides default class ordering to sort by <see cref="OrderAttribute"/>.
    /// </summary>
    protected override async ValueTask<RunSummary> RunTestClasses(
        XunitTestCollectionRunnerContext ctxt,
        Exception? exception)
    {
        var summary = new RunSummary();

        var classGroups = ctxt.TestCases
            .GroupBy(tc => tc.TestClass, TestClassComparer.Instance)
            .Select(group => new
            {
                TestClass = group.Key as IXunitTestClass,
                TestCases = group.ToArray() as IReadOnlyCollection<IXunitTestCase>,
                Order = GetClassOrder(group)
            })
            .OrderBy(g => g.Order)
            .ThenBy(g => g.TestClass?.TestClassName ?? string.Empty, StringComparer.Ordinal);

        foreach (var group in classGroups)
        {
            if (exception is not null)
                summary.Aggregate(await FailTestClass(ctxt, group.TestClass, group.TestCases, exception));
            else
                summary.Aggregate(await RunTestClass(ctxt, group.TestClass, group.TestCases));

            if (ctxt.CancellationTokenSource.IsCancellationRequested)
                break;
        }

        return summary;
    }

    private static int GetClassOrder(IEnumerable<IXunitTestCase> testCases)
    {
        var firstCase = testCases.FirstOrDefault();
        var method = (firstCase as IXunitTestCase)?.TestMethod?.Method;
        if (method is null)
            return 0;

        var classType = method.DeclaringType;
        if (classType is null)
            return 0;

        var attribute = classType.GetCustomAttribute<OrderAttribute>();
        return attribute?.Value ?? 0;
    }

    public async ValueTask<RunSummary> Run(
        IXunitTestCollection testCollection,
        IReadOnlyCollection<IXunitTestCase> testCases,
        ExplicitOption explicitOption,
        IMessageBus messageBus,
        ITestCaseOrderer testCaseOrderer,
        ExceptionAggregator aggregator,
        CancellationTokenSource cancellationTokenSource,
        FixtureMappingManager assemblyFixtureMappings)
    {
        var ctxt = new XunitTestCollectionRunnerContext(
            testCollection,
            testCases,
            explicitOption,
            messageBus,
            testCaseOrderer,
            aggregator,
            cancellationTokenSource,
            assemblyFixtureMappings);

        await ctxt.InitializeAsync();
        return await Run(ctxt);
    }
}
