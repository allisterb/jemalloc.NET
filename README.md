# jemalloc.NET: A native memory manager for .NET

![jembench](https://lh3.googleusercontent.com/9zFHRdddwBezYJGb2jgMGHT3lgDTFmBAcJ_s8NgOdmAF1nz1-sF-0p9ZMOjeFVc-HAJHMRyLNmO02aHjWL8F9JWlqPHmiypdcmDhSx8SK8unzENOE7sG7ZCEOZLvI5nSTk_H8DpKoQ=w958-h521-no)
jemalloc.NET is a .NET API over the [jemalloc](http://jemalloc.net/) native memory allocator and provides .NET applications with efficient data structures backed by native memory for large scale in-memory computation scenarios. jemalloc is "a general purpose malloc(3) implementation that emphasizes fragmentation avoidance and scalable concurrency support" that is [widely used](https://github.com/jemalloc/jemalloc/wiki/Background#adoption) in the industry, particularly in applications that must [scale and utilize](http://highscalability.com/blog/2015/3/17/in-memory-computing-at-aerospike-scale-when-to-choose-and-ho.html) large amounts of memory. In addition to its fragmentation and concurrency optimizations, jemalloc provides an array of developer options for debugging, monitoring and tuning allocations that make it a great choice for use in developing memory-intensive applications.

The jemalloc.NET project provides:
* A low-level .NET API over the native jemalloc API functions like je_malloc, je_calloc, je_free, je_mallctl...
* A safety-focused high-level .NET API providing data structures like arrays backed by native memory allocated using jemalloc together with management features like reference counting.
* A benchmark CLI program: `jembench` which uses the excellent [BenchmarkDotNet](http://benchmarkdotnet.org/index.htm) library for easy and accurate benchmarking operations on native data structures vs managed objects using different parameters.

Data structures provided by the high-level API are more efficient than managed .NET arrays and objects at the scale of millions of elements, and memory allocation is much more resistant to fragmentation, while still providing necessary safety features like array bounds checking. Large .NET arrays must be allocated on the Large Object Heap and are not relocatable which leads to fragmentation and lower performance. For example in the following `jembench` benchmark on my laptop, creating and filling a `UInt64[]` managed array of size 10000000 and 100000000 is more than 2x slower than using an equivalent native array provided by jemalloc.NET:

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
|                                                                          Method | Parameter |       Mean |     Error |    StdDev |     Median |    Gen 0 |    Gen 1 |    Gen 2 |   Allocated |
|-------------------------------------------------------------------------------- |---------- |-----------:|----------:|----------:|-----------:|---------:|---------:|---------:|------------:|
|                                     **'Fill a managed array with a single value.'** |  **10000000** |   **9.059 ms** | **0.1745 ms** | **0.4777 ms** |   **8.913 ms** |        **-** |        **-** |        **-** |       **208 B** |
|            'Fill a SafeArray on the system unmanaged heap with a single value.' |  10000000 |   8.715 ms | 0.1682 ms | 0.2466 ms |   8.623 ms |        - |        - |        - |       208 B |
|                          'Create and Fill a managed array with a single value.' |  10000000 |  32.867 ms | 0.9156 ms | 1.3420 ms |  32.175 ms | 142.8571 | 142.8571 | 142.8571 |  80000769 B |
| 'Create and Fill a SafeArray on the system unmanaged heap with a single value.' |  10000000 |  13.809 ms | 0.2679 ms | 0.2506 ms |  13.727 ms |        - |        - |        - |       192 B |
|                                     **'Fill a managed array with a single value.'** | **100000000** |  **90.326 ms** | **1.7718 ms** | **2.4253 ms** |  **89.468 ms** |        **-** |        **-** |        **-** |       **208 B** |
|            'Fill a SafeArray on the system unmanaged heap with a single value.' | 100000000 |  88.377 ms | 0.9775 ms | 0.8665 ms |  88.505 ms |        - |        - |        - |       208 B |
|                          'Create and Fill a managed array with a single value.' | 100000000 | 310.880 ms | 5.9732 ms | 8.1762 ms | 306.952 ms | 125.0000 | 125.0000 | 125.0000 | 800000624 B |
| 'Create and Fill a SafeArray on the system unmanaged heap with a single value.' | 100000000 | 137.288 ms | 0.9710 ms | 0.9083 ms | 137.111 ms |        - |        - |        - |       192 B |


You can run this benchmark with the command `jembench array --fill 10000000 100000000 -l -u`. In this case we see that using the managed array of size 10 million elements allocated  800 MB on the managed heap while using the native array did not cause any allocations on the managed heap for the array data. Avoiding the managed heap for very large but simple data structures like arrays is a key optimizarion for apps that do large-scale in-memory computation.


Managed .NET arays are also limited to `Int32` indexing and a maximum size of about 2.15 billion elements. jemalloc.NET provides huge arrays through the `HugeArray<T>` class which allows you to access all available memory as a flat contiguous buffer using array semantics. In the next benchmark `jembench hugearray --fill -i 4200000000`:

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


an `Int32[]` of maximum size can be allocated and filled in 3.2s. This array consumes 8.6GB on the managed heap. But a jemalloc.NET `HugeArray<Int32>` of nearly double the size at 4.2 billion elements can be allocated in only 4 s and again consumes no memory on the managed heap. The only limit on the size of a `HugeArray<T>` is the available system memory.

Perhaps the killer feature of the [recently introduced](https://blogs.msdn.microsoft.com/dotnet/2017/11/15/welcome-to-c-7-2-and-span/) `Span<T>` class in .NET is its ability to efficently zero-copy re-interpret numeric data structures (`Int32, Int64` and their siblings) into other structures like the `Vector<T>` SIMD-enabled data types introduced in 2016. `Vector<T>` types are special in that the .NET RyuJIT JIT compiler can compile operations on Vectors to use SIMD instructions like SSE, SSE2, and AVX for parallelizing operations on data on a single CPU core.

Using the SIMD-enabled `SafeBuffer<T>.VectoryMultiply(n)` method provided by the jemalloc.NET API yields a 4.5x speedup for a simple in-place multiplication of a `Uint16[]` array of 1 million elements, compared to the unoptimized linear approach, allowing the operation to complete in 3.3 ms:

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


For huge arrays of `UInt16[]` we see similar speedups:
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


For a huge array with 4.1 billion `UInt16` values it takes 12 seconds to do a SIMD-enabled multiplication operation on all the elements of the array. This is still 3x the performance of doing the same non-vectorized operation on a managed array of half the size.

Inside a .NET application, jemalloc.NET native arrays and data structures can be straightforwardly accessed by native libraries without the need to make additional copies or allocations. The goal of the jemalloc.NET project is to make accessible to .NET the kind of big-data in-memory numeric, scientific and other computing that typically would require coding in a low=level language like C/C++ or assembler.

## Installation
### Requirements
Currently only runs on 64bit Windows; support for Linux 64bit and other 64bit platforms supported by .NET Core will be added
soon. 

#### Windows
* The latest [.NET Core 2.0 x64 runtime](https://www.microsoft.com/net/download/thank-you/dotnet-runtime-2.0.3-windows-x64-installer)
* The latest version of the [Microsoft Visual C++ Redistributable for Visual Studio 2017](https://go.microsoft.com/fwlink/?LinkId=746572) 

### Steps
Grab the latest release from the [releases](https://github.com/allisterb/jemalloc.NET/releases) page and unzip to a folder. Type `jembench` to run the benchmark CLI program and you should see the program version and options printed. NuGet packagees can be found in x64\Release. The API library assembly files themselves are in x64\Release\netstandard2.0

Note that if using jemalloc.NET in your own projects you must put the native jemallocd.dll library somewhere where it can be  located by the .NET runtime. You can create a post-build step to copy it to the output folder of your project or put it somewhere on your %PATH%. 

## Building from source
Currently build instuctions are only provided for Visual Studio 2017 on Windows but instructions for building on Linux will also be provided. jemalloc.NET is a 64-bit library only.
### Requirements
[Visual Studio 2017 15.5](https://www.visualstudio.com/en-us/news/releasenotes/vs2017-relnotes#15.5.1) with at least the following components:
* C# 7.2 compiler
* .NET Core 2.0 SDK x64
* MSVC 2017 compiler toolset v141 or higher
* Windows 10 SDK for Desktop C++ version 10.0.10.15603 or higher. Note that if you only have higher versions installed you will need to retarget the jemalloc MSVC project to your SDK version from Visual Studio. 

Per the instructions for building the native jemalloc library for Windows, you will also need Cygwin (32- or 64-bit )with the following packages:
   * autoconf
   * autogen
   * gcc
   * gawk
   * grep
   * sed

Cygwin tools aren't actually used for compiling jemalloc but for generating the header files. jemalloc on Windows is built using MSVC.

### Steps
0. You must add the [.NET Core](https://dotnet.myget.org/gallery/dotnet-core) NuGet [feed](https://dotnet.myget.org/F/dotnet-core/api/v3/index.json) on MyGet and also the [CoreFxLab](https://dotnet.myget.org/gallery/dotnet-corefxlab) [feed](https://dotnet.myget.org/F/dotnet-core/api/v3/index.json) to your NuGet package sources. You can do this in Visual Studio 2017 from Tools->Options->NuGet Package Manager menu item.
1. Clone the project: `git clone https://github.com/alllisterb/jemalloc.NET` and init the submodules: `git submodule update --init --recursive`
2. Open a x64 Native Tools Command Prompt for VS 2017 and temporarily add `Cygwin\bin` to the PATH e.g `set PATH=%PATH%;C:\cygwin\bin`. Switch to the `jemalloc` subdirectory in your jemalloc.NET solution dir and run `sh -c "CC=cl ./autogen.sh"`. This will generate some files in the `jemalloc` subdirectory and only needs to be done once.
4. From a Visual Studio 2017 Developer Command prompt run `build.cmd`. Alternatively you can load the solution in Visual Studio and using the "Benchmark" solution configuration build the entire solution. 
5. The solution should build without errors.
6. Run `jembench` from the solution folder to see the project version and help.

## Usage

### jembench CLI
Examples:
* `jembench hugearray -l -u --math --cold-start -t 3 4096000000` Benchmark math operations on `HugeArray<UInt64>` arrays of size 4096000000 without benchmark warmup and only using 3 iterations of the target methods. Benchmarks on huge arrays can be lengthy so you should carefully choose the benchmark parameters affecting how long you want the benchmark to run,
