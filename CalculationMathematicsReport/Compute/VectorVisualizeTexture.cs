using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CalculationMathematicsReport.Basis;
using SlimDX;
using SlimDX.Direct3D11;
using SlimDX.DXGI;
using Device = SlimDX.Direct3D11.Device;

namespace CalculationMathematicsReport.Compute
{
    public class VectorVisualizeTexture:IDisposable
    {
        public Texture2D VisualizedTexture { get; private set; }

        public VectorVisualizeTexture(Device device,Vector[] vector,int count,int n)
        {
            Texture2DDescription desc=new Texture2DDescription();
            desc.Width = n;
            desc.Height = count;
            desc.Usage = ResourceUsage.Staging;
            desc.SampleDescription=new SampleDescription(1,0);
            desc.Format = Format.R8G8B8A8_UNorm;
            desc.MipLevels = 1;
            desc.OptionFlags=ResourceOptionFlags.None;
            desc.BindFlags = BindFlags.None;
            desc.ArraySize = 1;
            desc.CpuAccessFlags = CpuAccessFlags.Read;
            using (DataStream ds=new DataStream(4*4*count*n,true,true))
            {
                for (int i = 0; i < count; i++)
                {
                    for (int j = 0; j < n; j++)
                    {
                        float val = vector[i][j];
                        byte[] b = new byte[] {(byte) (val*255), (byte) ((-val)*255), 0, 255};
                        ds.WriteRange(b);
                    }
                }
                ds.Position = 0;
                DataRectangle rect=new DataRectangle(4*count*4,ds);
                VisualizedTexture = new Texture2D(device, desc,rect);
            }
        }

        public void Dispose()
        {
        
        }
    }
}
