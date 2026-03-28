[![Build](https://github.com/cimpal55/xunit.v3.extensions.ordering/actions/workflows/ci.yml/badge.svg)](https://github.com/cimpal55/xunit.v3.extensions.ordering/actions/workflows/ci.yml)
[![NuGet](https://img.shields.io/nuget/v/xunit.v3.extensions.ordering.svg)](https://www.nuget.org/packages/xunit.v3.extensions.ordering)
[![License](https://img.shields.io/github/license/cimpal55/xunit.v3.extensions.ordering)](LICENSE)

# xunit.v3.extensions.ordering

Simple ordering extensions for [xUnit.net v3](https://xunit.net/).

This package lets you control the execution order of test methods and test collections using a `[Order]` attribute.

Inspired by [Xunit.Extensions.Ordering](https://github.com/tomaszeman/Xunit.Extensions.Ordering), but implemented specifically for xUnit v3.

## Installation

```bash
dotnet add package xunit.v3.extensions.ordering
```

## Usage

### Register orderers

Register once in your test project (for example in `TestSetup.cs`):

```csharp
using Xunit;
using Xunit.v3;
using Xunit.v3.Extensions.Ordering;

[assembly: CollectionBehavior(DisableTestParallelization = true)]
[assembly: TestCaseOrderer(typeof(OrderedTestCaseOrderer))]
[assembly: TestCollectionOrderer(typeof(OrderedTestCollectionOrderer))]
```

> If collection order matters, parallelization should be disabled. Otherwise xUnit may run collections in parallel and ignore ordering.

### Order test methods

```csharp
using Xunit;
using Xunit.v3.Extensions.Ordering;

public class CheckoutTests
{
    [Fact, Order(1)]
    public void AddItemToCart() { }

    [Fact, Order(2)]
    public void ApplyDiscount() { }

    [Fact, Order(3)]
    public void CompletePayment() { }
}
```

Runs in order:

```
AddItemToCart → ApplyDiscount → CompletePayment
```

### Order test collections

Apply `[Order]` to collection definitions:

```csharp
[CollectionDefinition("Setup")]
[Order(1)]
public class SetupCollection;

[CollectionDefinition("Tests")]
[Order(2)]
public class TestsCollection;

[CollectionDefinition("Cleanup")]
[Order(3)]
public class CleanupCollection;
```

Then assign test classes:

```csharp
[Collection("Setup")]
public class DatabaseSetupTests
{
    [Fact, Order(1)]
    public void CreateSchema() { }

    [Fact, Order(2)]
    public void SeedData() { }
}

[Collection("Tests")]
public class BusinessLogicTests
{
    [Fact, Order(1)]
    public void ValidateOrder() { }
}

[Collection("Cleanup")]
public class TeardownTests
{
    [Fact, Order(1)]
    public void DropDatabase() { }
}
```

Execution order:

```
Setup → Tests → Cleanup
```

## Behavior

* ordering is ascending (`Order(1)` runs before `Order(2)`)
* negative values are allowed
* tests without `[Order]` default to `0`
* same order values are resolved by name (to keep execution deterministic)

## Limitations

* ordering test classes inside a collection is not supported (xUnit v3 does not expose a public API for this)
* this package does not replace the test framework or runner

## Migration notes

If you used `Xunit.Extensions.Ordering` (v2):

* `[Order]` works the same
* registration now uses `typeof(...)` instead of strings
* assembly fixtures are handled natively by xUnit v3

| Before (v2) | After (v3) |
|---|---|
| `[Order(1)]` on methods/classes | `[Order(1)]` — same |
| `[assembly: TestCaseOrderer("...", "...")]` | `[assembly: TestCaseOrderer(typeof(OrderedTestCaseOrderer))]` |
| `[assembly: TestCollectionOrderer("...", "...")]` | `[assembly: TestCollectionOrderer(typeof(OrderedTestCollectionOrderer))]` |
| Custom assembly fixtures | Use xUnit v3 native assembly fixtures |

## License

MIT

## Links

- [GitHub Repository](https://github.com/cimpal55/xunit.v3.extensions.ordering)
- [NuGet Package](https://www.nuget.org/packages/xunit.v3.extensions.ordering) *(coming soon)*
