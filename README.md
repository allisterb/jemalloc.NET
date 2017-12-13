# jemalloc.NET: A native memory manager for .NET

![jembench](https://lh3.googleusercontent.com/9zFHRdddwBezYJGb2jgMGHT3lgDTFmBAcJ_s8NgOdmAF1nz1-sF-0p9ZMOjeFVc-HAJHMRyLNmO02aHjWL8F9JWlqPHmiypdcmDhSx8SK8unzENOE7sG7ZCEOZLvI5nSTk_H8DpKoQ=w958-h521-no)
jemalloc.NET is a .NET API over the [jemalloc](http://jemalloc.net/) native memory allocator and provides .NET applications with efficient data structures backed by native memory for large scale in-memory computation scenarios. jemalloc is "a general purpose malloc(3) implementation that emphasizes fragmentation avoidance and scalable concurrency support" that is [widely used](http://highscalability.com/blog/2015/3/17/in-memory-computing-at-aerospike-scale-when-to-choose-and-ho.html) in the industry, particularly in applications that must [scale and utilize](http://highscalability.com/blog/2015/3/17/in-memory-computing-at-aerospike-scale-when-to-choose-and-ho.html) large amounts of memory. In addition to its fragmentation and concurrency optimizations, jemalloc provides an array of developer options for debugging, monitoring and tuning allocations that make it a great choice for use in developing memory-intensive applications.

The jemalloc.NET project provides:
* A low-level .NET API over the native jemalloc API functions like je_malloc, je_calloc, je_free, je_mallctl...
* A safety-focused high-level .NET API providing data structures like arrays backed by native memory allocated using jemalloc.
* A benchmark CLI program: `jembench` which uses the excellent [BenchmarkDotNet](http://benchmarkdotnet.org/index.htm) library for easy and accurate benchmarking operations on native data structures vs managed objects using different parameters.

Data structures provided by the high-level API are more efficient than managed .NET arrays and objects at the scale of millions of elements, and memory allocation is much more resistant to fragmentation. Large .NET arrays must be allocated on the Large Object Heap which leads to fragmentation and lower performance. For example in the following `jembench` benchmark on my laptop, filling a managed array of type UInt64[] of size 100 million is 2.6x slower than using an equivalent native array provided by jemalloc.NET:

``` ini

BenchmarkDotNet=v0.10.11, OS=Windows 10 Redstone 2 [1703, Creators Update] (10.0.15063.726)
Processor=Intel Core i7-6700HQ CPU 2.60GHz (Skylake), ProcessorCount=8
Frequency=2531251 Hz, Resolution=395.0616 ns, Timer=TSC
.NET Core SDK=2.1.2
  [Host] : .NET Core 2.0.3 (Framework 4.6.25815.02), 64bit RyuJIT

Job=JemBenchmark  Jit=RyuJit  Platform=X64  
Runtime=Core  AllowVeryLargeObjects=True  Toolchain=InProcessToolchain  

```
|                                                               Method | Parameter |     Mean |    Error |   StdDev |    Gen 0 |    Gen 1 |    Gen 2 |   Allocated |
|--------------------------------------------------------------------- |---------- |---------:|---------:|---------:|---------:|---------:|---------:|------------:|
|                          'Fill a managed array with a single value.' | 100000000 | 327.4 ms | 3.102 ms | 2.902 ms | 937.5000 | 937.5000 | 937.5000 | 800000192 B |
| 'Fill a SafeArray on the system unmanaged heap with a single value.' | 100000000 | 126.1 ms | 1.220 ms | 1.081 ms |        - |        - |        - |       264 B |

You can run this benchmark with the command `jembench array --fill -l -u 100000000`. In this case we see that using the managed array allocated  800 MB on the managed heap while using the native array did not cause any allocations on the managed heap for the array data. Avoiding the managed heap for very large but simple data structures like arrays is a key optimizarion for apps that do large-scale in-memory computations.

Perhaps the killer feature of the recently introduced `Span<T>` class in .NET is its ability to efficently re-interpret numeric data structures (`Int32, Int64` and their siblings) into other strucutres like the `Vector<T>` SIMD-enabled data types introduced in 2016. `Vector<T>` types are special in that the .NET RyuJIT JIT compiler can compile operations on Vectors to use SIMD instructions like SSE, SSE2, and AVX for parallelizing operations on data on a single CPU core.

Using the SIMD-enabled `SafeBuffer<T>.VectoryMultiply(n)` method provided by the jemalloc.NET API yields a 4.5x speedup for a simple in-place multiplication of a `Uint16[]` array of 1 million elements compared to the unoptimized linear approach, allowing the operation to complete in 3.3 ms:

``` ini

BenchmarkDotNet=v0.10.11, OS=Windows 10 Redstone 2 [1703, Creators Update] (10.0.15063.726)
Processor=Intel Core i7-6700HQ CPU 2.60GHz (Skylake), ProcessorCount=8
Frequency=2531251 Hz, Resolution=395.0616 ns, Timer=TSC
.NET Core SDK=2.1.2
  [Host] : .NET Core 2.0.3 (Framework 4.6.25815.02), 64bit RyuJIT

Job=JemBenchmark  Jit=RyuJit  Platform=X64  
Runtime=Core  AllowVeryLargeObjects=True  Toolchain=InProcessToolchain  
RunStrategy=Throughput  

```
|                                                              Method | Parameter |      Mean |     Error |    StdDev |     Gen 0 |  Allocated |
|-------------------------------------------------------------------- |---------- |----------:|----------:|----------:|----------:|-----------:|
|       'Multiply all values of a managed array with a single value.' |   1024000 | 15.861 ms | 0.3169 ms | 0.4231 ms | 7781.2500 | 24576000 B |
| 'Vector multiply all values of a native array with a single value.' |   1024000 |  3.299 ms | 0.0344 ms | 0.0287 ms |         - |       56 B |


Managed .NET arays are also limited to Int32 indexing and a maximum size of about 2.15 billion elements. jemalloc.NET provides huge arrays through the `HugeArray<T>` class which allows you to access all available memory as a flat contiguous buffer using array semantics. In the next benchmark `jembench hugearray --fill -i 4200000000`:

``` ini

BenchmarkDotNet=v0.10.11, OS=Windows 10 Redstone 2 [1703, Creators Update] (10.0.15063.726)
Processor=Intel Core i7-6700HQ CPU 2.60GHz (Skylake), ProcessorCount=8
Frequency=2531251 Hz, Resolution=395.0616 ns, Timer=TSC
.NET Core SDK=2.1.2
  [Host] : .NET Core 2.0.3 (Framework 4.6.25815.02), 64bit RyuJIT

Job=JemBenchmark  Jit=RyuJit  Platform=X64  
Runtime=Core  AllowVeryLargeObjects=True  Toolchain=InProcessToolchain  
RunStrategy=ColdStart  TargetCount=7  WarmupCount=-1  

```
|                                                                         Method |  Parameter |    Mean |    Error |   StdDev |    Allocated |
|------------------------------------------------------------------------------- |----------- |--------:|---------:|---------:|-------------:|
| 'Fill a managed array with the maximum size [2146435071] with a single value.' | 4200000000 | 3.177 s | 0.1390 s | 0.0617 s | 8585740456 B |
|           'Fill a HugeArray on the system unmanaged heap with a single value.' | 4200000000 | 4.029 s | 3.2233 s | 1.4312 s |          0 B |


an Int32[] array of maximum size can be allocated and filled in 3.2s. This array consumes 8.6GB on the managed heap. But a jemalloc.NET `HugeArray<Int32>` of nearly double the size at 4.2 billion elements can be allocated in only 4 s and again consumes no memory on the managed heap. The only limit on the size of a `HugeArray<T>` is the available system memory.

For huge arrays of `Int16[]` we see similar speedups:
``` ini

BenchmarkDotNet=v0.10.11, OS=Windows 10 Redstone 2 [1703, Creators Update] (10.0.15063.726)
Processor=Intel Core i7-6700HQ CPU 2.60GHz (Skylake), ProcessorCount=8
Frequency=2531251 Hz, Resolution=395.0616 ns, Timer=TSC
.NET Core SDK=2.1.2
  [Host] : .NET Core 2.0.3 (Framework 4.6.25815.02), 64bit RyuJIT

Job=JemBenchmark  Jit=RyuJit  Platform=X64  
Runtime=Core  AllowVeryLargeObjects=True  Toolchain=InProcessToolchain  
RunStrategy=ColdStart  TargetCount=1  

```
|                                                                                           Method |  Parameter |    Mean | Error |         Gen 0 |     Gen 1 |     Allocated |
|------------------------------------------------------------------------------------------------- |----------- |--------:|------:|--------------:|----------:|--------------:|
| 'Multiply all values of a managed array with the maximum size [2146435071] with a single value.' | 4096000000 | 34.25 s |    NA | 16375000.0000 | 3000.0000 | 51514441704 B |
|                              'Vector multiply all values of a native array with a single value.' | 4096000000 | 12.06 s |    NA |             - |         - |           0 B |


For a huge array with 4.1 billion `UInt16` values it takes 12 seconds to do a SIMD-enabled multiplication operation on all the elements of the array. This is still 3x the performance of doing the same non-vectorized operation on a managed array of hald the size
In a .NET application jemalloc.NET native arrays and data structures can be straightforwardly accessed by native libraries without the need to make additional copies. Buffer operations can be SIMD-vectorized which can make a significant performance difference for huge buffers with 10s of billions of values. 

The goal of the jemalloc.NET project is to make accessible to .NET the kind of big-data in-memory numeric, scientific and other computing that typically would require coding in a low=level language like C/C++ or assembler.



## Installation



## Usage

Currently build instuctions are only provided for Windows x64


## Building from source

### Requirements

[Visual Studio 2017 15.5](https://www.visualstudio.com/en-us/news/releasenotes/vs2017-relnotes#15.5.1)
with at least the following components:
* C# 7.2 compiler
* .NET Core 2.0 SDK
* VC++ 2017 compiler toolset v141 or higher
* Windows 10 SDK for Desktop C++ version 10.0.10.15603 or higher

### Steps
0. You must add the [.NET Core](https://dotnet.myget.org/gallery/dotnet-core) NuGet [feed](https://dotnet.myget.org/F/dotnet-core/api/v3/index.json) on MyGet and also the [CoreFxLab](https://dotnet.myget.org/gallery/dotnet-corefxlab) [feed](https://dotnet.myget.org/F/dotnet-core/api/v3/index.json) to your NuGet package sources. You can do this from Visual Studio 2017 from the Tools->Options->NuGet Package Manager menu item.
1. Clone the project: `git clone https://github.com/alllisterb/jemalloc.NET`
2. From a Visual Studio 2017 Developer Command prompt run `build.cmd`. 
3. The solution should build without errors.
4. Run `jembench` from the solution folder to see the project version and help.
