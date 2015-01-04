using System;
using CalculationMathematicsReport.Basis;
using SlimDX.Direct3D11;
using SlimDX.DXGI;
using Device = SlimDX.Direct3D11.Device;
using MapFlags = SlimDX.Direct3D11.MapFlags;

namespace CalculationMathematicsReport.Compute
{
    public class ReadableVectorTexture : IDisposable
    {
        private readonly Device _device;
        private readonly int _width;

        public ReadableVectorTexture(Device device, Format texFormat, int width)
        {
            _device = device;
            _width = width;
            var desc = new Texture1DDescription();
            desc.ArraySize = 1;
            desc.BindFlags = BindFlags.UnorderedAccess;
            desc.CpuAccessFlags = CpuAccessFlags.None;
            desc.Format = texFormat;
            desc.MipLevels = 1;
            desc.OptionFlags = ResourceOptionFlags.None;
            desc.Usage = ResourceUsage.Default;
            desc.Width = width;
            DefaultUsageTexture1D = new Texture1D(device, desc);
            UnorderedAccessView = new UnorderedAccessView(device, DefaultUsageTexture1D);
            desc.BindFlags = BindFlags.None;
            desc.Usage = ResourceUsage.Staging;
            desc.CpuAccessFlags = CpuAccessFlags.Read;
            StagingUsageTexture1D = new Texture1D(device, desc);
        }

        public UnorderedAccessView UnorderedAccessView { get; private set; }
        public Texture1D DefaultUsageTexture1D { get; private set; }
        public Texture1D StagingUsageTexture1D { get; private set; }

        public void Dispose()
        {
            if (DefaultUsageTexture1D != null && !DefaultUsageTexture1D.Disposed) DefaultUsageTexture1D.Dispose();
            if (StagingUsageTexture1D != null && !StagingUsageTexture1D.Disposed) StagingUsageTexture1D.Dispose();
            if (UnorderedAccessView != null && !UnorderedAccessView.Disposed) UnorderedAccessView.Dispose();
        }

        public VectorTextureDataWrapper MapResource()
        {
            _device.ImmediateContext.CopyResource(DefaultUsageTexture1D, StagingUsageTexture1D);
            var mapSubresource = _device.ImmediateContext.MapSubresource(StagingUsageTexture1D, 0, 0, MapMode.Read,
                MapFlags.None);
            return new VectorTextureDataWrapper(mapSubresource);
        }

        public void UnMapResource()
        {
            _device.ImmediateContext.UnmapSubresource(StagingUsageTexture1D, 0);
        }

        public Vector ToVector()
        {
            var data = MapResource();
            var ret = new Vector(new BasicVectorElementBuilder(_width, i => data[i]));
            UnMapResource();
            return ret;
        }
    }
}