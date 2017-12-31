using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;


namespace jemalloc.Benchmarks
{
    public class VectorBenchmark : JemBenchmark<float, int>
    {
        public int MandelbrotIterations => Parameter;
        public const uint Mandelbrot_Width = 768, Mandelbrot_Height = 512;
        public readonly int VectorWidth = Vector<float>.Count;
        
        [GlobalSetup]
        public override void GlobalSetup()
        {
            DebugInfoThis();
            base.GlobalSetup();
        }



        #region Mandelbrot
        [Benchmark(Description = "Create Mandelbrot plot bitmap with dimensions 768 x 512.")]
        [BenchmarkCategory("Mandelbrot")]
        public unsafe void Mandelbrotv1Unmanaged()
        {
            SafeArray<Vector<float>> Vectors = new SafeArray<Vector<float>>(8); // New unmanaged array of vectors
            FixedBuffer<Int32> output = new FixedBuffer<Int32>(((int)Mandelbrot_Width * (int)Mandelbrot_Height)); //New unmanaged array for bitmap output
            Span<float> VectorSpan = Vectors.AcquireSpan<float>(); //Lets us write to individual vector elements
            Span<Vector2> Vector2Span = Vectors.AcquireSpan<Vector2>(); //Lets us read to individual vectors

            VectorSpan[0] = -2f;
            VectorSpan[1] = -1f;
            VectorSpan[2] = 1f;
            VectorSpan[3] = 1f;

            Vector2 C0 = Vector2Span[0];
            Vector2 C1 = Vector2Span[1];
            Vector2 D = C1 - C0;
            Vector2 P = Vector2Span[2];
            
            int index;
            for (int j = 0; j < Mandelbrot_Height; j++)
            {
                for (int i = 0; i < Mandelbrot_Width; i+= VectorWidth)
                {
                    VectorSpan[4] = i;
                    VectorSpan[5] = j;
                    index = unchecked(j * (int) Mandelbrot_Width + 1);
                    Vector2 B = C0 + (P * D);
                    output[index] = MandelbrotGetByte(ref B, index);
                }
            }

        }

        private int MandelbrotGetByte(ref Vector2 c, int iterations)
        {
            Vector2 z = c; //make a copy
            int i;
            for (i = 0; i < iterations; i++)
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
        #endregion
    }
    }
