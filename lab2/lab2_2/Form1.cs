using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace lab2_2
{
    public partial class Form1 : Form
    {
        Bitmap image;
        public Form1()
        {
            InitializeComponent();
        }

        private void Task2()
        {
                Bitmap redChannel = new Bitmap(image.Width, image.Height);
                Bitmap greenChannel = new Bitmap(image.Width, image.Height);
                Bitmap blueChannel = new Bitmap(image.Width, image.Height);

                for (int i = 0; i < image.Width; i++)
                {
                    for (int j = 0; j < image.Height; j++)
                    {
                        Color pixel = image.GetPixel(i, j);
                        redChannel.SetPixel(i, j, Color.FromArgb(pixel.R, 0, 0));
                        greenChannel.SetPixel(i, j, Color.FromArgb(0, pixel.G, 0));
                        blueChannel.SetPixel(i, j, Color.FromArgb(0, 0, pixel.B));
                    }
                }

                pictureBox2.Image = redChannel;
                pictureBox3.Image = greenChannel;
                pictureBox4.Image = blueChannel;

                int[] redHistogram = GetHistogram(redChannel, "red");
                int[] greenHistogram = GetHistogram(greenChannel, "green");
                int[] blueHistogram = GetHistogram(blueChannel, "blue");

                DrawHistogram(redHistogram, "Red", panel1);
                DrawHistogram(greenHistogram, "Green", panel2);
                DrawHistogram(blueHistogram, "Blue", panel3);
            
        }

        private int[] GetHistogram(Bitmap channel, string color)
        {
            int[] histogram = new int[256];

            for (int i = 0; i < channel.Width; i++)
            {
                for (int j = 0; j < channel.Height; j++)
                {
                    Color pixel = channel.GetPixel(i, j);
                    if (color == "red")
                        histogram[pixel.R]++;
                    else if (color == "blue")
                        histogram[pixel.B]++;
                    else if (color == "green")
                        histogram[pixel.G]++;
                }
            }

            return histogram;
        }

        private void DrawHistogram(int[] histogram, string channelName, Panel panel)
        {
            panel.Controls.Clear();

            int maxValue = FindMaxValue(histogram);

            for (int i = 0; i < histogram.Length; i++)
            {
                Panel bar = new Panel();
                int barHeight = (int)((double)histogram[i] / maxValue * panel.Height);

                if (channelName == "Red")
                    bar = CreateBar(Color.FromArgb(i, 0, 0), barHeight);
                else if (channelName == "Blue")
                    bar = CreateBar(Color.FromArgb(0, 0, i), barHeight);
                else if (channelName == "Green")
                    bar = CreateBar(Color.FromArgb(0, i, 0), barHeight);

                bar.Location = new Point(i, panel.Height - bar.Height);
                panel.Controls.Add(bar);
            }
            

        }

        private Panel CreateBar(Color color, int height)
        {
            Panel bar = new Panel();
            bar.BackColor = color;
            bar.Width = 1;
            bar.Height = height;
            return bar;
        }

        private int FindMaxValue(int[] array)
        {
            int maxValue = int.MinValue;

            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] > maxValue)
                {
                    maxValue = array[i];
                }
            }

            return maxValue;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files|*.bmp;*.jpg;*.jpeg;*.png";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                image = new Bitmap(openFileDialog.FileName);
                pictureBox1.Image = image;
                Task2();
            }
        }
    }
}
