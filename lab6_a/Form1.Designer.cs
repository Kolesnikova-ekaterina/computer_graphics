
namespace lab6_a
{
    partial class Form1
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.comboBoxTypePolyhedra = new System.Windows.Forms.ComboBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // comboBoxTypePolyhedra
            // 
            this.comboBoxTypePolyhedra.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.comboBoxTypePolyhedra.FormattingEnabled = true;
            this.comboBoxTypePolyhedra.Items.AddRange(new object[] {
            "куб ",
            "тетраэдр",
            "октаэдрт",
            "гексаэдр",
            "икосаэдр",
            "додекаэдр"});
            this.comboBoxTypePolyhedra.Location = new System.Drawing.Point(937, 19);
            this.comboBoxTypePolyhedra.Name = "comboBoxTypePolyhedra";
            this.comboBoxTypePolyhedra.Size = new System.Drawing.Size(193, 33);
            this.comboBoxTypePolyhedra.TabIndex = 0;
            this.comboBoxTypePolyhedra.SelectedIndexChanged += new System.EventHandler(this.comboBoxTypePolyhedra_SelectedIndexChanged);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(12, 12);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(801, 609);
            this.pictureBox1.TabIndex = 1;
            this.pictureBox1.TabStop = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1147, 633);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.comboBoxTypePolyhedra);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox comboBoxTypePolyhedra;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}

