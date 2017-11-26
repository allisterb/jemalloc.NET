using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

using Xunit;

using jemalloc.Buffers;
namespace jemalloc.Tests
{
    public class JArrayTests
    {
        [Fact(DisplayName = "JArrays can be constructed.")]
        public void CanConstructJArray()
        {
            JArray<int> a = new JArray<int>(1000);
            var floats = a.Span().Non

        }

    }
}
