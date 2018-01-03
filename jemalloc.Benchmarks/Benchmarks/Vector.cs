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
       
        private const int _Mandelbrot_Width = 768 , _Mandelbrot_Height = 512;

        public int Mandelbrot_Width => _Mandelbrot_Width * Scale;
        public int Mandelbrot_Height => _Mandelbrot_Height * Scale;
        public int ArraySize => (Mandelbrot_Width * Mandelbrot_Height);
        public readonly int VectorWidth = Vector<float>.Count;
        
        [GlobalSetup]
        public override void GlobalSetup()
        {
            DebugInfoThis();
            base.GlobalSetup();

            byte[] managedArray = new byte[ArraySize];
            SetValue("managedArray", managedArray);
            byte[] managed3Array = new byte[ArraySize];
            SetValue("managed3Array", managed3Array);
         
            FixedBuffer<byte> nativeArray = new FixedBuffer<byte>(ArraySize);
            SetValue("nativeArray", nativeArray);
            FixedBuffer<byte> native2Array = new FixedBuffer<byte>(ArraySize);
            SetValue("native2Array", native2Array);
         
        }

        #region Mandelbrot
        [Benchmark(Description = "Create Mandelbrot plot bitmap with dimensions 768 x 512 single-threaded using managed memory v1.", Baseline = true)]
        [BenchmarkCategory("Mandelbrot")]
        public unsafe void MandelbrotManaged()
        {
            byte[] managedArray = GetValue<byte[]>("managedArray");
            ulong start = JemUtil.GetCurrentThreadCycles();
            _MandelbrotManaged(ref managedArray);
            ulong end = JemUtil.GetCurrentThreadCycles();
            SetStatistic($"{nameof(MandelbrotManaged)}_ThreadCycles", JemUtil.PrintSize(end - start));
        }

        [Benchmark(Description = "Create Mandelbrot plot bitmap with dimensions 768 x 512 single-threaded using unmanaged memory v1.")]
        [BenchmarkCategory("Mandelbrot")]
        public unsafe void MandelbrotUnmanagedv1()
        {
            FixedBuffer<byte> nativeArray = GetValue<FixedBuffer<byte>>("nativeArray");
            ulong start = JemUtil.GetCurrentThreadCycles();
            _MandelbrotUnmanagedv1(ref nativeArray);
            ulong end = JemUtil.GetCurrentThreadCycles();
           
            SetStatistic($"{nameof(MandelbrotUnmanagedv1)}_ThreadCycles", JemUtil.PrintSize(end - start));
        }

        /*
        [Benchmark(Description = "Create Mandelbrot plot bitmap with dimensions 768 x 512 single-threaded using managed memory vBGNetCore8.")]
        [BenchmarkCategory("Mandelbrot")]
        public unsafe void MandelbrotManagedv2()
        { 
            ulong start = JemUtil.GetCurrentThreadCycles();
            byte[] managedArray = _MandelbrotManagedv2();
            ulong end = JemUtil.GetCurrentThreadCycles();
            SetValue("managed2Array", managedArray);
            SetStatistic($"{nameof(MandelbrotManagedv2)}_ThreadCycles", JemUtil.PrintSize(end - start));
        }
        */
        
        [Benchmark(Description = "Create Mandelbrot plot bitmap with dimensions 768 x 512 single-threaded using managed memory v3.")]
        [BenchmarkCategory("Mandelbrot")]
        public unsafe void MandelbrotManagedv3()
        {
            byte[] managedArray = GetValue<byte[]>("managed3Array");
            ulong start = JemUtil.GetCurrentThreadCycles();
            _MandelbrotManagedv3(ref managedArray);
            ulong end = JemUtil.GetCurrentThreadCycles();
            SetStatistic($"{nameof(MandelbrotManagedv3)}_ThreadCycles", JemUtil.PrintSize(end - start));
        }

        [Benchmark(Description = "Create Mandelbrot plot bitmap with dimensions 768 x 512 single-threaded using unmanaged memory v2.")]
        [BenchmarkCategory("Mandelbrot")]
        public unsafe void MandelbrotUnmanagedv2()
        {
            FixedBuffer<byte> nativeArray = GetValue<FixedBuffer<byte>>("native2Array");
            ulong start = JemUtil.GetCurrentThreadCycles();
            _MandelbrotUnmanagedv2(ref nativeArray);
            ulong end = JemUtil.GetCurrentThreadCycles();
            SetStatistic($"{nameof(MandelbrotUnmanagedv2)}_ThreadCycles", JemUtil.PrintSize(end - start));
        }

        [Benchmark(Description = "Create Mandelbrot plot bitmap with dimensions 768 x 512 multi-threaded using managed memory v1.")]
        [BenchmarkCategory("Mandelbrot")]
        public unsafe void MandelbrotManagedv4()
        {
            
            ulong start = JemUtil.GetCurrentThreadCycles();
            byte[] managedArray = _MandelbrotManagedv4();
            ulong end = JemUtil.GetCurrentThreadCycles();
            SetValue("managed4Array", managedArray);
            SetStatistic($"{nameof(MandelbrotManagedv4)}_ThreadCycles", JemUtil.PrintSize(end - start));
        }

        [GlobalCleanup(Target = nameof(MandelbrotManagedv4))]
        public void MandelbrotValidateAndCleanup()
        {
            byte[] managedArray = GetValue<byte[]>("managedArray");
            //byte[] managed2Array = GetValue<byte[]>("managed2Array");
            byte[] managed3Array = GetValue<byte[]>("managed3Array");
            byte[] managed4Array = GetValue<byte[]>("managed4Array");
            FixedBuffer<byte> nativeArray = GetValue<FixedBuffer<byte>>("nativeArray");
            FixedBuffer<byte> native2Array = GetValue<FixedBuffer<byte>>("native2Array");
            for (int i = 0; i < ArraySize; i++)
            {
                if (!nativeArray[i].Equals(managedArray[i]))
                {
                    Error($"Native array at index {i} is {nativeArray[i]} not {managedArray[i]}.");
                    throw new Exception();
                }
                
                /*
                if (!managedArray[i].Equals(managed2Array[i]))
                {
                    Error($"Managed2 array at index {i} is {managed2Array[i]} not {managedArray[i]}.");
                    throw new Exception();
                }*/
                
                
                if (!managedArray[i].Equals(managed3Array[i]))
                {
                    Error($"Managed3 array at index {i} is {managed3Array[i]} not {managedArray[i]}.");
                    throw new Exception();
                }

                
                if (!managedArray[i].Equals(native2Array[i]))
                {
                    Error($"Native2 array at index {i} is {native2Array[i]} not {managedArray[i]}.");
                    throw new Exception();
                }

                
                if (!managedArray[i].Equals(managed4Array[i]))
                {
                    Error($"Managed4 array at index {i} is {managed4Array[i]} not {managedArray[i]}.");
                    throw new Exception();
                }
                
                
            }
            WriteMandelbrotPPM(managedArray, "mandelbrot-managed-v1.ppm");
            //WriteMandelbrotPPM(managed2Array, "mandelbrot-managed2.ppm");
            WriteMandelbrotPPM(managed3Array, "mandelbrot-managed-v3.ppm");
            WriteMandelbrotPPM(managed4Array, "mandelbrot-managed-v4.ppm");
            WriteMandelbrotPPM(nativeArray.AcquireSpan(), "mandelbrot-unmanaged-v1.ppm");
            WriteMandelbrotPPM(native2Array.AcquireSpan(), "mandelbrot-unmanaged-v2.ppm");
            
            nativeArray.Release();
            native2Array.Release();
     
            
        }
        #endregion

        #region Implementations
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private byte[] _MandelbrotManaged(ref byte[] output)
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

        private unsafe byte[] _MandelbrotManagedv2()
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

        private byte[] _MandelbrotManagedv3(ref byte[] output)
        {
            Vector<int> One = Vector<int>.One;
            Vector<int> Zero = Vector<int>.Zero;
            Vector<float> Limit = new Vector<float>(4);
            Vector<int> MaxIterations = new Vector<int>(255);
            Span<Byte> outputSpan = new Span<byte>(output);

            float[] Vectors = new float[6];
            float[] P = new float[VectorWidth * 2];
            Span<Vector2> Vector2Span = new Span<float>(Vectors).NonPortableCast<float, Vector2>(); //Lets us read individual Vector2
            Span<Vector<float>> PSpan = new Span<float>(P).NonPortableCast<float, Vector<float>>(); //Lets us read individual Vectors
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
                    Vector<int> outputVector = GetByte(ref Vre, ref Vim, 255);
                    for (int h = 0; h < VectorWidth; h++)
                    {
                        output[index + h] = outputVector[h] > 255 ? (byte) 255 : (byte)outputVector[h];
                    }
                }
            }
            return output;

            Vector<int> GetByte(ref Vector<float> Cre, ref Vector<float> Cim, int max_iterations)
            {
                Vector<float> Zre = Cre; //make a copy
                Vector<float> Zim = Cim; //make a copy

                Vector<int> Increment = One;
                Vector<int> I;
                for (I = Zero; Increment != Zero; I += Vector.Abs(Increment))
                {
                    Vector<float> S = SquareAbs(Zre, Zim);
                    Increment = Vector.LessThanOrEqual(S, Limit) & Vector.LessThan(I, MaxIterations);
                    if (Increment == Zero)
                    {
                        break;
                    }
                    else
                    {
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

        private byte[] _MandelbrotManagedv4()
        {
            byte[] output = new byte[ArraySize];
            Vector<int> One = Vector<int>.One;
            Vector<int> Zero = Vector<int>.Zero;
            float[] Vectors = new float[6];
            Span<Vector2> Vector2Span = new Span<float>(Vectors).NonPortableCast<float, Vector2>(); //Lets us read individual Vector2
           
            Vectors[0] = -2f;
            Vectors[1] = -1f;
            Vectors[2] = 1f;
            Vectors[3] = 1f;
            Vectors[4] = Mandelbrot_Width;
            Vectors[5] = Mandelbrot_Height;

            Vector2 C0 = Vector2Span[0];
            Vector2 C1 = Vector2Span[1];
            Vector2 B = Vector2Span[2];
            Vector2 D = (C1 - C0) / B;

            for (int j = 0; j < Mandelbrot_Height; j++)
            {
                Parallel.ForEach(MandelbrotBitmapLocation(j), (p) =>
                {
                    int i = p.Item1;
                    float[] Pre = new float[VectorWidth];
                    float[] Pim = new float[VectorWidth];
                    for (int h = 0; h < VectorWidth; h++)
                    {
                        Pre[h] = C0.X + (D.X * (p.Item1 + h));
                        Pim[h] = C0.Y + (D.Y * p.Item2);
                    }
                    int index = unchecked(p.Item2 * Mandelbrot_Width + p.Item1);
                    Vector<float> Vre = new Vector<float>(Pre);
                    Vector<float> Vim = new Vector<float>(Pim);
                    Vector<int> outputVector = GetByte(ref Vre, ref Vim, 255);
                    for (int h = 0; h < VectorWidth; h++)
                    {
                        output[index + h] = outputVector[h] < 255 ? (byte) outputVector[h] : (byte) 255;
                    }
                });
                
            }
            return output;

            Vector<int> GetByte(ref Vector<float> Cre, ref Vector<float> Cim, int max_iterations)
            {
                Vector<float> Limit = new Vector<float>(4);
                Vector<int> MaxIterations = new Vector<int>(max_iterations);
                Vector<float> Zre = Cre; //make a copy
                Vector<float> Zim = Cim; //make a copy

                Vector<int> Increment = One;
                Vector<int> I;
                for (I = Zero; Increment != Zero; I += Vector.Abs(Increment))
                {
                    Vector<float> S = SquareAbs(Zre, Zim);
                    Increment = Vector.LessThanOrEqual(S, Limit) & Vector.LessThan(I, MaxIterations);
                    if (Increment == Zero)
                    {
                        break;
                    }
                    else
                    {
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


        private FixedBuffer<byte> _MandelbrotUnmanagedv1(ref FixedBuffer<byte> output)
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

        private FixedBuffer<byte> _MandelbrotUnmanagedv2(ref FixedBuffer<byte> output)
        {
            Vector<int> One = Vector<int>.One;
            Vector<int> Zero = Vector<int>.Zero;

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
                        output[index + h] = outputVector[h] < 255 ? (byte)outputVector[h] : (byte)255;
                    }
                }
            }

            Vectors.Release();
            Vectors.Free();
            P.Release();
            P.Free();
            
            return output;

            Vector<int> GetByte(ref Vector<float> Cre, ref Vector<float> Cim, int max_iterations)
            {
                Vector<float> Zre = Cre; //make a copy
                Vector<float> Zim = Cim; //make a copy

                Vector<float> Limit = new Vector<float>(4);
                Vector<int> MaxIterations = new Vector<int>(max_iterations);
                Vector<int> Increment = One;
                Vector<int> I;
                for (I = Zero; Increment != Zero; I += Vector.Abs(Increment))
                {
                    Vector<float> S = SquareAbs(Zre, Zim);
                    Increment = Vector.LessThanOrEqual(S, Limit) & Vector.LessThanOrEqual(I, MaxIterations);
                    if (Increment == Zero)
                    {
                        break;
                    }
                    else
                    {
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

        public IEnumerable<ValueTuple<int, int>> MandelbrotBitmapLocation(int j)
        {
            for (int i = 0; i < Mandelbrot_Width; i += VectorWidth)
            {
                yield return (i, j);
            }
        }
    }
}
