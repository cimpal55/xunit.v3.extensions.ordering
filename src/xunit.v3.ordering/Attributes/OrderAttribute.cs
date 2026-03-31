using System;

namespace Xunit;

/// <summary>
/// Specifies the execution order for a test method or test class.
/// Lower values run first. Default is 0 when not specified.
/// </summary>
[AttributeUsage(
    AttributeTargets.Class | AttributeTargets.Method,
    AllowMultiple = false,
    Inherited = true)]
public sealed class OrderAttribute : Attribute
{
    public OrderAttribute(int value)
    {
        Value = value;
    }

    /// <summary>
    /// The order value. Tests are executed in ascending order.
    /// </summary>
    public int Value { get; }
}