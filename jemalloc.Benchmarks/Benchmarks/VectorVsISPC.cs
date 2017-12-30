using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;


namespace jemalloc.Benchmarks
{
    public class VectorVsISPC : JemBenchmark<float, int>
    {
        public int MandelbrotIterations => Parameter;
        public const uint Mandelbrot_Width = 768, Mandelbrot_Height = 512;
        public readonly int VectorWidth = Vector<float>.Count;
        public FixedBuffer<Int32> Mandelbrot_Output = new FixedBuffer<Int32>(((int)Mandelbrot_Width * (int) Mandelbrot_Height));

        [GlobalSetup]
        public override void GlobalSetup()
        {
            DebugInfoThis();
            base.GlobalSetup();
        }



        #region Mandelbrot
        [Benchmark(Description = "Fill a managed array with the maximum size [2146435071] with a single value.")]
        [BenchmarkCategory("Fill")]
        public unsafe void VectorMandelbrot()
        {
            Vector2 C0 = new Vector2(-2, -1);
            Vector2 C1 = new Vector2(1, 1);
            Vector2 D = C1 - C0;
            Vector2 P;
            int index;
            for (int j = 0; j < Mandelbrot_Height; j++)
            {
                for (int i = 0; i < Mandelbrot_Width; i+= VectorWidth)
                {
                    P = new Vector2(i, j);
                    index = unchecked(j * (int) Mandelbrot_Width + 1);
                    Mandelbrot_Output[index] = MandelbrotGetValue(C0 + (P * D), index);
                }
            }

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
        #endregion
    }
    }
