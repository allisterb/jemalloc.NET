## Catgeories

1. Malloc: Benchmark low-level malloc operations and creating Span<Y>

2. Arrays: Bench


1. Allocate uint arrays of various sizes from small to very large. Benchmark how long it takes
2. Generate random values and assign to random indices of the array. Benchmark.
3. Allocate memory and use span

Things to test:

Fragmentation: allocate small and large sizes consecutively.

LOH stress:
https://www.red-gate.com/simple-talk/dotnet/net-framework/the-dangers-of-the-large-object-heap/


    Create an Int32[100000000]; //goes on the LOH
    Create an Int32[30000]; //smaller but still big enough to go on the LOH
    Throw away the big array but keep the small array. //Creates a 100000000 hole on the LOH immediately followed by an in-use region for the small array.
    next iteration will create another big array slightly bigger the first. //This won't fit in the 'hole' of the previous array. The heap must be extended now for the new array to fit.
    Loop for a while.

We should find it taking longer and longer to allocate the large arrays and using lots of memory until eventually an OOM happens.

We can do the exact same operations with SafeArray<Int32> and then compare the performance.