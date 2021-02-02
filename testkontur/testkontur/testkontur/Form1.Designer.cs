namespace testkontur
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.button1 = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.textBox4 = new System.Windows.Forms.TextBox();
            this.textBox5 = new System.Windows.Forms.TextBox();
            this.textBox8 = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.dateTimePicker1 = new System.Windows.Forms.DateTimePicker();
            this.dateTimePicker2 = new System.Windows.Forms.DateTimePicker();
            this.label13 = new System.Windows.Forms.Label();
            this.ZakupkiGovCheckBox = new System.Windows.Forms.CheckBox();
            this.TecktorgCheckBox = new System.Windows.Forms.CheckBox();
            this.B2BCenterCheckBox = new System.Windows.Forms.CheckBox();
            this.EtpgpbCheckBox = new System.Windows.Forms.CheckBox();
            this.RosatomCheckBox = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(256, 10);
            this.button1.Margin = new System.Windows.Forms.Padding(2);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(242, 38);
            this.button1.TabIndex = 0;
            this.button1.Text = "Анализ";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.Button1_Click);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(9, 242);
            this.textBox1.Margin = new System.Windows.Forms.Padding(2);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(243, 141);
            this.textBox1.TabIndex = 1;
            this.textBox1.Text = resources.GetString("textBox1.Text");
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 133);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(47, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Цена от";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(147, 133);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(19, 13);
            this.label4.TabIndex = 5;
            this.label4.Text = "до";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 169);
            this.label5.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(109, 13);
            this.label5.TabIndex = 6;
            this.label5.Text = "Дата публикации от";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 196);
            this.label6.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(19, 13);
            this.label6.TabIndex = 7;
            this.label6.Text = "до";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(7, 219);
            this.label7.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(92, 13);
            this.label7.TabIndex = 8;
            this.label7.Text = "Ключевые слова";
            // 
            // textBox4
            // 
            this.textBox4.Location = new System.Drawing.Point(57, 126);
            this.textBox4.Margin = new System.Windows.Forms.Padding(2);
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new System.Drawing.Size(76, 20);
            this.textBox4.TabIndex = 11;
            // 
            // textBox5
            // 
            this.textBox5.Location = new System.Drawing.Point(176, 126);
            this.textBox5.Margin = new System.Windows.Forms.Padding(2);
            this.textBox5.Name = "textBox5";
            this.textBox5.Size = new System.Drawing.Size(76, 20);
            this.textBox5.TabIndex = 12;
            // 
            // textBox8
            // 
            this.textBox8.Location = new System.Drawing.Point(256, 53);
            this.textBox8.Margin = new System.Windows.Forms.Padding(2);
            this.textBox8.Multiline = true;
            this.textBox8.Name = "textBox8";
            this.textBox8.Size = new System.Drawing.Size(243, 331);
            this.textBox8.TabIndex = 15;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(7, 385);
            this.label8.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(41, 13);
            this.label8.TabIndex = 16;
            this.label8.Text = "Cтатус";
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(9, 401);
            this.progressBar1.Margin = new System.Windows.Forms.Padding(2);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(489, 19);
            this.progressBar1.TabIndex = 23;
            // 
            // dateTimePicker1
            // 
            this.dateTimePicker1.Location = new System.Drawing.Point(118, 163);
            this.dateTimePicker1.Margin = new System.Windows.Forms.Padding(2);
            this.dateTimePicker1.Name = "dateTimePicker1";
            this.dateTimePicker1.Size = new System.Drawing.Size(134, 20);
            this.dateTimePicker1.TabIndex = 24;
            // 
            // dateTimePicker2
            // 
            this.dateTimePicker2.Location = new System.Drawing.Point(118, 196);
            this.dateTimePicker2.Margin = new System.Windows.Forms.Padding(2);
            this.dateTimePicker2.Name = "dateTimePicker2";
            this.dateTimePicker2.Size = new System.Drawing.Size(134, 20);
            this.dateTimePicker2.TabIndex = 25;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(12, 9);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(60, 13);
            this.label13.TabIndex = 27;
            this.label13.Text = "Площадки";
            // 
            // ZakupkiGovCheckBox
            // 
            this.ZakupkiGovCheckBox.AutoSize = true;
            this.ZakupkiGovCheckBox.Checked = true;
            this.ZakupkiGovCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ZakupkiGovCheckBox.Location = new System.Drawing.Point(15, 31);
            this.ZakupkiGovCheckBox.Name = "ZakupkiGovCheckBox";
            this.ZakupkiGovCheckBox.Size = new System.Drawing.Size(85, 17);
            this.ZakupkiGovCheckBox.TabIndex = 28;
            this.ZakupkiGovCheckBox.Text = "ZakupkiGov";
            this.ZakupkiGovCheckBox.UseVisualStyleBackColor = true;
            // 
            // TecktorgCheckBox
            // 
            this.TecktorgCheckBox.AutoSize = true;
            this.TecktorgCheckBox.Location = new System.Drawing.Point(15, 54);
            this.TecktorgCheckBox.Name = "TecktorgCheckBox";
            this.TecktorgCheckBox.Size = new System.Drawing.Size(69, 17);
            this.TecktorgCheckBox.TabIndex = 29;
            this.TecktorgCheckBox.Text = "Tecktorg";
            this.TecktorgCheckBox.UseVisualStyleBackColor = true;
            // 
            // B2BCenterCheckBox
            // 
            this.B2BCenterCheckBox.AutoSize = true;
            this.B2BCenterCheckBox.Location = new System.Drawing.Point(15, 100);
            this.B2BCenterCheckBox.Name = "B2BCenterCheckBox";
            this.B2BCenterCheckBox.Size = new System.Drawing.Size(77, 17);
            this.B2BCenterCheckBox.TabIndex = 31;
            this.B2BCenterCheckBox.Text = "B2BCenter";
            this.B2BCenterCheckBox.UseVisualStyleBackColor = true;
            // 
            // EtpgpbCheckBox
            // 
            this.EtpgpbCheckBox.AutoSize = true;
            this.EtpgpbCheckBox.Location = new System.Drawing.Point(15, 77);
            this.EtpgpbCheckBox.Name = "EtpgpbCheckBox";
            this.EtpgpbCheckBox.Size = new System.Drawing.Size(60, 17);
            this.EtpgpbCheckBox.TabIndex = 30;
            this.EtpgpbCheckBox.Text = "Etpgpb";
            this.EtpgpbCheckBox.UseVisualStyleBackColor = true;
            // 
            // RosatomCheckBox
            // 
            this.RosatomCheckBox.AutoSize = true;
            this.RosatomCheckBox.Location = new System.Drawing.Point(128, 31);
            this.RosatomCheckBox.Name = "RosatomCheckBox";
            this.RosatomCheckBox.Size = new System.Drawing.Size(68, 17);
            this.RosatomCheckBox.TabIndex = 32;
            this.RosatomCheckBox.Text = "Rosatom";
            this.RosatomCheckBox.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(511, 430);
            this.Controls.Add(this.RosatomCheckBox);
            this.Controls.Add(this.B2BCenterCheckBox);
            this.Controls.Add(this.EtpgpbCheckBox);
            this.Controls.Add(this.TecktorgCheckBox);
            this.Controls.Add(this.ZakupkiGovCheckBox);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.dateTimePicker2);
            this.Controls.Add(this.dateTimePicker1);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.textBox8);
            this.Controls.Add(this.textBox5);
            this.Controls.Add(this.textBox4);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.button1);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox textBox4;
        private System.Windows.Forms.TextBox textBox5;
        private System.Windows.Forms.TextBox textBox8;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.DateTimePicker dateTimePicker1;
        private System.Windows.Forms.DateTimePicker dateTimePicker2;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.CheckBox ZakupkiGovCheckBox;
        private System.Windows.Forms.CheckBox TecktorgCheckBox;
        private System.Windows.Forms.CheckBox B2BCenterCheckBox;
        private System.Windows.Forms.CheckBox EtpgpbCheckBox;
        private System.Windows.Forms.CheckBox RosatomCheckBox;
    }
}

