using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;

namespace lab6_a
{
    public partial class Form1 : Form
    {
        List<PointD> list_points;
        List<Line> list_lines;
        List<Polygon> list_pols;
        List<PointD> line_axis; 
        List<PointD> line_polyline;
        bool is_draw = false;
        bool draw_axis = false;
        bool draw_polyline = false;
        int numb_vert = 0;
        public Form1()
        {
            InitializeComponent();
            list_points = new List<PointD>();
            list_lines = new List<Line>();
            list_pols = new List<Polygon>();

            line_axis = new List<PointD>();
            line_polyline = new List<PointD>();
            Cube();

        }


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
        }


        public class Line
        {

            public int a;
            public int b;

            public Line(int aa, int bb)
            {
                a = aa;
                b = bb;
            }
        }
        
        public class Polygon
        {

            public List<Line> lines;

            public Polygon(List<Line> l)
            {
                lines = l;
            }
        }

        public class Polyhedra
        {

            public List<Polygon> polygons;

            public Polyhedra(List<Polygon> l)
            {
                polygons = l;
            }
        }

        //                                                                                                 x  y  z
        double[,] matrixTranslation = new double[4, 4] { { 1, 0, 0, 0 }, { 0, 1, 0, 0 }, { 0, 0, 1, 0 }, { 1, 1, 1, 1} };
        //                                           x                  y                  z
        double[,] matrixScale = new double[4, 4] { { 1, 0, 0, 0 }, { 0, 1, 0, 0 }, { 0, 0, 1, 0 }, { 0, 0, 0, 1 } };
        //                                                              cos  sin       -sin  cos
        double[,] matrixRotateX = new double[4, 4] { { 1, 0, 0, 0 }, { 0, 1, 1, 0 }, { 0, 1, 1, 0 }, { 0, 0, 0, 1 } };
        //                                           cos  sin       -sin  cos
        double[,] matrixRotateZ = new double[4, 4] { { 1, 1, 0, 0 }, { 1, 1, 0, 0 }, { 0, 0, 1, 0 }, { 0, 0, 0, 1 } };
        //                                            cos   -sin                      sin    cos
        double[,] matrixRotateY = new double[4, 4] { { 1, 0, 1, 0 }, { 0, 1, 0, 0 }, { 1, 0, 1, 0 }, { 0, 0, 0, 1 } };

        double[,] currentRotate;

        double[,] matrixResult = new double[4, 4] { { 1, 0, 0, 0 }, { 0, 1, 0, 0 }, { 0, 0, 1, 0 }, { 0, 0, 0, 1 } };

        double[,] matrixAxonometric = new double[4, 4] { { 1, 0, 0, 0 }, { 0, 1, 0, 0 }, { 0, 0, 0, 0 }, { 0, 0, 0, 1 } };

        double[,] matrixPerspectieve = new double[4, 4] { { 1, 0, 0, 0 }, { 0, 1, 0, 0 }, { 0, 0, 0, -0.1 }, { 0, 0, 0, 1 } };

        double[,] matrixMirror = new double[4, 4] { { 1, 0, 0, 0 }, { 0, 1, 0, 0 }, { 0, 0, 1, 0 }, { 0, 0, 0, 1 } };

        double[,] multipleMatrix(double[,] a, double[,] b)
        {
            if (a.GetLength(1) != b.GetLength(0)) throw new Exception("Матрицы нельзя перемножить");
            double[,] r = new double[a.GetLength(0), b.GetLength(1)];
            for (int i = 0; i < a.GetLength(0); i++)
            {
                for (int j = 0; j < b.GetLength(1); j++)
                {
                    for (int k = 0; k < b.GetLength(0); k++)
                    {
                        r[i, j] += a[i, k] * b[k, j];
                    }
                }
            }
            return r;

        }


        private void comboBoxTypePolyhedra_SelectedIndexChanged(object sender, EventArgs e)
        {
            /*  куб 
                тетраэдр
                октаэдрт
                гексаэдр
                икосаэдр
                додекаэдр

                cube
                tetrahedron
                octahedron
                hexahedron
                icosahedron
                dodecahedron
            */
            switch (comboBoxTypePolyhedra.SelectedIndex)
            {
                case 0:
                    {
                        Cube();
                        break;
                    }
                case 1:
                    {
                        Tetrahedron();
                        break;
                    }
                case 2:
                    {
                        Octahedron();
                        break;
                    }
                case 3:
                    {
                        Icosahedron();
                        break;
                    }
                case 4:
                    {
                        Dodecahedron();
                        break;
                    }       
            }


        }

