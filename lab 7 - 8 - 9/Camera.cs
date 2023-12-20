using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static lab6_a.Form1;

namespace lab6_a
{
    public class Camera
    {
        public PointD position;
        public PointD positionInCameraCoordinates { private set; get; } = new PointD(0, 0, -200);
        public Vector view;
        public double[,] matrixPerspectieve = new double[4, 4] { { 1, 0, 0, 0 }, { 0, 1, 0, 0 }, { 0, 0, 0, -1f / -200 }, { 0, 0, 0, 1 } };
        //координаты камеры
        public double[,] matrixToCamera = Helper.IdentityMatrix();
        static float teta = (float)(5 * Math.PI / 180.0);
        public Camera()
        {
            position = new PointD(0, 0, -200);
            view = new Vector(0, 0, -1);
        }


        public void DrawPerspective(Graphics g, List<PointD> originalPoints, List<Line> list_lines, double[,] toWorldMatr, Vector shift)
        {
            var newPoints = Perspective(originalPoints, toWorldMatr);
            Draw(g, newPoints, list_lines, shift);
        }

        public void DrawAxonometric(Graphics g, List<PointD> originalPoints, List<Line> list_lines, double[,] toWorldMatr, Vector shift)
        {
            var newPoints = Parallel(originalPoints, toWorldMatr);
            Draw(g, newPoints, list_lines, shift);
        }


        private void Draw(Graphics g, List<PointD> newPoints, List<Line> list_lines, Vector shift)
        {
            for (int i = 0; i < list_lines.Count(); i++)
            {
                if (!list_lines[i].isvisible)
                    continue;
                Point a = new Point((int)(newPoints[list_lines[i].a].x) + (int)shift.y / 3, (int)(newPoints[list_lines[i].a].y) + (int)shift.x / 3);
                Point b = new Point((int)(newPoints[list_lines[i].b].x) + (int)shift.y / 3, (int)(newPoints[list_lines[i].b].y) + (int)shift.x / 3);
                g.DrawLine(new Pen(Color.Black, 2.0f), a, b);
            }
        }

        public void DrawWithoutNonVisible(Graphics g, Polyhedra polyhedra, double[,] matrixToWorld, Vector shift)
        {
            polyhedra.findcenter();
            var matr = matrixToWorld.Mult(matrixToCamera);
            var pen = new Pen(Color.Black);
            foreach (var poligon in polyhedra.polygons)
            {
                var result = IsVisible(poligon, matr,polyhedra);
                if (result.Item4)
                {
                    var p1 = result.Item1;
                    var p2 = result.Item2;
                    var p3 = result.Item3;
                    g.DrawLine(pen, new Point((int)p1.x + (int)shift.y / 3, (int)p1.y + (int)shift.x / 3), new Point((int)p2.x + (int)shift.y / 3, (int)p2.y + (int)shift.x / 3));
                    g.DrawLine(pen, new Point((int)p2.x + (int)shift.y / 3, (int)p2.y + (int)shift.x / 3), new Point((int)p3.x + (int)shift.y / 3, (int)p3.y + (int)shift.x / 3));
                    g.DrawLine(pen, new Point((int)p1.x + (int)shift.y / 3, (int)p1.y + (int)shift.x / 3), new Point((int)p3.x + (int)shift.y / 3, (int)p3.y + (int)shift.x / 3));
                }
            }
        }

