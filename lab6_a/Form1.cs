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

            
            

            

            

            

           

        }

        void Octahedron()
        {
            Cube();

            List<PointD> new_points = new List<PointD>();

            for (int i = 0; i < 12; i++)
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


        }


        void Tetrahedron()
        {
            Cube();
            var templist = new List<PointD> { list_points[4], list_points[1], list_points[2], list_points[7], };
            list_points.Clear();
            list_points = templist;

            var cur_lines = new List<Line>() {new Line(0,  1), new Line(0, 2), new Line(0, 3),  // 0, 1, 2 
                                          new Line(1,  2), new Line(2, 3), new Line(3, 1) }; // 3, 4, 5
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



        }


        void Icosahedron()
        {

        
        }


        void Dodecahedron()
        { }

    }
}
