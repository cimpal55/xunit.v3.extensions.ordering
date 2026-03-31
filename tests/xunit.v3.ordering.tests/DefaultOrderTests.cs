using Xunit;

namespace Xunit.v3.Ordering.Tests;

public class DefaultOrderTests
{
    [Fact, Order(-1)]
    public void NegativeOrderRunsFirst()
    {
        Assert.True(true);
    }

    [Fact]
    public void NoAttributeDefaultsToZero()
    {
        Assert.True(true);
    }

    [Fact, Order(1)]
    public void PositiveOrderRunsLast()
    {
        Assert.True(true);
    }
}