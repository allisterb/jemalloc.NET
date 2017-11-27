using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

using Xunit;

using jemalloc.Buffers;
namespace jemalloc.Tests
{
    public class VectorTests
    {
        [Fact(DisplayName = "Buffer elements can be accessed as vector.")]
        public void CanConstructVectors()
        {
            NativeMemory<uint> memory = new NativeMemory<uint>(1, 11, 94, 5, 0, 0, 0, 8);      
            NativeMemory<Vector<uint>> v = memory.AsVector();
            Assert.True(v.Span[0][0] == 1);
            Assert.True(v.Span[0][1] == 11);
            Assert.True(v.Span[0][2] == 99994);
        }

    }
}
