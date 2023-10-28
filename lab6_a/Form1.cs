using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace lab6_a
{
    public partial class Form1 : Form
    {
        List<PointD> list_points;
        List<Line> list_lines;
        List<Polygon> list_pols;

        public Form1()
        {
            InitializeComponent();
            list_points = new List<PointD>();
            list_lines = new List<Line>();
            list_pols = new List<Polygon>();
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
        double[,] matrixRotateY = new double[4, 4] { { 1, 1, 0, 0 }, { 1, 1, 0, 0 }, { 0, 0, 1, 0 }, { 0, 0, 0, 1 } };
        //                                            cos   -sin                      sin    cos
        double[,] matrixRotateZ = new double[4, 4] { { 1, 0, 1, 0 }, { 0, 1, 0, 0 }, { 1, 0, 1, 0 }, { 0, 0, 0, 1 } };


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
            list_pols.Add(cur_pol); //низ
            cur_pol = new Polygon(new List<Line>() { cur_lines[2], cur_lines[4], cur_lines[1] });
            list_pols.Add(cur_pol); //низ
            cur_pol = new Polygon(new List<Line>() { cur_lines[0], cur_lines[1], cur_lines[3] });
            list_pols.Add(cur_pol); //низ
            cur_pol = new Polygon(new List<Line>() { cur_lines[4], cur_lines[5], cur_lines[3] });
            list_pols.Add(cur_pol); //низ

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
            list_pols.Add(cur_pol); //низ
            cur_pol = new Polygon(new List<Line>() { cur_lines[11], cur_lines[2], cur_lines[1] });
            list_pols.Add(cur_pol); //низ
            cur_pol = new Polygon(new List<Line>() { cur_lines[10], cur_lines[3], cur_lines[2] });
            list_pols.Add(cur_pol); //низ
            cur_pol = new Polygon(new List<Line>() { cur_lines[9], cur_lines[0], cur_lines[3] });
            list_pols.Add(cur_pol); //низ

            cur_pol = new Polygon(new List<Line>() { cur_lines[4], cur_lines[5], cur_lines[8] });
            list_pols.Add(cur_pol); //вверх
            cur_pol = new Polygon(new List<Line>() { cur_lines[5], cur_lines[6], cur_lines[11] });
            list_pols.Add(cur_pol); //вверх
            cur_pol = new Polygon(new List<Line>() { cur_lines[6], cur_lines[7], cur_lines[10] });
            list_pols.Add(cur_pol); //вверх
            cur_pol = new Polygon(new List<Line>() { cur_lines[7], cur_lines[4], cur_lines[9] });
            list_pols.Add(cur_pol); //вверх
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
            if (comboBoxTypePolyhedra.SelectedIndex == -1)
                return;

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
                        //todo proection

                        break; 
                    }
                case 4:
                    {
                        pictureBox1.Refresh();
                        //todo another proection

                        break; 
                    }
            
            
            
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
    }
}
