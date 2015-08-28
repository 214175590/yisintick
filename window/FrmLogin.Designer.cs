namespace YisinTick
{
    partial class FrmLogin
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmLogin));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tbAcc = new System.Windows.Forms.ComboBox();
            this.remenberAcc = new System.Windows.Forms.CheckBox();
            this.button1 = new System.Windows.Forms.Button();
            this.remenberPwd = new System.Windows.Forms.CheckBox();
            this.btnFlushCode = new System.Windows.Forms.Button();
            this.btnLogin = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.tbCode = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.tbPwd = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.tbAcc);
            this.groupBox1.Controls.Add(this.remenberAcc);
            this.groupBox1.Controls.Add(this.button1);
            this.groupBox1.Controls.Add(this.remenberPwd);
            this.groupBox1.Controls.Add(this.btnFlushCode);
            this.groupBox1.Controls.Add(this.btnLogin);
            this.groupBox1.Controls.Add(this.pictureBox1);
            this.groupBox1.Controls.Add(this.tbCode);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.tbPwd);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(12, 11);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(261, 181);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "请输入12306账号密码登录";
            // 
            // tbAcc
            // 
            this.tbAcc.FormattingEnabled = true;
            this.tbAcc.Location = new System.Drawing.Point(63, 18);
            this.tbAcc.Name = "tbAcc";
            this.tbAcc.Size = new System.Drawing.Size(185, 20);
            this.tbAcc.TabIndex = 2;
            this.tbAcc.SelectedIndexChanged += new System.EventHandler(this.tbAcc_SelectedIndexChanged);
            // 
            // remenberAcc
            // 
            this.remenberAcc.AutoSize = true;
            this.remenberAcc.Location = new System.Drawing.Point(63, 101);
            this.remenberAcc.Name = "remenberAcc";
            this.remenberAcc.Size = new System.Drawing.Size(72, 16);
            this.remenberAcc.TabIndex = 12;
            this.remenberAcc.Text = "记住帐号";
            this.remenberAcc.UseVisualStyleBackColor = true;
            this.remenberAcc.CheckedChanged += new System.EventHandler(this.remenberAcc_CheckedChanged);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(139, 147);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(76, 26);
            this.button1.TabIndex = 11;
            this.button1.Text = "取消";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // remenberPwd
            // 
            this.remenberPwd.AutoSize = true;
            this.remenberPwd.Location = new System.Drawing.Point(63, 127);
            this.remenberPwd.Name = "remenberPwd";
            this.remenberPwd.Size = new System.Drawing.Size(72, 16);
            this.remenberPwd.TabIndex = 10;
            this.remenberPwd.Text = "记住密码";
            this.remenberPwd.UseVisualStyleBackColor = true;
            this.remenberPwd.CheckedChanged += new System.EventHandler(this.remenberPwd_CheckedChanged);
            // 
            // btnFlushCode
            // 
            this.btnFlushCode.Enabled = false;
            this.btnFlushCode.Location = new System.Drawing.Point(147, 113);
            this.btnFlushCode.Name = "btnFlushCode";
            this.btnFlushCode.Size = new System.Drawing.Size(75, 23);
            this.btnFlushCode.TabIndex = 9;
            this.btnFlushCode.Text = "刷新验证码";
            this.btnFlushCode.UseVisualStyleBackColor = true;
            this.btnFlushCode.Click += new System.EventHandler(this.btnFlushCode_Click);
            // 
            // btnLogin
            // 
            this.btnLogin.Enabled = false;
            this.btnLogin.Location = new System.Drawing.Point(38, 147);
            this.btnLogin.Name = "btnLogin";
            this.btnLogin.Size = new System.Drawing.Size(76, 26);
            this.btnLogin.TabIndex = 8;
            this.btnLogin.Text = "登录";
            this.btnLogin.UseVisualStyleBackColor = true;
            this.btnLogin.Click += new System.EventHandler(this.btnLogin_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(147, 74);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(99, 32);
            this.pictureBox1.TabIndex = 7;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Click += new System.EventHandler(this.pictureBox1_Click);
            // 
            // tbCode
            // 
            this.tbCode.Location = new System.Drawing.Point(63, 73);
            this.tbCode.MaxLength = 4;
            this.tbCode.Name = "tbCode";
            this.tbCode.Size = new System.Drawing.Size(78, 21);
            this.tbCode.TabIndex = 4;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 76);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 12);
            this.label3.TabIndex = 5;
            this.label3.Text = "验证码：";
            // 
            // tbPwd
            // 
            this.tbPwd.Location = new System.Drawing.Point(63, 46);
            this.tbPwd.Name = "tbPwd";
            this.tbPwd.PasswordChar = '*';
            this.tbPwd.Size = new System.Drawing.Size(185, 21);
            this.tbPwd.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(14, 50);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 12);
            this.label2.TabIndex = 3;
            this.label2.Text = "密  码：";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "帐  号：";
            // 
            // FrmLogin
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(286, 201);
            this.Controls.Add(this.groupBox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Location = new System.Drawing.Point(200, 100);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(302, 240);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(302, 240);
            this.Name = "FrmLogin";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "登录12306";
            this.Load += new System.EventHandler(this.FrmLogin_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox remenberPwd;
        private System.Windows.Forms.Button btnFlushCode;
        private System.Windows.Forms.Button btnLogin;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.TextBox tbCode;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tbPwd;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.CheckBox remenberAcc;
        private System.Windows.Forms.ComboBox tbAcc;
    }
}