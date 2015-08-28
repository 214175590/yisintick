using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace YisinTick
{
    public partial class ImageFrom : Form
    {
        static ImageFrom IF = new ImageFrom();
        private static Bitmap map = null;
        static System.Windows.Forms.Timer timer = null;

        public static ImageFrom GetImageFrom
        {
            get { return IF; }
        }

        public string Code { get { return textBox1.Text; } }

        public ImageFrom()
        {
            InitializeComponent();            
        }

        void timer_Tick(object sender, EventArgs e)
        {
            timer.Stop();
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        public void Show(Image image)
        {
            try
            {
                pictureBox1.Image = image;
                textBox1.Text = "";
                map = (Bitmap)image;
                if (ConfigStore.isAutoWriterVerify && ConfigStore.yesFailedTimes < ConfigStore.failedTimes)
                {
                    shibie();
                    if(timer == null){
                        timer = new System.Windows.Forms.Timer();
                        timer.Interval = 200;
                        timer.Tick += timer_Tick;
                    }                   
                    timer.Start();
                }
                this.ShowDialog(Program.mainForm);
            }
            catch(Exception e) {
                Program.mainForm.ShowMessage(e.Message);
            }
        }

        private void shibie()
        {
            string strNum = "";
            try
            {
                UnCode UnCheckobj = new UnCode(map);
                strNum = UnCheckobj.getPicnum(0); //识别图片
            }
            catch { }
            textBox1.Text = strNum;
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 13)
            {
                if (textBox1.Text.Length == 4)
                {
                    this.Close();
                }
            }
            else if (e.KeyValue == 27)
            {
                textBox1.Text = "";

                this.Close();
            }
        }

        private void ImageFrom_Load(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            GetVerifyImage();
        }

        public void GetVerifyImage()
        {
            var response = HttpHelper.CreateGetHttpResponse(_12306Class.GetTask_3_Image + new Random().NextDouble().ToString(),
                                null, "https://kyfw.12306.cn/otn/leftTicket/init",_12306Class.Cookies);
            System.IO.Stream resStream = response.GetResponseStream();//得到验证码数据流
            map = new Bitmap(resStream);//初始化Bitmap图片
            if (ConfigStore.isAutoWriterVerify && ConfigStore.yesFailedTimes < ConfigStore.failedTimes)
            {
                shibie();
            }
        }
    }
}