        void Cube()
        {
            pictureBox1.Refresh();
            List<PointD> cur_points = new List<PointD>();
            cur_points.AddRange(new List<PointD>(){new PointD(0, 0, 0), new PointD(0, 0, 1), new PointD(0, 1, 0),
                new PointD(0, 1, 1), new PointD(1, 0, 0), new PointD(1, 0, 1), new PointD(1, 1, 0), new PointD(1, 1, 1)});

            list_points = cur_points;

            List<Line> cur_lines = new List<Line>()
               {new Line(0,  1), new Line(0, 2), new Line(0, 4), // 0 1 2
                new Line(6, 7), new Line(6, 4), new Line(6, 2), // 3 4 5
                new Line(3, 1), new Line(3, 2), new Line(3, 7), // 6 7 8
                new Line(5, 7), new Line(5, 1), new Line(5, 4) // 9 10 11
               };

            list_lines.Clear();
            list_lines = cur_lines;

            list_pols.Clear();

            Polygon cur_pol = new Polygon(new List<Line>() { cur_lines[1], cur_lines[5], cur_lines[4], cur_lines[2] });
            list_pols.Add(cur_pol); //низ

            cur_pol = new Polygon(new List<Line>() { cur_lines[6], cur_lines[8], cur_lines[9], cur_lines[10] });
            list_pols.Add(cur_pol); // верх

            cur_pol = new Polygon(new List<Line>() { cur_lines[2], cur_lines[11], cur_lines[10], cur_lines[0] });
            list_pols.Add(cur_pol); //лево

            cur_pol = new Polygon(new List<Line>() { cur_lines[8], cur_lines[7], cur_lines[5], cur_lines[3] });
            list_pols.Add(cur_pol); // право

            cur_pol = new Polygon(new List<Line>() { cur_lines[11], cur_lines[9], cur_lines[3], cur_lines[4] });
            list_pols.Add(cur_pol); // ближняя

            cur_pol = new Polygon (new List<Line>() { cur_lines[0], cur_lines[6], cur_lines[7], cur_lines[1]});
            list_pols.Add(cur_pol); // дальняя



            var g = Graphics.FromHwnd(pictureBox1.Handle);

            for (int i = 0; i < list_points.Count(); i++)
            {
                list_points[i].x *= 200;
                list_points[i].y *= 200;
                list_points[i].z *= 200;
            }


            for (int i = 0; i < list_lines.Count(); i++)
            {
                

                Point a = new Point((int)(list_points[list_lines[i].a].x), (int)(list_points[list_lines[i].a].y));
                Point b = new Point((int)(list_points[list_lines[i].b].x), (int)(list_points[list_lines[i].b].y));



                g.DrawLine(new Pen(Color.Black, 2.0f), a, b);

            }








        }


        void Tetrahedron()
        {
            Cube();
            pictureBox1.Refresh();

            var templist = new List<PointD> { list_points[4], list_points[1], list_points[2], list_points[7], };
            list_points.Clear();
            list_points = templist;

            var cur_lines = new List<Line>() {new Line(0,  1), new Line(0, 2), new Line(0, 3),  // 0, 1, 2 
                                          new Line(1,  2), new Line(2, 3), new Line(3, 1) }; // 3, 4, 5
            list_lines.Clear();
            list_lines = cur_lines;

            list_pols.Clear();


            Polygon cur_pol = new Polygon(new List<Line>() { cur_lines[0], cur_lines[5], cur_lines[2] });
            list_pols.Add(cur_pol); 
            cur_pol = new Polygon(new List<Line>() { cur_lines[2], cur_lines[4], cur_lines[1] });
            list_pols.Add(cur_pol); 
            cur_pol = new Polygon(new List<Line>() { cur_lines[0], cur_lines[1], cur_lines[3] });
            list_pols.Add(cur_pol); 
            cur_pol = new Polygon(new List<Line>() { cur_lines[4], cur_lines[5], cur_lines[3] });
            list_pols.Add(cur_pol); 

            var g = Graphics.FromHwnd(pictureBox1.Handle);

            for (int i = 0; i < list_lines.Count(); i++)
            {
                Point a = new Point((int)(list_points[list_lines[i].a].x), (int)(list_points[list_lines[i].a].y));
                Point b = new Point((int)(list_points[list_lines[i].b].x), (int)(list_points[list_lines[i].b].y));

                g.DrawLine(new Pen(Color.Black, 2.0f), a, b);

            }

        }


