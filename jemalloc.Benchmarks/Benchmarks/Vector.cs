using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Text;

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using BenchmarkDotNet.Diagnosers;


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
        Vector<int> One = Vector<int>.One;
        Vector<int> Zero = Vector<int>.Zero;
        Vector<float> Limit = new Vector<float>(4);
        Vector<int> MaxIterations = new Vector<int>(255);

        [GlobalSetup]
        public override void GlobalSetup()
        {
            DebugInfoThis();
            base.GlobalSetup();

            //byte[] managed0Array = new byte[ArraySize];
            //SetValue("managed0Array", managed0Array);
            byte[] managedArray = new byte[ArraySize];
            SetValue("managedArray", managedArray);
            byte[] managed3Array = new byte[ArraySize];
            SetValue("managed3Array", managed3Array);
            int[] managed5Array = new int[ArraySize];
            SetValue("managed5Array", managed5Array);
            int[] managed7Array = new int[ArraySize];
            SetValue("managed7Array", managed5Array);
            FixedBuffer<byte> nativeArray = new FixedBuffer<byte>(ArraySize);
            SetValue("nativeArray", nativeArray);
            FixedBuffer<byte> native2Array = new FixedBuffer<byte>(ArraySize);
            SetValue("native2Array", native2Array);
        }

        #region Mandelbrot
        [Benchmark(Description = "Create Mandelbrot plot bitmap single-threaded using managed memory v1.", Baseline = true)]
        [BenchmarkCategory("Mandelbrot")]
        public unsafe void MandelbrotManaged()
        {
            byte[] managedArray = GetValue<byte[]>("managedArray");
            ulong start = JemUtil.GetCurrentThreadCycles();
            _MandelbrotManagedv1(ref managedArray);
            ulong end = JemUtil.GetCurrentThreadCycles();
            SetStatistic($"{nameof(MandelbrotManaged)}_ThreadCycles", JemUtil.PrintSize(end - start));
            SetStatistic($"{nameof(MandelbrotManaged)}_ISPCResult", GetISPCResult());
        }

        [Benchmark(Description = "Create Mandelbrot plot bitmap single-threaded using unmanaged memory v1.")]
        [BenchmarkCategory("Mandelbrot")]
        public unsafe void MandelbrotUnmanagedv1()
        {
            FixedBuffer<byte> nativeArray = GetValue<FixedBuffer<byte>>("nativeArray");
            ulong start = JemUtil.GetCurrentThreadCycles();
            _MandelbrotUnmanagedv1(ref nativeArray);
            ulong end = JemUtil.GetCurrentThreadCycles();
            SetStatistic($"{nameof(MandelbrotUnmanagedv1)}_ThreadCycles", JemUtil.PrintSize(end - start));
            SetStatistic($"{nameof(MandelbrotUnmanagedv1)}_ISPCResult", GetISPCResult());
        }
        /*
        [Benchmark(Description = "Create Mandelbrot plot bitmap single-threaded linear.", Baseline = true)]
        [BenchmarkCategory("Mandelbrot")]
        public unsafe void MandelbrotManagedv0()
        {
            byte[] managedArray = GetValue<byte[]>("managed0Array");
            ulong start = JemUtil.GetCurrentThreadCycles();
            _MandelbrotManagedv0(ref managedArray);
            ulong end = JemUtil.GetCurrentThreadCycles();
            SetStatistic($"{nameof(MandelbrotManagedv0)}_ThreadCycles", JemUtil.PrintSize(end - start));
            SetStatistic($"{nameof(MandelbrotManagedv0)}_ISPCResult", GetISPCResult());
        }
        */

        /*
        [Benchmark(Description = "Create Mandelbrot plot bitmap with dimensions 768 x 512 single-threaded using managed memory vBGNetCore8.")]
        [BenchmarkCategory("Mandelbrot")]
        public unsafe void MandelbrotManagedv2()
        { 
            uulong start = JemUtil.GetCurrentThreadCycles();
            byte[] managedArray = _MandelbrotManagedv2();
            ulong end = JemUtil.GetCurrentThreadCycles();
            SetValue("managed2Array", managedArray);
            SetStatistic($"{nameof(MandelbrotManagedv2)}_ThreadCycles", JemUtil.PrintSize(end - start));
        }
        */


        [Benchmark(Description = "Create Mandelbrot plot bitmap single-threaded using managed memory v3.")]
        [BenchmarkCategory("Mandelbrot")]
        public unsafe void MandelbrotManagedv3()
        {
            byte[] managedArray = GetValue<byte[]>("managed3Array");
            ulong start = JemUtil.GetCurrentThreadCycles();
            _MandelbrotManagedv3(ref managedArray);
            ulong end = JemUtil.GetCurrentThreadCycles();
            SetStatistic($"{nameof(MandelbrotManagedv3)}_ThreadCycles", JemUtil.PrintSize(end - start));
            SetStatistic($"{nameof(MandelbrotManagedv3)}_ISPCResult", GetISPCResult());
        }

        [Benchmark(Description = "Create Mandelbrot plot bitmap single-threaded using unmanaged memory v2.")]
        [BenchmarkCategory("Mandelbrot")]
        public unsafe void MandelbrotUnmanagedv2()
        {
            FixedBuffer<byte> nativeArray = GetValue<FixedBuffer<byte>>("native2Array");
            ulong start = JemUtil.GetCurrentThreadCycles();
            _MandelbrotUnmanagedv2(ref nativeArray);
            ulong end = JemUtil.GetCurrentThreadCycles();
            SetStatistic($"{nameof(MandelbrotUnmanagedv2)}_ThreadCycles", JemUtil.PrintSize(end - start));
            SetStatistic($"{nameof(MandelbrotUnmanagedv2)}_ISPCResult", GetISPCResult());
        }

        [Benchmark(Description = "Create Mandelbrot plot bitmap multi-threaded using managed memory v4.")]
        [BenchmarkCategory("Mandelbrot")]
        public unsafe void MandelbrotManagedv4()
        {
            
            ulong start = JemUtil.GetCurrentThreadCycles();
            byte[] managedArray = _MandelbrotManagedv4();
            ulong end = JemUtil.GetCurrentThreadCycles();
            SetValue("managed4Array", managedArray);
            SetStatistic($"{nameof(MandelbrotManagedv4)}_ThreadCycles", JemUtil.PrintSize(end - start));
            SetStatistic($"{nameof(MandelbrotManagedv4)}_ISPCResult", GetISPCTasksResult());
        }

        [Benchmark(Description = "Create Mandelbrot plot bitmap single-threaded using managed memory v5.")]
        [BenchmarkCategory("Mandelbrot")]
        public void MandelbrotManagedv5()
        {
            int[] managedArray = GetValue<int[]>("managed5Array");
            ulong start = JemUtil.GetCurrentThreadCycles();
            _MandelbrotManagedv5(ref managedArray);
            ulong end = JemUtil.GetCurrentThreadCycles();
            SetStatistic($"{nameof(MandelbrotManagedv5)}_ThreadCycles", JemUtil.PrintSize(end - start));
            SetStatistic($"{nameof(MandelbrotManagedv5)}_ISPCResult", GetISPCResult());
        }

        [Benchmark(Description = "Create Mandelbrot plot bitmap multi-threaded using managed memory v6.")]
        [BenchmarkCategory("Mandelbrot")]
        public void MandelbrotManagedv6()
        {
             
            ulong start = JemUtil.GetCurrentThreadCycles();
            int[] managedArray = _MandelbrotManagedv6();
            ulong end = JemUtil.GetCurrentThreadCycles();
            SetValue("managed6Array", managedArray);
            SetStatistic($"{nameof(MandelbrotManagedv6)}_ThreadCycles", JemUtil.PrintSize(end - start));
            SetStatistic($"{nameof(MandelbrotManagedv6)}_ISPCResult", GetISPCTasksResult());
        }

        [Benchmark(Description = "Create Mandelbrot plot bitmap single-threaded using managed memory v7.")]
        [BenchmarkCategory("Mandelbrot")]
        public void MandelbrotManagedv7()
        {
            int[] managedArray = GetValue<int[]>("managed7Array");
            ulong start = JemUtil.GetCurrentThreadCycles();
            _MandelbrotManagedv7(ref managedArray);
            ulong end = JemUtil.GetCurrentThreadCycles();
            SetStatistic($"{nameof(MandelbrotManagedv7)}_ThreadCycles", JemUtil.PrintSize(end - start));
            SetStatistic($"{nameof(MandelbrotManagedv7)}_ISPCResult", GetISPCResult());
        }

        [Benchmark(Description = "Create Mandelbrot plot bitmap multi-threaded using managed memory v8.")]
        [BenchmarkCategory("Mandelbrot")]
        public void MandelbrotManagedv8()
        {
            ulong start = JemUtil.GetCurrentThreadCycles();
            int[] managedArray = _MandelbrotManagedv8();
            ulong end = JemUtil.GetCurrentThreadCycles();
            SetValue("managed8Array", managedArray);
            SetStatistic($"{nameof(MandelbrotManagedv8)}_ThreadCycles", JemUtil.PrintSize(end - start));
            SetStatistic($"{nameof(MandelbrotManagedv8)}_ISPCResult", GetISPCTasksResult());
        }


        [GlobalCleanup(Target = nameof(MandelbrotManagedv8))]
        public void MandelbrotValidateAndCleanup()
        {
            byte[] managedArray = GetValue<byte[]>("managedArray");
            //byte[] managed2Array = GetValue<byte[]>("managed2Array");
            byte[] managed3Array = GetValue<byte[]>("managed3Array");
            byte[] managed4Array = GetValue<byte[]>("managed4Array");
            int[] managed5Array = GetValue<int[]>("managed5Array");
            int[] managed6Array = GetValue<int[]>("managed6Array");
            int[] managed7Array = GetValue<int[]>("managed7Array");
            int[] managed8Array = GetValue<int[]>("managed8Array");
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
                }
                */

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

                
                if (managed5Array[i] <= 255)
                {
                    if (!managedArray[i].Equals( (byte) managed5Array[i]))
                    {
                        Error($"Managed5 array at index {i} is {managed5Array[i]} not {managedArray[i]}.");
                        throw new Exception();
                    }
                }

                if (managed6Array[i] <= 255)
                {
                    if (!managedArray[i].Equals((byte)managed6Array[i]))
                    {
                        Error($"Managed6 array at index {i} is {managed6Array[i]} not {managedArray[i]}.");
                        throw new Exception();
                    }
                }

                if (managed7Array[i] <= 255)
                {
                    if (!managedArray[i].Equals((byte)managed7Array[i]))
                    {
                        Error($"Managed7 array at index {i} is {managed7Array[i]} not {managedArray[i]}.");
                        throw new Exception();
                    }
                }

                if (managed8Array[i] <= 255)
                {
                    if (!managedArray[i].Equals((byte)managed8Array[i]))
                    {
                        Error($"Managed8 array at index {i} is {managed8Array[i]} not {managedArray[i]}.");
                        throw new Exception();
                    }
                }

            }
            WriteMandelbrotPPM(managedArray, "mandelbrot-managed-v1.ppm");
            //WriteMandelbrotPPM(managed0Array, "mandelbrot-managed-v0.ppm");
            //WriteMandelbrotPPM(managed2Array, "mandelbrot-managed2.ppm");
            WriteMandelbrotPPM(managed3Array, "mandelbrot-managed-v3.ppm");
            WriteMandelbrotPPM(managed4Array, "mandelbrot-managed-v4.ppm");
            WriteMandelbrotPPM(managed5Array, "mandelbrot-managed-v5.ppm");
            WriteMandelbrotPPM(managed6Array, "mandelbrot-managed-v6.ppm");
            WriteMandelbrotPPM(managed7Array, "mandelbrot-managed-v7.ppm");
            WriteMandelbrotPPM(managed8Array, "mandelbrot-managed-v8.ppm");
            WriteMandelbrotPPM(nativeArray.AcquireSpan(), "mandelbrot-unmanaged-v1.ppm");
            WriteMandelbrotPPM(native2Array.AcquireSpan(), "mandelbrot-unmanaged-v2.ppm");
            
            nativeArray.Release();
            native2Array.Release();
     
            
        }
        #endregion

        #region Implementations
        private byte[] _MandelbrotManagedv0(ref byte[] output)
        {
            float[] B = new float[2] { Mandelbrot_Width, Mandelbrot_Height };
            float[] C0 = new float[2] { -2, -1 };
            float[] C1 = new float[2] { 1, 1 };
            float[] D = new float[2] { (C1[0] - C0[0]) / B[0], (C1[1] - C0[1]) / B[1] };
            float[] P = new float[2];
            float[] V = new float[2];
            int index;
            for (int j = 0; j < Mandelbrot_Height; j++)
            {
                for (int i = 0; i < Mandelbrot_Width; i++)
                {
                    P[0] = i;
                    P[1] = j ;
                    index = unchecked(j * Mandelbrot_Width + i);
                    V[0] = C0[0] + (P[0] * D[0]);
                    V[1] = C0[1] + (P[1] * D[1]);
                    output[index] = GetByte(V, 256);
                }
            }
            return output;
        }

        private byte[] _MandelbrotManagedv1(ref byte[] output)
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

        private unsafe byte[] _MandelbrotManagedv3(ref byte[] output)
        {
        
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
                        output[index + h] = outputVector[h] > 255 ? (byte)255 : (byte)outputVector[h];
                    }
                }
            }
            return output;
        }


        private byte[] _MandelbrotManagedv4()
        {
            byte[] output = new byte[ArraySize];
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
        }

        private unsafe int[] _MandelbrotManagedv5(ref int[] output)
        {
            Vector<float> C0re = new Vector<float>(-2);
            Vector<float> C0im = new Vector<float>(-1);
            Vector<float> C1re = Vector<float>.One;
            Vector<float> C1im = Vector<float>.One;

            Vector<float> Bx = new Vector<float>(Mandelbrot_Width);
            Vector<float> By = new Vector<float>(Mandelbrot_Height);
            Vector<float> Dx = (C1re - C0re) / Bx;
            Vector<float> Dy = (C1im - C0im) / By;

            int index;
            Vector<float> Pre = new Vector<float>();
            Vector<float> Pim;

            Span<float> sPre = new Span<float>(Unsafe.AsPointer(ref Pre), VectorWidth);

            for (int j = 0; j < Mandelbrot_Height; j++)
            {
                for (int i = 0; i < Mandelbrot_Width; i += VectorWidth)
                {                        
                    for (int h = 0; h < VectorWidth; h++)
                    {
                        sPre[h] = C0re[0] + (Dx[0] * (i + h));
                    }
                    
                    Pim = C0im + (Dy * j);
                    index = unchecked(j * Mandelbrot_Width + i);
                    Vector<int> outputVector = GetByte(ref Pre, ref Pim, 256);
                    outputVector.CopyTo(output, index);
                }
            }
            return output;
        }

        private int[] _MandelbrotManagedv6()
        {
            int[] output = new int[ArraySize];
 
            Vector<float> C0re = new Vector<float>(-2);
            Vector<float> C0im = new Vector<float>(-1);
            Vector<float> C1re = Vector<float>.One;
            Vector<float> C1im = Vector<float>.One;

            Vector<float> Bx = new Vector<float>(Mandelbrot_Width);
            Vector<float> By = new Vector<float>(Mandelbrot_Height);
            Vector<float> Dx = (C1re - C0re) / Bx;
            Vector<float> Dy = (C1im - C0im) / By;

            
            for (int j = 0; j < Mandelbrot_Height; j++)
            {
                Parallel.ForEach(MandelbrotBitmapLocation(j), (p) =>
                {
                    int i = p.Item1;
          
                    Vector<float> Pre = new Vector<float>();
                    unsafe
                    {
                        Span<float> sPre = new Span<float>(Unsafe.AsPointer(ref Pre), VectorWidth);

                        for (int h = 0; h < VectorWidth; h++)
                        {
                            sPre[h] = C0re[0] + (Dx[0] * (p.Item1 + h));
                        }
                    }
                    Vector<float> Pim = C0im + (Dy * p.Item2);
                    int index = unchecked(p.Item2 * Mandelbrot_Width + p.Item1);
                    Vector<int> outputVector = GetByte(ref Pre, ref Pim, 255);
                    outputVector.CopyTo(output, index);

                });

            }
            return output;
        }

        private unsafe int[] _MandelbrotManagedv7(ref int[] output)
        {        
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

            int index;
            float[] PreArray = new float[VectorWidth];
            float[] PimArray = new float[VectorWidth];


            for (int j = 0; j < Mandelbrot_Height; j++)
            {
                for (int i = 0; i < Mandelbrot_Width; i += VectorWidth)
                {
                    for (int h = 0; h < VectorWidth; h++)
                    {
                        PreArray[h] = C0.X + (D.X * (i + h));
                        PimArray[h] = C0.Y + (D.Y * j);
                    }
                    Vector<float> Pre = new Vector<float>(PreArray);
                    Vector<float> Pim = new Vector<float>(PimArray);
                    index = unchecked(j * Mandelbrot_Width + i);
                    Vector<int> outputVector = GetByte(ref Pre, ref Pim, 256);
                    outputVector.CopyTo(output, index);
                }
            }
            return output;
        }

        private int[] _MandelbrotManagedv8()
        {
            int[] output = new int[ArraySize];

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
                    float[] PimArray = new float[VectorWidth];
                    float[] PreArray = new float[VectorWidth];
                    for (int h = 0; h < VectorWidth; h++)
                    {
                        PreArray[h] = C0.X + (D.X * (i + h));
                        PimArray[h] = C0.Y + (D.Y * j);

                    }
                    Vector<float> Pre = new Vector<float>(PreArray);
                    Vector<float> Pim = new Vector<float>(PimArray);
                    int index = unchecked(p.Item2 * Mandelbrot_Width + p.Item1);
                    Vector<int> outputVector = GetByte(ref Pre, ref Pim, 255);
                    outputVector.CopyTo(output, index);

                });

            }
            return output;
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
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        byte GetByte(float[] c, int maxIterations)
        {
            float[] Z = new float[] { c[0], c[1] }; // make a copy
            int i;
            for (i = 0; i < maxIterations; i++)
            {
                if ((Z[0] * Z[0]) + (Z[1] * Z[1]) > 4f)
                {
                    return (byte)i;
                }
                float z0 = Z[0];
                float z1 = Z[1];
                float[] w = new float[] { (z0 * z0), (z1 * z1) };
                Z[0] = c[0] + w[0] - w[1];
                Z[1] = c[1] + 2f * z0 * z1;
            }
            return (byte)(i - 1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        byte GetByte(ref Vector2 c, int max_iterations)
        {
            Vector2 z = c; //make a copy
            int i;
            for (i = 0; i < max_iterations; i++)
            {
                if (z.LengthSquared() > 4f)
                {
                    return (byte)i;
                }
                Vector2 w = z * z;
                z = c + new Vector2(w.X - w.Y, 2f * z.X * z.Y);
            }
            return (byte)(i - 1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        Vector<float> SquareAbs(Vector<float> Vre, Vector<float> Vim)
        {
            return (Vre * Vre) + (Vim * Vim);
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
                    byte b = (output[i] & 0x01) == 1 ? (byte) 20 : (byte) 240;
                    bw.Write(b);
                    bw.Write(b);
                    bw.Write(b);
                }
            }

        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void WriteMandelbrotPPM(int[] output, string name)
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
                    byte b = (output[i] & 0x01) == 1 ? (byte) 20 : (byte) 240;
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

        public string GetISPCResult()
        {
            switch (Scale)
            {
                case 1:
                    return "26.8 M";
                case 6:
                    return "907 M"; 
                default:
                    return string.Empty;
            }
        }

        public string GetISPCTasksResult()
        {
            switch (Scale)
            {
                case 1:
                    return "42.1 M";
                default:
                    return string.Empty;
            }
        }
    }
}
