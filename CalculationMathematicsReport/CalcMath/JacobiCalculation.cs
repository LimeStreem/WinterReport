using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CalculationMathematicsReport.Basis;

namespace CalculationMathematicsReport.CalcMath
{
    public class JacobiCalculation
    {
        public Vector Equation { get; private set; }

        public int Iteration { get; private set; }

        public Matrix Coefficients { get; private set; }

        public Vector Answer { get; private set; }

        public Matrix MInv { get; private set; }

        public Matrix N { get; private set; }

        public JacobiCalculation(Vector equation, Matrix coefficients)
        {
            Equation = equation;
            Coefficients = coefficients;
            Answer = Vector.Zero(equation.Size);
            MInv=new Matrix(new BasicMatrixElementBuilder(equation.Size,(i,j)=>i==j?1f/coefficients[i,j]:0f));
            N=-1*MInv*new Matrix(new BasicMatrixElementBuilder(equation.Size,(i,j)=>i!=j?coefficients[i,j]:0));
        }

        public Vector Next()
        {
            Iteration++;
            Answer = N*Answer + MInv*Equation;
            return Answer;
        }

        
    }
}
