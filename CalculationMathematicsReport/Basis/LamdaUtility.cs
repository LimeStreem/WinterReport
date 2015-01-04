using System;

namespace CalculationMathematicsReport.Basis
{
    public static class LamdaUtility
    {
        public static float Sum(int n, Func<int, float> nFunc)
        {
            float sum = 0;
            for (var i = 0; i < n; i++)
            {
                sum += nFunc(i);
            }
            return sum;
        }

        public static float[] ArraySet(int size, Func<int, float> nFunc)
        {
            var arr = new float[size];
            for (var q = 0; q < size; q++)
            {
                arr[q] = nFunc(q);
            }
            return arr;
        }

        public static string StringfySum(int size, Func<int, string> nFunc)
        {
            var st = "";
            for (var i = 0; i < size; i++)
            {
                st += nFunc(i);
            }
            return st;
        }
    }
}