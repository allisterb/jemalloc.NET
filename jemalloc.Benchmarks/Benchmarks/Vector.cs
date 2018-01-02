using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Text;

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;


namespace jemalloc.Benchmarks
{
    public class VectorBenchmark : JemBenchmark<float, int>
    {
        public int Scale => Parameter;
        public int Mandelbrot_Width = 768, Mandelbrot_Height = 512;
        public int ArraySize => (int) (Scale * Mandelbrot_Width * Mandelbrot_Height);
        public readonly int VectorWidth = Vector<float>.Count;
        
        [GlobalSetup]
        public override void GlobalSetup()
        {
            DebugInfoThis();
            base.GlobalSetup();
            Mandelbrot_Width = Scale * Mandelbrot_Width;
            Mandelbrot_Height = Scale * Mandelbrot_Height;
            byte[] managedArray = new byte[ArraySize];
            SetValue("managedArray", managedArray);
            FixedBuffer<byte> nativeArray = new FixedBuffer<byte>(ArraySize);
            nativeArray.Acquire();
            SetValue("nativeArray", nativeArray);
        }

        #region Mandelbrot

        [Benchmark(Description = "Create Mandelbrot plot bitmap with dimensions 768 x 512 using managed memory.")]
        [BenchmarkCategory("Mandelbrot")]
        public unsafe void MandelbrotManaged()
        {
            byte[] managedArray = GetValue<byte[]>("managedArray");
            ulong start = JemUtil.GetCurrentThreadCycles();
            managedArray = _MandelbrotManaged(managedArray);
            WriteMandelbrotPPM(managedArray, "mandlebrot_manged.ppm");
            ulong end = JemUtil.GetCurrentThreadCycles();
            SetStatistic($"{nameof(MandelbrotManaged)}_ThreadCycles", JemUtil.PrintSize(end - start));
        }

        
        [Benchmark(Description = "Create Mandelbrot plot bitmap with dimensions 768 x 512 using managed memory BGNetCore8.")]
        [BenchmarkCategory("Mandelbrot")]
        public unsafe void MandelbrotBGNetCore8()
        {
            byte[] managedArray = GetValue<byte[]>("managedArray");
            ulong start = JemUtil.GetCurrentThreadCycles();
            managedArray = _MandelbrotManaged(managedArray);
            WriteMandelbrotPPM(managedArray, "mandlebrot_managed_bgnetcore8.ppm");
            ulong end = JemUtil.GetCurrentThreadCycles();
            SetStatistic($"{nameof(MandelbrotBGNetCore8)}_ThreadCycles", JemUtil.PrintSize(end - start));
        }

        [Benchmark(Description = "Create Mandelbrot plot bitmap with dimensions 768 x 512 using unmanaged memory.")]
        [BenchmarkCategory("Mandelbrot")]
        public unsafe void Mandelbrotv1Unmanaged()
        {
            FixedBuffer<byte> nativeArray = GetValue<FixedBuffer<byte>>("nativeArray");
            ulong start = JemUtil.GetCurrentThreadCycles();
            FixedBuffer<byte> o = _Mandelbrotv1Unmanaged(ref nativeArray);
            WriteMandelbrotPPM(o.AcquireSpan(), "mandlebrot_unmanged.ppm");
            o.Release();
            ulong end = JemUtil.GetCurrentThreadCycles();
            SetStatistic($"{nameof(Mandelbrotv1Unmanaged)}_ThreadCycles", JemUtil.PrintSize(end - start));
        }

        [Benchmark(Description = "Create Mandelbrot plot bitmap with dimensions 768 x 512 using unmanaged memory 2.")]
        [BenchmarkCategory("Mandelbrot")]
        public unsafe void Mandelbrotv2Unmanaged()
        {
            FixedBuffer<byte> nativeArray = GetValue<FixedBuffer<byte>>("nativeArray");
            ulong start = JemUtil.GetCurrentThreadCycles();
            FixedBuffer<byte> o = _Mandelbrotv2Unmanaged(ref nativeArray);
            WriteMandelbrotPPM(o.AcquireSpan(), "mandlebrot_unmanged.ppm");
            o.Release();
            ulong end = JemUtil.GetCurrentThreadCycles();
            SetStatistic($"{nameof(Mandelbrotv2Unmanaged)}_ThreadCycles", JemUtil.PrintSize(end - start));
        }

        [GlobalCleanup(Target = nameof(Mandelbrotv1Unmanaged))]
        public void MandelbrotValidateAndCleanup()
        {
            byte[] managedArray = GetValue<byte[]>("managedArray");
            FixedBuffer<byte> nativeArray = GetValue<FixedBuffer<byte>>("nativeArray");
            if(!nativeArray.EqualTo(managedArray))
            {
                throw new Exception();
            }
            nativeArray.Release();
        }
        #endregion

