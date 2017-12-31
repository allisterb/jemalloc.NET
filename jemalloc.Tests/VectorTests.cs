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
        public const int Mandelbrot_Size = (int)Mandelbrot_Width * (int)Mandelbrot_Height;
        public readonly int VectorWidth = Vector<float>.Count;
        public readonly Vector<float> Limit = new Vector<float>(4f);
        public readonly Vector<float> Zero = Vector<float>.Zero;
        public readonly Vector<float> One = Vector<float>.One;


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

        [Fact(DisplayName = "Can correctly run Mandelbrot algoritm using Vector2.")]
        public void CanVectorize2Mandelbrot()
        {
            FixedBuffer<Int32> o = Vector2Mandelbrot();
            Assert.Equal(0, o[0]);
            Assert.Equal(2, o[1000]);


        }

        [Fact(DisplayName = "Can correctly run real Mandelbrot algoritm using Vector2.")]
        public void CanVectorizeDoubleMandelbrot()
        {
            FixedBuffer<int> o = VectorDoubleMandelbrot();
            Assert.Equal(0, o[0]);
            Assert.Equal(2, o[1000]);


        }

        private FixedBuffer<Int32> Vector2Mandelbrot()
        {
            FixedBuffer<Int32> output = new FixedBuffer<Int32>(((int)Mandelbrot_Width * (int)Mandelbrot_Height));
            Vector2 B = new Vector2(Mandelbrot_Width, Mandelbrot_Height);
            Vector2 C0 = new Vector2(-2, -1);
            Vector2 C1 = new Vector2(1, 1);
            Vector2 D = (C1 - C0) / B;
            Vector2 P;
            int index;
            int GetValue(Vector2 c, int count)
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
            for (int j = 0; j < Mandelbrot_Height; j++)
            {
                for (int i = 0; i < Mandelbrot_Width; i++)
                {
                    P = new Vector2(i, j);
                    index = unchecked(j * (int)Mandelbrot_Width + i);
                    output[index] = GetValue(C0 + (P * D), 256);
                }
            }
            return output;
        }

        private unsafe FixedBuffer<int> VectorDoubleMandelbrot()
        {
            //Allocate heap and stack memory for our Vector constants and variables
            FixedBuffer<int> output = new FixedBuffer<int>(Mandelbrot_Size); //Output bitmap on unmanaged heap
            float* ptrC0 = stackalloc float[VectorWidth * 2]; 
            float* ptrC1 = stackalloc float[VectorWidth * 2];
            float* ptrB = stackalloc float[VectorWidth * 2];
            float* ptrD = stackalloc float[VectorWidth * 2];
            float* ptrP = stackalloc float[VectorWidth * 2];
            float* ptrV = stackalloc float[VectorWidth * 2];

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
            Span<Vector<float>> C0 = new Span<Vector<float>>(ptrC0, 2);
            Span<Vector<float>> C1 = new Span<Vector<float>>(ptrC1, 2);
            Span<Vector<float>> B = new Span<Vector<float>>(ptrB, 2);
            Span<Vector<float>> D = new Span<Vector<float>>(ptrD, 2);
            Span<Vector<float>> P = new Span<Vector<float>>(ptrP, 2);
            Span<Vector<float>> V = new Span<Vector<float>>(ptrV, 2);

            Span<Vector<int>> O = output.AcquireVectorWriteSpan();

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
                    Vector<int> G = GetValue(V, 256);
                    O[index] = G;
                    
                }
            }
            output.Release();

            return output;

            Vector<float> SquareAbs(Vector<float> Vre, Vector<float> Vim)
            {
                return (Vre * Vre) + (Vim * Vim);
            }

            Vector<int> GetValue(Span<Vector<float>> C, int count)
            {
                int* ptrCount = stackalloc int[VectorWidth]; //memory for Count vector
                Span<int> sCount = new Span<int>(ptrCount, VectorWidth); //write to Count vector

                Vector<int> Count = sCount.NonPortableCast<int, Vector<int>>()[0]; //Count vector

                Span<Vector<float>> Z = C;

                Span<float> sZ = Z.NonPortableCast<Vector<float>, float>(); //Write to individual components of Z

                for (int i = 0; i < count; i++)
                {
                    Vector<float> S = SquareAbs(Z[0], Z[1]);
                    if (Vector.GreaterThanAll(S, Limit))
                    {
                        sCount.Fill(i);
                        break;
                    }
                    else
                    {
                        Vector<int> greaterThan = Vector.GreaterThan(S, Limit);
                        Vector<float> complete = Vector.ConditionalSelect(greaterThan, S, Zero);
                        bool stay = false;
                        do
                        {
                            for (int z = 0; z < VectorWidth; z++)
                            {
                                if (complete[z] != 0)
                                {
                                    sCount[z] = i;
                                }
                                else
                                {

                                    stay = true;
                                    sZ[z] = C[0][z] + sZ[z] * sZ[z] - (sZ[z + VectorWidth] * sZ[z + VectorWidth]); //Zre = Zre * Zre - Zim * Zim
                                    sZ[z + VectorWidth] = C[1][z] + 2f * sZ[z] * sZ[z + VectorWidth];
                                }
                            }
                            i += 1;
                        }
                        while (stay);
                        
                        Vector<float> Fre = Vector.ConditionalSelect(greaterThan, Z[0], Vector<float>.Zero);
                        Vector<float> Fim = Vector.ConditionalSelect(greaterThan, Z[1], Vector<float>.Zero);
               
                    }
                }
                return new Vector<int>(sCount.ToArray());
            }

        }


    }
}
