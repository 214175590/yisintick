using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using System.Xml;

namespace YisinTick
{
    public partial class VerifyStudy : Form
    {
        static Bitmap v = null;
        static String libId = "";
        static string dir = System.Environment.CurrentDirectory;
        GenerateVerify gv = new GenerateVerify();
        int tmpSeed = 1;
        static String verifyText = "";
        int studycount = 0;
        static bool threadRun = false;

        public VerifyStudy()
        {
            InitializeComponent();
            this.FormClosing += VerifyStudy_FormClosing;
        }

        void VerifyStudy_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (threadRun)
            {
                e.Cancel = true;
                MessageBox.Show("请先停止自动学习！");
            } 
            else
            {
                checkBox2.Checked = false;
                changeStudyStatus();
            }
        }

        private void VerifyStudy_Load(object sender, EventArgs e)
        {
            textBox1.KeyDown += textBox1_KeyDown;
            GetLoginImage();
            LoadLibaray();
            LoadSysConfig();

            //getStudyImage();            
        }

        void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btn_save.PerformClick();
            }
            if (e.KeyCode == Keys.Enter && e.Modifiers == Keys.Control)
            {
                btn_next.PerformClick();
            }
        }

        /// <summary>
        /// 学习
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_shibie_Click(object sender, EventArgs e)
        {
            shibie();
            textBox1.Enabled = true;
        }

        
        /// <summary>
        /// 保存学习结果
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_save_Click(object sender, EventArgs e)
        {
            if(comboBox1.SelectedIndex == -1){
                MessageBox.Show(Program.mainForm, "请先选择一个特征库！");
                return;
            }            
            if (textBox1.Text.Equals(text_Box1.Text))
            {
                DialogResult dr;
                dr = MessageBox.Show(Program.mainForm, "您确定学习此结果吗（" + textBox1.Text + "）？", "准确性提醒", MessageBoxButtons.YesNoCancel,
                         MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                if (dr != DialogResult.Yes)
                {
                    return;
                }                   
            }

            SaveResult();

            if (checkBox1.Checked)
            {
                GetLoginImage();
                shibie();
            }
        }

        
        private void pictureBox1_Click(object sender, EventArgs e)
        {
            GetLoginImage();
        }

        /// <summary>
        /// 识别下一个
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_next_Click(object sender, EventArgs e)
        {
            GetLoginImage();
            //getStudyImage();
            shibie();
            textBox1.Enabled = true;
            Console.WriteLine("总数量-----" + ConfigStore.codeTable.Count);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            String strNum = textBox1.Text;
            if (!strNum.Equals("") && textBox1.Focused)
            {
                char[] cha = strNum.ToCharArray();
                if(cha != null && cha.Length > 0){
                    tbc_11.Text = cha.Length > 0 ? "" + cha[0] : "";
                    tbc_22.Text = cha.Length > 1 ? "" + cha[1] : "";
                    tbc_33.Text = cha.Length > 2 ? "" + cha[2] : "";
                    tbc_44.Text = cha.Length > 3 ? "" + cha[3] : "";
                }                
            }
            btn_save.Enabled = true;
        }

        private void tbc_11_TextChanged(object sender, EventArgs e)
        {
            if (tbc_11.Focused)
            {
                setStrNum();
            }                
        }

        private void tbc_22_TextChanged(object sender, EventArgs e)
        {
            if (tbc_22.Focused)
            {
                setStrNum();
            }
        }

        private void tbc_33_TextChanged(object sender, EventArgs e)
        {
            if (tbc_33.Focused)
            {
                setStrNum();
            }
        }

        private void tbc_44_TextChanged(object sender, EventArgs e)
        {
            if (tbc_44.Focused)
            {
                setStrNum();
            }
        }



        private void btn_libedit_Click(object sender, EventArgs e)
        {
            LibraryMgt gm = new LibraryMgt();
            gm.ShowDialog(Program.mainForm);
            LoadLibaray();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            libId = GetLibId();
        }

        /// <summary>
        /// 新增特征库按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_addlib_Click(object sender, EventArgs e)
        {
            AddLibaray al = new AddLibaray();
            al.ShowDialog();
            LoadLibaray();
        }

        private void rb_1_CheckedChanged(object sender, EventArgs e)
        {
            if (rb_1.Checked)
            {
                ConfigStore.algorithm = "1";
            }
        }

        private void rb_2_CheckedChanged(object sender, EventArgs e)
        {
            if (rb_2.Checked)
            {
                ConfigStore.algorithm = "2";
            }
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            getStudyImage();
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            changeStudyStatus();
        }

        public void changeStudyStatus()
        {
            if (checkBox2.Checked)
            {
                btn_shibie.Enabled = false;
                btn_save.Enabled = false;
                btn_next.Enabled = false;
                textBox1.Enabled = false;
                tbc_11.Enabled = false;
                tbc_22.Enabled = false;
                tbc_33.Enabled = false;
                tbc_44.Enabled = false;
                checkBox1.Enabled = false;
                pictureBox1.Image = null;
                studycount = 0;
                RunThread();
            }
            else
            {
                btn_shibie.Enabled = true;
                btn_save.Enabled = true;
                btn_next.Enabled = true;
                textBox1.Enabled = true;
                tbc_11.Enabled = true;
                tbc_22.Enabled = true;
                tbc_33.Enabled = true;
                tbc_44.Enabled = true;
                checkBox1.Enabled = true;
                pictureBox2.Image = null;
                GetLoginImage();
                label7.Text = "";
            }
        }

        public void SaveResult()
        {
            Program.mainForm.ShowMessage("正在保存验证码学习结果...", false);
            label2.Text = "保存中...";
            String pic1 = CreateDir(tbc_11.Text) + "\\" + CreateUID() + ".bmp";
            String pic2 = CreateDir(tbc_22.Text) + "\\" + CreateUID() + ".bmp";
            String pic3 = CreateDir(tbc_33.Text) + "\\" + CreateUID() + ".bmp";
            String pic4 = CreateDir(tbc_44.Text) + "\\" + CreateUID() + ".bmp";

            VerifyEntity ve = new VerifyEntity();
            if (!ConfigStore.codeTable.Contains(tbd_11.Text))
            {
                ve.LibId = libId;
                ve.Code = tbc_11.Text;
                ve.Data = tbd_11.Text;
                ve.ImgPath = pic1;
                ConfigStore.codeTable.Add(ve.Data, ve);
            }
            ve = new VerifyEntity();
            if (!ConfigStore.codeTable.Contains(tbd_22.Text))
            {
                ve.LibId = libId;
                ve.Code = tbc_22.Text;
                ve.Data = tbd_22.Text;
                ve.ImgPath = pic2;
                ConfigStore.codeTable.Add(ve.Data, ve);
            }
            ve = new VerifyEntity();
            if (!ConfigStore.codeTable.Contains(tbd_33.Text))
            {
                ve.LibId = libId;
                ve.Code = tbc_33.Text;
                ve.Data = tbd_33.Text;
                ve.ImgPath = pic3;
                ConfigStore.codeTable.Add(ve.Data, ve);
            }
            ve = new VerifyEntity();
            if (!ConfigStore.codeTable.Contains(tbd_44.Text))
            {
                ve.LibId = libId;
                ve.Code = tbc_44.Text;
                ve.Data = tbd_44.Text;
                ve.ImgPath = pic4;
                ConfigStore.codeTable.Add(ve.Data, ve);
            }
            System.Drawing.Imaging.ImageFormat imgformat = System.Drawing.Imaging.ImageFormat.Bmp;
            // 保存图片
            pic_11.Image.Save(dir + pic1, imgformat);
            pic_22.Image.Save(dir + pic2, imgformat);
            pic_33.Image.Save(dir + pic3, imgformat);
            pic_44.Image.Save(dir + pic4, imgformat);

            Program.mainForm.ShowMessage("完成");
            label2.Text = "保存成功！";
            btn_save.Enabled = false;
            allcount.Text = "当前总特征数：" + ConfigStore.codeTable.Count;
            SetZhunQuelv(ConfigStore.codeTable.Count);
            btn_next.Focus();
        }

        public void setStrNum()
        {
            String str1 = tbc_11.Text;
            String str2 = tbc_22.Text;
            String str3 = tbc_33.Text;
            String str4 = tbc_44.Text;
            textBox1.Text = (str1.Equals("") ? "?" : str1)
                + (str2.Equals("") ? "?" : str2)
                + (str3.Equals("") ? "?" : str3)
                + (str4.Equals("") ? "?" : str4);
            btn_save.Enabled = true;
        }

        public String CreateUID()
        {
            long i = 1;
            foreach (byte b in Guid.NewGuid().ToByteArray())
            {
                i *= ((int)b + 1);
            }
            return string.Format("{0:x}", i - DateTime.Now.Ticks);
        }

        public String GetLibId()
        {
            Libaray lib = (Libaray)ConfigStore.libarays[comboBox1.SelectedIndex];
            return lib.id;
        }

        public String CreateDir(String code)
        {
            code = code.ToLower();
            DirectoryInfo d = new DirectoryInfo(dir + "\\data");
            if (!d.Exists)
            {
                d.Create();
            }
            d = new DirectoryInfo(dir + "\\data\\" + libId);
            if (!d.Exists)
            {
                d.Create();
            }
            d = new DirectoryInfo(dir + "\\data\\" + libId + "\\" + code);
            if (!d.Exists)
            {
                d.Create();
            }
            d = null;
            return "\\data\\" + libId + "\\" + code;
        }

        public void getStudyImage()
        {
            unchecked
            {
                tmpSeed = (int)(tmpSeed * DateTime.Now.Ticks);
                ++tmpSeed;
            }
            //string code = gv.CreateVerifyCode();
            //gv.CreateImageOnPage(code);
            v = gv.ShowValidationCode(tmpSeed, 4, 16, GenerateVerify.RandomStringMode.Mix, Color.FromArgb(200, 200, 200));
            pictureBox2.Image = v;
            verifyText = gv.getCurrCode();
        }

        public void LoadLibaray()
        {
            int index = comboBox1.SelectedIndex;
            comboBox1.Items.Clear();
            Libaray lib = null;
            for (int i = 0; i < ConfigStore.libarays.Count; i++)
            {
                lib = (Libaray)ConfigStore.libarays[i];
                if (lib.status != 999)
                {
                    comboBox1.Items.Add(lib.name);
                    if (lib.status == 1)
                    {
                        comboBox1.SelectedIndex = i;
                        libId = lib.id;
                    }
                }
            }
            if (index > 0 && index < comboBox1.Items.Count)
            {
                comboBox1.SelectedIndex = index;
            }
            allcount.Text = "当前总特征数：" + ConfigStore.codeTable.Count;
            SetZhunQuelv(ConfigStore.codeTable.Count);
        }

        public void LoadSysConfig()
        {
            // 验证码相似度计算法
            if (ConfigStore.algorithm.Equals("1"))
            {
                rb_1.Checked = true;
            }
            else if (ConfigStore.algorithm.Equals("2"))
            {
                rb_2.Checked = true;
            }
        }

        /// <summary>
        /// 获取登录验证码
        /// </summary>
        private void GetLoginImage()
        {
            Program.mainForm.ShowMessage("正在获取验证码...", false);
            do
            {
                v = _12306Class.GetLoginImage();
            } while (v == null);
            pictureBox1.Image = v;
            Program.mainForm.ShowMessage("完成");
        }

        public void shibie()
        {
            UnCode UnCheckobj = new UnCode(v);           
            if (ConfigStore.algorithm.Equals("1"))
            {
                string strNum1 = UnCheckobj.getPicnum(1);     //识别图片
                verifyText = strNum1;
                text_Box1.Text = strNum1;
            }
            else if (ConfigStore.algorithm.Equals("2"))
            {
                string strNum2 = UnCheckobj.getPicnum(2);     //识别图片
                verifyText = strNum2;
                text_Box2.Text = strNum2;
            }             
            SetTbcd();          
        }

        public void SetTbcd()
        {
            textBox1.Text = verifyText;
            UnCodebase uc = new UnCodebase(v);
            Bitmap[] bb = uc.getImg();
            pic_11.Image = bb[0];
            pic_22.Image = bb[1];
            pic_33.Image = bb[2];
            pic_44.Image = bb[3];

            char[] cha = verifyText.ToCharArray();
            tbc_11.Text = "" + cha[0];
            tbd_11.Text = uc.getData(bb[0]);

            tbc_22.Text = "" + cha[1];
            tbd_22.Text = uc.getData(bb[1]);

            tbc_33.Text = "" + cha[2];
            tbd_33.Text = uc.getData(bb[2]);

            tbc_44.Text = "" + cha[3];
            tbd_44.Text = uc.getData(bb[3]);

            label2.Text = "识别完成！";
            textBox1.Focus();
        }

        public void RunThread()
        {
            ThreadPool.QueueUserWorkItem((m) =>
            {
                threadRun = true;
                Thread.Sleep(500);
                while (checkBox2.Checked)
                {
                    getStudyImage();
                    SetTbcd();
                    Thread.Sleep(300);
                    SaveResult();
                    Thread.Sleep(1000);
                    studycount ++;
                    if (studycount > 1000)
                    {
                        studycount = 0;
                        XmlUtils.WriteLibarayXml();
                        ConfigStore.SaveConfig();
                        Thread.Sleep(4000);
                    }
                }
                threadRun = false;
            });

        }

        public void SetZhunQuelv(int count)
        {
            String zql = "0%";
            if(count >= 1000000){
                zql = "99.999%";
            }
            else
            {
                int x = 50000 * (5 - count / 200000);
                float a = (float)count / (float)(x + count);
                zql = sishewuru(a * 100) + "%";
            }            
            zhunquelv.Text = "识别准确率：" + zql;
        }

        public String sishewuru(float f)
        {
            String s = Convert.ToString(f);
            String r = s;
            int i = s.IndexOf('.');
            if(i > 0){
                r = s.Substring(0, i) + ".";
                char[] cha = s.ToCharArray();
                i++;
                for (; i < cha.Length; i++)
                {
                    if (cha[i] != '0')
                    {
                        r += cha[i];
                        break;
                    }
                    else
                    {
                        continue;                        
                    }
                }
            }
            return r;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int x = int.Parse(textBox2.Text);
            SetZhunQuelv(x);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // 导入
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = "c:\\";//注意这里写路径时要用c:\\而不是c:\
            openFileDialog.Filter = "特征库文件(*.yar)|*.yar";
            openFileDialog.RestoreDirectory = true;
            openFileDialog.FilterIndex = 1;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                String fName = openFileDialog.FileName;

                Libaray lib = new Libaray();
                lib.id = ConfigStore.libarays.Count < 10 ? "lib_0" + ConfigStore.libarays.Count : "lib_" + ConfigStore.libarays.Count;
                //lib.name = name;
                lib.status = 0;
                String undir = dir + "\\data\\" + lib.id + "\\";
                CommonUtil.CreateDir(undir);
                
                CommonUtil.unZipFile(fName, undir);

                String title = fName.Substring(fName.LastIndexOf("\\") + 1);                
                String xmlFile = undir + "data.xml";
                if (File.Exists(xmlFile))
                {
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.Load(xmlFile);
                    // root节点
                    XmlNode root = xmlDoc.SelectSingleNode("datas");
                    //获取到所有<datas>的子节点
                    XmlNodeList nodeList = root.ChildNodes;
                    XmlElement xe = null;
                    //遍历所有子节点
                    foreach (XmlNode xn in nodeList)
                    {
                        xe = (XmlElement)xn;
                        if (xe.Name.Equals("title"))
                        {
                            title = xe.InnerText;
                            break;
                        }
                    }
                }

                lib.name = title;
                ConfigStore.libarays.Add(lib);
                XmlUtils.loadLibarayXml(xmlFile, lib.id);

                LoadLibaray();
                ConfigStore.SaveConfig();
                MessageBox.Show("导入完成！");
            }

        }

        private void button3_Click(object sender, EventArgs e)
        {
            // 导出
            SaveFileDialog sfd = new SaveFileDialog();
            //设置文件类型 
            sfd.Filter = "特征库包（*.yar）|*.yar";

            //保存对话框是否记忆上次打开的目录 
            sfd.RestoreDirectory = true;

            //点了保存按钮进入 
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                string localFilePath = sfd.FileName.ToString(); //获得文件路径
                String imgDir = dir + "\\data\\" + libId + "\\";//待压缩文件目录
                String toFile = localFilePath;//压缩后的目标文件
                CommonUtil.ZipFile(imgDir, toFile, null);
                String FilePath = localFilePath.Substring(0, localFilePath.LastIndexOf("\\"));
                System.Diagnostics.Process.Start("explorer.exe", FilePath);
            }           
        }

    }
}
