using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CalculationMathematicsReport.Basis;
using SlimDX;
using SlimDX.Direct3D11;
using SlimDX.Direct3D9;
using Device = SlimDX.Direct3D11.Device;
using Format = SlimDX.DXGI.Format;
using Matrix = CalculationMathematicsReport.Basis.Matrix;

namespace CalculationMathematicsReport.Compute
{
    public class EquationVectorTexture:IDisposable
    {
        public Texture1D ImutableTexture1D { get; private set; }

        public ShaderResourceView ShaderResourceView { get; private set; }

        public EquationVectorTexture(Device device,IVectorElementBuilder vec)
        {
            Texture1DDescription desc=new Texture1DDescription();
            desc.ArraySize = 1;
            desc.Width = vec.GetSize();
            desc.BindFlags = BindFlags.ShaderResource;
            desc.CpuAccessFlags=CpuAccessFlags.None;
            desc.Format=Format.R32_Float;
            desc.MipLevels = 1;
            desc.OptionFlags=ResourceOptionFlags.None;
            desc.Usage=ResourceUsage.Immutable;
            using (DataStream ds=new DataStream(desc.Width*4,true,true))
            {
                for (int i = 0; i < desc.Width; i++)
                {
                    ds.Write(vec.GetAt(i));
                }
                ds.Seek(0, SeekOrigin.Begin);
                ImutableTexture1D = new Texture1D(device, desc,ds);
            }
            ShaderResourceView=new ShaderResourceView(device,ImutableTexture1D);
        }

        public EquationVectorTexture(Device device,Vector vec):this(device,new VectorCaster(vec))
        {
            
        }

        public void Dispose()
        {
            if (ImutableTexture1D != null && !ImutableTexture1D.Disposed) ImutableTexture1D.Dispose();
            if (ShaderResourceView != null && !ShaderResourceView.Disposed) ShaderResourceView.Dispose();
        }

        private class VectorCaster:IVectorElementBuilder
        {
            private readonly Vector _vec;

            public VectorCaster(Vector vec)
            {
                _vec = vec;
            }

            public int GetSize()
            {
                return _vec.Size;
            }

            public float GetAt(int i)
            {
                return _vec[i];
            }
        }
    }
}
