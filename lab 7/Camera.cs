using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static lab6_a.Form1;

namespace lab6_a
{
    public class Camera
    {
        public PointD position;
        public Vector view;
        public double[,] matrixPerspectieve = new double[4, 4] { { 1, 0, 0, 0 }, { 0, 1, 0, 0 }, { 0, 0, 0, -1f / -200 }, { 0, 0, 0, 1 } };
        double[,] matrixToCamera = new double[4, 4] { { 1, 0, 0, 0 }, { 0, 1, 0, 0 }, { 0, 0, 1, 0 }, { 0, 0, 0, 1 } };
        static float teta = (float)(5 * Math.PI / 180.0);
        public Camera()
        {
            position = new PointD(0, 0, -200);
            view = new Vector (0, 0, 1);
        }

        public void Perspective(Graphics g, List<PointD> list_points, List<Line> list_lines, Vector shift) 
        {

            var newimage = new List<PointD>();
            double[,] helpMatrix = multipleMatrix(matrixToCamera, this.matrixPerspectieve);
            for (int i = 0; i < list_points.Count; i++)
            {
                double[,] matrixPoint = new double[1, 4] { { list_points[i].x, list_points[i].y, list_points[i].z, 1.0 } };
                var res = (multipleMatrix(matrixPoint, helpMatrix));
                double c = 10.0;
                res[0, 0] /= 1.0 - res[0, 3] / c;
                res[0, 1] /= 1.0 - res[0, 3] / c;
                newimage.Add(new PointD(res[0, 0], res[0, 1], res[0, 2]));
            }

            for (int i = 0; i < list_lines.Count(); i++)
            {
                if (!list_lines[i].isvisible)
                    continue;
                Point a = new Point((int)(newimage[list_lines[i].a].x) + (int)shift.y / 3, (int)(newimage[list_lines[i].a].y) + (int)shift.x / 3);
                Point b = new Point((int)(newimage[list_lines[i].b].x) + (int)shift.y / 3, (int)(newimage[list_lines[i].b].y) + (int)shift.x / 3);
                g.DrawLine(new Pen(Color.Black, 2.0f), a, b);
            }
        }

        void RotateCamera(double[,] matrixRotate)
        {

            matrixToCamera = multipleMatrix(matrixToCamera, matrixRotate);
            double[,] matrixPoint = new double[1, 4] { { position.x, position.y, position.z, 1.0 } };

            var res = (multipleMatrix(matrixPoint, matrixRotate));
            position = new PointD(res[0, 0], res[0, 1], res[0, 2]);
            Console.WriteLine($"pos: {position.x} {position.y} {position.z}");

            view = new Vector(position.x, position.y, position.z);
            view.reverse();
            view.Normalize();
            Console.WriteLine($"view: {view.x} {view.y} {view.z}");

        }

         public void RotateDown ()
        {
            var currentRotate = new double[4, 4] { { 1, 0, 0, 0 }, { 0, 1, 1, 0 }, { 0, 1, 1, 0 }, { 0, 0, 0, 1 } };
            currentRotate[1, 1] = Math.Cos(teta);
            currentRotate[1, 2] = Math.Sin(teta);
            currentRotate[2, 1] = -Math.Sin(teta);
            currentRotate[2, 2] = Math.Cos(teta);
            RotateCamera(currentRotate);
            
        }
       public void RotateUp()
        {
            var currentRotate = new double[4, 4] { { 1, 0, 0, 0 }, { 0, 1, 1, 0 }, { 0, 1, 1, 0 }, { 0, 0, 0, 1 } };
            currentRotate[1, 1] = Math.Cos(-teta);
            currentRotate[1, 2] = Math.Sin(-teta);
            currentRotate[2, 1] = -Math.Sin(-teta);
            currentRotate[2, 2] = Math.Cos(-teta);
            RotateCamera(currentRotate);
        }
        public void RotateLeft()
        {
            var currentRotate = new double[4, 4] { { 1, 0, 1, 0 }, { 0, 1, 0, 0 }, { 1, 0, 1, 0 }, { 0, 0, 0, 1 } }; ;
            currentRotate[0, 0] = Math.Cos(teta);
            currentRotate[0, 2] = -Math.Sin(teta);
            currentRotate[2, 0] = Math.Sin(teta);
            currentRotate[2, 2] = Math.Cos(teta);
            RotateCamera(currentRotate);
        }
        public void RotateRight()
        {
            var currentRotate = new double[4, 4] { { 1, 0, 1, 0 }, { 0, 1, 0, 0 }, { 1, 0, 1, 0 }, { 0, 0, 0, 1 } }; ;
            currentRotate[0, 0] = Math.Cos(-teta);
            currentRotate[0, 2] = -Math.Sin(-teta);
            currentRotate[2, 0] = Math.Sin(-teta);
            currentRotate[2, 2] = Math.Cos(-teta);
            RotateCamera(currentRotate);
        }
        public void RefreshPos()
        {
            matrixToCamera = new double[4, 4] { { 1, 0, 0, 0 }, { 0, 1, 0, 0 }, { 0, 0, 1, 0 }, { 0, 0, 0, 1 } };
            position = new PointD(0, 0, -200);
            view = new Vector(0, 0, 1);

        }
    }
}
