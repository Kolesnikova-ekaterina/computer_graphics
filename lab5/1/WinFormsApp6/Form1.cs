using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Text;
using static System.Windows.Forms.LinkLabel;

namespace WinFormsApp6
{
    public partial class Form1 : Form
    {
        class CraftLine
        {
            public float currentX, currentY, newX, newY;
            public CraftLine(float currentX, float currentY, float newX, float newY)
            {
                this.currentX = currentX;
                this.currentY = currentY;
                this.newX = newX;
                this.newY = newY;
            }
        }
        private string atom;
        private double angle;
        private int direction;
        private List<string> rules;
        private Dictionary<string, List<string>> rulesMap;
        private int startX;
        private int startY;
        double maxh;
        double maxw;
        double minh;
        double minw;
        int maxit = 5;
        int maxlevel = 0;
        private double scale;
        private bool randomize;
        //Panel panel;
        private Graphics graphics;
        private Pen pen;
        Color brown = Color.FromArgb( 150, 75, 0);
        Color lime = Color.FromArgb(0, 255, 0);
        List<CraftLine> lines = new List<CraftLine>();
        public Form1()
        {
            //color 1 = 150 75 0 
            // color2 = 0 255 0
            InitializeComponent();
            //panel = new Panel();
        }
        private void MainForm_Load(object sender, EventArgs e)
        {
            graphics = panel.CreateGraphics();
            pen = new Pen(Color.Black);

            atom = "";
            angle = 0;
            direction = 0;
            rules = new List<string>();
            rulesMap = new Dictionary<string, List<string>>();
            //lines = new List<CraftLine>();
            startX = panel.Width / 2;
            startY = panel.Height;

            maxh = startY;
            maxw = startX;
            minh = startY;
            minw = startX;
            scale = 50;
            randomize = false;
        }

        private void buttonGenerate_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Text files (*.txt)|*.txt";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = openFileDialog.FileName;
                string[] lines = File.ReadAllLines(filePath);

