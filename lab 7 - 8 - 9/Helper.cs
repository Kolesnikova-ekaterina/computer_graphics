using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace lab6_a
{
    public static class Helper
    {
        public static double[,] IdentityMatrix()
        {
            return new double[4, 4] { { 1, 0, 0, 0 }, { 0, 1, 0, 0 }, { 0, 0, 1, 0 }, { 0, 0, 0, 1 } };
        }
        public static double[,] Mult(this double[,] a, double[,] b)
        {
            return Form1.multipleMatrix(a, b);
        }
    }
}
