using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;

using Xunit;

using jemalloc.Buffers;
namespace jemalloc.Tests
{
    public class VectorTests
    {
        [Fact(DisplayName = "Buffer elements can be accessed as vector.")]
        public void CanConstructJArray()
        {
            NativeMemory<float> buffer = new NativeMemory<float>(3);
            Span<float> span = buffer.Span;
            span[0] = 1f;
            NativeMemoryVectors<float> vectorsBuffer = buffer.AsVectors();
            Span<Vector<float>> vectors = vectorsBuffer.Span;
        }

    }
}
