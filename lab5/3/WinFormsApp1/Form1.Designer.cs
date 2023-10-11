namespace WinFormsApp1
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

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

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            Canvas = new PictureBox();
            addDotButton = new Button();
            drawButton = new Button();
            deleteButton = new Button();
            moveButton = new Button();
            ((System.ComponentModel.ISupportInitialize)Canvas).BeginInit();
            SuspendLayout();
            // 
            // Canvas
            // 
            Canvas.BackColor = SystemColors.Window;
            Canvas.Location = new Point(0, 0);
            Canvas.Name = "Canvas";
            Canvas.Size = new Size(795, 535);
            Canvas.TabIndex = 0;
            Canvas.TabStop = false;
            Canvas.Click += Canvas_Click;
            // 
            // button1
            // 
            addDotButton.Location = new Point(821, 141);
            addDotButton.Size = new Size(127, 39);
            addDotButton.TabIndex = 3;
            addDotButton.Text = "Добавить точки";
            addDotButton.UseVisualStyleBackColor = true;
            addDotButton.Click += addDotButton_Click;
            // 
            // button2
            // 
            drawButton.Location = new Point(821, 200);
            drawButton.Size = new Size(127, 39);
            drawButton.TabIndex = 4;
            drawButton.Text = "Рисовать";
            drawButton.UseVisualStyleBackColor = true;
            drawButton.Click += drawButton_Click;
            // 
            // button3
            // 
            deleteButton.Location = new Point(821, 244);
            deleteButton.Size = new Size(127, 39);
            deleteButton.TabIndex = 5;
            deleteButton.Text = "Удалить";
            deleteButton.UseVisualStyleBackColor = true;
            deleteButton.Click += deleteButton_Click;
            // 
            // button4
            // 
            moveButton.Location = new Point(821, 288);
            moveButton.Size = new Size(127, 39);
            moveButton.TabIndex = 6;
            moveButton.Text = "Двинуть";
            moveButton.UseVisualStyleBackColor = true;
            moveButton.Click += moveButton_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(969, 533);
            Controls.Add(moveButton);
            Controls.Add(deleteButton);
            Controls.Add(drawButton);
            Controls.Add(addDotButton);
            Controls.Add(Canvas);
            Name = "Form1";
            Text = "Графика. Лабораторная 5";
            ((System.ComponentModel.ISupportInitialize)Canvas).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private PictureBox Canvas;
        private Button addDotButton;
        private Button drawButton;
        private Button deleteButton;
        private Button moveButton;
    }
}