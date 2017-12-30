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
        public const uint Mandelbrot_Width = 768, Mandelbrot_Height = 512;
        public readonly int VectorWidth = Vector<float>.Count;
        
        [Fact(DisplayName = "Buffer elements can be accessed as vector.")]
        public void CanConstructVectors()
        {
            NativeMemory<uint> memory = new NativeMemory<uint>(1, 11, 94, 5, 0, 0, 0, 8);      
            Vector<uint> v = memory.AsVector();
            Assert.True(v[0] == 1);
            Assert.True(v[1] == 11);
            Assert.True(v[2] == 94);
            NativeMemory<Vector<uint>> vectors = new NativeMemory<Vector<uint>>(4);
            vectors.Retain();
            //vectors.Span[0]
        }

        [Fact(DisplayName = "Can.")]
        public void CanVectorizeMandelbrot()
        {
            FixedBuffer<Int32> o = VectorMandelbrot();
        }

        private FixedBuffer<Int32> VectorMandelbrot()
        {
            FixedBuffer<Int32> output = new FixedBuffer<Int32>(((int)Mandelbrot_Width * (int)Mandelbrot_Height));
            Vector2 B = new Vector2(Mandelbrot_Width, Mandelbrot_Height);
            Vector2 C0 = new Vector2(-2, -1);
            Vector2 C1 = new Vector2(1, 1);
            Vector2 D = (C1 - C0) / B;
            Vector2 P;
            int index;
            for (int j = 0; j < Mandelbrot_Height; j++)
            {
                for (int i = 0; i < Mandelbrot_Width; i += VectorWidth)
                {
                    P = new Vector2(i, j);
                    index = unchecked(j * (int)Mandelbrot_Width + i);
                    output[index] = MandelbrotGetValue(C0 + (P * D), 256);
                }
            }
            return output;
        }

        private int MandelbrotGetValue(Vector2 c, int count)
        {
            Vector2 z = c;
            int i;
            for (i = 0; i < count; i++)
            {
                if (z.LengthSquared() > 4f)
                {
                    break;
                }
                Vector2 w = z * z;
                z = c + new Vector2(w.X - w.Y, 2f * c.X * c.Y);
            }
            return i;
        }


    }
}
