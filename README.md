# jemalloc.NET: A native memory manager for .NET
jemalloc.NET is a .NET interface to the jemalloc memory allocator
A low-level .NET API over the native jemalloc API functions like je_malloc, je
A high-level .NET API providing safety-aware data structures like arrays backed by native memory allocated using jemalloc

Huge arrays. Managed .NET arays are limited to eleme. jemalloc provides HugeArray<T>. Huge arrays allow you to access available memory as a flat contiguous buffer using array semantics.