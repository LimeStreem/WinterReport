using System;
using System.Windows.Forms;
using SlimDX.D3DCompiler;
using SlimDX.Direct3D11;
using SlimDX.DXGI;
using Device = SlimDX.Direct3D11.Device;

namespace CalculationMathematicsReport.Util
{

    public static class ComputeHelper
    {

        public static ComputeShader LoadComputeShader(Device device, string filename, string entryPoint)
        {
            try
            {
                string errors;
                var shaderByteCode = ShaderBytecode.CompileFromFile(filename, entryPoint, "cs_5_0", ShaderFlags.None,
                    EffectFlags.None, null, null, out errors);
                if (!string.IsNullOrEmpty(errors))
                {
                    MessageBox.Show(errors, "Shader compilation errors");
                }
                return new ComputeShader(device, shaderByteCode);
            }
            catch (Exception e)
            {
                if (e.InnerException != null)
                    MessageBox.Show(e.Message + "\nInner: " + e.InnerException.Message, e.Source + " Error!");
                else
                    MessageBox.Show(
                        e.Message + "\n\nTrace:" + e.StackTrace + "\n\nData: " + e.Data + "\n\nType: " + e.GetType(),
                        e.Source + " Error!");

                return null;
            }
        }

    }
}