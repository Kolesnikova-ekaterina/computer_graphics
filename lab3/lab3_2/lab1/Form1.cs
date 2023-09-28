using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;

using System.Windows.Forms;

namespace lab1
{
    public partial class Form1 : Form
    {

        private PointF start;
        private PointF finish;

        private int position_dx;
        private int position_dy;
        bool change_f;
        Graphics g;

        private string algorithm = "Bresenham";
        public Form1()
        {
            InitializeComponent();
            // this.WindowState = FormWindowState.Maximized;
            // this.Size = new Size(2000, 1000);
            MouseDown += new MouseEventHandler(Mouse_Click);
            //g = this.CreateGraphics();


        }

        private void Mouse_Click(object sender, MouseEventArgs e)
        {
            if (start.IsEmpty)
            {
                start = e.Location;
            }
            else
            {
                finish = e.Location;
                if (algorithm == "Bresenham")
                {
                    DrawWithBresenham(start, finish);
                }
                else if (algorithm == "WuAlgorithm")
                {
                    WuAlgorithm((int)start.X, (int)start.Y, (int)finish.X, (int)finish.Y);
                }
                start = PointF.Empty;
                finish = PointF.Empty;
            }
        }

        private int Sign(int x)
        {
            return (x > 0) ? 1 : (x < 0) ? -1 : 0;
        }

        private void DrawWithBresenham(PointF start, PointF finish)
        {
            g = this.CreateGraphics();
            int dx = (int)Math.Abs(finish.X - start.X);
            int dy = (int)Math.Abs(finish.Y - start.Y);
            int signX = Sign((int)(finish.X - start.X));
            int signY = Sign((int)(finish.Y - start.Y));
            int x = (int)start.X;
            int y = (int)start.Y;
            if (dy > dx)
            {
                int cur = dx;
                dx = dy;
                dy = cur;
                change_f = true;
            }
            else
                change_f = false;
            int error = 2 * dy - dx;

            for (int i = 1; i < dx; i++)
            {
                g.FillRectangle(Brushes.BlueViolet, x, y, 1, 1);
                //g.FillRectangle(Brushes.BlueViolet, x, y, 1, 1);
                if (start == finish)
                    break;
                while (error >= 0)
                {
                    if (change_f)
                        x += signX;
                    else
                        y += signY;
                    error = error - 2 * dx;
                }
                if (change_f)
                    y += signY;
                else
                    x += signX;
                error = error + 2 * dy;
            }

            g.Dispose();

        }

        private void Swap (ref int x, ref int y)
        {
            int step = y;
            y = x;
            x = step;
        }

        private void DrawPoint(bool steep, int x, int y, float a)
        {
            Pen pen = new Pen(Color.FromArgb((int)(a*255), 255, 0, 0), 1);
            if (steep)
            {
                g.DrawRectangle(pen, new Rectangle(y, x, 1, 1));
            }
            else
            {
                g.DrawRectangle(pen, new Rectangle(x, y, 1, 1));
            }
        }
        private void WuAlgorithm(int x0, int y0, int x1, int y1)
        {
            g = this.CreateGraphics();
            var steep = Math.Abs(y1 - y0) > Math.Abs(x1 - x0);
            if (steep)
            {
                Swap( ref x0, ref y0);
                Swap(ref x1, ref y1);
            }
            if (x0 > x1)
            {
                Swap(ref x0, ref x1);
                Swap(ref y0, ref y1);
            }

            DrawPoint(steep, x0, y0, 1); // Эта функция автоматом меняет координаты местами в зависимости от переменной steep
            DrawPoint(steep, x1, y1, 1); // Последний аргумент — интенсивность в долях единицы
            float dx = x1 - x0;
            float dy = y1 - y0;
            float gradient = dy / dx;
            float y = y0 + gradient;
            for (var x = x0 + 1; x <= x1 - 1; x++)
            {
                DrawPoint(steep, x, (int)y, 1 - (y - (int)y));
                DrawPoint(steep, x, (int)y + 1, y - (int)y);
                y += gradient;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            button1.Click += button1_Click;
            button2.Click += button2_Click;
        }
        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            algorithm = "Bresenham";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            algorithm = "WuAlgorithm";
        }
    }
}