        void Octahedron()
        {
            Cube();
            pictureBox1.Refresh();

            List<PointD> new_points = new List<PointD>();

            for (int i = 0; i < 6; i++)
            {
                Polygon cur = list_pols[i];
                double sum_x = 0;
                double sum_y = 0;
                double sum_z = 0;

                for (int j = 0; j < cur.lines.Count(); j++)
                {
                    sum_x += list_points[cur.lines[j].a].x;
                    sum_y += list_points[cur.lines[j].a].y;
                    sum_z += list_points[cur.lines[j].a].z;
                    
                    sum_x += list_points[cur.lines[j].b].x;
                    sum_y += list_points[cur.lines[j].b].y;
                    sum_z += list_points[cur.lines[j].b].z;

                }

                PointD new_p = new PointD(sum_x / 16.0, sum_y / 16.0, sum_z / 16.0);
                new_points.Add(new_p);
            }


            /*
            низ   0
            верх  1
            лево  2 
            право 3 
            близ  4
            дал   5
             */

            list_points = new_points;

            List<Line> cur_lines = new List<Line>()
               {new Line(0,  2), new Line(0, 4), new Line(0, 3), new Line(0, 5), // 0 1 2 3
                new Line(1,  2), new Line(1, 4), new Line(1, 3), new Line(1, 5),  //  4 5 6 7
                new Line(4,  2), new Line(2, 5), new Line(5, 3), new Line(3, 4),  //  8 9 10 11
               };

            list_lines.Clear();
            list_lines = cur_lines;

            list_pols.Clear();


            Polygon cur_pol = new Polygon(new List<Line>() { cur_lines[0], cur_lines[1], cur_lines[8] });
            list_pols.Add(cur_pol); 
            cur_pol = new Polygon(new List<Line>() { cur_lines[11], cur_lines[2], cur_lines[1] });
            list_pols.Add(cur_pol); 
            cur_pol = new Polygon(new List<Line>() { cur_lines[10], cur_lines[3], cur_lines[2] });
            list_pols.Add(cur_pol); 
            cur_pol = new Polygon(new List<Line>() { cur_lines[9], cur_lines[0], cur_lines[3] });
            list_pols.Add(cur_pol); 

            cur_pol = new Polygon(new List<Line>() { cur_lines[4], cur_lines[5], cur_lines[8] });
            list_pols.Add(cur_pol); 
            cur_pol = new Polygon(new List<Line>() { cur_lines[5], cur_lines[6], cur_lines[11] });
            list_pols.Add(cur_pol); 
            cur_pol = new Polygon(new List<Line>() { cur_lines[6], cur_lines[7], cur_lines[10] });
            list_pols.Add(cur_pol); 
            cur_pol = new Polygon(new List<Line>() { cur_lines[7], cur_lines[4], cur_lines[9] });
            list_pols.Add(cur_pol); 

            var g = Graphics.FromHwnd(pictureBox1.Handle);
            for (int i = 0; i < list_points.Count(); i++)
            {
                list_points[i].x *= 2;
                list_points[i].y *= 2;
                list_points[i].z *= 2;
            }
            for (int i = 0; i < list_lines.Count(); i++)
            {
                Point a = new Point((int)(list_points[list_lines[i].a].x ), (int)(list_points[list_lines[i].a].y ));
                Point b = new Point((int)(list_points[list_lines[i].b].x ), (int)(list_points[list_lines[i].b].y ));

                g.DrawLine(new Pen(Color.Black, 2.0f), a, b);

            }

        }



        void Icosahedron()
        {

            pictureBox1.Refresh();
            list_points.Clear();
            list_points.Add(new PointD(0, 0, Math.Sqrt(5) / 2.00)); // 0

            //верхние
            for (int i = 0; i <= 4; i++)
            {
                double x = Math.Cos(i * 72 * 3.14 / 180);
                double y = Math.Sin(i * 72 * 3.14 / 180);
                double z = 0.5;

                list_points.Add(new PointD(x, y, z));
            }

            //нижние
            for (int i = 0; i <= 4; i++)
            {
                double x = Math.Cos((36 + i * 72) * 3.14 / 180);
                double y = Math.Sin((36 + i * 72) * 3.14 / 180);
                double z = -0.5;

                list_points.Add(new PointD(x, y, z));
            }


            list_points.Add(new PointD(0, 0, -Math.Sqrt(5) / 2.0)); // 11


            var cur_lines = new List<Line>() {  new Line(0,  1), new Line(1, 2), new Line(2, 0),  // 0 1 2 
                                                new Line(0,  2), new Line(2, 3), new Line(3, 0),  // 0 2 3
                                                new Line(0,  3), new Line(3, 4), new Line(4, 0),  // 0 3 4
                                                new Line(0,  4), new Line(4, 5), new Line(5, 0),  // 0 4 5
                                                new Line(0,  5), new Line(5, 1), new Line(1, 0),  // 0 5 1
                                                new Line(10,  11), new Line(11, 6), new Line(6, 10),  // 10 11 6
                                                new Line(11,  7), new Line(7, 6), new Line(6, 11),  // 11 7 6
                                                new Line(11,  8), new Line(8, 7), new Line(7, 11),  // 11 8 7
                                                new Line(11,  9), new Line(9, 8), new Line(8, 11),  // 11 9 8
                                                new Line(11,  10), new Line(10, 9), new Line(9, 11),  // 11 10 9
                                                new Line(6,  1), new Line(1, 10), new Line(10, 6),  // 6 1 10
                                                new Line(2,  6), new Line(6, 7), new Line(7, 2),  // 2 6 7
                                                new Line(3,  7), new Line(7, 8), new Line(8, 3),  // 3 7 8
                                                new Line(4,  8), new Line(8, 9), new Line(9, 4),  // 4 8 9
                                                new Line(5,  9), new Line(9, 10), new Line(10, 5),  // 5 9 10
                                                new Line(6,  2), new Line(2, 1), new Line(1, 6),  // 6 2 1
                                                new Line(7,  3), new Line(3, 2), new Line(2, 7),  // 7 3 2
                                                new Line(8,  4), new Line(4, 3), new Line(3, 8),  // 8 4 3
                                                new Line(9,  5), new Line(5, 4), new Line(4, 9),  // 9 5 4
                                                new Line(10,  1), new Line(1, 5), new Line(5, 10)}; // 10 1 5

            list_lines.Clear();
            list_lines = cur_lines;

            list_pols.Clear();
            for (int i = 0; i < list_lines.Count(); i += 3)
            {
                list_pols.Add(new Polygon(new List<Line>() { list_lines[i], list_lines[i + 1], list_lines[i + 2] }));
            }


            var g = Graphics.FromHwnd(pictureBox1.Handle);
            for (int i = 0; i < list_points.Count(); i++)
            {
                list_points[i].x *= 100;
                list_points[i].y *= 100;
                list_points[i].z *= 100;
                list_points[i].x += 100;
                list_points[i].y += 100;
                list_points[i].z += 100;
            }


            for (int i = 0; i < list_lines.Count(); i++)
            {
                Point a = new Point((int)(list_points[list_lines[i].a].x), (int)(list_points[list_lines[i].a].y));
                Point b = new Point((int)(list_points[list_lines[i].b].x), (int)(list_points[list_lines[i].b].y));

                g.DrawLine(new Pen(Color.Black, 2.0f), a, b);

            }
        }




