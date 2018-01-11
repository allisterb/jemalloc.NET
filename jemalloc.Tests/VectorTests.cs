using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

using Xunit;

using jemalloc.Buffers;

namespace jemalloc.Tests
{
    public class VectorTests
    {
        public const int Mandelbrot_Width = 768, Mandelbrot_Height = 512;
        public const int Mandelbrot_Size = (int)Mandelbrot_Width * (int)Mandelbrot_Height;
        public int VectorWidth = Vector<float>.Count;
        public readonly Vector<double> Limit = new Vector<double>(4f);
        public readonly Vector<double> Zero = Vector<double>.Zero;
        public readonly Vector<double> MinusOne = Vector.Negate(Vector<double>.One);

        public VectorTests()
        {
           
        }

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
            
        }

        [Fact(DisplayName = "Can correctly run Mandelbrot algorithm using Vector2 and managed arrays.")]
        public void CanVectorizeMandelbrotManaged()
        {
            byte[] o = _MandelbrotManagedv4();
            Assert.Equal(0, o[0]);
            Assert.Equal(2, o[1000]);
            Assert.Equal(10, o[500]);
            WriteMandelbrotPPM(o, "mandelbrot-managed-v4.ppm");
        }

        [Fact(DisplayName = "Can correctly run Mandelbrot algorithm using Vector2 and managed arrays.")]
        public void CanVectorizeMandelbrotManaged5()
        {
            int[] o = new int[Mandelbrot_Height * Mandelbrot_Width];
            _MandelbrotManagedv5(ref o);
            Assert.Equal(0, o[0]);
            Assert.Equal(2, o[1000]);
            Assert.Equal(10, o[500]);
            WriteMandelbrotPPM(o, "mandelbrot-managed-v5.ppm");
        }

        [Fact(DisplayName = "Can correctly run Mandelbrot algorithm using Vector2 and unmanaged arrays.")]
        public void CanVectorizeMandelbrotUnmanaged()
        {
            FixedBuffer<byte> output = new FixedBuffer<byte>(Mandelbrot_Height * Mandelbrot_Width);
            FixedBuffer<byte> o = _Mandelbrotv2Unmanaged(ref output);
            Assert.Equal(0, o[0]);
            Assert.Equal(2, o[1000]);
            Assert.Equal(10, o[500]);
            WriteMandelbrotPPM(o.CopyToArray(), "mandelbrot_unmanaged.ppm");
            o.Free();
        }