        private (PointD, PointD, PointD, bool) IsVisible(Polygon poligon, double[,] matrix, Polyhedra polyhedra)
        {
            var pp1 = new double[1, 4] { { Form1.list_points[poligon.lines[0].a].x, Form1.list_points[poligon.lines[0].a].y, Form1.list_points[poligon.lines[0].a].z, 1 } }.Mult( matrix);
            var pp2 = new double[1, 4] { { Form1.list_points[poligon.lines[1].a].x, Form1.list_points[poligon.lines[1].a].y, Form1.list_points[poligon.lines[1].a].z, 1 } }.Mult(matrix);
            var pp3 = new double[1, 4] { { Form1.list_points[poligon.lines[2].a].x, Form1.list_points[poligon.lines[2].a].y, Form1.list_points[poligon.lines[2].a].z, 1 } }.Mult(matrix);


            var tmpCenter = new double[1, 4] { { polyhedra.center.x, polyhedra.center.y, polyhedra.center.z, 1 } }.Mult(matrix);

            var p3 = new PointD(pp3[0, 0], pp3[0, 1], pp3[0, 2]);
            var p2 = new PointD(pp2[0, 0], pp2[0, 1], pp2[0, 2]);
            var p1 = new PointD(pp1[0, 0], pp1[0, 1], pp1[0, 2]);

            var center = new PointD(tmpCenter[0,0], tmpCenter[0, 1], tmpCenter[0, 1]);

            //since encoding acts weirdly when I commit to github, I'll leave a comment in English
            //got it from some article on calculating a surface normal - it works given that face is a triangle
            double nx = (p2.y - p1.y) * (p3.z - p1.z) - (p2.z - p1.z) * (p3.y - p1.y);
            double ny = (p2.z - p1.z) * (p3.x - p1.x) - (p2.x - p1.x) * (p3.z - p1.z);
            double nz = (p2.x - p1.x) * (p3.y - p1.y) - (p2.y - p1.y) * (p3.x - p1.x);

            //var cam_pos = _viewer.Position;

            //var center = Form1.list_points.GetCenter();
            Vector3 vec = new Vector3((float)(positionInCameraCoordinates.x - center.x), (float)(positionInCameraCoordinates.y - center.y), (float)(positionInCameraCoordinates.z - center.z));
            Vector3 normal = new Vector3((float)nx, (float)ny, (float)nz);
            normal = Vector3.Normalize(normal);

            var cross = Vector3.Cross(vec, normal);
            var dot = Vector3.Dot(vec, normal);

            var angle = Math.PI - Math.Atan2(cross.Length(), dot);
            angle = angle * 360 / (2 * Math.PI);
            //Debug.WriteLine((float)angle);

            return (p1, p2, p3, angle < 90);
        }



        private List<PointD> Perspective(List<PointD> list_points, double[,] toWorldMatr)
        {
            //для пересчета точек когда сдвигаем камеру (сохраненеие точек после того, как умножили на соответствующие матрицы)
            var newimage = new List<PointD>();
            //
            double[,] helpMatrix = toWorldMatr.Mult(matrixToCamera).Mult(this.matrixPerspectieve);
            //каждую точку представляем так, как увидит камера ее
            for (int i = 0; i < list_points.Count; i++)
            {
                //представляем точку в виде матрицы, чтобы умножить потом на матрицы
                double[,] matrixPoint = new double[1, 4] { { list_points[i].x, list_points[i].y, list_points[i].z, 1.0 } };
                var res = matrixPoint.Mult(helpMatrix);
                //центральная проекция, так надо, было в презенташке у Яны
                double c = 10.0;
                res[0, 0] /= 1.0 - res[0, 3] / c;
                res[0, 1] /= 1.0 - res[0, 3] / c;
                //сохраняем пересчитанные точки, чтобы отрисовать потом
                newimage.Add(new PointD(res[0, 0], res[0, 1], res[0, 2]));
            }
            return newimage;
        }

        private List<PointD> Parallel(List<PointD> list_points, double[,] toWorldMatr)
        {
            double[,] helpMatrix = toWorldMatr.Mult(matrixToCamera);
            var newimage = new List<PointD>();
            for (int i = 0; i < list_points.Count; i++)
            {
                double[,] matrixPoint = new double[1, 4] { { list_points[i].x, list_points[i].y, list_points[i].z, 1.0 } };

                var res = matrixPoint.Mult(helpMatrix);

                newimage.Add(new PointD(res[0, 0], res[0, 1], res[0, 2]));
            }

            return newimage;
        }

        void RotateCamera(double[,] matrixRotate)
        {

            matrixToCamera = matrixToCamera.Mult(matrixRotate);
            double[,] matrixPoint = new double[1, 4] { { position.x, position.y, position.z, 1.0 } };

            var res = matrixPoint.Mult(matrixRotate);
            position = new PointD(res[0, 0], res[0, 1], res[0, 2]);
            Console.WriteLine($"pos: {position.x} {position.y} {position.z}");
            //вектор направления взгляда камеры
            view = new Vector(position.x, position.y, position.z);
            view.reverse();
            view.Normalize();
            Console.WriteLine($"view: {view.x} {view.y} {view.z}");

        }

        public void RotateDown()
        {
            var currentRotate = Helper.IdentityMatrix();
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
            view = new Vector(0, 0, -1);    

        }
    }
}
