using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CalculationMathematicsReport.Basis
{
    public static class LamdaUtility
    {
        public static float Sum(int n, Func<int, float> nFunc)
        {
            float sum = 0;
            for (int i = 0; i < n; i++)
            {
                sum += nFunc(i);
            }
            return sum;
        }

        public static float[] ArraySet(int size, Func<int, float> nFunc)
        {
            float[] arr = new float[size];
            for (int q = 0; q < size; q++)
            {
                arr[q] = nFunc(q);
            }
            return arr;
        }

        public static string StringfySum(int size, Func<int, string> nFunc)
        {
            string st = "";
            for (int i = 0; i < size; i++)
            {
                st += nFunc(i);
            }
            return st;
        }
    }
}
