using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace lab22
{
    public partial class Form1 : Form
    {
        Bitmap image;
        
        public Form1()
        {
            InitializeComponent();
        }

        public void Task1()
        {
            int w = image.Width;
            int h = image.Height;


            Bitmap myImage1 = new Bitmap(w, h);
            Bitmap myImage2 = new Bitmap(w, h);
            Bitmap diffImage = new Bitmap(w, h);

            for (int x = 0; x < w; x++)
            {
                for (int y = 0; y < h; y++)
                {
                    Color pixelColor = ((Bitmap)image).GetPixel(x, y);

                    int r = pixelColor.R;
                    int g = pixelColor.G;
                    int b = pixelColor.B;

                    int val = (int)(0.299 * r + 0.587 * g + 0.114 * b);
                    int val2 = (int)(0.2126 * r + 0.7152 * g + 0.0722 * b);

                    myImage1.SetPixel(x, y, Color.FromArgb(val, val, val));

                    pictureBox2.Image = myImage1;
                    myImage2.SetPixel(x, y, Color.FromArgb(val2, val2, val2));
                    pictureBox3.Image = myImage2;
                    diffImage.SetPixel(x, y, Color.FromArgb(Math.Abs(val - val2), Math.Abs(val - val2), Math.Abs(val - val2)));
                    pictureBox4.Image = diffImage;
                }
            }

            int[] arr1 = new int[256];
            int[] arr2 = new int[256];
            for (int i = 0; i < 256; i++)
            {
                arr1[i] = 0;
                arr2[i] = 0;
            }

            

            for (int x = 0; x < w; x++)
            {

                for (int y = 0; y < h; y++)
                {
                    int grey1 = ((Bitmap)myImage1).GetPixel(x, y).R;
                    int grey2 = ((Bitmap)myImage2).GetPixel(x, y).R;
                    arr1[grey1] += 1;
                    arr2[grey2] += 1;
                }
            }

            chart1.Series[0].IsVisibleInLegend = false;
            chart1.Series[0].Points.DataBindY(arr1);
            chart2.Series[0].IsVisibleInLegend = false;
            chart2.Series[0].Points.DataBindY(arr2);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            //openFileDialog.Filter = "Image Files|.bmp;.jpg;.jpeg;.png";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                image = new Bitmap(openFileDialog.FileName);
                pictureBox1.Image = image;

                Task1();
            }

        }
    }

}
