using Xunit;
using Xunit.v3.Extensions.Ordering;

namespace Xunit.v3.Ordering.Tests;

/// <summary>
/// Shared execution log to verify class ordering within a single collection.
/// </summary>
public static class ClassExecutionLog
{
    public static List<string> Log { get; } = [];
}

[CollectionDefinition("ClassOrdering")]
public class ClassOrderingDefinition;

/// <summary>
/// This class is declared first in the file but has [Order(3)] — should run LAST.
/// </summary>
[Collection("ClassOrdering")]
[Order(3)]
public class ThirdClass
{
    [Fact, Order(1)]
    public void RunsThird()
    {
        ClassExecutionLog.Log.Add("ThirdClass");

        // By this point, both FirstClass and SecondClass should have run
        Assert.Contains("FirstClass", ClassExecutionLog.Log);
        Assert.Contains("SecondClass", ClassExecutionLog.Log);
        Assert.Equal(3, ClassExecutionLog.Log.Count);
    }
}

/// <summary>
/// This class has [Order(1)] — should run FIRST despite file order.
/// </summary>
[Collection("ClassOrdering")]
[Order(1)]
public class FirstClass
{
    [Fact, Order(1)]
    public void RunsFirst()
    {
        ClassExecutionLog.Log.Add("FirstClass");

        // Should be the very first entry
        Assert.Single(ClassExecutionLog.Log);
    }
}

/// <summary>
/// This class has [Order(2)] — should run SECOND.
/// </summary>
[Collection("ClassOrdering")]
[Order(2)]
public class SecondClass
{
    [Fact, Order(1)]
    public void RunsSecond()
    {
        ClassExecutionLog.Log.Add("SecondClass");

        // FirstClass should already be logged
        Assert.Contains("FirstClass", ClassExecutionLog.Log);
        Assert.Equal(2, ClassExecutionLog.Log.Count);
    }
}
