using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace lab6_a
{
    public class PointD
    {
        public double x;
        public double y;
        public double z;

        public PointD(double xx, double yy, double zz)
        {
            x = xx;
            y = yy;
            z = zz;
        }
        public double dist(PointD otherpoint)
        {
            return Math.Sqrt(Math.Pow(x - otherpoint.x, 2) + Math.Pow(y - otherpoint.y, 2) + Math.Pow(z - otherpoint.z, 2));
        }

        public Vector3 ToVector3()
        {
            return new Vector3((float)x, (float)y, (float)z);
        }
    }
}
