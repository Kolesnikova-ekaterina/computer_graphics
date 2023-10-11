using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace lab5_2
{
    public partial class Form1 : Form
    {
        private int clickCount;
        private Point start;
        private Point end;
        Random rnd = new Random();
        LinkedList<Point> points = new LinkedList<Point>();
        Bitmap bmp;

        public Form1()
        {
            InitializeComponent();
            bmp = new Bitmap(pictureBox1.Width, pictureBox1.Height);
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                clickCount++;

                if (clickCount == 1)
                {
                    start = e.Location;
                    DrawPoint();

                }
                if (clickCount == 2)
                {
                    end = e.Location;
                    DrawLines(); 
                    points.AddFirst(start);
                    points.AddLast(end);
                    clickCount = 0;
                }
            }
        }

        void DrawPoint()
        {
            var g = Graphics.FromHwnd(pictureBox1.Handle);
            g.FillRectangle(Brushes.Gray, start.X, start.Y, 2, 2);
        }

        void DrawLines()
        {
            pictureBox1.Refresh();
            var g = Graphics.FromHwnd(pictureBox1.Handle);

            g.DrawLine(new Pen(Color.Gray, 2), start, end);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            funcMidpoint();
        }

        void funcMidpoint()
        {
            double rough = Convert.ToDouble(textBox1.Text);

            var g = Graphics.FromHwnd(pictureBox1.Handle);

            var first_point = points.First;

            while (first_point != points.Last )
            {
                Point h = new Point();
                var next_point = first_point.Next;

                var dist = Math.Sqrt(Math.Pow(next_point.Value.X - first_point.Value.X, 2) + Math.Pow(next_point.Value.Y - first_point.Value.Y, 2));
                h.X = (next_point.Value.X - first_point.Value.X) / 2 + first_point.Value.X;
                h.Y = (next_point.Value.Y - first_point.Value.Y) / 2 + first_point.Value.Y + rnd.Next(-(int)(rough * dist), (int)(rough * dist));

                points.AddAfter(first_point, h);
                first_point = next_point;
            }

            pictureBox1.Refresh();
            g.DrawLine(new Pen(Color.LightGray, 2), start, end);

            for (var iter = points.First; iter != points.Last; iter = iter.Next)
            {
                g.DrawLine(new Pen(Color.Black, 2), iter.Value.X, iter.Value.Y, iter.Next.Value.X, iter.Next.Value.Y);
            }

        }


        private void button2_Click(object sender, EventArgs e)
        { 
            points.Clear();
            start = new Point();
            end = new Point();
            pictureBox1.Refresh();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var g = Graphics.FromHwnd(pictureBox1.Handle);
            pictureBox1.Refresh();

            points.Clear();
            points.AddFirst(start);
            points.AddLast(end);
            g.DrawLine(new Pen(Color.LightGray, 2), start, end);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            FillMountain();
        }

        void FillMountain()
        {
            pictureBox1.Refresh();

            var g = Graphics.FromHwnd(pictureBox1.Handle);
            GraphicsPath gp = new GraphicsPath();
            gp.AddLine(new Point(0, pictureBox1.Height), new Point(0, start.Y));
            gp.AddLine(new Point(0, start.Y), start);
            for (int i = 0; i < points.Count() - 1; i++)
            {
                gp.AddLine(points.ElementAt(i), points.ElementAt(i+1));
            
            }
            gp.AddLine(end, new Point(pictureBox1.Width, end.Y));
            gp.AddLine(new Point(pictureBox1.Width, end.Y), new Point(pictureBox1.Width, pictureBox1.Height));
            gp.CloseFigure();
            g.FillPath(Brushes.Black, gp);
        }

    }
}
