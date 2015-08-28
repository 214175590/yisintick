using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Collections;
using System.Threading.Tasks;
using System.Windows.Forms;
using ImgText;
using System.Threading;
using System.IO;

namespace YisinTick
{
    public partial class LibraryMgt : Form
    {
        private ArrayList libArr = new ArrayList();
        private String tempKey = "-*";
        private ArrayList conArr = null;
        private int pageIndex = 1, startIndex = 0, endIndex = 20, pageCount = 65, allIndex = 0;

        public LibraryMgt()
        {
            InitializeComponent();
        }

        private void LibraryMgt_Load(object sender, EventArgs e)
        {
            LoadLibaray();
            comboBox1.Text = "每页65条";
            timer1.Interval = 500;
            timer1.Tick += timer1_Tick;
            timer1.Start();
        }

        void timer1_Tick(object sender, EventArgs e)
        {
            conArr = getCheckedUserControl();
            if (conArr.Count > 0)
            {
                btn_del.Enabled = true;
            }
            else
            {
                btn_del.Enabled = false;
            }
            conArr = getChangeUserControl();
            if (conArr.Count > 0)
            {
                btn_save.Enabled = true;
            }
            else
            {
                btn_save.Enabled = false;
            }
        }

        public void LoadLibaray()
        {
            int index = cb_lib.SelectedIndex;
            cb_lib.Items.Clear();
            Libaray lib = null;
            for (int i = 0; i < ConfigStore.libarays.Count; i++)
            {
                lib = (Libaray)ConfigStore.libarays[i];
                if (lib.status != 999)
                {
                    cb_lib.Items.Add(lib.name);
                    if (lib.status == 1)
                    {
                        cb_lib.SelectedIndex = i;
                    }
                }
            }
            if (index > 0 && index < cb_lib.Items.Count)
            {
                cb_lib.SelectedIndex = index;
            }
        }

        public String GetLibId()
        {
            Libaray lib = (Libaray)ConfigStore.libarays[cb_lib.SelectedIndex];
            return lib.id;
        }

