using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace YisinTick
{
    public partial class UserInfoPanel : Form
    {
        public UserInfoPanel()
        {
            InitializeComponent();
        }

        private void UserInfoPanel_Load(object sender, EventArgs e)
        {
            UserInfo ui = _88448Class.user;

            pbox_status.ImageLocation = ui.onlineImg;
            label_userName.Text = ui.name + " (UID：" + ui.uid + ")";
            label_sex.Text = "性别：" + ui.sex;
            label_email.Text = ui.emailOrQQ;
            label_group.Text = "用户组：" + ui.userGroup;
            l_fatielevel.Text = ui.fatieLevel;
            l_registerDate.Text = ui.registerDate;
            l_readPri.Text = ui.readPri;
            l_shangTime.Text = ui.shangTime;
            l_tiezi.Text = ui.tiezi;
            l_lastFabiao.Text = ui.lastFabiao;
            l_avgTiezi.Text = ui.avgTiezi;
            l_registerIp.Text = ui.registerIP;
            l_jinghua.Text = ui.jinghua;
            l_onlineTime.Text = ui.onlineTime;
            l_dajifen.Text = "积分：" + ui.dajifen;
            l_expri.Text = "经验：" + ui.expri;
            l_goden.Text = "金钱：" + ui.godle;
            l_jifen.Text = "积分：" + ui.jifen;
            pbox_head.ImageLocation = ui.headImg;

            if(ui.levelPic != null){
                PictureBox pbox = null;
                for (int i = 0; i < ui.levelPic.Length; i++)
                {
                    pbox = new PictureBox();
                    pbox.ImageLocation = ui.levelPic[i];
                    pbox.Width = 16;
                    pbox.Height = 16;
                    pbox.Location = new Point(163 + (i * 20), 115);
                    this.Controls.Add(pbox);
                }
            }

            if (ui.xuanzhangImg != null)
            {
                l_xunzhang.Visible = true;
                PictureBox pbox = null;
                for (int i = 0; i < ui.xuanzhangImg.Length; i++)
                {
                    pbox = new PictureBox();
                    pbox.ImageLocation = ui.xuanzhangImg[i];
                    pbox.Width = 20;
                    pbox.Height = 35;
                    pbox.Location = new Point(18 + (i * 25), 396);
                    this.Controls.Add(pbox);
                }
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            BuyCode bc = new BuyCode();
            bc.ShowDialog(Program.mainForm);
        }
    }
}
