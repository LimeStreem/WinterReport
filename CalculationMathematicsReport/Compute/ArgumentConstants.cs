using System;
using System.Runtime.InteropServices;
using SlimDX;
using SlimDX.Direct3D11;
using Buffer = SlimDX.Direct3D11.Buffer;

namespace CalculationMathematicsReport.Compute
{
    public class ArgumentConstants : IDisposable
    {
        public ArgumentConstants(Device device, uint n)
        {
            using (var ds = new DataStream(ArgumentSize, true, true))
            {
                var str = new ArgumentStructure();
                str.N = n;
                ds.Write(str);
                ds.Position = 0;
                ArgumentConstantBuffer = new Buffer(device, ds, new BufferDescription
                {
                    BindFlags = BindFlags.ConstantBuffer,
                    Usage = ResourceUsage.Default,
                    SizeInBytes = ArgumentSize,
                    CpuAccessFlags = CpuAccessFlags.None,
                    OptionFlags = ResourceOptionFlags.None
                });
                //device.ImmediateContext.UpdateSubresource(new DataBox(0, 0, ds), ArgumentConstantBuffer,0);
            }
        }

        public Buffer ArgumentConstantBuffer { get; private set; }

        public int ArgumentSize
        {
            get { return Marshal.SizeOf(typeof (ArgumentStructure)); }
        }

        public void Dispose()
        {
            if (ArgumentConstantBuffer != null && !ArgumentConstantBuffer.Disposed) ArgumentConstantBuffer.Dispose();
        }

        private struct ArgumentStructure
        {
            public uint A1;
            public uint A2;
            public uint A3;
            public uint N;
        }
    }
}