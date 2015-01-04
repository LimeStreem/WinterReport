using System;

namespace CalculationMathematicsReport.Basis
{
    public static class FloatArrayExtension
    {
        public static float Sum(this float[] array)
        {
            var ret = 0f;
            foreach (var f in array)
            {
                ret += f;
            }
            return ret;
        }

        public static float Pow2Sum(this float[] array)
        {
            float ret = 0;
            foreach (var f in array)
            {
                ret += f*f;
            }
            return ret;
        }

        public static float MaxElement(this float[] array)
        {
            var ret = float.MinValue;
            foreach (var f in array)
            {
                ret = Math.Max(f, ret);
            }
            return ret;
        }

        public static float MinElement(this float[] array)
        {
            var ret = float.MaxValue;
            foreach (var f in array)
            {
                ret = Math.Min(f, ret);
            }
            return ret;
        }

        public static float[] ElementAdd(float[] array1, float[] array2)
        {
            var array3 = new float[array1.Length];
            for (var i = 0; i < array1.Length; i++)
            {
                array3[i] = array1[i] + array2[i];
            }
            return array3;
        }

        public static float[] ElementSubtract(float[] array1, float[] array2)
        {
            var array3 = new float[array1.Length];
            for (var i = 0; i < array1.Length; i++)
            {
                array3[i] = array1[i] - array2[i];
            }
            return array3;
        }

        public static float[][] InstantinateSquareArray(int size)
        {
            var ret = new float[size][];
            for (var i = 0; i < size; i++)
            {
                ret[i] = new float[size];
            }
            return ret;
        }
    }
}