        void Dodecahedron()
        {
            Icosahedron();
            pictureBox1.Refresh(); 

            List<PointD> new_points = new List<PointD>();

            for (int i = 0; i < 20; i++)
            {
                Polygon cur = list_pols[i];
                double sum_x = 0;
                double sum_y = 0;
                double sum_z = 0;

                for (int j = 0; j < cur.lines.Count(); j++)
                {
                    sum_x += list_points[cur.lines[j].a].x;
                    sum_y += list_points[cur.lines[j].a].y;
                    sum_z += list_points[cur.lines[j].a].z;
                }

                PointD new_p = new PointD(sum_x / 12.0, sum_y / 12.0, sum_z / 12.0);
                new_points.Add(new_p);
            }


            /*
            низ   0
            верх  1
            лево  2 
            право 3 
            близ  4
            дал   5
             */

            list_points = new_points;

            List<Line> cur_lines = new List<Line>()
               {
                new Line(0,  1), new Line(1, 2), new Line(2, 3), new Line(3, 4), new Line(4, 0), // 0 1 2 3 4
                new Line(0,  1), new Line(1, 16), new Line(16, 11), new Line(11, 15), new Line(15, 0), // 0 1 16 11 15 
                new Line(1,  2), new Line(2, 17), new Line(17, 12), new Line(12, 16), new Line(16, 1), // 1 2 17 12 16 
                new Line(2,  3), new Line(3, 18), new Line(18, 13), new Line(13, 17), new Line(17, 2), // 2 3 18 13 17  
                new Line(3,  4), new Line(4, 19), new Line(19, 14), new Line(14, 18), new Line(18, 3), // 3 4 19 14 18 
                new Line(4,  0), new Line(0, 15), new Line(15, 10), new Line(10, 19), new Line(19, 4), // 4 0 15 10 19 
                new Line(10,  15), new Line(15, 11), new Line(11, 6), new Line(6, 5), new Line(5, 10), // 10 15 11 6 5 
                new Line(11,  16), new Line(16, 12), new Line(12, 7), new Line(7, 6), new Line(6, 11), // 11 16 12 7 6 
                new Line(12,  17), new Line(17, 13), new Line(13, 8), new Line(8, 7), new Line(7, 12), // 12 17 13 8 7 
                new Line(13,  18), new Line(18, 14), new Line(14, 9), new Line(9, 8), new Line(8, 13), // 13 18 14 9 8 
                new Line(14,  19), new Line(19, 10), new Line(10, 5), new Line(5, 9), new Line(9, 14), // 14 19 10 5 9 
                new Line(5,  6), new Line(6, 7), new Line(7, 8), new Line(8, 9), new Line(9, 5), // 5 6 7 8 9 
               };

            list_lines.Clear();
            list_lines = cur_lines;

            list_pols.Clear();
            for (int i = 0; i < list_lines.Count(); i += 5)
            {
                list_pols.Add(new Polygon(new List<Line>() { list_lines[i], list_lines[i + 1], list_lines[i + 2], list_lines[i + 3], list_lines[i + 4] }));
            }

            var g = Graphics.FromHwnd(pictureBox1.Handle);
            for (int i = 0; i < list_points.Count(); i++)
            {
                list_points[i].x *= 5;
                list_points[i].y *= 5;
                list_points[i].z *= 5;
                list_points[i].x -= 5;
                list_points[i].y -= 5;
                list_points[i].z -= 5;
            }
            for (int i = 0; i < list_lines.Count(); i++)
            {
                Point a = new Point((int)(list_points[list_lines[i].a].x ), (int)(list_points[list_lines[i].a].y ));
                Point b = new Point((int)(list_points[list_lines[i].b].x ), (int)(list_points[list_lines[i].b].y ));

                g.DrawLine(new Pen(Color.Black, 2.0f), a, b);

            }
        }

