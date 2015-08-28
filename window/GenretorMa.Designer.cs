namespace YisinTick
{
    partial class GenretorMa
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GenretorMa));
            this.pwd_text = new System.Windows.Forms.TextBox();
            this.jiqima = new System.Windows.Forms.TextBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.zh_text = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.errorInfo = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // pwd_text
            // 
            this.pwd_text.Location = new System.Drawing.Point(37, 11);
            this.pwd_text.Name = "pwd_text";
            this.pwd_text.PasswordChar = '*';
            this.pwd_text.Size = new System.Drawing.Size(274, 21);
            this.pwd_text.TabIndex = 0;
            this.pwd_text.UseSystemPasswordChar = true;
            // 
            // jiqima
            // 
            this.jiqima.Location = new System.Drawing.Point(37, 48);
            this.jiqima.Name = "jiqima";
            this.jiqima.Size = new System.Drawing.Size(274, 21);
            this.jiqima.TabIndex = 1;
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(12, 145);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox1.Size = new System.Drawing.Size(359, 71);
            this.textBox1.TabIndex = 4;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(331, 11);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(40, 96);
            this.button1.TabIndex = 3;
            this.button1.Text = "GO";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // zh_text
            // 
            this.zh_text.Location = new System.Drawing.Point(37, 86);
            this.zh_text.Name = "zh_text";
            this.zh_text.Size = new System.Drawing.Size(274, 21);
            this.zh_text.TabIndex = 2;
            this.zh_text.Text = "0001";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(11, 12);
            this.label1.TabIndex = 5;
            this.label1.Text = "P";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(15, 54);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(11, 12);
            this.label2.TabIndex = 6;
            this.label2.Text = "M";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(15, 91);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(11, 12);
            this.label3.TabIndex = 7;
            this.label3.Text = "C";
            // 
            // errorInfo
            // 
            this.errorInfo.AutoSize = true;
            this.errorInfo.Font = new System.Drawing.Font("宋体", 13F);
            this.errorInfo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.errorInfo.Location = new System.Drawing.Point(40, 118);
            this.errorInfo.Name = "errorInfo";
            this.errorInfo.Size = new System.Drawing.Size(0, 18);
            this.errorInfo.TabIndex = 8;
            // 
            // GenretorMa
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(386, 228);
            this.Controls.Add(this.errorInfo);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.zh_text);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.jiqima);
            this.Controls.Add(this.pwd_text);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(402, 267);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(402, 267);
            this.Name = "GenretorMa";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "GPMC";
            this.Load += new System.EventHandler(this.GenretorMa_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox pwd_text;
        private System.Windows.Forms.TextBox jiqima;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox zh_text;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label errorInfo;
    }
}