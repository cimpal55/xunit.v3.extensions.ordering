using System.Reflection;
using Xunit.Sdk;
using Xunit.v3;

namespace Xunit.v3.Extensions.Ordering;

/// <summary>
/// Custom test framework that enables test class ordering within collections
/// via the <see cref="OrderAttribute"/>.
/// <para>
/// Register in your test project with:
/// <code>[assembly: TestFramework(typeof(OrderedTestFramework))]</code>
/// </para>
/// </summary>
public class OrderedTestFramework : XunitTestFramework
{
    protected override ITestFrameworkExecutor CreateExecutor(Assembly assembly)
    {
        var testAssembly = new XunitTestAssembly(assembly);
        return new OrderedTestFrameworkExecutor(testAssembly);
    }
}
