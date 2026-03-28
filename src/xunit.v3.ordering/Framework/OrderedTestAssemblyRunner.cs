using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit.Sdk;
using Xunit.v3;

namespace Xunit.v3.Extensions.Ordering;

/// <summary>
/// An assembly runner that uses <see cref="OrderedTestCollectionRunner"/>
/// to support test class ordering within collections.
/// </summary>
public class OrderedTestAssemblyRunner :
    XunitTestAssemblyRunnerBase<XunitTestAssemblyRunnerContext, IXunitTestAssembly, IXunitTestCollection, IXunitTestCase>
{
    protected OrderedTestAssemblyRunner() { }

    public static OrderedTestAssemblyRunner Instance { get; } = new();

    /// <summary>
    /// Delegates to <see cref="OrderedTestCollectionRunner"/> instead of the default runner.
    /// </summary>
    protected override ValueTask<RunSummary> RunTestCollection(
        XunitTestAssemblyRunnerContext ctxt,
        IXunitTestCollection testCollection,
        IReadOnlyCollection<IXunitTestCase> testCases)
    {
        var testCaseOrderer = ctxt.AssemblyTestCaseOrderer ?? DefaultTestCaseOrderer.Instance;

        return OrderedTestCollectionRunner.Instance.Run(
            testCollection,
            testCases,
            ctxt.ExplicitOption,
            ctxt.MessageBus,
            testCaseOrderer,
            ctxt.Aggregator.Clone(),
            ctxt.CancellationTokenSource,
            ctxt.AssemblyFixtureMappings);
    }

    public async ValueTask<RunSummary> Run(
        IXunitTestAssembly testAssembly,
        IReadOnlyCollection<IXunitTestCase> testCases,
        IMessageSink executionMessageSink,
        ITestFrameworkExecutionOptions executionOptions,
        CancellationToken cancellationToken)
    {
        var ctxt = new XunitTestAssemblyRunnerContext(
            testAssembly,
            testCases,
            executionMessageSink,
            executionOptions,
            cancellationToken);

        await ctxt.InitializeAsync();
        return await Run(ctxt);
    }
}
