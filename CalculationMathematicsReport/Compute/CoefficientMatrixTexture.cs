using System;
using System.IO;
using CalculationMathematicsReport.Basis;
using SlimDX;
using SlimDX.Direct3D11;
using SlimDX.DXGI;
using Device = SlimDX.Direct3D11.Device;
using Matrix = CalculationMathematicsReport.Basis.Matrix;

namespace CalculationMathematicsReport.Compute
{
    public class CoefficientMatrixTexture : IDisposable
    {
        public CoefficientMatrixTexture(Device device, IMatrixElementBuilder matBuilder)
        {
            var desc = new Texture2DDescription();
            desc.Width = desc.Height = matBuilder.GetSize();
            desc.ArraySize = 1;
            desc.CpuAccessFlags = CpuAccessFlags.None;
            desc.BindFlags = BindFlags.ShaderResource;
            desc.Format = Format.R32_Float;
            desc.OptionFlags = ResourceOptionFlags.None;
            desc.SampleDescription = new SampleDescription(1, 0);
            desc.Usage = ResourceUsage.Immutable;
            desc.MipLevels = 1;
            using (var ds = new DataStream(4*desc.Width*desc.Height, true, true))
            {
                for (var i = 0; i < desc.Width; i++)
                {
                    for (var j = 0; j < desc.Height; j++)
                    {
                        ds.Write(matBuilder.GetAt(i, j));
                    }
                }
                ds.Seek(0, SeekOrigin.Begin);
                var rect = new DataRectangle(4*desc.Width, ds);
                ImutableTexture2D = new Texture2D(device, desc, rect);
            }
            ShaderResourceView = new ShaderResourceView(device, ImutableTexture2D);
        }

        public CoefficientMatrixTexture(Device device, Matrix mat) : this(device, new MatrixCaster(mat))
        {
        }

        public Texture2D ImutableTexture2D { get; private set; }
        public ShaderResourceView ShaderResourceView { get; private set; }

        public void Dispose()
        {
            if (ImutableTexture2D != null && !ImutableTexture2D.Disposed) ImutableTexture2D.Dispose();
            if (ShaderResourceView != null && !ShaderResourceView.Disposed) ShaderResourceView.Dispose();
        }

        private class MatrixCaster : IMatrixElementBuilder
        {
            private readonly Matrix _mat;

            public MatrixCaster(Matrix mat)
            {
                _mat = mat;
            }

            public float GetAt(int i, int j)
            {
                return _mat[i, j];
            }

            public int GetSize()
            {
                return _mat.Size;
            }
        }
    }
}