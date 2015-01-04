using SlimDX;

namespace CalculationMathematicsReport.Compute
{
    /// <summary>
    ///     1次元データの読みだし
    /// </summary>
    public class VectorTextureDataWrapper
    {
        private readonly DataBox _dbox;

        public VectorTextureDataWrapper(DataBox dbox)
        {
            _dbox = dbox;
        }

        public unsafe float this[int x]
        {
            get { return ((float*) _dbox.Data.DataPointer)[x]; }
        }
    }
}