        void peremalui()
        {
            //if (comboBoxTypePolyhedra.SelectedIndex == -1)
               // return;

            if (comboBoxTypeProection.SelectedIndex == -1)
                return;

            var g = Graphics.FromHwnd(pictureBox1.Handle);

            switch (comboBoxTypeProection.SelectedIndex)
            {
                /*
                XY
                XZ
                YZ
                аксонометрическая
                перспективная
                 
                 */

                case 0:
                    {
                        pictureBox1.Refresh();
                        for (int i = 0; i < list_lines.Count(); i++)
                        {
                            Point a = new Point((int)(list_points[list_lines[i].a].x), (int)(list_points[list_lines[i].a].y));
                            Point b = new Point((int)(list_points[list_lines[i].b].x), (int)(list_points[list_lines[i].b].y));

                            g.DrawLine(new Pen(Color.Black, 2.0f), a, b);

                        }

                        break; 
                    }
                case 1:
                    {
                        pictureBox1.Refresh();
                        for (int i = 0; i < list_lines.Count(); i++)
                        {
                            Point a = new Point((int)(list_points[list_lines[i].a].x), (int)(list_points[list_lines[i].a].z));
                            Point b = new Point((int)(list_points[list_lines[i].b].x), (int)(list_points[list_lines[i].b].z));

                            g.DrawLine(new Pen(Color.Black, 2.0f), a, b);

                        }

                        break; 
                    }
                case 2:
                    {
                        pictureBox1.Refresh();
                        for (int i = 0; i < list_lines.Count(); i++)
                        {
                            Point a = new Point((int)(list_points[list_lines[i].a].y), (int)(list_points[list_lines[i].a].z));
                            Point b = new Point((int)(list_points[list_lines[i].b].y), (int)(list_points[list_lines[i].b].z));

                            g.DrawLine(new Pen(Color.Black, 2.0f), a, b);

                        }

                        break; 
                    }
                case 3:
                    {
                        pictureBox1.Refresh();
                        axonometric();
                        break; 
                    }
                case 4:
                    {
                        pictureBox1.Refresh();
                        parallperpective();
                        break; 
                    }
            
            
            
            }
        
        
        
        
        }
        public void parallperpective()
        {
            var newimage = new List<PointD>();
            for (int i = 0; i < list_points.Count; i++)
            {
                double[,] matrixPoint = new double[1, 4] { { list_points[i].x, list_points[i].y, list_points[i].z, 1.0 } };

                var res = (multipleMatrix(matrixPoint, matrixPerspectieve));
                double c = 10.0 ;
                res[0, 0] /= 1.0 - res[0, 3]/c;
                res[0, 1] /= 1.0 - res[0, 3]/c;
                newimage.Add(new PointD(res[0, 0], res[0, 1], res[0, 2]));
            }
            var g = Graphics.FromHwnd(pictureBox1.Handle);
            for (int i = 0; i < list_lines.Count(); i++)
            {
                Point a = new Point((int)(newimage[list_lines[i].a].x) + pictureBox1.Width / 3, (int)(newimage[list_lines[i].a].y) + pictureBox1.Height / 3);
                Point b = new Point((int)(newimage[list_lines[i].b].x) + pictureBox1.Width / 3, (int)(newimage[list_lines[i].b].y) + pictureBox1.Height / 3);

                g.DrawLine(new Pen(Color.Black, 2.0f), a, b);

            }
        }
        private void axonometric()
        {
            double anglephi = -45.0 * Math.PI / 180.0;
            double anglepsi = 35.26 * Math.PI / 180.0;
            matrixAxonometric[0, 0] = Math.Cos(anglepsi);
            matrixAxonometric[0, 1] = Math.Sin(anglephi) * Math.Sin(anglepsi);
            matrixAxonometric[1, 1] = Math.Cos(anglephi);
            matrixAxonometric[2, 0] = Math.Sin(anglepsi);
            matrixAxonometric[2, 1] = -Math.Cos(anglepsi) * Math.Sin(anglephi);

            var newimage = new List<PointD>();
            for (int i = 0; i < list_points.Count; i++)
            {
                double[,] matrixPoint = new double[1, 4] { { list_points[i].x, list_points[i].y, list_points[i].z, 1.0 } };

                var res = (multipleMatrix(matrixPoint, matrixAxonometric));

                newimage.Add( new PointD(res[0, 0], res[0, 1], res[0, 2]));
            }
            var g = Graphics.FromHwnd(pictureBox1.Handle);
            for (int i = 0; i < list_lines.Count(); i++)
            {
                Point a = new Point((int)(newimage[list_lines[i].a].x) + pictureBox1.Width / 3, (int)(newimage[list_lines[i].a].y) + pictureBox1.Height / 3);
                Point b = new Point((int)(newimage[list_lines[i].b].x) + pictureBox1.Width / 3, (int)(newimage[list_lines[i].b].y) + pictureBox1.Height / 3);

                g.DrawLine(new Pen(Color.Black, 2.0f), a  , b);

            }
        }
        private void buttonTranslite_Click(object sender, EventArgs e)
        {
            if (comboBoxTypePolyhedra.SelectedIndex == -1)
                return;

            var g = Graphics.FromHwnd(pictureBox1.Handle);
            //                                                                                                   x  y  z
            //double[,] matrixTranslation = new double[4, 4] { { 1, 0, 0, 0 }, { 0, 1, 0, 0 }, { 0, 0, 1, 0 }, { 1, 1, 1, 1 } };
            matrixTranslation[3, 0] = Convert.ToDouble(textBox1.Text);
            matrixTranslation[3, 1] = Convert.ToDouble(textBox2.Text);
            matrixTranslation[3, 2] = Convert.ToDouble(textBox3.Text);

            for (int i = 0; i < list_points.Count; i++)
            {
                double[,] matrixPoint = new double[1, 4] { { list_points[i].x, list_points[i].y, list_points[i].z, 1.0 } };

                var res = (multipleMatrix(matrixPoint, matrixTranslation));

                list_points[i] = new PointD(res[0, 0], res[0, 1], res[0, 2]);
            }

            peremalui();
        }