        #region Implementations
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private byte[] _MandelbrotManaged(byte[] output)
        {
            Vector2 B = new Vector2(Mandelbrot_Width, Mandelbrot_Height);
            Vector2 C0 = new Vector2(-2, -1);
            Vector2 C1 = new Vector2(1, 1);
            Vector2 D = (C1 - C0) / B;

            int index;
            for (int j = 0; j < Mandelbrot_Height; j++)
            {
                for (int i = 0; i < Mandelbrot_Width; i++)
                {
                    Vector2 P = new Vector2(i, j);
                    index = unchecked(j * Mandelbrot_Width + i);
                    Vector2 V = C0 + (P * D);
                    output[index] = GetByte(V, 256);
                }
            }
            return output;

            byte GetByte(Vector2 c, int count)
            {
                Vector2 z = c;
                int i;
                for (i = 0; i < count; i++)
                {
                    if (z.LengthSquared() > 4f)
                    {
                        return (byte) i;
                    }
                    Vector2 w = z * z;
                    z = c + new Vector2(w.X - w.Y, 2f * z.X * z.Y);
                }
                return (byte) (i - 1);
            }

        }
        
        private unsafe FixedBuffer<byte> _Mandelbrotv1Unmanaged(ref FixedBuffer<byte> output)
        {
            FixedBuffer<float> Vectors = new FixedBuffer<float>(10);
            Span<Vector2> Vector2Span = Vectors.AcquireWriteSpan().NonPortableCast<float, Vector2>(); //Lets us read individual vectors

            Vectors[0] = -2f;
            Vectors[1] = -1f;
            Vectors[2] = 1f;
            Vectors[3] = 1f;
            Vectors[4] = Mandelbrot_Width;
            Vectors[5] = Mandelbrot_Height;

            ref Vector2 C0 = ref Vector2Span[0];
            ref Vector2 C1 = ref Vector2Span[1];
            ref Vector2 B = ref Vector2Span[2];
            ref Vector2 P = ref Vector2Span[3];
            Vector2 D = (C1 - C0) / B;

            int index;
            for (int j = 0; j < Mandelbrot_Height; j++)
            {
                for (int i = 0; i < Mandelbrot_Width; i++)
                {
                    Vectors[6] = i;
                    Vectors[7] = j;
                    index = unchecked(j * Mandelbrot_Width + i);
                    Vector2 V = C0 + (P * D);
                    output[index] = GetByte(ref V, 256);
                }
            }
            Vectors.Release();
            return output;

            byte GetByte(ref Vector2 c, int max_iterations)
            {
                Vector2 z = c; //make a copy
                int i;
                for (i = 0; i < max_iterations; i++)
                {
                    if (z.LengthSquared() > 4f)
                    {
                        return (byte) i;
                    }
                    Vector2 w = z * z;
                    z = c + new Vector2(w.X - w.Y, 2f * z.X * z.Y);
                }
                return (byte) (i - 1);
            }

        }

