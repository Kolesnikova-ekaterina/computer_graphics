using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace WinFormsApp4
{
    public partial class Form1 : Form
    {
        public class BorderPoint
        {
            public Point Point { get; set; }
            public byte Type { get; set; }
        }
        Bitmap image;
        Bitmap photo2;
        Bitmap photo3;
        bool photo3_is_here = false;
        Point start;
        private List<Point> BorderPixels;
        private List<List<Point>> points;
        private bool drawing = false;
        Color boundColor = Color.Black;
        Color backColor = Color.PaleVioletRed;
        public Form1()
        {
            InitializeComponent();
            Bitmap bmp = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            pictureBox1.Image = bmp;
            image = new Bitmap(pictureBox1.Image);
            points = new List<List<Point>>();
            BorderPixels = new List<Point>();
            colorDialog1.FullOpen = true;
            photo2 = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            photo3 = new Bitmap(pictureBox1.Width, pictureBox1.Height);
        }
        private void md_1(object sender, MouseEventArgs e)
        {
            start = new Point(e.X, e.Y);
            points.Add(new List<Point>());
            points.Last().Add(start);
            drawing = true;

        }
        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            switch (this.comboBox2.SelectedIndex)
            {
                case 0:
                    if (this.comboBox1.SelectedIndex == 0)
                        md_1(sender, e);
                    else
                    {
                        fill_the_field(new Point(e.X, e.Y));
                    }
                    break;
                case 1:
                    if (this.comboBox1.SelectedIndex == 0)
                        md_1(sender, e);
                    else
                    {
                        fill_the_field_with_photo(new Point(e.X, e.Y));
                    }
                    break;
                case 2:
                    if (photo3_is_here) { 
                        //find_the_bound(e.Location);
                        var l = GetBoundaryPoints(e.Location);
                        DrawBoundary(l);
                        pictureBox1.Image = photo3;
                    }

                    break;
                default:
                    break;

            }
        }

        private double dist(Color c1, Color c2)
        {
            return Math.Sqrt(Math.Pow(c1.R - c2.R, 2) + Math.Pow(c1.G - c2.G, 2) + Math.Pow(c1.B - c2.B, 2) + Math.Pow(c1.A - c2.A, 2) );

        }



        private List<Point> GetBoundaryPoints(Point startPoint)
        {
            List<Point> boundaryPoints = new List<Point>();
            Stack<Point> stack = new Stack<Point>();
            HashSet<Point> visitedPoints = new HashSet<Point>();

            stack.Push(startPoint);

            Color targetColor = photo3.GetPixel(startPoint.X, startPoint.Y);

            while (stack.Count > 0)
            {
                Point currentPoint = stack.Pop();

                if (currentPoint.X < 0 || currentPoint.X >= photo3.Width
                    || currentPoint.Y < 0 || currentPoint.Y >= photo3.Height)
                {
                    continue;
                }

                if (photo3.GetPixel(currentPoint.X, currentPoint.Y) == targetColor)
                {
                    boundaryPoints.Add(currentPoint);
                }
                else
                {
                    continue;
                }

                if (visitedPoints.Contains(currentPoint))
                {
                    continue;
                }

                visitedPoints.Add(currentPoint);
                stack.Push(new Point(currentPoint.X - 1, currentPoint.Y-1));
                stack.Push(new Point(currentPoint.X + 1, currentPoint.Y-1));
                stack.Push(new Point(currentPoint.X+1, currentPoint.Y - 1));
                stack.Push(new Point(currentPoint.X-1, currentPoint.Y + 1));
                stack.Push(new Point(currentPoint.X - 1, currentPoint.Y));
                stack.Push(new Point(currentPoint.X + 1, currentPoint.Y));
                stack.Push(new Point(currentPoint.X, currentPoint.Y - 1));
                stack.Push(new Point(currentPoint.X, currentPoint.Y + 1));
            }

            return boundaryPoints;
        }

        private void DrawBoundary(List<Point> boundaryPoints)
        {
            Random r = new Random();
            Color[] colors = { Color.Aqua, Color.MintCream, Color.RebeccaPurple, Color.CornflowerBlue, Color.DarkSalmon, Color.Magenta, Color.OrangeRed, Color.Salmon, Color.Coral };
                
            Color c = colors[r.Next(0, colors.Length)];
            foreach (Point point in boundaryPoints)
            {
                photo3.SetPixel(point.X, point.Y, c);
            }
        }







        private void find_the_bound(Point current)
        {
            var photo = new Bitmap(pictureBox1.Image);
            int[][] ints = new int[photo.Width][];
            for (int i = 0; i < photo3.Width; i++)
            {
                ints[i] = new int[photo3.Height];
            }
            Color b = photo.GetPixel(current.X, current.Y);
            Queue<Point> p = new Queue<Point>();
            p.Enqueue(current);
            while (p.Count > 0) { 
                Point point = p.Dequeue();
                photo.SetPixel(point.X, point.Y, Color.FromArgb((b.R + 100) % 256, (b.G + 100) % 256, (b.B + 100)%256));
                ints[point.X][point.Y] = 1;

                for (int i = point.X - 1;i < point.X + 2; i++)
                {
                    if (i < 0 || i >= photo.Width)
                        continue;
                    for (int j = point.Y - 1; j < point.Y + 2; j++)
                    {
                        if (j < 0 || j >= photo.Height)
                            continue;
                        if (ints[i][j]!=1 && dist(b, photo.GetPixel(i,j)) < 10)
                             p.Enqueue(new Point(i, j));
                    }
                }


            }
            pictureBox1.Image = photo;

        }


        private void fill_the_field_with_photo(Point current)
        {
            if (backColor == Color.Black)
                return;
            Color back = image.GetPixel(current.X, current.Y);
            int[][] ints = new int[image.Width][];
            for (int i = 0; i < image.Width; i++)
            {
                ints[i] = new int[image.Height];
            }
            foreach (var p in BorderPixels)
            {
                if (p.X < image.Width && p.Y < image.Height && p.X >= 0 && p.Y >= 0)
                {
                    image.SetPixel(p.X, p.Y, boundColor);
                    ints[p.X][p.Y] = 1;
                }
            }
            int x0 = current.X; 
            int y0 = current.Y;


            fillthefield_rec_with_photo(current, x0, y0, back, ints);
            pictureBox1.Image = image;
        }

        private void fillthefield_rec_with_photo(Point current, int x0, int y0, Color back, int[][] ints)
        {
            int current_x = current.X;
            int current_y = current.Y;

            int photow = photo2.Width;
            int photoh = photo2.Height;


            if (colormatch(boundColor, image.GetPixel(current_x, current_y))) { return; }
            if (colormatch(backColor, image.GetPixel(current_x, current_y))) { return; }

            while (current_x < image.Width && !colormatch(boundColor, image.GetPixel(current_x, current_y)))
            {
                int newx = (photow + ( current_x - x0) % photow) % photow;
                int newy = (photoh +( current_y - y0) % photoh) % photoh;
                Color c = photo2.GetPixel(newx, newy);
                image.SetPixel(current_x, current_y, c);
                ints[current_x][current_y] = 2;
                current_x++;
            }
            int x_right = --current_x;
            current_x = current.X - 1;
            while (current_x >= 0 && !colormatch(boundColor, image.GetPixel(current_x, current_y)))
            {
                int newx = (photow + (current_x - x0) % photow) % photow;
                int newy = (photoh + (current_y - y0) % photoh) % photoh;
                Color c = photo2.GetPixel(newx, newy);
                image.SetPixel(current_x, current_y, c);
                ints[current_x][current_y] = 2;
                current_x--;
            }
            //pictureBox1.Image = image;
            int x_left = ++current_x;

            while (current_x <= x_right)
            {
                //if (colormatch(boundColor, image.GetPixel(current_x, current_y)))
                //continue;
                if (current_y + 1 < pictureBox1.Height && ints[current_x][current_y + 1] == 0)
                    fillthefield_rec_with_photo(new Point(current_x, current_y + 1), x0, y0, back, ints);
                if (current_y - 1 >= 0 && ints[current_x][current_y -1] == 0)
                    fillthefield_rec_with_photo(new Point(current_x, current_y - 1), x0, y0, back, ints);

                current_x++;
            }
        }


        private void fill_the_field(Point current)
        {
            if (backColor == Color.Black)
                return;
            foreach (var p in BorderPixels)
            {
                if (p.X<image.Width && p.Y < image.Height && p.X >= 0 && p.Y>= 0)
                  image.SetPixel(p.X, p.Y, boundColor);
            }


            fillthefield_rec(current);
            pictureBox1.Image = image;



        }
        private bool colormatch(Color c1, Color c2)
        {
            return c1.R == c2.R && c1.B == c2.B && c1.G == c2.G && c1.A == c2.A ;
        }

        private void fillthefield_rec(Point current)
        {
            int current_x = current.X;
            int current_y = current.Y;
            
            if (colormatch(boundColor, image.GetPixel(current_x, current_y))) { return; }
            if (colormatch(backColor, image.GetPixel(current_x, current_y))) { return; }

            while (current_x < image.Width && !colormatch(boundColor, image.GetPixel(current_x, current_y)))
            {
                image.SetPixel(current_x, current_y, backColor);
                current_x++;
            }
            int x_right = --current_x;
            current_x = current.X - 1;
            while (current_x >= 0 && !colormatch(boundColor, image.GetPixel(current_x, current_y)))
            {
                image.SetPixel(current_x, current_y, backColor);
                current_x--;
            }
            pictureBox1.Image = image;
            int x_left = ++current_x;

            while(current_x<=x_right)
            {
                //if (colormatch(boundColor, image.GetPixel(current_x, current_y)))
                    //continue;
                if (current_y+1 < pictureBox1.Height && !colormatch(backColor, image.GetPixel(current_x, current_y+1)))
                    fillthefield_rec(new Point(current_x, current_y + 1));
                if (current_y - 1 >= 0 && !colormatch(backColor, image.GetPixel(current_x, current_y - 1)))
                     fillthefield_rec(new Point(current_x, current_y - 1));

                current_x++;
            }

        }


        private bool pointcontains(int x, int y)
        {
            foreach (var p in BorderPixels)
                if (p.X == x && p.Y == y)
                    return true;
            return false;
        }
        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            switch (this.comboBox2.SelectedIndex)
            {
                case 0:
                    if (this.comboBox1.SelectedIndex == 0)
                        mm_1(sender, e);
                    break;
                case 1:
                    if (this.comboBox1.SelectedIndex == 0)
                        mm_1(sender, e);
                    break;
                default:
                    break;

            }
        }
        private void mm_1(object sender, MouseEventArgs e)
        {
            if (!drawing) return;

            var finish = new Point(e.X, e.Y);
            points.Last().Add(finish);
            var g = Graphics.FromHwnd(pictureBox1.Handle);
            g.DrawLine(new Pen(boundColor), points.Last()[points.Last().Count - 1], points.Last()[points.Last().Count - 2]);
            /*
             var gr = pictureBox1.CreateGraphics();

             //Рисуем точку границы
             pictureBox1.CreateGraphics().DrawRectangle(new Pen(boundColor), new Rectangle(e.X, e.Y, 1, 1)); 

             var bp = new BorderPoint
             {
                 Point = new Point(e.X, e.Y),
                 Type = (byte)PathPointType.Line
             };
             BorderPixels.Add(bp);*/
            //label1.Text += BorderPixels.Last().Point.X.ToString() + '\n';  

        }

        private List<Point> recover_thepoints(Point a, Point b)
        {
            var res = new List<Point>();
            int x1 = a.X;
            int x2 = b.X;
            int y1 = a.Y;
            int y2 = b.Y;
            if (x1 == x2)
            {
                for (int j = Math.Min(y1, y2); j <= Math.Max(y2, y1); j++)
                {
                    res.Add(new Point(x1, j));

                }
                return res;
            }
            if (x1 > x2)
            {
                int t = x1;
                x1 = x2;
                x2 = t;
                t = y1;
                y1 = y2;
                y2 = t;
            }
            int prevy = y1;
            for (int i = x1; i <= x2; i++)
            {
                int y = (int)((double)(i - x1) * (y2 - y1) / (x2 - x1)) + y1;
                for (int j = Math.Min(y, prevy); j <= Math.Max(y, prevy); j++)
                {
                    res.Add(new Point(i, j));

                }
                prevy = y;
                res.Add(new Point(i, y));
            }

            return res;
        }
        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            switch (this.comboBox2.SelectedIndex)
            {
                case 0:
                    if (this.comboBox1.SelectedIndex == 0)
                        mu_1(sender, e);
                    break;
                case 1:
                    if (this.comboBox1.SelectedIndex == 0)
                        mu_1(sender, e);
                    break;
                default:
                    break;

            }
        }
        private void mu_1(object sender, MouseEventArgs e)
        {
            if (points.Last().Count < 2)
            {
                drawing = false;
                return; 
            }
            var g = Graphics.FromHwnd(pictureBox1.Handle);
            g.DrawLine(new Pen(boundColor), points.Last()[points.Last().Count - 1], start);
            drawing = false;
            var list = new List<Point>();
            for (int j = 0; j < points.Count; j++)
            {
                for (int i = 1; i < points[j].Count; i++)
                {
                    var rec = recover_thepoints(points[j][i - 1], points[j][i]);
                    list.AddRange(rec);

                }
                var rec1 = recover_thepoints(points[j][points[j].Count - 1], points[j][0]);
                list.AddRange(rec1);
            }
            BorderPixels.AddRange(list);
        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (this.comboBox2.SelectedIndex)
            {
                case 0:
                    if (this.comboBox1.SelectedIndex == 0)
                    {
                        this.button1.Enabled = false;

                    }
                    else
                    {
                        this.button1.Enabled = true;
                    }
                    break;
                default:
                    if (this.comboBox1.SelectedIndex == 0)
                    {
                        this.button2.Enabled = false;
                    }
                    else
                    {
                        this.button2.Enabled = true;
                    }
                    break;

            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog() == DialogResult.Cancel)
                return;
            backColor = colorDialog1.Color;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files|*.bmp;*.jpg;*.jpeg;*.png";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                var phototemp = new Bitmap(openFileDialog.FileName);

                if (this.comboBox2.SelectedIndex == 2) {
                    photo3 = phototemp;
                    photo3_is_here = true;
                    pictureBox1.Image = photo3;
                    //pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
                }
                if (this.comboBox2.SelectedIndex == 1)
                {
                    photo2 = phototemp;
                }

            }
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (this.comboBox2.SelectedIndex)
            {
                case 0:
                    button3_Click(sender, e);
                    this.comboBox1.SelectedIndex = 0;
                    this.comboBox1.Enabled = true;
                    this.button1.Enabled = false;
                    this.button2.Enabled = false;
                    break;
                case 1:
                    button3_Click(sender, e);
                    this.comboBox1.SelectedIndex = 0;
                    this.comboBox1.Enabled = true;
                    this.button1.Enabled = false;
                    this.button2.Enabled = false;
                    break;
                default:
                    button3_Click(sender, e);
                    this.comboBox1.Enabled = false;
                    this.button1.Enabled = false;
                    this.button2.Enabled = true;
                    break;

            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            points = new List<List<Point>>();
            BorderPixels = new List<Point>();
            image = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            pictureBox1.Image = image;
        }
    }
}