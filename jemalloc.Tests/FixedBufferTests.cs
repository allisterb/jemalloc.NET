using System;
using System.Collections.Generic;
using System.Text;

using Xunit;

namespace jemalloc.Tests
{
    public class FixedBufferTests : jemallocTest
    {
        [Fact(DisplayName = "Can create a fixed buffer of bytes")]
        public void CanCreateFixedArray()
        {
            FixedBuffer<byte> buffer = new FixedBuffer<byte>(4096);
            byte[] managedArray = new byte[4096];
            SafeArray<FixedBuffer<byte>> byteBuffer = new SafeArray<FixedBuffer<byte>>(1000);
            byteBuffer[0] = new FixedBuffer<byte>(100);
            byteBuffer[0][16]= 0xff;
            Assert.Equal(0xff, byteBuffer[0][16]);
            byteBuffer[0][16] = 4;
            ref FixedBuffer<byte> a = ref byteBuffer[0];
            for (int i = 0; i < buffer.Length; i++)
            {
                byte v = (byte)Rng.Next(0, 255);
                managedArray[i] = v;
                buffer[i] = v;
            }
            FixedBuffer<byte> copy = buffer;
            Assert.True(buffer.EqualTo(managedArray));
            Assert.True(copy.EqualTo(managedArray));
            buffer.Free();
            Assert.Throws<InvalidOperationException>(() => buffer[0] = 1);
            Assert.Throws<InvalidOperationException>(() => copy[0]);
        }

        [Fact(DisplayName = "Can deallocate a fixed buffer of bytes")]
        public void CanDeAllocateFixedArray()
        {
            SafeArray<FixedBuffer<byte>> byteBuffer = new SafeArray<FixedBuffer<byte>>(100);
            for (int i = 0; i < 100; i++)
            {
                ulong jem_before_alloc = Jem.AllocatedPages;
                byteBuffer[i] = new FixedBuffer<byte>((i * 16) + (1024 * 1024));
                ulong jem_after_alloc = Jem.AllocatedPages;
                //Assert.True(jem_after_alloc > jem_before_alloc);
                byteBuffer.Release();
                long mem_after_free = JemUtil.ProcessPrivateMemory;
                //Assert.True(mem_after_free < mem_after_alloc);
                
            }
        }
    }
}
