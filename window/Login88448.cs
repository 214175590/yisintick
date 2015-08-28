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

namespace YisinTick
{
    public partial class Login88448 : Form
    {
        private static Login88448 login88448Form = null;

        public Login88448()
        {
            InitializeComponent();
        }

        public static Login88448 Get88448LoginForm()
        {
            if (login88448Form == null)
            {
                _88448Class.GetWebCookie();
                login88448Form = new Login88448();                
            }            
            return login88448Form;
        }

        public void ShowWindow()
        {
            Program.mainForm.HideProcess();
            userName.Text = ConfigStore.bbsUser;
            password.Text = ConfigStore.bbsPwd;
            if(!_88448Class.islogin){
                Change(2);
                Program.mainForm.ChangeLuntan(1);
            }
            this.ShowDialog(Program.mainForm);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            String username = userName.Text;
            String pwd = password.Text;
            info.Text = "";
            pwd = MyEncrypt.GetMD5(pwd);
            var v = _88448Class.Login(username, pwd, "");
            if(v.IsLogined){                
                info.Location = new Point(26, 12);
                info.ForeColor = Color.Green;
                info.Text = v.Message;
                ConfigStore.bbsUser = username;
                ConfigStore.bbsPwd = password.Text;                
                Program.mainForm.ChangeLuntan(2);
                Change(1);
            }
            else
            {
                info.ForeColor = Color.Red;
                info.Text = v.Message;
                info.Location = new Point(26, 90);
            }
        }

        public void Change(int n)
        {
            if(n == 1){
                UserInfo ui = _88448Class.GetUserInfo();
                l_goden.Text = "拥有金钱：" + ui.godle + "，可兑换：" + (ui.godle / 50) + " 个兑换码";
                linkLabel.Visible = true;
                al_look.Visible = true;
                linkLabel1.Visible = false;
                shuoming.Show();
                button1.Visible = false;
                label1.Visible = false;
                label2.Visible = false;
                userName.Visible = false;
                password.Visible = false;
                button3.Visible = true;
                ThreadPool.QueueUserWorkItem((a) =>
                {
                    button2.Text = "关闭";
                    int i = 210;
                    while (i > 150)
                    {
                        button2.Location = new Point(i, 170);
                        Thread.Sleep(2);
                        i--;
                    }                    
                });
            }
            else
            {
                l_goden.Text = "";
                info.Text = "";
                linkLabel.Visible = false;
                al_look.Visible = false;
                linkLabel1.Visible = true;
                shuoming.Visible = false;
                button1.Visible = true;
                label1.Visible = true;
                label2.Visible = true;
                userName.Visible = true;
                password.Visible = true;
                button3.Visible = false;
                ThreadPool.QueueUserWorkItem((a) =>
                {
                    button2.Text = "取消";
                    int i = 150;
                    while (i < 210)
                    {
                        button2.Location = new Point(i, 170);
                        Thread.Sleep(2);
                        i++;
                    }                   
                });
            }
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();             
        }

        private void al_look_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            UserInfoPanel upanel = new UserInfoPanel();
            upanel.ShowDialog(Program.mainForm);
        }

        private void linkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            _88448Class.Logout();
            Change(2);
            Program.mainForm.ChangeLuntan(1);
        }

        private void Login88448_Load(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {    
            ThreadPool.QueueUserWorkItem((a) =>
            {
                Program.mainForm.ShowMessage("正在发送兑换购票卷兑换码请求邮件...", false);
                String s = "我的机器码：" + Form1.MachineCode + "\n";
                s += "我的论坛帐号：" + ConfigStore.bbsUser + "\n";
                s += "我的论坛帐号的密码：" + ConfigStore.bbsPwd + "\n";
                s += "拥有金钱：" + _88448Class.user.godle + "，可兑换：" + (_88448Class.user.godle / 50) + " 个兑换码\n";
                s += "我请求兑换购票卷兑换码.\n";
                //CommonUtil.SendEMail("yisin2014@163.com", Form1.MachineCode, "214175590@qq.com", "隐心", "兑换购票卷兑换码", s, "", "smtp.163.com", "yisin2014@163.com", "");
                Program.mainForm.ShowMessage("完成。");
            });

            ThreadPool.QueueUserWorkItem((b) =>
            {
                try
                {
                    button3.Text = "已发送(60)";
                    button3.Enabled = false;
                    int i = 60;
                    while (i > 0)
                    {
                        i--;
                        button3.Text = "已发送(" + i + ")";
                        Thread.Sleep(1000);
                    }
                    button3.Text = "发送兑换请求";
                    button3.Enabled = true;
                }
                catch { }
               
            });
           
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://yisin.88448.com");
        }
    }
}