        private unsafe FixedBuffer<byte> _Mandelbrotv2Unmanaged(ref FixedBuffer<byte> output)
        {
            Vector<int> Zero = Vector<int>.Zero;
            Vector<int> One = Vector<int>.One;

            FixedBuffer<float> Vectors = new FixedBuffer<float>(6);
            FixedBuffer<float> P = new FixedBuffer<float>(VectorWidth * 2);
            Span<Vector2> Vector2Span = Vectors.AcquireWriteSpan().NonPortableCast<float, Vector2>(); //Lets us read individual Vector2
            Span<Vector<float>> PSpan = P.AcquireWriteSpan().NonPortableCast<float, Vector<float>>(); //Lets us read individual Vectors
            Vectors[0] = -2f;
            Vectors[1] = -1f;
            Vectors[2] = 1f;
            Vectors[3] = 1f;
            Vectors[4] = Mandelbrot_Width;
            Vectors[5] = Mandelbrot_Height;

            ref Vector2 C0 = ref Vector2Span[0];
            ref Vector2 C1 = ref Vector2Span[1];
            ref Vector2 B = ref Vector2Span[2];
            Vector2 D = (C1 - C0) / B;

            int index;
            for (int j = 0; j < Mandelbrot_Height; j++)
            {
                for (int i = 0; i < Mandelbrot_Width; i += VectorWidth)
                {

                    for (int h = 0; h < VectorWidth; h++)
                    {
                        P[h] = C0.X + (D.X * (i + h));
                        P[h + VectorWidth] = C0.Y + (D.Y * j);
                    }
                    index = unchecked(j * Mandelbrot_Width + i);
                    Vector<float> Vre = PSpan[0];
                    Vector<float> Vim = PSpan[1]; ;
                    Vector<int> outputVector = GetByte(ref Vre, ref Vim, 256);
                    for (int h = 0; h < VectorWidth; h++)
                    {
                        output[index + h] = (byte)outputVector[h];
                    }
                }
            }
            Vectors.Release();
            return output;

            Vector<int> GetByte(ref Vector<float> Cre, ref Vector<float> Cim, int max_iterations)
            {
                Vector<float> Zre = Cre; //make a copy
                Vector<float> Zim = Cim; //make a copy

                Vector<float> Limit = new Vector<float>(4);
                Vector<int> MaxIterations = new Vector<int>(256);
                Vector<int> Increment = One;
                Vector<int> I;
                for (I = Zero; Increment != Zero; I+=Increment)
                {
                    Vector<float> S = SquareAbs(Zre, Zim);
                    Increment = Vector.GreaterThan(S, Limit) & Vector.LessThan(I, MaxIterations);
                    if (Increment == One)
                    {
                        return I;
                    }
                    else
                    {
                        Zre &= Vector.ConvertToSingle(Increment);
                        Zim &= Vector.ConvertToSingle(Increment);
                        Limit &= Vector.ConvertToSingle(Increment);
                        MaxIterations &= Increment;
                        Vector<float> Tre = Zre;
                        Vector<float> Tim = Zim;
                        Zre = Cre + (Tre * Tre - Tim * Tim);
                        Zim = Cim + 2f * Tre * Tim;
                    }
                }
                return I;
            }

            Vector<float> SquareAbs(Vector<float> Vre, Vector<float> Vim)
            {
                return (Vre * Vre) + (Vim * Vim);
            }

        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private unsafe byte[] _MandelbrotManagedBGNetCore8(int[] output)
        {
            var size = Mandelbrot_Width;
            var Crb = new double[size + 2];
            var lineLength = size >> 3;
            var data = new byte[size * lineLength];
            fixed (double* pCrb = &Crb[0])
            fixed (byte* pdata = &data[0])
            {
                var value = new Vector<double>(
                      new double[] { 0, 1, 0, 0, 0, 0, 0, 0 }
                );
                var invN = new Vector<double>(2.0 / size);
                var onePtFive = new Vector<double>(1.5);
                var step = new Vector<double>(2);
                for (var i = 0; i < size; i += 2)
                {
                    Unsafe.Write(pCrb + i, value * invN - onePtFive);
                    value += step;
                }
                var _Crb = pCrb;
                var _pdata = pdata;
                Parallel.For(0, size, y =>
                {
                    var Ciby = _Crb[y] + 0.5;
                    for (var x = 0; x < lineLength; x++)
                    {
                        _pdata[y * lineLength + x] = GetByte(_Crb + x * 8, Ciby);
                    }
                });
                return data;

                byte GetByte(double* _pCrb, double Ciby)
                {
                    var res = 0;
                    for (var i = 0; i < 8; i += 2)
                    {
                        var vCrbx = Unsafe.Read<Vector<double>>(_pCrb + i);
                        var vCiby = new Vector<double>(Ciby);
                        var Zr = vCrbx;
                        var Zi = vCiby;
                        int b = 0, j = 49;
                        do
                        {
                            var nZr = Zr * Zr - Zi * Zi + vCrbx;
                            var ZrZi = Zr * Zi;
                            Zi = ZrZi + ZrZi + vCiby;
                            Zr = nZr;
                            var t = Zr * Zr + Zi * Zi;
                            if (t[0] > 4.0) { b |= 2; if (b == 3) break; }
                            if (t[1] > 4.0) { b |= 1; if (b == 3) break; }
                        } while (--j > 0);
                        res = (res << 2) + b;
                    }
                    return (byte)(res ^ -1);
                }

            }


        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void WriteMandelbrotPPM(ReadOnlySpan<byte> output, string name)
        {
            using (StreamWriter sw = new StreamWriter(name))
            {
                sw.Write("P6\n");
                sw.Write(string.Format("{0} {1}\n", Mandelbrot_Width, Mandelbrot_Height));
                sw.Write("255\n");
                sw.Close();
            }
            using (BinaryWriter bw = new BinaryWriter(new FileStream(name, FileMode.Append)))
            {
                for (int i = 0; i < Mandelbrot_Width * Mandelbrot_Height; i++)
                {
                    byte b = output[i] == 255 ? (byte) 20 : (byte) 240;
                    bw.Write(b);
                    bw.Write(b);
                    bw.Write(b);
                }
            }

        }
        #endregion
    }
}