        private void comboBoxTypeProection_SelectedIndexChanged(object sender, EventArgs e)
        {
            peremalui();
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            /* z  y  x*/
            switch(comboBoxAxis.SelectedIndex){
                case 0:
                    matrixMirror[0, 0] = 1;
                    matrixMirror[1, 1] = 1;
                    matrixMirror[2, 2] = -1;
                    break;
                case 1:
                    matrixMirror[0, 0] = 1;
                    matrixMirror[1, 1] = -1;
                    matrixMirror[2, 2] = 1;
                    break; 
                case 2:
                    matrixMirror[0, 0] = -1;
                    matrixMirror[1, 1] = 1;
                    matrixMirror[2, 2] = 1;
                    break; 
            }

        }

        private void buttonMirror_Click(object sender, EventArgs e)
        {
            if (comboBoxAxis.SelectedIndex == -1) 
            { 
                return; 
            }

            for (int i = 0; i < list_points.Count; i++)
            {
                double[,] matrixPoint = new double[1, 4] { { list_points[i].x, list_points[i].y, list_points[i].z, 1.0 } };

                var res = (multipleMatrix(matrixPoint, matrixMirror));

                list_points[i] = new PointD(res[0, 0], res[0, 1], res[0, 2]);
            }

            peremalui();


        }

        private void buttonScale_Click(object sender, EventArgs e)
        {
            double kx = Convert.ToDouble(textBox11.Text);
            double ky = Convert.ToDouble(textBox12.Text);
            double kz = Convert.ToDouble(textBox13.Text);

            matrixScale[0 ,0] = kx; 
            matrixScale[1, 1] = ky;
            matrixScale[2, 2] = kz;

            for (int i = 0; i < list_points.Count; i++)
            {
                double[,] matrixPoint = new double[1, 4] { { list_points[i].x, list_points[i].y, list_points[i].z, 1.0 } };

                var res = (multipleMatrix(matrixPoint, matrixScale));

                list_points[i] = new PointD(res[0, 0], res[0, 1], res[0, 2]);
            }

            peremalui();

        }

        private void buttonRotate_Click(object sender, EventArgs e)
        {
            if (comboBoxPlane.SelectedIndex == -1)
                return;
            if (textBox4.Text.Length < 1)
                return;
            double teta = Convert.ToDouble(textBox4.Text);
            switch (comboBoxPlane.SelectedIndex)
            {
                case 0:
                    currentRotate = matrixRotateX;
                    currentRotate[1, 1] = Math.Cos(teta* Math.PI / 180.0);
                    currentRotate[1, 2] = Math.Sin(teta * Math.PI / 180.0);
                    currentRotate[2, 1] = -Math.Sin(teta * Math.PI / 180.0);
                    currentRotate[2, 2] = Math.Cos(teta * Math.PI / 180.0);
                    break;
                case 1:
                    currentRotate = matrixRotateY;
                    currentRotate[0, 0] = Math.Cos(teta * Math.PI / 180.0);
                    currentRotate[0, 2] = -Math.Sin(teta * Math.PI / 180.0);
                    currentRotate[2, 0] = Math.Sin(teta * Math.PI / 180.0);
                    currentRotate[2, 2] = Math.Cos(teta * Math.PI / 180.0);
                    break;
                case 2:
                    currentRotate = matrixRotateZ;
                    currentRotate[0, 0] = Math.Cos(teta * Math.PI / 180.0);
                    currentRotate[0, 1] = Math.Sin(teta * Math.PI / 180.0);
                    currentRotate[1, 0] = -Math.Sin(teta * Math.PI / 180.0);
                    currentRotate[1, 1] = Math.Cos(teta * Math.PI / 180.0);
                    break;
            }


            for (int i = 0; i < list_points.Count; i++)
            {
                double[,] matrixPoint = new double[1, 4] { { list_points[i].x, list_points[i].y, list_points[i].z, 1.0 } };

                var res = (multipleMatrix(matrixPoint, currentRotate));

                list_points[i] = new PointD(res[0, 0], res[0, 1], res[0, 2]);
            }

            peremalui();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
           
        }