        private void btn_load_Click(object sender, EventArgs e)
        {
            if (cb_lib.SelectedIndex == -1)
            {
                MessageBox.Show(Program.mainForm, "请选择一个特征库！");
                return;
            }
            String key = textBox1.Text;
            Search(key);            
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_del_Click(object sender, EventArgs e)
        {
            DialogResult dr;
            dr = MessageBox.Show(Program.mainForm, "您确定要删除选择的项吗（" + textBox1.Text + "）？", "删除提醒", MessageBoxButtons.YesNoCancel,
                     MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
            if (dr != DialogResult.Yes)
            {
                return;
            }

            UserControl1 uc = null;
            ArrayList tempArr = getCheckedUserControl();
            for (int i = 0; i < tempArr.Count; i++)
            {
                uc = (UserControl1)tempArr[i];
                flowLayoutPanel1.Controls.Remove(uc);
                ConfigStore.codeTable.Remove(uc.GetAttr1());
            }
            tempArr.Clear();
            tempArr = null;
        }

        private void btn_add_Click(object sender, EventArgs e)
        {
            AddLibaray al = new AddLibaray();
            al.ShowDialog();
            LoadLibaray();
        }

        public ArrayList getCheckedUserControl()
        {
            UserControl1 uc = null;
            ArrayList tempArr = new ArrayList();
            for (int i = 0; i < flowLayoutPanel1.Controls.Count; i++)
            {
                uc = (UserControl1)flowLayoutPanel1.Controls[i];
                if (uc.isChecked())
                {
                    tempArr.Add(uc);
                }
            }
            return tempArr;
        }

        public ArrayList getChangeUserControl()
        {
            UserControl1 uc = null;
            ArrayList tempArr = new ArrayList();
            for (int i = 0; i < flowLayoutPanel1.Controls.Count; i++)
            {
                uc = (UserControl1)flowLayoutPanel1.Controls[i];
                if (uc.isChange())
                {
                    tempArr.Add(uc);
                }
            }
            return tempArr;
        }

        private void btn_save_Click(object sender, EventArgs e)
        {
            ArrayList tempArr = getChangeUserControl();
            UserControl1 uc = null;
            VerifyEntity ve = null;
            for (int i = 0; i < tempArr.Count; i++)
            {
                uc = (UserControl1) tempArr[i];
                ve = (VerifyEntity) ConfigStore.codeTable[uc.GetAttr1()];
                ve.Code = uc.GetText();
                uc.SetChange(false);
                uc.SetChecked(false);
            }
            tempArr.Clear();
            tempArr = null;
            MessageBox.Show(Program.mainForm, "保存成功！");
        }

        public void Search(String key)
        {
            if (tempKey == key)
            {
                return;
            }
            tempKey = key;
            libArr.Clear();
            String libId = GetLibId();
            ArrayList akeys = new ArrayList(ConfigStore.codeTable.Keys);
            if (String.IsNullOrEmpty(key))
            {
                VerifyEntity ve = null;             
                for (int i = 0; i < akeys.Count; i++)
                {
                    ve = (VerifyEntity)ConfigStore.codeTable[akeys[i]];
                    if (libId.Equals(ve.LibId))
                    {
                        libArr.Add(ve);
                    }
                }
            }
            else
            {
                VerifyEntity ve = null;
                foreach (string skey in akeys)
                {
                    ve = (VerifyEntity)ConfigStore.codeTable[skey];
                    if (key != null && !key.Equals(""))
                    {
                        if (key.IndexOf(ve.Code) != -1)
                        {
                            if (libId.Equals(ve.LibId))
                            {
                                libArr.Add(ve);
                            }
                        }
                    }
                }
            }
            if (libArr.Count == 0)
            {
                btn_per.Enabled = false;
                btn_next.Enabled = false;
                label3.Text = "当前第0/0页。共0条记录";
                flowLayoutPanel1.Controls.Clear();
            }
            else
            {
                allIndex = libArr.Count / pageCount + (libArr.Count % pageCount > 0 ? 1 : 0);
                label3.Text = "当前第" + pageIndex + "/" + allIndex + "页。共" + libArr.Count + "条记录";
                dragImg();
                
            }
            
        }

        public void dragImg()
        {
            flowLayoutPanel1.Controls.Clear();
            UserControl1 uc = null;
            VerifyEntity ve = null;
            startIndex = (pageIndex - 1) * pageCount;
            endIndex = pageIndex * pageCount;
            endIndex = endIndex > libArr.Count ? libArr.Count : endIndex;
            for (int i = startIndex; i < endIndex; i++)
            {
                ve = (VerifyEntity)libArr[i];
                uc = new UserControl1();
                uc.SetText(ve.Code);
                uc.SetPic(ConfigStore.appDir + ve.ImgPath);
                uc.SetAttr1(ve.Data);
                flowLayoutPanel1.Controls.Add(uc);
            }
            label3.Text = "当前第" + pageIndex + "/" + allIndex + "页。共" + libArr.Count + "条记录";
            if (allIndex > 1 && pageIndex < allIndex)
            {
                btn_next.Enabled = true;
            }
            else
            {
                btn_next.Enabled = false;
            }

            if (pageIndex > 1)
            {
                btn_per.Enabled = true;
                button1.Enabled = true;
            }
            else
            {
                btn_per.Enabled = false;
                button1.Enabled = false;
            }
        }

        private void btn_per_Click(object sender, EventArgs e)
        {
            if(pageIndex > 1){
                pageIndex--;
                dragImg();
            }
        }

        private void btn_next_Click(object sender, EventArgs e)
        {
            if (pageIndex < allIndex)
            {
                pageIndex++;
                dragImg();
            }
        }

        private void cb_lib_SelectedIndexChanged(object sender, EventArgs e)
        {
            tempKey = "-*";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            pageIndex = 1;
            dragImg();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            bool check = checkBox1.Checked;
            UserControl1 uc = null;
            for (int i = 0; i < flowLayoutPanel1.Controls.Count; i++)
            {
                uc = (UserControl1)flowLayoutPanel1.Controls[i];
                uc.SetChecked(check);
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBox1.SelectedIndex)
            {
                case 0: pageCount = 65; break;
                case 1: pageCount = 95; break;
                case 2: pageCount = 130; break;
                case 3: pageCount = 260; break;
                case 4: pageCount = 520; break;
                case 5: pageCount = 1040; break;
                case 6: pageCount = 2080; break;
                case 7: pageCount = 5160; break;
                case 8: pageCount = 10320; break;
                default: pageCount = 65; break;
            }
            tempKey = tempKey.Equals("-*") ? "*-" : "-*";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult dr;
            dr = MessageBox.Show(Program.mainForm, "您确定要删除此特征库吗？\n【"+cb_lib.Text+"】", "提醒", MessageBoxButtons.YesNoCancel,
                     MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
            if (dr != DialogResult.Yes)
            {
                return;
            }
            String libId = GetLibId();
            foreach (Libaray lib in ConfigStore.libarays)
            {
                if(lib.id.Equals(libId)){
                    lib.status = 999;
                    break;
                }
            }

            VerifyEntity ve = null;
            foreach (String key in ConfigStore.codeTable.Keys)
            {
                ve = (VerifyEntity)ConfigStore.codeTable[key];
                if (ve.LibId.Equals(libId))
                {
                    ConfigStore.codeTable.Remove(key);                    
                }
            }       

            LoadLibaray();
            ConfigStore.SaveConfig();
            MessageBox.Show("删除成功！");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (textBox2.Visible)
            {
                String libId = GetLibId();
                foreach (Libaray lib in ConfigStore.libarays)
                {
                    if (lib.id.Equals(libId))
                    {
                        lib.name = textBox2.Text;
                        break;
                    }
                }

                VerifyEntity ve = null;
                foreach (String key in ConfigStore.codeTable.Keys)
                {
                    ve = (VerifyEntity)ConfigStore.codeTable[key];
                    if (ve.LibId.Equals(libId))
                    {
                        ve.LibName = textBox2.Text;
                    }
                }
                LoadLibaray();
                ConfigStore.SaveConfig();
                button3.Text = "修改特征库名称";
                cb_lib.Enabled = true;
                textBox2.Visible = false;
                button4.Visible = false;
            }
            else
            {
                textBox2.Text = cb_lib.Text;
                button3.Text = "保存修改";
                cb_lib.Enabled = false;
                textBox2.Visible = true;
                button4.Visible = true;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            button3.Text = "修改特征库名称";
            textBox2.Visible = false;
            button4.Visible = false;
            textBox2.Text = "";
        }

    }
}
