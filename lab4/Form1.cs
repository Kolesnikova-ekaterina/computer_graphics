using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace lab4
{

    public partial class Form1 : Form
    {
        public class Line
        {

            public Point a;
            public Point b;

            public Line(Point aa, Point bb)
            {
                a = aa;
                b = bb;
            }
        }
        //                                                                a  b
        double[,] matrixReplace = new double[3, 3] { { 1, 0, 0 }, { 0, 1, 0 }, { 1, 1, 1 } };
        //                                        cos  sin  -sin  cos
        double[,] matrixRotate = new double[3, 3] { { 1, 0, 0 }, { 0, 1, 0 }, { 0, 0, 1 } };
        //                                           a               b
        double[,] matrixScale = new double[3, 3] { { 1, 0, 0 }, { 0, 1, 0 }, { 0, 0, 1 } };

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


        bool draw_state = false;
        bool we_drawing = false;
        bool del_state = false;
        bool rotate_choosed = false;
        bool scale_choosed = false;
        bool edge_draw = false;
        bool is_convex = false;
        bool is_non_convex = false;
        bool how_to_edge = false;

        Point point_change = new Point(0, 0);

        private List<List<Point>> points;
        private List<Line> lines;
        List<Point> newEdge;
        private Point start;
        private int clickCount;

        public Form1()
        {
            InitializeComponent();
            Bitmap bmp = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            pictureBox1.Image = bmp;
            points = new List<List<Point>>();
            lines = new List<Line>();
            newEdge = new List<Point>();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            rotate_choosed = false;
            scale_choosed = false;
            del_state = false;
            edge_draw = false;
            is_convex = false;
            is_non_convex = false;
            how_to_edge = false;
            var g = Graphics.FromHwnd(pictureBox1.Handle);

            button2.Enabled = true;

            if (!draw_state)
                points.Add(new List<Point>());

            draw_state = !draw_state;
            button1.Enabled = false;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            rotate_choosed = false;
            scale_choosed = false;
            is_convex = false;
            button1.Enabled = true;
            edge_draw = false;
            is_non_convex = false;
            how_to_edge = false;
            var g = Graphics.FromHwnd(pictureBox1.Handle);
            for (int i = 0; i < points.Last().Count() - 1; i++)
            {
                lines.Add(new Line(points.Last()[i], points.Last()[i + 1]));
                comboBox2.Items.Add("ребро" + lines.Count().ToString());
            }
            if (draw_state && points.Last().Count > 2)
            {
                lines.Add(new Line(start, points.Last()[points.Last().Count - 1]));
                comboBox2.Items.Add("ребро" + lines.Count().ToString());
                g.DrawLine(new Pen(Color.Black), start, points.Last()[points.Last().Count - 1]);
            }
            if (draw_state)
                comboBox1.Items.Add("фигура" + points.Count().ToString());


            if (!draw_state)
                points.Add(new List<Point>());



            draw_state = !draw_state;
            clickCount = 0; // Сбрасываем счетчик щелчков
            button2.Enabled = false;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            rotate_choosed = false;
            scale_choosed = false;
            edge_draw = false;
            del_state = true;
            is_convex = false;
            is_non_convex = false;
            how_to_edge = false;
            Refresh();
            points.Clear();
            points = new List<List<Point>>();
            draw_state = false;
            button2.Enabled = false;
            button1.Enabled = true;

            comboBox1.Items.Clear();

        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (rotate_choosed || scale_choosed || edge_draw || is_convex || is_non_convex || how_to_edge)
            {
                point_change = e.Location;

                if (rotate_choosed)
                    funcRotate();
                if (scale_choosed)
                    funcScale();
                if (edge_draw)
                {
                    clickCount++;
                    if (clickCount == 1)
                    {
                        newEdge.Add(e.Location);
                    }
                    if (clickCount == 2)
                    {
                        newEdge.Add(e.Location);
                        funcPointOfEdge();
                    }

                }
                if (is_convex)
                {
                    funcPointIntoConvex();
                }
                if (is_non_convex)
                    funcPointIntoNONConvex();
                if (how_to_edge)
                    funcHowPointToEdge();
                return;
            }
            else if (!draw_state)
                return;

            if (del_state)
                return;

            if (e.Button == MouseButtons.Left)
            {
                points.Last().Add(e.Location);
                clickCount++;

                if (clickCount == 1)
                {
                    start = e.Location;
                    DrawPoint();

                }
                if (clickCount == 2)
                    DrawLines();
                if (clickCount >= 3)
                    DrawPolygon();
            }

        }

        void DrawPoint()
        {
            var g = Graphics.FromHwnd(pictureBox1.Handle);
            g.FillRectangle(Brushes.Black, start.X, start.Y, 2, 2);
        }
        void DrawLines()
        {
            pictureBox1.Refresh();
            var g = Graphics.FromHwnd(pictureBox1.Handle);

            for (int i = 0; i < points.Count() - 1; i++)
            {
                if (points[i].Count() == 1)
                {
                    g.FillRectangle(Brushes.Black, points[i][0].X, points[i][0].Y, 2, 2);
                }
                for (int j = 0; j < points[i].Count() - 1; j++)
                    g.DrawLine(new Pen(Color.Black), points[i][j], points[i][j + 1]);

                g.DrawLine(new Pen(Color.Black), points[i][0], points[i][points[i].Count() - 1]);
            }

            g.DrawLine(new Pen(Color.Black), points.Last()[points.Last().Count - 1], start);
        }

        void DrawPolygon()
        {
            var g = Graphics.FromHwnd(pictureBox1.Handle);
            for (int i = 0; i < points.Last().Count() - 1; i++)
            {
                g.DrawLine(new Pen(Color.Black), points.Last()[i], points.Last()[i + 1]);
            }

        }


        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            var g = Graphics.FromHwnd(pictureBox1.Handle);

            if (!we_drawing)
                return;

            we_drawing = false;

            if (points.Last().Count == 1) //если только один элемент - рисуем точку
            {
                g.DrawLine(new Pen(Color.Black), start, start);
            }
            else if (points.Last().Count == 3) //если только три элемента - рисуем ребро
            {
                points.Last().Clear();
                return;
            }
            else
            {
                if (points.Last().Count == 4 && points.Last()[points.Last().Count - 1] == start)
                {
                    points.Last().Clear();
                    return;
                }

                g.DrawLine(new Pen(Color.Black), points.Last()[points.Last().Count - 1], start);
            }

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            edge_draw = false;
            rotate_choosed = false;
            scale_choosed = false;
            draw_state = false;
            we_drawing = false;
            del_state = false;
            is_convex = false;
            is_non_convex = false;
            how_to_edge = false;

            pictureBox1.Refresh();
            var g = Graphics.FromHwnd(pictureBox1.Handle);

            for (int i = 0; i < points.Count(); i++)
            {
                if (points[i].Count() == 1)
                {
                    if (i != comboBox1.SelectedIndex)
                        g.FillRectangle(Brushes.Black, points[i][0].X, points[i][0].Y, 2, 2);
                    else
                        g.FillRectangle(Brushes.Red, points[i][0].X, points[i][0].Y, 2, 2);
                }
                for (int j = 0; j < points[i].Count() - 1; j++)
                {
                    if (i != comboBox1.SelectedIndex)
                        g.DrawLine(new Pen(Color.Black), points[i][j], points[i][j + 1]);
                    else
                        g.DrawLine(new Pen(Color.Red), points[i][j], points[i][j + 1]);
                }

                if (i != comboBox1.SelectedIndex)
                    g.DrawLine(new Pen(Color.Black), points[i][0], points[i][points[i].Count() - 1]);
                else
                    g.DrawLine(new Pen(Color.Red), points[i][0], points[i][points[i].Count() - 1]);

            }



        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            edge_draw = false;
            rotate_choosed = false;
            scale_choosed = false;
            draw_state = false;
            we_drawing = false;
            del_state = false;
            is_convex = false;
            is_non_convex = false;
            how_to_edge = false;

            pictureBox1.Refresh();
            var g = Graphics.FromHwnd(pictureBox1.Handle);

            for (int i = 0; i < lines.Count(); i++)
            {
                if (i != comboBox2.SelectedIndex)
                    g.DrawLine(new Pen(Color.Black), lines[i].a, lines[i].b);
                else
                    g.DrawLine(new Pen(Color.BlueViolet), lines[i].a, lines[i].b);

            }

        }

        private void peremalui()
        {
            var g = Graphics.FromHwnd(pictureBox1.Handle);
            pictureBox1.Refresh();

            for (int i = 0; i < points.Count(); i++)
            {
                if (points[i].Count() == 1)
                {
                    if (i != comboBox1.SelectedIndex)
                        g.FillRectangle(Brushes.Black, points[i][0].X, points[i][0].Y, 2, 2);
                    else
                        g.FillRectangle(Brushes.Red, points[i][0].X, points[i][0].Y, 2, 2);
                }
                for (int j = 0; j < points[i].Count() - 1; j++)
                {
                    if (i != comboBox1.SelectedIndex)
                        g.DrawLine(new Pen(Color.Black), points[i][j], points[i][j + 1]);
                    else
                        g.DrawLine(new Pen(Color.Red), points[i][j], points[i][j + 1]);
                }

                if (i != comboBox1.SelectedIndex)
                    g.DrawLine(new Pen(Color.Black), points[i][0], points[i][points[i].Count() - 1]);
                else
                    g.DrawLine(new Pen(Color.Red), points[i][0], points[i][points[i].Count() - 1]);

            }

        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == -1)
                return;

            rotate_choosed = false;
            scale_choosed = false;
            edge_draw = false;
            is_non_convex = false;
            how_to_edge = false;

            int ind = comboBox1.SelectedIndex;

            var g = Graphics.FromHwnd(pictureBox1.Handle);
            matrixReplace[2, 0] = Convert.ToDouble(textBox1.Text);
            matrixReplace[2, 1] = Convert.ToDouble(textBox2.Text);

            for (int i = 0; i < points[ind].Count; i++)
            {
                double[,] matrixPoint = new double[1, 3] { { (double)points[ind][i].X, (double)points[ind][i].Y, 1.0 } };

                var res = (multipleMatrix(matrixPoint, matrixReplace));

                points[ind][i] = new Point((int)res[0, 0], (int)res[0, 1]);
            }

            updateLines();
            peremalui();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            rotate_choosed = true;
            scale_choosed = false;
            edge_draw = false;
            is_non_convex = false;
            how_to_edge = false;
        }

        private void button7_Click(object sender, EventArgs e)
        {
            rotate_choosed = false;
            scale_choosed = false;
            edge_draw = false;
            is_convex = false;
            is_non_convex = false;
            how_to_edge = false;
            funcRotate();
        }
        private void updateLines()
        { 
            lines = new List<Line>();

            for (int i = 0; i < points.Count(); i++)
            { 
                for (int j = 0; j < points[i].Count() - 1; j++)
                    lines.Add(new Line(points[i][j], points[i][j + 1]));
                lines.Add(new Line(points[i][points[i].Count()-1], points[i][0]));
            }
            



        }


        private void funcRotate()
        {
            if (comboBox1.SelectedIndex == -1)
                return;

            int ind = comboBox1.SelectedIndex;

            double a = 0;
            double b = 0;

            if (rotate_choosed)
            {
                a = (double)point_change.X;
                b = (double)point_change.Y;
            }
            else
            {
                for (int i = 0; i < points[ind].Count; i++)
                {
                    a += (double)points[ind][i].X;
                    b += (double)points[ind][i].Y;
                }

                a /= points[ind].Count;
                b /= points[ind].Count;

            }

            var g = Graphics.FromHwnd(pictureBox1.Handle);

            matrixRotate[0, 0] = Math.Cos(Math.PI * Convert.ToDouble(textBox3.Text) / 180.0);
            matrixRotate[0, 1] = Math.Sin(Math.PI * Convert.ToDouble(textBox3.Text) / 180.0);
            matrixRotate[1, 0] = -Math.Sin(Math.PI * Convert.ToDouble(textBox3.Text) / 180.0);
            matrixRotate[1, 1] = Math.Cos(Math.PI * Convert.ToDouble(textBox3.Text) / 180.0);

            for (int i = 0; i < points[ind].Count; i++)
            {
                double[,] matrixPoint = new double[1, 3] { { (double)points[ind][i].X, (double)points[ind][i].Y, 1.0 } };

                matrixReplace[2, 0] = Convert.ToDouble(-a);
                matrixReplace[2, 1] = Convert.ToDouble(-b);
                var res = multipleMatrix(matrixPoint, matrixReplace);

                res = multipleMatrix(res, matrixRotate);

                matrixReplace[2, 0] = Convert.ToDouble(a);
                matrixReplace[2, 1] = Convert.ToDouble(b);
                res = multipleMatrix(res, matrixReplace);

                points[ind][i] = new Point((int)res[0, 0], (int)res[0, 1]);
            }


            updateLines();
            peremalui();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            scale_choosed = true;
            rotate_choosed = false;
            edge_draw = false;

        }

        private void button10_Click(object sender, EventArgs e)
        {
            scale_choosed = false;
            rotate_choosed = false;
            edge_draw = false;
            is_convex = false;
            is_non_convex = false;
            how_to_edge = false;
            funcScale();
        }


        private void funcScale()
        {
            if (comboBox1.SelectedIndex == -1)
                return;

            int ind = comboBox1.SelectedIndex;

            double a = 0;
            double b = 0;

            if (scale_choosed)
            {
                a = (double)point_change.X;
                b = (double)point_change.Y;
            }
            else
            {
                for (int i = 0; i < points[ind].Count; i++)
                {
                    a += (double)points[ind][i].X;
                    b += (double)points[ind][i].Y;
                }

                a /= points[ind].Count;
                b /= points[ind].Count;

            }

            var g = Graphics.FromHwnd(pictureBox1.Handle);

            matrixScale[0, 0] = Convert.ToDouble(textBox4.Text);
            matrixScale[1, 1] = Convert.ToDouble(textBox5.Text);

            for (int i = 0; i < points[ind].Count; i++)
            {
                double[,] matrixPoint = new double[1, 3] { { (double)points[ind][i].X, (double)points[ind][i].Y, 1.0 } };

                matrixReplace[2, 0] = Convert.ToDouble(-a);
                matrixReplace[2, 1] = Convert.ToDouble(-b);
                var res = multipleMatrix(matrixPoint, matrixReplace);

                res = multipleMatrix(res, matrixScale);

                matrixReplace[2, 0] = Convert.ToDouble(a);
                matrixReplace[2, 1] = Convert.ToDouble(b);
                res = multipleMatrix(res, matrixReplace);

                points[ind][i] = new Point((int)res[0, 0], (int)res[0, 1]);
            }

            updateLines();
            peremalui();

        }

        private void button5_Click(object sender, EventArgs e)
        {
            rotate_choosed = false;
            scale_choosed = false;
            edge_draw = false;
            is_convex = false;
            is_non_convex = false;
            how_to_edge = false;

            if (comboBox2.SelectedIndex == -1)
                return;

            int ind = comboBox2.SelectedIndex;
            Point first = lines[ind].a;
            Point second = lines[ind].b;

            double a = (lines[ind].a.X + lines[ind].b.X) / 2.0;
            double b = (lines[ind].a.Y + lines[ind].b.Y) / 2.0;

            int abaz = -1;

            for (int i = 0; i < points.Count(); i++)
                for (int j = 0; j < points[i].Count() - 1; j++)
                    if (points[i][j] == first && points[i][j+1] == second)
                        abaz = i;

            if (abaz != -1)
            {
                List<Point> temp = points[abaz];
                points.RemoveAt(abaz);
                comboBox1.Items.RemoveAt(comboBox1.Items.Count - 1);

                for (int i = 0; i < temp.Count() - 1; i++)
                {
                    points.Add(new List<Point>());
                    points.Last().Add(temp[i]);
                    points.Last().Add(temp[i + 1]);
                    comboBox1.Items.Add("фигура" + points.Count().ToString());
                }
                points.Add(new List<Point>());
                points.Last().Add(temp[0]);
                points.Last().Add(temp[temp.Count()-1]);
                comboBox1.Items.Add("фигура" + points.Count().ToString());
            }

            var g = Graphics.FromHwnd(pictureBox1.Handle);

            matrixRotate[0, 0] = Math.Cos(Math.PI * 90.0 / 180.0);
            matrixRotate[0, 1] = Math.Sin(Math.PI * 90.0 / 180.0);
            matrixRotate[1, 0] = -Math.Sin(Math.PI * 90.0 / 180.0);
            matrixRotate[1, 1] = Math.Cos(Math.PI * 90.0 / 180.0);

            double[,] matrixPoint1 = new double[1, 3] { { (double)lines[ind].a.X, (double)lines[ind].a.Y, 1.0 } };
            double[,] matrixPoint2 = new double[1, 3] { { (double)lines[ind].b.X, (double)lines[ind].b.Y, 1.0 } };

            matrixReplace[2, 0] = Convert.ToDouble(-a);
            matrixReplace[2, 1] = Convert.ToDouble(-b);
            var res1 = multipleMatrix(matrixPoint1, matrixReplace);
            var res2 = multipleMatrix(matrixPoint2, matrixReplace);

            res1 = multipleMatrix(res1, matrixRotate);
            res2 = multipleMatrix(res2, matrixRotate);

            matrixReplace[2, 0] = Convert.ToDouble(a);
            matrixReplace[2, 1] = Convert.ToDouble(b);

            res1 = multipleMatrix(res1, matrixReplace);
            res2 = multipleMatrix(res2, matrixReplace);

            lines[ind].a.X = (int)res1[0, 0];
            lines[ind].a.Y = (int)res1[0, 1];
            lines[ind].b.X = (int)res2[0, 0];
            lines[ind].b.Y = (int)res2[0, 1];


            peremalui2();

        }

        private void peremalui2()
        {
            var g = Graphics.FromHwnd(pictureBox1.Handle);
            pictureBox1.Refresh();

            for (int i = 0; i < lines.Count(); i++)
            {
                if (i != comboBox2.SelectedIndex)
                    g.DrawLine(new Pen(Color.Black), lines[i].a, lines[i].b);
                else
                    g.DrawLine(new Pen(Color.BlueViolet), lines[i].a, lines[i].b);

            }



        }

        private void button8_Click(object sender, EventArgs e)
        {
            edge_draw = true;
            rotate_choosed = false;
            scale_choosed = false;
            draw_state = false;
            we_drawing = false;
            del_state = false;
            is_convex = false;
            is_non_convex = false;
            how_to_edge = false;

            newEdge = new List<Point>();

        }

        private void funcPointOfEdge()
        {
            var g = Graphics.FromHwnd(pictureBox1.Handle);
            g.DrawLine(new Pen(Color.Black), newEdge[0], newEdge[1]);

            double a1 = newEdge[0].Y - newEdge[1].Y;
            double b1 = newEdge[1].X - newEdge[0].X;
            double c1 = newEdge[0].X * newEdge[1].Y - newEdge[1].X * newEdge[0].Y;

            for (int i = 0; i < lines.Count(); i++)
            {
                double a2 = lines[i].a.Y - lines[i].b.Y;
                double b2 = lines[i].b.X - lines[i].a.X;
                double c2 = lines[i].a.X * lines[i].b.Y - lines[i].b.X * lines[i].a.Y;

                double divis = a1 * b2 - a2 * b1;
                if (divis == 0)
                    return;
                float x = (float)((b1 * c2 - b2 * c1) / divis);
                float y = (float)((c1 * a2 - c2 * a1) / divis);
                if (x <= Math.Max(newEdge[0].X, newEdge[1].X) && x >= Math.Min(newEdge[0].X, newEdge[1].X) && 
                    y <= Math.Max(newEdge[0].Y, newEdge[1].Y) && y >= Math.Min(newEdge[0].Y, newEdge[1].Y) &&
                    x <= Math.Max(lines[i].a.X, lines[i].b.X) && x >= Math.Min(lines[i].a.X, lines[i].b.X) &&
                    y <= Math.Max(lines[i].a.Y, lines[i].b.Y) && y >= Math.Min(lines[i].a.Y, lines[i].b.Y))
                    g.FillRectangle(Brushes.Red, x, y, 3, 3);
            }

        }

        private void button11_Click(object sender, EventArgs e)
        {
            edge_draw = false;
            rotate_choosed = false;
            scale_choosed = false;
            draw_state = false;
            we_drawing = false;
            del_state = false;
            is_convex = true;
            is_non_convex = false;
            how_to_edge = false;
            peremalui();
        }

        private bool isPointRightToEdge(Point a, Line line)
        {
            //yb·xa- xb·ya < 0 => b с
            return ((a.Y - line.a.Y) * (line.b.X - line.a.X) - (a.X - line.a.X) * (line.b.Y - line.a.Y)) < 0;
        }
        private void funcPointIntoConvex()
        {
            var g = Graphics.FromHwnd(pictureBox1.Handle);
            if (comboBox1.SelectedIndex == -1)
                return;
            int ind = comboBox1.SelectedIndex;


            int cnt_side = 0;

            for (int i = 0; i < points[ind].Count()-1; i++)
            {
                if (isPointRightToEdge(point_change, new Line( points[ind][i], points[ind][i+1])))
                    cnt_side++;
            }

            if (isPointRightToEdge(point_change, new Line(points[ind][points[ind].Count()-1], points[ind][0])))
                cnt_side++;

            if (cnt_side == points[ind].Count() || cnt_side == 0)
                g.FillRectangle(Brushes.LawnGreen, point_change.X, point_change.Y, 4, 4);
            else
                g.FillRectangle(Brushes.Red, point_change.X, point_change.Y, 4, 4);
        }

        private void button12_Click(object sender, EventArgs e)
        {
            edge_draw = false;
            rotate_choosed = false;
            scale_choosed = false;
            draw_state = false;
            we_drawing = false;
            del_state = false;
            is_convex = false;
            is_non_convex = true;
            how_to_edge = false;
            peremalui();
        }

        private void funcPointIntoNONConvex()
        {
            var g = Graphics.FromHwnd(pictureBox1.Handle);
            if (comboBox1.SelectedIndex == -1)
                return;
            int ind = comboBox1.SelectedIndex;

            Line helpline = new Line(point_change, new Point(pictureBox1.Width - 1, point_change.Y));
           

            HashSet<Point> intersect = new HashSet<Point>();

            double a1 = helpline.a.Y - helpline.b.Y;
            double b1 = helpline.b.X - helpline.a.X;
            double c1 = helpline.a.X * helpline.b.Y - helpline.b.X * helpline.a.Y;

            for (int i = 0; i < points[ind].Count() - 1; i++)
            {
                if (points[ind][i].Y == points[ind][i + 1].Y)
                    continue;

                double a2 = points[ind][i].Y - points[ind][i + 1].Y;
                double b2 = points[ind][i + 1].X - points[ind][i].X;
                double c2 = points[ind][i].X * points[ind][i + 1].Y - points[ind][i + 1].X * points[ind][i].Y;

                double divis = a1 * b2 - a2 * b1;
                if (divis == 0)
                    return;
                float x = (float)((b1 * c2 - b2 * c1) / divis);
                float y = (float)((c1 * a2 - c2 * a1) / divis);
                if (x <= Math.Max(helpline.a.X, helpline.b.X) && x >= Math.Min(helpline.a.X, helpline.b.X) &&
                    y <= Math.Max(helpline.a.Y, helpline.b.Y) && y >= Math.Min(helpline.a.Y, helpline.b.Y) &&
                    x <= Math.Max(points[ind][i].X, points[ind][i + 1].X) && x >= Math.Min(points[ind][i].X, points[ind][i + 1].X) &&
                    y <= Math.Max(points[ind][i].Y, points[ind][i + 1].Y) && y >= Math.Min(points[ind][i].Y, points[ind][i + 1].Y))
                    intersect.Add(new Point((int)x, (int)y));
            }

            if (points[ind][points[ind].Count() - 1].Y != points[ind][0].Y)
            {
                double aa2 = points[ind][points[ind].Count() - 1].Y - points[ind][0].Y;
                double bb2 = points[ind][0].X - points[ind][points[ind].Count() - 1].X;
                double cc2 = points[ind][points[ind].Count() - 1].X * points[ind][0].Y - points[ind][0].X * points[ind][points[ind].Count() - 1].Y;

                double ddivis = a1 * bb2 - aa2 * b1;
                if (ddivis == 0)
                    return;
                float xx = (float)((b1 * cc2 - bb2 * c1) / ddivis);
                float yy = (float)((c1 * aa2 - cc2 * a1) / ddivis);
                if (xx <= Math.Max(helpline.a.X, helpline.b.X) && xx >= Math.Min(helpline.a.X, helpline.b.X) &&
                    yy <= Math.Max(helpline.a.Y, helpline.b.Y) && yy >= Math.Min(helpline.a.Y, helpline.b.Y) &&
                    xx <= Math.Max(points[ind][points[ind].Count() - 1].X, points[ind][0].X) && xx >= Math.Min(points[ind][points[ind].Count() - 1].X, points[ind][0].X) &&
                    yy <= Math.Max(points[ind][points[ind].Count() - 1].Y, points[ind][0].Y) && yy >= Math.Min(points[ind][points[ind].Count() - 1].Y, points[ind][0].Y))
                    intersect.Add(new Point((int)xx, (int)yy));
            }

            if (intersect.Count() % 2 != 0 )
                g.FillRectangle(Brushes.LawnGreen, point_change.X, point_change.Y, 4, 4);
            else
                g.FillRectangle(Brushes.Red, point_change.X, point_change.Y, 4, 4);

        }

        private void button13_Click(object sender, EventArgs e)
        {
            edge_draw = false;
            rotate_choosed = false;
            scale_choosed = false;
            draw_state = false;
            we_drawing = false;
            del_state = false;
            is_convex = false;
            is_non_convex = false;
            how_to_edge = true;
            peremalui2();

        }

        private void sortLines()
        { 
        
        
        }

        private void funcHowPointToEdge()
        {
            var g = Graphics.FromHwnd(pictureBox1.Handle);
            if (comboBox2.SelectedIndex == -1)
                return;
            int ind = comboBox2.SelectedIndex;

            if (isPointRightToEdge(point_change, lines[ind]))
                g.FillRectangle(Brushes.Red, point_change.X, point_change.Y, 4, 4);
            else
                g.FillRectangle(Brushes.LawnGreen, point_change.X, point_change.Y, 4, 4);

        }

        
    }
}
