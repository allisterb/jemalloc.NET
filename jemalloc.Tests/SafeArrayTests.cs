using System;

using Xunit;
namespace jemalloc.Tests
{
    public class SafeArrayTests : jemallocTest
    {
        [Fact(DisplayName = "Can construct SafeArrau and manipulate it.")]
        public void CanConstructSafeArray()
        {
            SafeArray<int> a = new SafeArray<int>(500);
            a[1] = 1000;
            Assert.Equal(1000, a[1]);
            a.Acquire();
            Assert.Equal(1000, a[1]);
            a.Acquire();
            a.Release();
            Assert.Equal(1000, a[1]);
            a.Release();
            Assert.Equal(1000, a[1]);
            a.Close();
            Assert.True(a.IsClosed);
            Assert.Throws<ObjectDisposedException>(() => a[1] == 1000);
        }
    }
}
