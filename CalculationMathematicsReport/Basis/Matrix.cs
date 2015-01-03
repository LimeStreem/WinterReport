using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace CalculationMathematicsReport.Basis
{
    public struct Matrix
    {
        private float[][] _elements;

        public float[][] Elements
        {
            get { return _elements; }
            set
            {
                int size = value.Length;
                foreach (var element in value)
                {
                    if(size!=element.Length)throw new InvalidDataException("これは正方行列じゃないので扱うことはできません。");
                }
                _elements = value;
            }
        }

        public int Size
        {
            get { return Elements.Length; }
        }

        public Matrix(IMatrixElementBuilder builder):this()
        {
            var size = builder.GetSize();
            //配列の初期化
            float[][] elements=new float[size][];
            for (int i = 0; i < size; i++)
            {
                elements[i]=new float[size];
            }
            //内容の代入
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    elements[i][j] = builder.GetAt(i, j);
                }
            }
            this.Elements = elements;
        }

        public Matrix(int size, params float[] elements):this()
        {
            if(elements.Length!=size*size)throw new InvalidDataException("サイズが不正です");
            _elements = FloatArrayExtension.InstantinateSquareArray(size);
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    _elements[i][j] = elements[i*3 + j];
                }
            }
        }

        public Matrix(float[][] elements) : this()
        {
            Elements = elements;
        }

        public float[] this[int x]
        {
            get { return Elements[x]; }
            set
            {
                if (value.Length != Size) throw new InvalidDataException("これは正方行列じゃないので扱うことはできません。");
                _elements[x] = value;
            }
        }

        public float this[int x, int y]
        {
            get { return Elements[x][y]; }
            set { _elements[x][y] = value; }
        }

        public Matrix Transpose()
        {
            Matrix tmpThis = this;
            return new Matrix(new BasicMatrixElementBuilder(tmpThis.Size,(i,j)=>tmpThis[j,i]));
        }

        public static Matrix operator +(Matrix mat1, Matrix mat2)
        {
            if(mat1.Size!=mat2.Size)throw new InvalidDataException("この演算は定義されません");
            int size = mat1.Size;
            return new Matrix(new BasicMatrixElementBuilder(size,(i,j)=>mat1[i,j]+mat2[i,j]));
        }

        public static Matrix operator -(Matrix mat1, Matrix mat2)
        {
            if (mat1.Size != mat2.Size) throw new InvalidDataException("この演算は定義されません");
            int size = mat1.Size;
            return new Matrix(new BasicMatrixElementBuilder(size, (i, j) => mat1[i, j] - mat2[i, j]));
        }

        public static Matrix operator *(float sc, Matrix mat)
        {
            return new Matrix(new BasicMatrixElementBuilder(mat.Size,(i,j)=>sc*mat[i,j]));
        }

        public static Matrix operator *(Matrix mat1, Matrix mat2)
        {
            int size = mat1.Size;
            if (mat1.Size != mat2.Size) throw new InvalidDataException("この演算は定義されません");
            return new Matrix(new BasicMatrixElementBuilder(mat1.Size,(i,j)=>LamdaUtility.Sum(size,(k)=>mat1[i,k]*mat2[k,j])));
        }

        public static Vector operator *(Matrix mat, Vector vec)
        {
            int size = mat.Size;
            if (mat.Size != vec.Size) throw new InvalidDataException("この演算は定義されません");
            return new Vector(new BasicVectorElementBuilder(size,(i)=>LamdaUtility.Sum(size,(k)=>mat[i,k]*vec[k])));
        }

        public override string ToString()
        {
            Matrix tmpThis = this;
            return LamdaUtility.StringfySum(tmpThis.Size,
                (i) => "|" + LamdaUtility.StringfySum(tmpThis.Size, (j) => tmpThis[i, j].ToString() + " ") + "|\n");
        }
    }

    public interface IMatrixElementBuilder
    {
        float GetAt(int i, int j);

        int GetSize();
    }

    public class BasicMatrixElementBuilder:IMatrixElementBuilder
    {
        public int Size { get; private set; }

        public Func<int,int,float> Generator { get; private set; }

        public BasicMatrixElementBuilder(int size, Func<int, int, float> generator)
        {
            Size = size;
            Generator = generator;
        }

        public float GetAt(int i, int j)
        {
            return Generator(i, j);
        }

        public int GetSize()
        {
            return Size;
        }
    }
}