        private void buttonRotateRound_Click(object sender, EventArgs e)
        {
            if (textBox5.Text == textBox8.Text && textBox6.Text == textBox9.Text && textBox7.Text == textBox10.Text)
                return;
            if (textBox14.Text.Length < 1)
                return;
            PointD a = new PointD(Convert.ToDouble(textBox5.Text), Convert.ToDouble(textBox6.Text), Convert.ToDouble(textBox7.Text));
            PointD b = new PointD(Convert.ToDouble(textBox8.Text), Convert.ToDouble(textBox9.Text), Convert.ToDouble(textBox10.Text));

            List<double> myv = new List<double>() {b.x - a.x, b.y - a.y, b.z - a.z  };
            double modv = Math.Sqrt(Math.Pow(myv[0],2) + Math.Pow(myv[1], 2) + Math.Pow(myv[2], 2));
            myv[0] /= modv; //l
            myv[1] /= modv; //m
            myv[2] /= modv; //n
            double phi = Convert.ToDouble(textBox14.Text) * Math.PI / 180.0;
            matrixResult[0, 0] = Math.Pow(myv[0], 2) + Math.Cos(phi)*(1.0 - Math.Pow(myv[0], 2)) ;
            matrixResult[0, 1] = myv[0] * (1.0 - Math.Cos(phi)) * myv[1] + myv[2] * Math.Sin(phi);
            matrixResult[0, 2] = myv[0] * (1.0 - Math.Cos(phi)) * myv[2] - myv[1] * Math.Sin(phi);
            matrixResult[1, 0] = myv[0] * (1.0 - Math.Cos(phi)) * myv[1] - myv[2] * Math.Sin(phi);
            matrixResult[1, 1] = Math.Pow(myv[1], 2) + Math.Cos(phi) * (1.0 - Math.Pow(myv[1], 2));
            matrixResult[1, 2] = myv[1] * (1.0 - Math.Cos(phi)) * myv[2] + myv[0] * Math.Sin(phi);
            matrixResult[2, 0] = myv[0] * (1.0 - Math.Cos(phi)) * myv[2] + myv[1] * Math.Sin(phi);
            matrixResult[2, 1] = myv[1] * (1.0 - Math.Cos(phi)) * myv[2] - myv[0] * Math.Sin(phi);
            matrixResult[2, 2] = Math.Pow(myv[2], 2) + Math.Cos(phi) * (1.0 - Math.Pow(myv[2], 2));


            for (int i = 0; i < list_points.Count; i++)
            {
                double[,] matrixPoint = new double[1, 4] { { list_points[i].x, list_points[i].y, list_points[i].z, 1.0 } };

                var res = (multipleMatrix(matrixPoint, matrixResult));

                list_points[i] = new PointD(res[0, 0], res[0, 1], res[0, 2]);
            }

            peremalui();
        
        }

        private OpenFileDialog openFileDialog1;
        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1 = new OpenFileDialog();
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    var sr = new StreamReader(openFileDialog1.FileName);
                    var text = sr.ReadToEnd();

