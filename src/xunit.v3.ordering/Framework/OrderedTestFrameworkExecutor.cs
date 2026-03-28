using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit.Sdk;
using Xunit.v3;

namespace Xunit.v3.Extensions.Ordering;

/// <summary>
/// Test framework executor that uses <see cref="OrderedTestAssemblyRunner"/>
/// for class ordering support.
/// </summary>
public class OrderedTestFrameworkExecutor : XunitTestFrameworkExecutor
{
    public OrderedTestFrameworkExecutor(IXunitTestAssembly testAssembly)
        : base(testAssembly)
    { }

    public override async ValueTask RunTestCases(
        IReadOnlyCollection<IXunitTestCase> testCases,
        IMessageSink executionMessageSink,
        ITestFrameworkExecutionOptions executionOptions,
        CancellationToken cancellationToken)
    {
        await OrderedTestAssemblyRunner.Instance.Run(
            TestAssembly,
            testCases,
            executionMessageSink,
            executionOptions,
            cancellationToken);
    }
}
