using System;
using System.Collections.Generic;
using System.Text;

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
    }
}
