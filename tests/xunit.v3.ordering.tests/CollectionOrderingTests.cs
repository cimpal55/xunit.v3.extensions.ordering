using Xunit;
using Xunit.v3.Extensions.Ordering;

namespace Xunit.v3.Ordering.Tests;

/// <summary>
/// Shared execution log across collection tests.
/// </summary>
public static class CollectionExecutionLog
{
    public static List<string> Log { get; } = [];
}

[CollectionDefinition("StepOne")]
[Order(1)]
public class StepOneDefinition;

[CollectionDefinition("StepTwo")]
[Order(2)]
public class StepTwoDefinition;

[Collection("StepTwo")]
public class StepTwoTests
{
    [Fact, Order(1)]
    public void RunsAfterStepOne()
    {
        CollectionExecutionLog.Log.Add("StepTwo.First");

        Assert.Contains("StepOne.First", CollectionExecutionLog.Log);
    }
}

[Collection("StepOne")]
public class StepOneTests
{
    [Fact, Order(1)]
    public void RunsFirst()
    {
        CollectionExecutionLog.Log.Add("StepOne.First");

        Assert.DoesNotContain("StepTwo.First", CollectionExecutionLog.Log);
    }
}