        #region Mandelbrot algorithms
        private int[] VectorizeMandelbrotManaged()
        {
            
            int[] output = new int[((int)Mandelbrot_Width * (int)Mandelbrot_Height)];
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
                    index = unchecked(j * (int)Mandelbrot_Width + i);
                    Vector2 V = C0 + (P * D);
                    output[index] = GetByte(ref V, 256);
                }
            }
            return output;

            int GetByte(ref Vector2 c, int count)
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
                    z = c + new Vector2(w.X - w.Y, 2f * z.X * z.Y);
                }
                return i;
            }

        }

        private FixedBuffer<int> Mandelbrotv1Unmanaged()
        {
            SafeArray<Vector<float>> Vectors = new SafeArray<Vector<float>>(8); // New unmanaged array of vectors
            FixedBuffer<int> output = new FixedBuffer<int>(((int)Mandelbrot_Width * (int)Mandelbrot_Height)); //New unmanaged array for bitmap output
            Span<float> VectorSpan = Vectors.GetSpan<float>(); //Lets us write to individual vector elements
            Span<Vector2> Vector2Span = Vectors.GetSpan<Vector2>(); //Lets us read to individual vectors

            VectorSpan[0] = -2f;
            VectorSpan[1] = -1f;
            VectorSpan[2] = 1f;
            VectorSpan[3] = 1f;
            VectorSpan[4] = Mandelbrot_Width;
            VectorSpan[5] = Mandelbrot_Height;

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
                    VectorSpan[6] = i;
                    VectorSpan[7] = j;
                    index = unchecked(j * (int)Mandelbrot_Width + i);
                    Vector2 V = C0 + (P * D);
                    output[index] = GetByte(ref V, 256);
                }
            }
            Vectors.Close();
            return output;

            int GetByte(ref Vector2 c, int max_iterations)
            {
                Vector2 z = c; //make a copy
                int i;
                for (i = 0; i < max_iterations; i++)
                {
                    if (z.LengthSquared() > 4f)
                    {
                        break;
                    }
                    Vector2 w = z * z;
                    z = c + new Vector2(w.X - w.Y, 2f * z.X * z.Y);
                }
                return i;
            }

        }

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
                    byte b = output[i] == 256 ? (byte) 20 : (byte) 240;
                    bw.Write(b);
                    bw.Write(b);
                    bw.Write(b);
                }
            }
            
        }

        private void WriteMandelbrotPPM(byte[] output, string name)
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
                    byte b = output[i] == 255 ? (byte)20 : (byte)240;
                    bw.Write(b);
                    bw.Write(b);
                    bw.Write(b);
                }
            }

        }

        #region WIP
        private unsafe FixedBuffer<long> VectorDoubleMandelbrot()
        {
            //Allocate heap and stack memory for our Vector constants and variables
            FixedBuffer<long> output = new FixedBuffer<long>(Mandelbrot_Size); //Output bitmap on unmanaged heap
            double* ptrC0 = stackalloc double[VectorWidth * 2]; 
            double* ptrC1 = stackalloc double[VectorWidth * 2];
            double* ptrB = stackalloc double[VectorWidth * 2];
            double* ptrD = stackalloc double[VectorWidth * 2];
            double* ptrP = stackalloc double[VectorWidth * 2];
            double* ptrV = stackalloc double[VectorWidth * 2];

            //Fill memory with the constant values for vectors C0, C1, B
            for (int i = 0; i < VectorWidth; i++)
            {
                ptrC0[i] = -2f; //x0
                ptrC0[i + VectorWidth] = -1; //y0
                ptrC1[i] = 1; //x1
                ptrC1[i + VectorWidth] = 1; //y1
                ptrB[i] = Mandelbrot_Width; //width
                ptrB[i + VectorWidth] = Mandelbrot_Height; //height
            }

            //Declare spans for reading and writing to memory locations of Vector constants and variables
            Span<Vector<double>> C0 = new Span<Vector<double>>(ptrC0, 2);
            Span<Vector<double>> C1 = new Span<Vector<double>>(ptrC1, 2);
            Span<Vector<double>> B = new Span<Vector<double>>(ptrB, 2);
            Span<Vector<double>> D = new Span<Vector<double>>(ptrD, 2);
            Span<Vector<double>> P = new Span<Vector<double>>(ptrP, 2);
            Span<Vector<double>> V = new Span<Vector<double>>(ptrV, 2);

            Span<Vector<long>> O = output.AcquireVectorWriteSpan();

            D[0] = (C1[0] - C0[0]) / B[0];
            D[1] = (C1[0] - C0[0]) / B[0];
            
            int index;

            for (int j = 0; j < Mandelbrot_Height; j++)
            {
                for (int i = 0; i < Mandelbrot_Width; i += VectorWidth)
                {
                    for (int h = 0; h < VectorWidth; h++)
                    {
                        ptrP[h] = i + h;
                    }
                    index = unchecked((int)Mandelbrot_Width * j + i);
                    V[0] = C0[0] + (P[0] * D[0]);
                    V[1] = C0[1] + (P[1] * D[1]);
                    Vector<long> G = GetValue(V[0], V[1], 256);
                    O[index] = G;
                    
                }
            }
            output.Release();

            return output;

 
        }

        private Vector<double> SquareAbs(Vector<double> Vre, Vector<double> Vim)
        {
            return (Vre * Vre) + (Vim * Vim);
        }

        private unsafe Vector<long> GetValue(Vector<double> Cx, Vector<double> Cy, int maxIterations)
        {
            //int* ptrIterations = stackalloc int[VectorWidth];
            double[] iterationsArr = new double[VectorWidth]; //memory for Iterations vector
            Span<double> sIterations = new Span<double>(iterationsArr); //write to Iterations vector
            Vector<double> Iterations = sIterations.NonPortableCast<double, Vector<double>>()[0];
            Vector<double> MaxIterations = new Vector<double>(256);
            double[] zArr = new double[VectorWidth * 2];
            //double* ptrZ = stackalloc double[VectorWidth * 2];
            Span<double> sZ = new Span<double>(zArr); //Write to individual components of Z
            Span<Vector<double>> Z = sZ.NonPortableCast<double, Vector<double>>();
            Z[0] = Cx;
            Z[1] = Cy;
            for (int i = 0; i < maxIterations; i++)
            {
                sIterations.Fill(i);
                Vector<double> S = SquareAbs(Z[0], Z[1]);
                if (Vector.GreaterThanAll(S, Limit))
                {
                    break;
                }
                else
                {
                    Vector<long> increment;
                    do
                    {
                        Z[0] = Cx + (Z[0] * Z[0]) - (Z[0] * Z[1]);
                        Z[1] = Cy + 2f * Z[0] * Z[1];
                        S = SquareAbs(Z[0], Z[1]);
                        Vector<long> greaterThanLimitMask = Vector.GreaterThan(S, Limit);
                        Vector<long> lessThanOrEqualMaxIterationsMask = Vector.LessThanOrEqual(Iterations, MaxIterations);
                        increment = greaterThanLimitMask & lessThanOrEqualMaxIterationsMask;
                        i += 1;
                    }
                    while (increment != Vector<long>.Zero);

                }
            }
            return Vector.ConvertToInt64(new Vector<double>(iterationsArr));
        }

        private FixedBuffer<byte> _Mandelbrotv2Unmanaged(ref FixedBuffer<byte> output)
        {
            VectorWidth = Vector<Single>.Count;
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
                        output[index + h] = outputVector[h] < 255 ? (byte)outputVector[h] : (byte) 255;
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

        private byte[] _MandelbrotManagedv4()
        {
            byte[] output = new byte[Mandelbrot_Height * Mandelbrot_Width];
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
                        Pre[h] = C0.X + (D.X * (i + h));
                        Pim[h] = C0.Y + (D.Y * p.Item2);
                    }
                    int index = unchecked(p.Item2 * Mandelbrot_Width + i);
                    Vector<float> Vre = new Vector<float>(Pre);
                    Vector<float> Vim = new Vector<float>(Pim);
                    Vector<int> outputVector = GetByte(ref Vre, ref Vim, 255);
                    for (int h = 0; h < VectorWidth; h++)
                    {
                        output[index + h] = outputVector[h] < 255 ? (byte)outputVector[h] : (byte)255;
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

            IEnumerable<ValueTuple<int, int>> MandelbrotBitmapLocation(int j)
            {
                for (int i = 0; i < Mandelbrot_Width; i += VectorWidth)
                {
                    yield return (i, j);
                }
            }
        }

        private unsafe int[] _MandelbrotManagedv5(ref int[] output)
        {

            Vector<int> One = Vector<int>.One;
            Vector<int> Zero = Vector<int>.Zero;
            Vector<float> Limit = new Vector<float>(4);

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
                    Vector<int> outputVector = GetByte(ref Vre, ref Vim, 256);
                    outputVector.CopyTo(output, index);
                }
            }
            return output;


            Vector<int> GetByte(ref Vector<float> Cre, ref Vector<float> Cim, int max_iterations)
            {
                Vector<float> Zre = Cre; //make a copy
                Vector<float> Zim = Cim; //make a copy

                Vector<int> Increment = One;
                Vector<int> MaxIterations = new Vector<int>(max_iterations);
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
                        return (byte)i;
                    }
                    Vector2 w = z * z;
                    z = c + new Vector2(w.X - w.Y, 2f * z.X * z.Y);
                }
                return (byte)(i - 1);
            }

        }

        #endregion

        #endregion

    }
}