                    parceobjfile(text);
                    peremalui();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Security error.\n\nError message: {ex.Message}\n\n" +
                    $"Details:\n\n{ex.StackTrace}");
                }
            }
        }

        private void parceobjfile(string text)
        {
            list_points.Clear();
            list_lines.Clear();
            list_pols.Clear();

            var lines = text.Split('\n');
            Regex rv = new Regex(@"v\s*(?<first>[0-9.-]+) (?<second>[0-9.-]+) (?<third>[0-9.-]+)");
            Regex rf = new Regex(@"\s[0-9]+(\/[0-9]*)?(\/[0-9]+)?");
            for (int i = 0; i< lines.Length; i++)
            {
                if (rv.IsMatch(lines[i])){
                    var m = rv.Match(lines[i]);

                    double x = 10 *Convert.ToDouble(m.Groups["first"].ToString().Replace('.', ','));
                    double y = 10 * Convert.ToDouble(m.Groups["second"].ToString().Replace('.', ','));
                    double z = 10 * Convert.ToDouble(m.Groups["third"].ToString().Replace('.', ','));
                    PointD p = new PointD(x, y, z);

                    list_points.Add(p);

                }

                if (lines[i].StartsWith("f "))
                {
                    var m = rf.Matches(lines[i]);

                    List<Line> ll = new List<Line>();

                    for(int j = 0; j <m.Count ; j ++ )
                    {
                        int v1 = Convert.ToInt32(m[j % m.Count].ToString().Split('/')[0]) - 1;
                        int v2 = Convert.ToInt32(m[(j + 1) % m.Count].ToString().Split('/')[0]) - 1;
                        ll.Add(new Line(v1, v2));
                    }
                    list_lines.AddRange(ll);
                    list_pols.Add(new Polygon(ll));


                }
            }
        }

        private void buttonFigureRotation_Click(object sender, EventArgs e)
        {
            pictureBox1.Refresh();
            list_points.Clear();
            list_lines.Clear();
            list_pols.Clear();

            comboBoxTypeProection.SelectedIndex = 0;
            buttonDrawAxis.Enabled = true;
            buttonDrawLine.Enabled = true;
        }

        private void buttonDrawAxis_Click(object sender, EventArgs e)
        {
            clickCount = 0;
            is_draw = true;
            draw_axis = true;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
        }

        int clickCount;

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (is_draw)
            {
                var g = Graphics.FromHwnd(pictureBox1.Handle);

                if (draw_axis)
                {
                    if (e.Button == MouseButtons.Left)
                    {
                        line_axis.Add(new PointD(e.X, e.Y, 0));
                        clickCount++;

                        if (clickCount == 1)
                        {
                            g.FillRectangle(Brushes.Black, e.X, e.Y, 2, 2);
                        }
                        if (clickCount == 2)
                        {
                            pictureBox1.Refresh();
                            if (line_polyline.Count > 0)
                            { 
                                //todo
                            }
                            g.DrawLine(new Pen(Color.Red, 2), (float)line_axis[0].x, (float)line_axis[0].y, 
                                                              (float)line_axis[1].x, (float)line_axis[1].y);
                            draw_axis = false;
                            is_draw = false;
                            clickCount = 0;
                        }
                    }

                }
                if (draw_polyline)
                {
                    if (e.Button == MouseButtons.Left)
                    {
                        line_polyline.Add(new PointD(e.X, e.Y, 0));
                        clickCount++;

                        if (clickCount >= 2)
                        {
                            g.DrawLine(new Pen(Color.Black, 2), (float)line_polyline[line_polyline.Count()-1].x, (float)line_polyline[line_polyline.Count()-1].y, 
                                (float)line_polyline[line_polyline.Count() - 2].x, (float)line_polyline[line_polyline.Count() - 2].y);
                        }
                    }
                }


            }
        }
        private void buttonDrawLine_Click(object sender, EventArgs e)
        {
            draw_polyline = true;
            is_draw = true;
            buttonEnterLine.Enabled = true;
        }
        private void buttonEnterLine_Click(object sender, EventArgs e)
        {
            numb_vert = clickCount;
            clickCount = 0;
            draw_polyline = false;
            is_draw = false;
        }

        private void buttonEnterFigure_Click(object sender, EventArgs e)
        {
            if (textBoxNumbSplit.Text.Length == 0)
                return;
            
            int split = Convert.ToInt32(textBoxNumbSplit.Text);
            
            CreateRotation(split);

            peremalui();
        }

        void CreateRotation(int split)
        {
            List<PointD> buf = new List<PointD>();
            buf.AddRange(line_polyline);

            double angle = 360.0 / split;
            for (int i = 1; i < split; i++)
            {
                 buf.AddRange(MakeOneRotation(angle*i * Math.PI / 180.0));
            }
            list_points.AddRange(buf);
            list_points.AddRange(line_polyline);

            for (int i = 0; i < buf.Count() / numb_vert; i++)
            {
                int it1 = (numb_vert * i);
                int it2 = (numb_vert * (i+1));


                for (int j = 0; j < numb_vert - 1; j++)
                {
                    Line l1 = new Line(it1 + j, it1 + j + 1);
                    Line l2 = new Line(it1 + j + 1, it2 + j + 1);
                    Line l3 = new Line(it2 + j + 1, it2 + j);
                    Line l4 = new Line(it2 + j, it1 + j);

                    List<Line> lines_buf = new List<Line>() { l1, l2, l3, l4 };

                    list_lines.AddRange(lines_buf);
                    list_pols.Add(new Polygon(lines_buf));
                }


            }

            



        }


        List<PointD> MakeOneRotation(double phi)
        {
            List<PointD> s = new List<PointD>();
            PointD b = line_axis[0];
            PointD a = line_axis[1];

            List<double> myv = new List<double>() { b.x - a.x, b.y - a.y, b.z - a.z };
            double modv = Math.Sqrt(Math.Pow(myv[0], 2) + Math.Pow(myv[1], 2) + Math.Pow(myv[2], 2));
            myv[0] /= modv; //l
            myv[1] /= modv; //m
            myv[2] /= modv; //n

            matrixResult[0, 0] = Math.Pow(myv[0], 2) + Math.Cos(phi) * (1.0 - Math.Pow(myv[0], 2));
            matrixResult[0, 1] = myv[0] * (1.0 - Math.Cos(phi)) * myv[1] + myv[2] * Math.Sin(phi);
            matrixResult[0, 2] = myv[0] * (1.0 - Math.Cos(phi)) * myv[2] - myv[1] * Math.Sin(phi);
            matrixResult[1, 0] = myv[0] * (1.0 - Math.Cos(phi)) * myv[1] - myv[2] * Math.Sin(phi);
            matrixResult[1, 1] = Math.Pow(myv[1], 2) + Math.Cos(phi) * (1.0 - Math.Pow(myv[1], 2));
            matrixResult[1, 2] = myv[1] * (1.0 - Math.Cos(phi)) * myv[2] + myv[0] * Math.Sin(phi);
            matrixResult[2, 0] = myv[0] * (1.0 - Math.Cos(phi)) * myv[2] + myv[1] * Math.Sin(phi);
            matrixResult[2, 1] = myv[1] * (1.0 - Math.Cos(phi)) * myv[2] - myv[0] * Math.Sin(phi);
            matrixResult[2, 2] = Math.Pow(myv[2], 2) + Math.Cos(phi) * (1.0 - Math.Pow(myv[2], 2));


            for (int i = 0; i < line_polyline.Count; i++)
            {
                double[,] matrixPoint = new double[1, 4] { { line_polyline[i].x, line_polyline[i].y, line_polyline[i].z, 1.0 } };

                var res = (multipleMatrix(matrixPoint, matrixResult));

                s.Add(new PointD(res[0, 0], res[0, 1], res[0, 2]));
            }

            return s;
        }

    }
}
