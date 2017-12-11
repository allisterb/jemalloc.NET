using System;
using System.Collections.Generic;
using System.Numerics;

using Xunit;

namespace jemalloc.Tests
{
    public class HugeArrayTests : jemallocTest
    {
        Random rng = new Random();

        [Fact(DisplayName = "Can construct huge array")]
        public void CanConstructHugeArray()
        {
            ulong arraySize = 2L * Int32.MaxValue;
            ulong point = 1L * Int32.MaxValue;
            HugeArray<int> array = new HugeArray<int>(arraySize);
            array[point] = 1;
            Assert.Equal(1, array[point]);
        }

        [Fact(DisplayName = "Can correctly assign to HugeArray elements")]
        public void CanAssignToHugeArrayElements()
        {
            ulong arraySize = 2L * Int32.MaxValue;
            HugeArray<int> array = new HugeArray<int>(arraySize);
            ulong[] indices = new ulong[10000];
            int scale = 0, value = 0;
            ulong v = 0;
            for (int i = 0; i < indices.Length; i++)
            {
                scale = rng.Next(1, 2);
                value = rng.Next(0, Int32.MaxValue - 1);
                v = (ulong)scale * (ulong)value;
                array[v] = i;
                indices[i] = v;
            }
            for (int i = 0; i < indices.Length; i++)
            {
                Assert.Equal(i, array[indices[i]]);
            }
        }

        [Fact(DisplayName = "Can convert to Vector")]
        public void CanConvertToVector()
        {
            HugeArray<uint> a = new HugeArray<uint>(1, 11, 94, 5, 0, 0, 0, 8);
            Vector<uint> v = a.ToVector();
            Assert.Equal(a[0], v[0]);
            Assert.Equal(a[3], v[3]);
            Assert.Equal(a[7], v[7]);
            HugeArray<uint> a2 = new HugeArray<uint>(11, 112, 594, 65, 0, 0, 0, 8, 14, 90, 2, 8);
            Vector<uint> v2 = a2.ToVector(2);
            Assert.Equal(594u, v2[0]);
        }
    }
}
