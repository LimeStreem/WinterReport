using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CalculationMathematicsReport.Basis;
using CalculationMathematicsReport.CalcMath;
using CalculationMathematicsReport.Compute;
using CalculationMathematicsReport.Util;
using Microsoft.SqlServer.Server;
using SlimDX;
using SlimDX.Direct3D11;
using Format = SlimDX.DXGI.Format;
using Matrix = CalculationMathematicsReport.Basis.Matrix;
using Resource = SlimDX.DXGI.Resource;

namespace CalculationMathematicsReport
{
    class Program
    {

        private const float Epsilon = 1.0E-5f;
        static void Main(string[] args)
        {
            //Experiment1();
            Experiment2();
        }

        private static void Experiment2()
        {
            using (Device device=new Device(DriverType.Hardware,DeviceCreationFlags.None,FeatureLevel.Level_11_0))
            using(ComputeShader compute = ComputeHelper.LoadComputeShader(device, "Compute\\Experiment2.hlsl", "main"))
            {
                string result = "";
                for (int N = 5; N <= 1000; N++)
                {
                    Stopwatch st=new Stopwatch();
                    st.Start();
                    Matrix mat = new Matrix(new BasicMatrixElementBuilder(N, matrixGen));
                    Vector eq = mat*Vector.One(N);
                    Vector x = Vector.Zero(N);
                    using (ReadableVectorTexture tex = new ReadableVectorTexture(device, Format.R32_Float, N))
                    using (EquationVectorTexture evec = new EquationVectorTexture(device, eq))
                    using (CoefficientMatrixTexture matTex = new CoefficientMatrixTexture(device, mat))
                    using (ArgumentConstants constants = new ArgumentConstants(device, (uint) N))
                    {
                        device.ImmediateContext.ComputeShader.Set(compute);
                        device.ImmediateContext.ComputeShader.SetUnorderedAccessView(tex.UnorderedAccessView, 0);
                        bool isOK = false;
                        int i;
                        for (i = 0; i < 10000; i++)
                        {
                            using (EquationVectorTexture lastX = new EquationVectorTexture(device, x))
                            {
                                device.ImmediateContext.ComputeShader.SetShaderResources(
                                    new ShaderResourceView[]
                                    {evec.ShaderResourceView, matTex.ShaderResourceView, lastX.ShaderResourceView}, 0, 3);
                                device.ImmediateContext.ComputeShader.SetConstantBuffers(
                                    new[] {constants.ArgumentConstantBuffer}, 0, 1);
                                device.ImmediateContext.Dispatch(N, 1, 1);
                                x = tex.ToVector();
                                float relativeError = (x - Vector.One(N)).TwoNorm()/Vector.One(N).TwoNorm();
                                if (relativeError < Epsilon)
                                {
                                    isOK = true;
                                    break;
                                }
                            }
                        }
                        st.Stop();
                        if (isOK)
                        {
                            Console.WriteLine("N={0}のとき、{1}回で収束。{2}ms", N, i,st.ElapsedMilliseconds);
                            result += string.Format("{0},{1},{2}\n", N, i, st.ElapsedMilliseconds);
                        }
                        else
                        {
                            Console.WriteLine("N={0}のとき、収束せず", N);
                            result += string.Format("{0},{1},{2}\n", N, -1, st.ElapsedMilliseconds);

                        }
                        if (N % 100 == 0)
                        {
                            using (FileStream fs = File.OpenWrite("resultG.csv"))
                            using (StreamWriter wr = new StreamWriter(fs))
                            {
                                fs.Seek(0, SeekOrigin.End);
                                wr.Write(result);
                                wr.Flush();
                            }
                            result = "";
                        }
                    }
                }
            }
        }

        private static void Experiment1()
        {
            string result = "";
            for (int n = 5; n <= 1000; n++)
            {
                Stopwatch st = new Stopwatch();
                st.Start();
                Matrix mat = new Matrix(new BasicMatrixElementBuilder(n, matrixGen));
                Vector eq = mat * Vector.One(n);
                JacobiCalculation calc = new JacobiCalculation(eq, mat);
                bool isOK = false;
                int i;
                for (i = 0; i < 10000; i++)
                {
                    calc.Next();
                    float relativeError = (calc.Answer - Vector.One(n)).TwoNorm() / Vector.One(n).TwoNorm();
                    if (relativeError < Epsilon)
                    {
                        isOK = true;
                        break;
                    }
                }
                st.Stop();
                if (isOK)
                {
                    Console.WriteLine("n={0}のとき、{1}回で収束。計算時間{2}ms", n, i, st.ElapsedMilliseconds);
                    result += string.Format("{0},{1},{2}\n",n,i,st.ElapsedMilliseconds);
                }
                else
                {
                    result += string.Format("{0},{1},{2}\n", n, -1, st.ElapsedMilliseconds);
                }
                if (n%100 == 0)
                {
                    using (FileStream fs=File.OpenWrite("result.csv"))
                    using (StreamWriter wr = new StreamWriter(fs))
                    {
                        fs.Seek(0, SeekOrigin.End);
                        wr.Write(result);
                        wr.Flush();
                    }
                    result = "";
                }
            }
        }

        private static float matrixGen(int i, int j)
        {
            int ab = Math.Abs(i - j);
            if (ab == 0)
            {
                return 9;
            }else if (ab == 1)
            {
                return 4;
            }
            else
            {
                return 0;
            }
        }


    }
}