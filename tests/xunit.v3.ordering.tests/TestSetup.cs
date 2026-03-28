using Xunit;
using Xunit.v3.Extensions.Ordering;

[assembly: CollectionBehavior(DisableTestParallelization = true)]
[assembly: TestCaseOrderer(typeof(OrderedTestCaseOrderer))]
[assembly: TestCollectionOrderer(typeof(OrderedTestCollectionOrderer))]