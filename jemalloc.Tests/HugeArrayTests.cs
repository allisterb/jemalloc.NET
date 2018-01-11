using System;
using System.Collections.Generic;
using System.Linq;
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
            array.Close();
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
                v = indices[0];
                while (indices.Contains(v))
                {
                    scale = rng.Next(1, 2);
                    value = rng.Next(0, Int32.MaxValue - 1);
                    v = (ulong)scale * (ulong)value;
                }
                array[v] = i;
                indices[i] = v;
            }
            for (int i = 0; i < indices.Length; i++)
            {
                Assert.Equal(i, array[indices[i]]);
            }
            array.Close();
        }

        [Fact(DisplayName = "Can convert to Vector")]
        public void CanConvertToVector()
        {
            HugeArray<uint> a = new HugeArray<uint>(8, 1, 11, 94, 5, 0, 0, 0, 8);
            Vector<uint> v = a.GetAsSingleVector();
            Assert.Equal(a[0], v[0]);
            Assert.Equal(a[3], v[3]);
            Assert.Equal(a[7], v[7]);
            HugeArray<uint> a2 = new HugeArray<uint>(12, 11, 112, 594, 65, 0, 0, 0, 8, 14, 90, 2, 8);
            Vector<uint> v2 = a2.GetSliceAsSingleVector(0);
            Assert.Equal(11u, v2[0]);
            Assert.Equal(8u, v2[7]);
            HugeArray<uint> a3 = new HugeArray<uint>((ulong)Int32.MaxValue + 10000);
            a3.Fill(7u);
            a3[(ulong)Int32.MaxValue + 100] = 9;
            a3[(ulong)Int32.MaxValue + 101] = 4;
            Vector<uint> v3 = a3.GetSliceAsSingleVector((ulong)Int32.MaxValue + 99);
            Assert.Equal(9u, v3[1]);
            Assert.Equal(4u, v3[2]);
            Assert.Equal(a3[(ulong)Int32.MaxValue + 99], v3[0]);
            Assert.Equal(7u, v3[0]);
            Assert.Equal(7u, v3[7]);
            a.Close();
            a2.Close();
            a3.Close();
        }

        [Fact(DisplayName = "Can correctly fill")]
        public void CanFill()
        {
            HugeArray<int> array = new HugeArray<int>(1000);
            array.Fill(33);
            Assert.Equal(33, array[999]);
            array.Close();
        }
    }
}
