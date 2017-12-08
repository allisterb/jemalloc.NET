using Xunit;
namespace jemalloc.Tests
{
    public class NativeArrayTests : jemallocTest
    {
        [Fact]
        public void CanConstructNativeArray()
        {
            NativeArray<int> a = new NativeArray<int>(500);
            a[1] = 1000;
            Assert.Equal(1000, a[1]);
            
        }
    }
}
