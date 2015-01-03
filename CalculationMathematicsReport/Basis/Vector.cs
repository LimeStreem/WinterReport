using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CalculationMathematicsReport.Basis
{
    public struct Vector
    {

        public static Vector Zero(int size)
        {
            Vector vec=new Vector();
            vec.Elements=new float[size];
            return vec;
        }

        public static Vector One(int size)
        {
            Vector vec=new Vector();
            vec.Elements=new float[size];
            for (int i = 0; i < size; i++)
            {
                vec.Elements[i] = 1f;
            }
            return vec;
        }

        public float this[int x]
        {
            get { return Elements[x]; }
            set { Elements[x] = value; }
        }

        public float[] Elements { get; private set; }

        public int Size
        {
            get { return Elements.Length; }
        }

        public Vector(params float[] elements) : this()
        {
            Elements=new float[elements.Length];
            Array.Copy(elements,Elements,elements.Length);
        }

        public Vector(IVectorElementBuilder builder) : this()
        {
            Elements=new float[builder.GetSize()];
            for (int i = 0; i < Size; i++)
            {
                Elements[i] = builder.GetAt(i);
            }
        }

        public float TwoNorm()
        {
            return (float) Math.Sqrt(Elements.Pow2Sum());
        }

        public float OneNorm()
        {
            return (float) Elements.Sum();
        }

        public float InfNorm()
        {
            return Elements.MaxElement();
        }

        public static Vector operator +(Vector a, Vector b)
        {
            return new Vector(FloatArrayExtension.ElementAdd(a.Elements, b.Elements));
        }

        public static Vector operator -(Vector a, Vector b)
        {
            return new Vector(FloatArrayExtension.ElementSubtract(a.Elements,b.Elements));
        }

        public static Vector operator *(float sc, Vector vec)
        {
            return new Vector(new BasicVectorElementBuilder(vec.Size,(i)=>sc*vec[i]));
        }

        public override string ToString()
        {
            Vector tmpThis = this;
            return "(" + LamdaUtility.StringfySum(tmpThis.Size, (i) => (tmpThis[i].ToString() + ",")) + "){Norm:" +
                   TwoNorm()+"}";
        }
    }

    public interface IVectorElementBuilder
    {
        int GetSize();

        float GetAt(int i);
    }

    public class BasicVectorElementBuilder:IVectorElementBuilder
    {
        public int Size { get; private set; }

        public Func<int,float> Generator { get; private set; }

        public BasicVectorElementBuilder(int size, Func<int, float> generator)
        {
            Size = size;
            Generator = generator;
        }

        public int GetSize()
        {
            return Size;
        }

        public float GetAt(int i)
        {
            return Generator(i);
        }
    }
}
