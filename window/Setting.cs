using Sunisoft.IrisSkin;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace YisinTick
{
    public partial class Setting : Form
    {
        public Setting()
        {
            InitializeComponent();
        }

        private void Setting_Load(object sender, EventArgs e)
        {
            LoadLibaray();
            LoadSkinList();
            // 是否开机启动设置
            rb_yes.Checked = ConfigStore.startType.Equals("1");

            // 程序更新设置
            if (ConfigStore.updateType.Equals("1"))
            {
                rb_update1.Checked = true;
            }
            else if (ConfigStore.updateType.Equals("2"))
            {
                rb_update2.Checked = true;
            }
            else if (ConfigStore.updateType.Equals("3"))
            {
                rb_update3.Checked = true;
            }
            // 验证码相似度计算法
            if (ConfigStore.algorithm.Equals("1"))
            {
                radioButton1.Checked = true;
            }
            else if (ConfigStore.algorithm.Equals("2"))
            {
                radioButton2.Checked = true;
            }

            // 皮肤
            cb_useskin.Checked = !String.IsNullOrEmpty(ConfigStore.skin);

        }


        public void LoadLibaray()
        {
            checkedListBox.Items.Clear();
            Libaray lib = null;
            for (int i = 0; i < ConfigStore.libarays.Count; i++)
            {
                lib = (Libaray)ConfigStore.libarays[i];
                checkedListBox.Items.Add(lib.name);
                if (lib.status == 1)
                {
                    //checkedListBox.SelectedIndex = i;
                    checkedListBox.SetItemChecked(i, true);
                }
            }
        }

        public void LoadSkinList()
        {
            String dirs = ConfigStore.appDir +"\\skin\\";
            string[] filenames = Directory.GetFileSystemEntries(dirs);
            int index = 0, chooseIndex = 0;
            String fileName = "";
            foreach (string file in filenames)
            {
                fileName = file.Substring(file.LastIndexOf("\\") + 1);
                cb_skin.Items.Add(fileName);
                if (ConfigStore.skin.Equals(fileName))
                {
                    chooseIndex = index;
                }
                index++;
            }
            cb_skin.SelectedIndex = chooseIndex;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ConfigStore.skin = cb_useskin.Checked ? cb_skin.Text : "";
            SaveConfig();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(ConfigStore.skin))
            {
                // 初始化皮肤
                Program.mainForm.skin.SkinFile = ConfigStore.appDir + "\\skin\\" + ConfigStore.skin;
            }
            else
            {
                Program.mainForm.skin.Active = false;
            }
            this.Dispose();
        }

        private void SaveConfig()
        {
            ConfigStore.startType = rb_yes.Checked ? "1" : "2";
            ConfigStore.updateType = rb_update1.Checked ? "1" : rb_update2.Checked ? "2" : "3";
            ConfigStore.algorithm = radioButton1.Checked ? "1" : "2";

            ConfigStore.SetAllLibarayStatus(0);
            for (int i = 0; i < checkedListBox.Items.Count; i++)
            {
                if (checkedListBox.GetItemChecked(i))
                {
                    ConfigStore.SetLibarayStatus(i, 1);
                }
            }
            ConfigStore.SaveConfig();
            System.Threading.Thread.Sleep(200);
            this.Dispose();
        }

        private void cb_useskin_CheckedChanged(object sender, EventArgs e)
        {
            cb_skin.Enabled = cb_useskin.Checked;
            if (cb_useskin.Checked)
            {
                Program.mainForm.skin.Active = true;                
            }
            else
            {
                ConfigStore.skin = "";
                Program.mainForm.skin.Active = false;                
            }
        }

        private void cb_skin_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        private void button3_Click(object sender, EventArgs e)
        {
            String skin = cb_skin.Text;
            // 初始化皮肤
            Program.mainForm.skin.SkinFile = ConfigStore.appDir + "\\skin\\" + skin;
        }

    }
}