                ParseLSystem(lines);
                rulesaremadetobebroken();
                DrawLSystem();
                //DrawLSystemwithstack();
            }
        }
        private void rulesaremadetobebroken()
        {
            rulesMap.Clear();
            foreach (var r in rules)
            {
                Regex regex = new Regex(@"(?<KEY>[A-Z])\s*->\s*(?<ITEMRULE>[A-Z\[\]\-\+@]+)");
                Match match = regex.Match(r);
                if (rulesMap.ContainsKey(match.Groups["KEY"].ToString()))
                {
                    rulesMap[match.Groups["KEY"].ToString()].Add(match.Groups["ITEMRULE"].ToString());
                }
                else
                {
                    rulesMap[match.Groups["KEY"].ToString()] = new List<string>();
                    rulesMap[match.Groups["KEY"].ToString()].Add(match.Groups["ITEMRULE"].ToString());
                }
            }
        }
        private void ParseLSystem(string[] lines)
        {
            atom = lines[0].Split(' ')[0];
            angle = Convert.ToDouble(lines[0].Split(' ')[1]);
            direction = Convert.ToInt32(lines[0].Split(' ')[2]);

            rules.Clear();
            for (int i = 1; i < lines.Length; i++)
            {
                rules.Add(lines[i]);
            }
        }

        private string makethefractalhavefun()
        {
            if (rules.Count == 0)
                return "";
            StringBuilder sb = new StringBuilder();
            //sb.Append(rulesMap[atom][0]);
            sb.Append(atom);
            for (int i = 1; i < maxit; i++)
            {
                foreach (var k in rulesMap.Keys)
                    sb.Replace(k, rulesMap[k][0] );

            }

            return sb.ToString();
        }

        private void DrawLSystem()
        {
            graphics.Clear(Color.White);

            Stack<int> positionStack = new Stack<int>();
            Stack<int> iterations = new Stack<int>();
            Stack<double> angleStack = new Stack<double>();
            int level = 0;
            maxlevel = 0;
            randomize = false;
            double currentAngle = direction;
            double currentX = startX;
            double currentY = startY;
            double maxh = startY;
            double maxw = startX;
            double minh = startY;
            double minw = startX;
            Random random = new Random();
            string newLine = makethefractalhavefun();
            List<CraftLine> lines = new List<CraftLine>();
            List<int> levels = new List<int>();
            double randomd = random.NextDouble() * angle; 
            foreach (char character in newLine)
            {
                if (character == 'F')
                {
                    double newX = currentX + scale * Math.Cos(currentAngle * Math.PI / 180);
                    double newY = currentY + scale * Math.Sin(currentAngle * Math.PI / 180);
                    if (randomize)
                    {
                        newX = currentX + scale * Math.Cos( randomd * Math.PI / 180);
                        newY = currentY + scale * Math.Sin( randomd * Math.PI / 180);
                    }
                    if (maxh < newY)
                        maxh = newY;

                    if (maxw < newX)
                        maxw = newX;

                    if (minh > newY)
                        minh = newY;

                    if (minw > newX)
                        minw = newX;

                    levels.Add(level);
                    lines.Add(new CraftLine((float)currentX, (float)currentY, (float)newX, (float)newY));
                    //graphics.DrawLine(pen, (float)currentX, (float)currentY, (float)newX, (float)newY);

                    currentX = newX;
                    currentY = newY;
                }
                else if (character == '+')
                {
                    currentAngle += angle;
                }
                else if (character == '@')
                {
                    randomize = true;
                    randomd = currentAngle + (random.NextDouble() - 0.5 ) * (angle + 40 ); 
                }
                else if (character == '-')
                {
                    currentAngle -= angle;
                }
                else if (character == '[')
                {
                    positionStack.Push((int)currentX);
                    positionStack.Push((int)currentY);
                    angleStack.Push(currentAngle);
                    level++;
                    if (maxlevel < level)
                        maxlevel = level;
                }
                else if (character == ']')
                {
                    level--;
                    randomize = false; 
                    currentAngle = angleStack.Pop();
                    currentY = positionStack.Pop();
                    currentX = positionStack.Pop();
                }

            }

            float xScale = (float)(panel.Width / (maxw - minw));
            float yScale = (float)(panel.Height / (maxh - minh));
            float scalexy = Math.Min(xScale, yScale);
            //graphics.ScaleTransform(xScale, yScale);
            int cnt = 0;
            int dist = lime.R - brown.R;
            foreach (var line in lines)
            {
                float xStep = (float)(panel.Width / (maxw - minw));
                if (checkBox1.Checked)
                {
                    int l = levels[cnt];
                    if (maxlevel == 0) maxlevel = 1 ;
                    double llmax = l / (double)maxlevel;
                    double newR = Math.Min(255, Math.Max (0 , brown.R + llmax * (lime.R - brown.R)));
                    double newG = Math.Min(255, Math.Max(0, brown.G + llmax * (lime.G - brown.G)));
                    double newB = Math.Min(255, Math.Max(0, brown.B + llmax * (lime.B - brown.B)));
                    cnt++;
                    //(float)((xi - minw) * xStep);
                    //(float)(1 - (line.currentX - minh) / (maxh - minh))
                    Color ctemp = Color.FromArgb((int)newR, (int)newG, (int)newB);
                    Pen pem1 = new Pen(ctemp, (float)(maxlevel - l + 1 ));
                    graphics.DrawLine(pem1, (line.currentX - (float)minw) * xStep, panel.Height * (float)((line.currentY - minh) / (maxh - minh)), (line.newX - (float)minw) * xStep, panel.Height * (float)((line.newY - minh) / (maxh - minh)));
                    // graphics.DrawLine(pen, (line.currentX  * scalexy) - (float)minw, (line.currentY * scalexy - (float)minh), (line.newX  * scalexy- (float)minw), (line.newY  * scalexy- (float)minh));
                } else
                {
                    graphics.DrawLine(pen, (line.currentX - (float)minw) * xStep, panel.Height * (float)((line.currentY - minh) / (maxh - minh)), (line.newX - (float)minw) * xStep, panel.Height * (float)((line.newY - minh) / (maxh - minh)));

                }
            }
        }

        /*
        private void DrawLSystemwithstack()
        {
            graphics.Clear(Color.White);

            Stack<int> positionStack = new Stack<int>();
            Stack<int> iterations = new Stack<int>();
            Stack<double> angleStack = new Stack<double>();
            Stack<string> s = new Stack<string>();


            List<CraftLine> lines = new List<CraftLine>();
            List<int> levels = new List<int>();

            double currentAngle = direction;
            double currentX = startX;
            double currentY = startY;
            double maxh = startY;
            double maxw = startX;
            double minh = startY;
            double minw = startX;
            Random random = new Random();
            double randomd = random.NextDouble() * angle;
            int level = 0;
            maxlevel = 0;
            randomize = false;

            iterations.Push(0);
            s.Push(atom);
            angleStack.Push(currentAngle);
            positionStack.Push((int)currentX);
            positionStack.Push((int)currentY);


            while (iterations.Count > 0)
            {
                string newline = s.Pop();
                int it = iterations.Pop();
                currentY = positionStack.Pop();
                currentX = positionStack.Pop();
                currentAngle = angleStack.Pop();

                if (it > maxit)
                    continue;

                foreach (char character in rulesMap[newline][0])
                {
                    if (character == 'F' || !rulesMap.ContainsKey("F"))
                    {
                        if (it == maxit) { 
                        double newX = currentX + scale * Math.Cos(currentAngle * Math.PI / 180);
                        double newY = currentY + scale * Math.Sin(currentAngle * Math.PI / 180);
                        if (randomize)
                        {
                            newX = currentX + scale * Math.Cos(randomd * Math.PI / 180);
                            newY = currentY + scale * Math.Sin(randomd * Math.PI / 180);
                        }
                        if (maxh < newY)
                            maxh = newY;

                        if (maxw < newX)
                            maxw = newX;

                        if (minh > newY)
                            minh = newY;

                        if (minw > newX)
                            minw = newX;

                        levels.Add(level);
                        lines.Add(new CraftLine((float)currentX, (float)currentY, (float)newX, (float)newY));
                        //graphics.DrawLine(pen, (float)currentX, (float)currentY, (float)newX, (float)newY);

                        currentX = newX;
                        currentY = newY;
                        }
                        if (rulesMap.ContainsKey("F")) { 
                        iterations.Push(it + 1);
                        s.Push("F");
                        angleStack.Push(currentAngle);
                        positionStack.Push((int)currentX);
                        positionStack.Push((int)currentY);
                        }

                    }
                    else if (character == '+')
                    {
                        currentAngle += angle;
                    }
                    else if (character == '@')
                    {
                        randomize = true;
                        randomd = currentAngle + (random.NextDouble() - 0.5) * (angle + 40);
                    }
                    else if (character == '-')
                    {
                        currentAngle -= angle;
                    }
                    else if (character == '[')
                    {
                        positionStack.Push((int)currentX);
                        positionStack.Push((int)currentY);
                        angleStack.Push(currentAngle);
                        level++;
                        if (maxlevel < level)
                            maxlevel = level;
                    }
                    else if (character == ']')
                    {
                        level--;
                        randomize = false;
                        currentAngle = angleStack.Pop();
                        currentY = positionStack.Pop();
                        currentX = positionStack.Pop();
                    } else if (character >= 'A' && character <= 'Z')
                    {
                        iterations.Push(it + 1);
                        s.Push(character.ToString());
                        angleStack.Push(currentAngle);
                        positionStack.Push((int)currentX);
                        positionStack.Push((int)currentY);
                    }

                }

            }


            float xScale = (float)(panel.Width / (maxw - minw));
            float yScale = (float)(panel.Height / (maxh - minh));
            float scalexy = Math.Min(xScale, yScale);
            //graphics.ScaleTransform(xScale, yScale);
            int cnt = 0;
            int dist = lime.R - brown.R;
            foreach (var line in lines)
            {
                float xStep = (float)(panel.Width / (maxw - minw));
                if (checkBox1.Checked)
                {
                    int l = levels[cnt];
                    double llmax = l / (double)maxlevel;
                    double newR = brown.R + llmax * (lime.R - brown.R);
                    double newG = brown.G + llmax * (lime.G - brown.G);
                    double newB = brown.B + llmax * (lime.B - brown.B);
                    cnt++;
                    //(float)((xi - minw) * xStep);
                    //(float)(1 - (line.currentX - minh) / (maxh - minh))
                    Color ctemp = Color.FromArgb((int)newR, (int)newG, (int)newB);
                    Pen pem1 = new Pen(ctemp, maxlevel - l + 1);
                    graphics.DrawLine(pem1, (line.currentX - (float)minw) * xStep, panel.Height * (float)((line.currentY - minh) / (maxh - minh)), (line.newX - (float)minw) * xStep, panel.Height * (float)((line.newY - minh) / (maxh - minh)));
                    // graphics.DrawLine(pen, (line.currentX  * scalexy) - (float)minw, (line.currentY * scalexy - (float)minh), (line.newX  * scalexy- (float)minw), (line.newY  * scalexy- (float)minh));
                }
                else
                {
                    graphics.DrawLine(pen, (line.currentX - (float)minw) * xStep, panel.Height * (float)((line.currentY - minh) / (maxh - minh)), (line.newX - (float)minw) * xStep, panel.Height * (float)((line.newY - minh) / (maxh - minh)));

                }
            }
        }
        */
        private void MainForm_Resize(object sender, EventArgs e)
        {
            //panel.Height = this.Height - 150;
            //panel.Width = this.Width - 50;
            //panel.Size = new Size(this.Width - 50, this.Height - 150);
            //label1.Text += panel.Height.ToString() + " " + panel.Width.ToString() + " ";
            // Обновление графика при изменении размера окна
            //panel.Refresh();
            //graphics.Clear(Color.White);
            //DrawLSystem();
            /*
            foreach (var line in lines)
            {
                float xStep = (float)(panel.Width / (maxw - minw));
                //(float)((xi - minw) * xStep);
                //(float)(1 - (line.currentX - minh) / (maxh - minh))
                graphics.DrawLine(pen, (line.currentX - (float)minw) * xStep, panel.Height * (float)((line.currentY - minh) / (maxh - minh)), (line.newX - (float)minw) * xStep, panel.Height * (float)((line.newY - minh) / (maxh - minh)));
                // graphics.DrawLine(pen, (line.currentX  * scalexy) - (float)minw, (line.currentY * scalexy - (float)minh), (line.newX  * scalexy- (float)minw), (line.newY  * scalexy- (float)minh));
            }
            //OnPaint(e);*/
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            foreach (var line in lines)
            {
                float xStep = (float)(panel.Width / (maxw - minw));
                //(float)((xi - minw) * xStep);
                //(float)(1 - (line.currentX - minh) / (maxh - minh))
                graphics.DrawLine(pen, (line.currentX - (float)minw) * xStep, panel.Height * (float)((line.currentY - minh) / (maxh - minh)), (line.newX - (float)minw) * xStep, panel.Height * (float)((line.newY - minh) / (maxh - minh)));
                // graphics.DrawLine(pen, (line.currentX  * scalexy) - (float)minw, (line.currentY * scalexy - (float)minh), (line.newX  * scalexy- (float)minw), (line.newY  * scalexy- (float)minh));
            }
        }

        private void panel_Paint(object sender, PaintEventArgs e)
        {

            // panel.Height = this.ClientSize.Height - 150;
            //panel.Width = this.ClientSize.Width - 50;
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked) { 
            pen = new Pen(brown);
            graphics.Clear(Color.White);
            DrawLSystem();}
            else
            {
                pen.Color = Color.Black;
                graphics.Clear(Color.White);
                DrawLSystem();
            }
        }
    }
}