using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using CalculationMathematicsReport.Basis;
using CalculationMathematicsReport.CalcMath;
using CalculationMathematicsReport.Compute;
using CalculationMathematicsReport.Util;
using SlimDX.Direct3D11;
using SlimDX.DXGI;
using Device = SlimDX.Direct3D11.Device;

namespace CalculationMathematicsReport
{
    internal class Program
    {
        private const float Epsilon = 1.0E-5f;

        private static void Main(string[] args)
        {
            //Experiment1();
            //Experiment2();
            Experiment3(16000);
        }

        private static void Experiment3(int N)
        {
            using (var device = new Device(DriverType.Hardware, DeviceCreationFlags.None, FeatureLevel.Level_11_0))
            using (var compute = ComputeHelper.LoadComputeShader(device, "Compute\\Experiment3.hlsl", "main"))
            {
                var result = "";

                var mat = new Matrix(new BasicMatrixElementBuilder(N, matrixGen));
                var eq = mat*Vector.One(N);
                var x = Vector.Zero(N);
                Vector[] vecs=new Vector[100];
                using (var tex = new ReadableVectorTexture(device, Format.R32_Float, N))
                using (var evec = new EquationVectorTexture(device, eq))
                using (var matTex = new CoefficientMatrixTexture(device, mat))
                using (var constants = new ArgumentConstants(device, (uint) N))
                {
                    device.ImmediateContext.ComputeShader.Set(compute);
                    device.ImmediateContext.ComputeShader.SetUnorderedAccessView(tex.UnorderedAccessView, 0);
                    var isOK = false;
                    int i;
                    for (i = 0; i < 10000; i++)
                    {
                        var st = new Stopwatch();
                        st.Start();
                        using (var lastX = new EquationVectorTexture(device, x))
                        {
                            device.ImmediateContext.ComputeShader.SetShaderResources(
                                new[] {evec.ShaderResourceView, matTex.ShaderResourceView, lastX.ShaderResourceView}, 0,
                                3);
                            device.ImmediateContext.ComputeShader.SetConstantBuffers(
                                new[] {constants.ArgumentConstantBuffer}, 0, 1);
                            device.ImmediateContext.Dispatch(1024,1,1);
                            x = tex.ToVector();
                            var relativeError = (x - Vector.One(N)).TwoNorm()/Vector.One(N).TwoNorm();
                            st.Stop();
                            Console.WriteLine("{0}回目の反復:相対誤差{1}計算時間{2}",i,relativeError,st.ElapsedMilliseconds);
                            result += string.Format("{0},{1},{2}\n",i,relativeError,st.ElapsedMilliseconds);
                            if (relativeError < Epsilon)
                            {
                                isOK = true;
                                break;
                            }
                        }
                    }
                    //VectorVisualizeTexture vtex=new VectorVisualizeTexture(device,vecs,i,N);
                    //Texture2D.ToFile(device.ImmediateContext, vtex.VisualizedTexture, ImageFileFormat.Jpg, "test.png");
//
//                    if (isOK)
//                    {
//                        Console.WriteLine("N={0}のとき、{1}回で収束。{2}ms", N, i, st.ElapsedMilliseconds);
//                        result += string.Format("{0},{1},{2}\n", N, i, st.ElapsedMilliseconds);
//                    }
//                    else
//                    {
//                        Console.WriteLine("N={0}のとき、収束せず", N);
//                        result += string.Format("{0},{1},{2}\n", N, -1, st.ElapsedMilliseconds);
//                    }
                        using (var fs = File.OpenWrite("resultG-"+N+".csv"))
                        using (var wr = new StreamWriter(fs))
                        {
                            fs.Seek(0, SeekOrigin.End);
                            wr.Write(result);
                            wr.Flush();
                        }
                }
            }
        }

        private static void Experiment2()
        {
            using (var device = new Device(DriverType.Hardware, DeviceCreationFlags.None, FeatureLevel.Level_11_0))
            using (var compute = ComputeHelper.LoadComputeShader(device, "Compute\\Experiment3.hlsl", "main"))
            {
                var result = "";
                for (var N = 5; N <= 1000; N++)
                {
                    var st = new Stopwatch();
                    st.Start();
                    var mat = new Matrix(new BasicMatrixElementBuilder(N, matrixGen));
                    var eq = mat*Vector.One(N);
                    var x = Vector.Zero(N);
                    using (var tex = new ReadableVectorTexture(device, Format.R32_Float, N))
                    using (var evec = new EquationVectorTexture(device, eq))
                    using (var matTex = new CoefficientMatrixTexture(device, mat))
                    using (var constants = new ArgumentConstants(device, (uint) N))
                    {
                        device.ImmediateContext.ComputeShader.Set(compute);
                        device.ImmediateContext.ComputeShader.SetUnorderedAccessView(tex.UnorderedAccessView, 0);
                        var isOK = false;
                        int i;
                        for (i = 0; i < 10000; i++)
                        {
                            using (var lastX = new EquationVectorTexture(device, x))
                            {
                                device.ImmediateContext.ComputeShader.SetShaderResources(
                                    new[]
                                    {evec.ShaderResourceView, matTex.ShaderResourceView, lastX.ShaderResourceView}, 0, 3);
                                device.ImmediateContext.ComputeShader.SetConstantBuffers(
                                    new[] {constants.ArgumentConstantBuffer}, 0, 1);
                                device.ImmediateContext.Dispatch(N, 1, 1);
                                x = tex.ToVector();
                                var relativeError = (x - Vector.One(N)).TwoNorm()/Vector.One(N).TwoNorm();
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
                            Console.WriteLine("N={0}のとき、{1}回で収束。{2}ms", N, i, st.ElapsedMilliseconds);
                            result += string.Format("{0},{1},{2}\n", N, i, st.ElapsedMilliseconds);
                        }
                        else
                        {
                            Console.WriteLine("N={0}のとき、収束せず", N);
                            result += string.Format("{0},{1},{2}\n", N, -1, st.ElapsedMilliseconds);
                        }
                        if (N%100 == 0)
                        {
                            using (var fs = File.OpenWrite("resultG.csv"))
                            using (var wr = new StreamWriter(fs))
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
            var result = "";
            for (var n = 5; n <= 1000; n++)
            {
                var st = new Stopwatch();
                st.Start();
                var mat = new Matrix(new BasicMatrixElementBuilder(n, matrixGen));
                var eq = mat*Vector.One(n);
                var calc = new JacobiCalculation(eq, mat);
                var isOK = false;
                int i;
                for (i = 0; i < 10000; i++)
                {
                    calc.Next();
                    var relativeError = (calc.Answer - Vector.One(n)).TwoNorm()/Vector.One(n).TwoNorm();
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
                    result += string.Format("{0},{1},{2}\n", n, i, st.ElapsedMilliseconds);
                }
                else
                {
                    result += string.Format("{0},{1},{2}\n", n, -1, st.ElapsedMilliseconds);
                }
                if (n%100 == 0)
                {
                    using (var fs = File.OpenWrite("result.csv"))
                    using (var wr = new StreamWriter(fs))
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
            var ab = Math.Abs(i - j);
            if (ab == 0)
            {
                return 9;
            }
            if (ab == 1)
            {
                return 4;
            }
            return 0;
        }
    }
}