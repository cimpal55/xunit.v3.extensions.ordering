using xunit.v3.ordering.tests;
using Xunit;
using Xunit.v3.Extensions.Ordering;

namespace Xunit.v3.Ordering.Tests;

public class MethodOrderingTests
{
    public static List<string> ExecutionLog { get; } = [];

    [Fact, Order(3)]
    public void Third()
    {
        ExecutionLog.Add(nameof(Third));
        Assert.Equal(3, ExecutionLog.Count);
    }

    [Fact, Order(1)]
    public void First()
    {
        ExecutionLog.Add(nameof(First));
        Assert.Single(ExecutionLog);
    }

    [Fact, Order(2)]
    public void Second()
    {
        ExecutionLog.Add(nameof(Second));
        Assert.Equal(2, ExecutionLog.Count);
    }
}