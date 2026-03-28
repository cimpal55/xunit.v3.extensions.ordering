[![Build](https://github.com/cimpal55/xunit.v3.extensions.ordering/actions/workflows/ci.yml/badge.svg)](https://github.com/cimpal55/xunit.v3.extensions.ordering/actions/workflows/ci.yml)
[![NuGet](https://img.shields.io/nuget/v/xunit.v3.extensions.ordering.svg)](https://www.nuget.org/packages/xunit.v3.extensions.ordering)
[![License](https://img.shields.io/github/license/cimpal55/xunit.v3.extensions.ordering)](LICENSE)

# xunit.v3.extensions.ordering

Ordering extensions for [xUnit.net v3](https://xunit.net/).

This package lets you control the execution order of **test methods**, **test classes**, and **test collections** using a single `[Order]` attribute.

Inspired by [Xunit.Extensions.Ordering](https://github.com/tomaszeman/Xunit.Extensions.Ordering), but built specifically for xUnit v3.

## Installation

```bash
dotnet add package xunit.v3.extensions.ordering
```

## Setup

Register once in your test project (for example in `TestSetup.cs`):

```csharp
using Xunit;
using Xunit.v3.Extensions.Ordering;

[assembly: TestFramework(typeof(OrderedTestFramework))]
[assembly: CollectionBehavior(DisableTestParallelization = true)]
[assembly: TestCaseOrderer(typeof(OrderedTestCaseOrderer))]
[assembly: TestCollectionOrderer(typeof(OrderedTestCollectionOrderer))]
```

> **Note:** `OrderedTestFramework` replaces xUnit's default execution pipeline to enable class ordering. If you only need method and collection ordering, you can omit the `TestFramework` line.

> If collection or class order matters, parallelization should be disabled. Otherwise xUnit may run collections in parallel and ignore ordering.

## Usage

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
AddItemToCart -> ApplyDiscount -> CompletePayment
```

### Order test classes within a collection

Apply `[Order]` to test classes that share a collection:

```csharp
[CollectionDefinition("Integration")]
public class IntegrationDefinition;

[Collection("Integration")]
[Order(1)]
public class SetupTests
{
    [Fact, Order(1)]
    public void CreateSchema() { }
}

[Collection("Integration")]
[Order(2)]
public class BusinessLogicTests
{
    [Fact, Order(1)]
    public void ValidateOrder() { }
}

[Collection("Integration")]
[Order(3)]
public class CleanupTests
{
    [Fact, Order(1)]
    public void DropTempData() { }
}
```

Execution order:

```
SetupTests -> BusinessLogicTests -> CleanupTests
```

> **Requires** `[assembly: TestFramework(typeof(OrderedTestFramework))]` to be registered.

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

## Behavior

* ordering is ascending (`Order(1)` runs before `Order(2)`)
* negative values are allowed
* tests without `[Order]` default to `0`
* same order values are resolved by name (deterministic)
* `[Order]` works on methods, classes, and collection definitions

## How it works

The package provides:

| Component | Purpose |
|---|---|
| `[Order(n)]` | Attribute for methods, classes, and collection definitions |
| `OrderedTestCaseOrderer` | Orders test methods within a class |
| `OrderedTestCollectionOrderer` | Orders collections by `[Order]` on their definition |
| `OrderedTestFramework` | Custom test framework that enables class ordering |

The `OrderedTestFramework` replaces xUnit's default execution pipeline with a custom chain:

```
OrderedTestFramework -> OrderedTestFrameworkExecutor -> OrderedTestAssemblyRunner -> OrderedTestCollectionRunner
```

The `OrderedTestCollectionRunner` overrides `RunTestClasses()` to sort test classes by their `[Order]` attribute before execution.

## Migration notes

If you used `Xunit.Extensions.Ordering` (v2):

* `[Order]` works the same way
* registration now uses `typeof(...)` instead of strings
* class ordering requires `[assembly: TestFramework(typeof(OrderedTestFramework))]`
* assembly fixtures are handled natively by xUnit v3

| Before (v2) | After (v3) |
|---|---|
| `[Order(1)]` on methods/classes | `[Order(1)]` — same |
| `[assembly: TestCaseOrderer("...", "...")]` | `[assembly: TestCaseOrderer(typeof(OrderedTestCaseOrderer))]` |
| `[assembly: TestCollectionOrderer("...", "...")]` | `[assembly: TestCollectionOrderer(typeof(OrderedTestCollectionOrderer))]` |
| `[assembly: TestFramework("...", "...")]` | `[assembly: TestFramework(typeof(OrderedTestFramework))]` |
| Custom assembly fixtures | Use xUnit v3 native assembly fixtures |

## License

MIT

## Links

- [GitHub Repository](https://github.com/cimpal55/xunit.v3.extensions.ordering)
- [NuGet Package](https://www.nuget.org/packages/xunit.v3.extensions.ordering)
