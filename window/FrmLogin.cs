using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections;

namespace YisinTick
{
    public partial class FrmLogin : Form
    {
        public bool isLoginClose = false;
        private bool isLoaded = false;

        public FrmLogin()
        {
            InitializeComponent();
            isLoginClose = false;
            this.FormClosing += FrmLogin_FormClosing;            
        }

        void FrmLogin_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!isLoginClose)
            {
                Program.mainForm.ShowMessage("取消了登录！");
            }
        }

        public void ShowFrm()
        {
            tbCode.Text = "";
            GetLoginImage();
            Program.mainForm.HideProcess();
            if (!isLoaded)
            {
                isLoaded = true;
                this.ShowDialog(Program.mainForm);  
            }                      
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            System.Threading.ThreadPool.QueueUserWorkItem((m) =>
            {
                Program.mainForm.ShowMessage("正在登录...", false);
                btnLogin.Enabled = false;
                btnFlushCode.Enabled = false;

                var v = _12306Class.Login(tbAcc.Text, tbPwd.Text, tbCode.Text);
                if (v.IsLogined)
                {
                    Program.mainForm.ShowMessage("成功  ");
                    Program.mainForm.ShowMessage(string.Format("{0}  您好！", v.LoginName));
                    if (Program.mainForm.GetUserInfo(v.LoginName))
                    {
                        Program.mainForm.GetFavoriteContacts();
                        Program.mainForm.ChangeOtherButton(true);
                    }
                    if(remenberAcc.Checked){
                        if(remenberPwd.Checked){
                            ConfigStore.AddUser(tbAcc.Text, tbPwd.Text, 1, v.LoginName);
                        }
                        else
                        {
                            ConfigStore.AddUser(tbAcc.Text, "", 1, v.LoginName);
                        }
                    }
                    else
                    {
                        ConfigStore.RemoveUser(tbAcc.Text);
                    }
                    isLoginClose = true;
                    this.Hide();
                    Form1.isLogin = true;
                    Program.mainForm.LoginSuccess();
                }
                else
                {
                    Program.mainForm.ShowMessage("失败");
                    MessageBox.Show(Program.mainForm, v.Message);
                    btnLogin.Enabled = true;
                    btnFlushCode.Enabled = true;

                    if (v.type == ErrorType.VerificationError)
                    {
                        tbCode.Text = "";
                        GetLoginImage();
                    }
                }
            });
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            ThreadPool.QueueUserWorkItem((m) =>
            {
                GetLoginImage();
            });
        }

        /// <summary>
        /// 获取登录验证码
        /// </summary>
        private void GetLoginImage()
        {
            Program.mainForm.ShowMessage("正在获取验证码...", false);
            Thread12306.Stop();
            //Program.mainForm.ChangeOtherButton(false);
            btnLogin.Enabled = false;
            btnFlushCode.Enabled = false;

            Bitmap v = null;
            do
            {
                v = _12306Class.GetLoginImage();
            } while (v == null);
            pictureBox1.Image = v;

            btnLogin.Enabled = true;
            btnFlushCode.Enabled = true;

            Program.mainForm.ShowMessage("完成");
            //ImageFrom.GetImageFrom.Show(v);
            //string str = ImageFrom.GetImageFrom.Code;
            //tbCode.Text = str;
            //tbCode.Text = imgOCR.GetCodeText(v);
            if(ConfigStore.isAutoWriterVerify){
                UnCode UnCheckobj = new UnCode(v);
                string strNum = UnCheckobj.getPicnum(0);     //识别图片
                tbCode.Text = strNum;
            }            
        }

        private void btnFlushCode_Click(object sender, EventArgs e)
        {
            btnFlushCode.Enabled = false;
            ThreadPool.QueueUserWorkItem((m) =>
            {
                GetLoginImage();
            });
        }

        private void FrmLogin_Load(object sender, EventArgs e)
        {
            User user = null;
            ArrayList userArray = new ArrayList(ConfigStore.userTable.Keys);
            foreach (String uacc in userArray)
            {
                user = (User)ConfigStore.userTable[uacc];
                tbAcc.Items.Add(user.Acc);
            }

            user = ConfigStore.GetActiveUser();
            if (user != null)
            {
                remenberAcc.Checked = true;
                tbAcc.Text = user.Acc;
                if(!String.IsNullOrEmpty(user.Pwd)){
                    remenberPwd.Checked = true;
                    tbPwd.Text = user.Pwd;
                    remenberAcc.Enabled = false;
                }
                tbAcc.Text = user.Acc;
            }
            tbCode.KeyUp += tbCode_KeyUp;
        }

        void tbCode_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && tbCode.Text.Length == 4)
            {
                btnLogin.PerformClick();
            }
        }

        private void remenberPwd_CheckedChanged(object sender, EventArgs e)
        {
            if(remenberPwd.Checked){
                remenberAcc.Checked = true;
                remenberAcc.Enabled = false;
            }
            else
            {
                remenberAcc.Enabled = true;
            }
        }

        private void remenberAcc_CheckedChanged(object sender, EventArgs e)
        {
            
        }

        private void tbAcc_SelectedIndexChanged(object sender, EventArgs e)
        {
            String text = tbAcc.Text;
            User user = null;
            ArrayList userArray = new ArrayList(ConfigStore.userTable.Keys);
            foreach (String uacc in userArray)
            {
                user = (User)ConfigStore.userTable[uacc];
                if(user.Acc.Equals(text)){
                    tbPwd.Text = user.Pwd;
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
    }
}
