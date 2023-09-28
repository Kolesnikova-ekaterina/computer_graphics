using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace lab3_3
{
    public partial class Form1 : Form
    {
        private Point[] trianglePoints;
        private Color[] colors = { Color.Aqua, Color.BlueViolet, Color.MistyRose };
        private int clickCount;
        public static Bitmap image;
        public Form1()
        {
            InitializeComponent();
            trianglePoints = new Point[3];
            clickCount = 0;
            this.MouseDown += pictureBox1_MouseDown;
            this.Paint += pictureBox1_Paint;
            button1.BackColor = colors[0];
            button2.BackColor = colors[1];
            button3.BackColor = colors[2];

            image = new Bitmap(pictureBox1.Width, pictureBox1.Height);
        }


        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                trianglePoints[clickCount] = e.Location;
                clickCount++;

                if (clickCount == 3)
                {
                    //this.Refresh(); // Перерисовываем форму после трех щелчков
                    //Gradient3();
                    //Gradient4();
                    Rasterization();
                    clickCount = 0; // Сбрасываем счетчик щелчков
                }
            }
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        { }

        public void Rasterization()
        {
            for (int y = 0; y < pictureBox1.Height; y++)
                for (int x = 0; x < pictureBox1.Width; x++)
                    image.SetPixel(x, y, ShadeBackgroundPixel(x, y));
            pictureBox1.Image = image;
        }

        public Color ShadeBackgroundPixel(int x, int y)
        {
            double l1, l2, l3;
            int i;
            Color res = Color.Empty;

            l1 = ((trianglePoints[1].Y - trianglePoints[2].Y) * ((double)(x) - trianglePoints[2].X) + (trianglePoints[2].X - trianglePoints[1].X) * ((double)(y) - trianglePoints[2].Y)) /
                ((trianglePoints[1].Y - trianglePoints[2].Y) * (trianglePoints[0].X - trianglePoints[2].X) + (trianglePoints[2].X - trianglePoints[1].X) * (trianglePoints[0].Y - trianglePoints[2].Y));
            l2 = ((trianglePoints[2].Y - trianglePoints[0].Y) * ((double)(x) - trianglePoints[2].X) + (trianglePoints[0].X - trianglePoints[2].X) * ((double)(y) - trianglePoints[2].Y)) /
                ((trianglePoints[1].Y - trianglePoints[2].Y) * (trianglePoints[0].X - trianglePoints[2].X) + (trianglePoints[2].X - trianglePoints[1].X) * (trianglePoints[0].Y - trianglePoints[2].Y));
            l3 = 1 - l1 - l2;

            if (l1 >= 0 && l1 <= 1 && l2 >= 0 && l2 <= 1 && l3 >= 0 && l3 <= 1)
            {
                

                var red = (int)(l1 * colors[0].R + l2 * colors[1].R + l3 * colors[2].R);
                var green = (int)(l1 * colors[0].G + l2 * colors[1].G + l3 * colors[2].G);
                var blue = (int)(l1 * colors[0].B + l2 * colors[1].B + l3 * colors[2].B);

                res = Color.FromArgb(red, green, blue);


            }

            return res;
        }


        private void button1_Click(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                colors[0] = colorDialog1.Color;
                button1.BackColor = colorDialog1.Color;
            }
            Rasterization();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                colors[1] = colorDialog1.Color;
                button2.BackColor = colorDialog1.Color;
            }
            Rasterization();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                colors[2] = colorDialog1.Color;
                button3.BackColor = colorDialog1.Color;
            }
            Rasterization();
        }


    }
}

