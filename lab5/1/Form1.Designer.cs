namespace WinFormsApp6
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        //Panel panel;
        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            panel = new Panel();
            buttonGenerate = new Button();
            checkBox1 = new CheckBox();
            SuspendLayout();
            // 
            // panel
            // 
            panel.Location = new Point(65, 13);
            panel.Margin = new Padding(3, 4, 3, 4);
            panel.Name = "panel";
            panel.Size = new Size(500, 500);
            panel.TabIndex = 0;
            panel.Paint += panel_Paint;
            // 
            // buttonGenerate
            // 
            buttonGenerate.Location = new Point(241, 522);
            buttonGenerate.Margin = new Padding(3, 4, 3, 4);
            buttonGenerate.Name = "buttonGenerate";
            buttonGenerate.Size = new Size(134, 40);
            buttonGenerate.TabIndex = 1;
            buttonGenerate.Text = "Generate Fractal";
            buttonGenerate.UseVisualStyleBackColor = true;
            buttonGenerate.Click += buttonGenerate_Click;
            // 
            // checkBox1
            // 
            checkBox1.AutoSize = true;
            checkBox1.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            checkBox1.Location = new Point(381, 531);
            checkBox1.Name = "checkBox1";
            checkBox1.Size = new Size(196, 24);
            checkBox1.TabIndex = 2;
            checkBox1.Text = "красивое показываете?\r\n";
            checkBox1.UseVisualStyleBackColor = true;
            checkBox1.CheckedChanged += checkBox1_CheckedChanged;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            ClientSize = new Size(643, 592);
            Controls.Add(checkBox1);
            Controls.Add(buttonGenerate);
            Controls.Add(panel);
            Margin = new Padding(3, 4, 3, 4);
            MaximizeBox = false;
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "L-System Fractals";
            Load += MainForm_Load;
            Paint += Form1_Paint;
            Resize += MainForm_Resize;
            ResumeLayout(false);
            PerformLayout();
        }

        private System.Windows.Forms.Panel panel;
        private System.Windows.Forms.Button buttonGenerate;
        private CheckBox checkBox1;
    }
}