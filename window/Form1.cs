using Sunisoft.IrisSkin;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Speech.Synthesis;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace YisinTick
{
    public partial class Form1 : Form
    {
        public static bool isLogin = false;
        public static string MachineCode = "";// 机器码
        public static int TickCount = 0;
        public static bool firstExit = true;
        public bool iscancel = false;
        public SkinEngine skin = new SkinEngine();

        public Form1()
        {
            InitializeComponent();
            登录ToolStripMenuItem.Enabled = false;
            Control.CheckForIllegalCrossThreadCalls = false;

            richTextBox1.BackColor = Color.OliveDrab;
            richTextBox1.ForeColor = Color.White;

            Console.WriteLine("Environment.CurrentDirectory==" + Environment.CurrentDirectory);

            Trains.TrainsChanged += Trains_TrainsChanged;
            Seat.SelectSeatsChange += TrainSeat_SelectSeatsChange;
            Thread12306.ShowMessage += Thread12306_Message;
            TranTime.TimeChanged += TranTime_TimeChanged;
            Thread12306.ThreadClosed += Thread12306_ThreadClosed;

            ThreadPool.QueueUserWorkItem((n) =>
            {
                Thread.Sleep(100);
                // 获取机器码
                string info = MyEncrypt.GetInfo();
                MachineCode = MyEncrypt.GetMd5_16(info, false);
                this.tstbJqm.Text = "机器码：" + MachineCode;

                //GetTickCount();
            }); 

            ThreadPool.QueueUserWorkItem((a) =>
            {
                Thread.Sleep(2500);
                if (!String.IsNullOrEmpty(MachineCode))
                {
                    GetTickCount();
                }
            });

            ThreadPool.QueueUserWorkItem((m) =>
            {
                ShowMessage("正在初始化...", false);
                _12306Class.GetMainPage(cookie: new CookieCollection());
                ShowMessage("完成");

                //  先获取车站信息
                //var v = Stations.List;

                TickCute.CreateDir();
            });

            ThreadPool.QueueUserWorkItem((m) =>
            {
                // 初始化配置
                ConfigStore.InitConfig();

                SetAutoBaojing(ConfigStore.isAutoBaojing);
                SetAutoWriterVerify(ConfigStore.isAutoWriterVerify);
                setFailedTimes(ConfigStore.failedTimes);
                button1.Enabled = ConfigStore.islocalhost;
            });


            ThreadPool.QueueUserWorkItem((a) =>
            {
                Thread.Sleep(3500);
                String dateStr = DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day + " " +
                    DateTime.Now.Hour + ":" + DateTime.Now.Minute + ":" + DateTime.Now.Second;

                String s = "我的机器码：" + Form1.MachineCode + "\n";
                s += "我使用了【隐心抢票助手】：" + dateStr + "\n";
                CommonUtil.SendEMail("yisin2014@163.com", Form1.MachineCode, "214175590@qq.com", "隐心", "使用【隐心抢票助手】报告", s, "", "smtp.163.com", "yisin2014@163.com", "sendemail");
            });
        }

        public void GetTickCount()
        {
            // 获取剩余票数
            string s = TickCute.GetRegeditInfo(MachineCode);
            TickCount = TickCute.GetUseTick(MachineCode, s, false);
            SetTickText();
            btnDuihuan.Enabled = true;
            btn_luntan.Enabled = true;
            if (TickCount > 0)
            {
                //登录ToolStripMenuItem.Enabled = true;                
                int hour = DateTime.Now.Hour;
                if (hour > 7 && hour < 23)
                {
                    登录ToolStripMenuItem.Enabled = true;
                    labUserInfo.ForeColor = Color.Green;
                    labUserInfo.Text = "请登录！";
                }
                else
                {
                    登录ToolStripMenuItem.Enabled = true;
                    labUserInfo.ForeColor = Color.Red;
                    labUserInfo.Text = "晚上23点后到凌晨7点前\n是12306系统维护时间，将无法购票！";
                }                 
            }
            else
            {
                labUserInfo.ForeColor = Color.Red;
                labUserInfo.Text = "剩余购票卷数量不足，请购买！";
            }
        }

        public void SetTickText()
        {
            tickinfo.Text = "购票卷：" + TickCount;
            if (TickCount > 0)
            {
                if (!isLogin)
                {
                    //int hour = DateTime.Now.Hour;
                    //if (hour > 7 && hour < 23)
                    //{
                        登录ToolStripMenuItem.Enabled = true;
                        labUserInfo.ForeColor = Color.Green;
                        labUserInfo.Text = "请登录！";
                    //}
                }
            }
        }

        void Thread12306_Message(object sender, List<Message> e)
        {
            ShowMessage(e);
        }

        void Thread12306_ThreadClosed(object sender, EventArgs e)
        {
            button12.Enabled = false;
            button7.Enabled = true;
            checkedListBox1.Enabled = true;
        }

        void TranTime_TimeChanged(object sender, List<string> e)
        {
            listBox2.Items.Clear();

            foreach (var VARIABLE in e)
            {
                listBox2.Items.Add(VARIABLE);
            }
        }

        void TrainSeat_SelectSeatsChange(object sender, List<SeatsType> e)
        {
            listBox1.Items.Clear();
            foreach (var seatsType in e)
            {
                listBox1.Items.Add(seatsType);
            }
        }

        void Trains_TrainsChanged(object sender, List<Train> e)
        {
            checkedListBox2.Items.Clear();
            foreach (var train in e)
            {
                checkedListBox2.Items.Add(train);
            }
        }

        public void ChangeOtherButton(bool enabled)
        {
            button3.Enabled = enabled;
            button4.Enabled = enabled;
            button5.Enabled = enabled;
            button6.Enabled = enabled;
            button7.Enabled = enabled;
        }

        public void LoginSuccess()
        {
            if (isLogin)
            {
                登录ToolStripMenuItem.Text = "更换账号";
            }
        }

        public bool GetUserInfo(string name)
        {
            labUserInfo.ForeColor = Color.Blue;
            labUserInfo.Text = name + " 您好！\n\n欢迎使用隐心抢票助手！";
            return true;
        }

        List<Contact> listContact = new List<Contact>();

        /// <summary>
        /// 获取联系人信息
        /// </summary>
        public void GetFavoriteContacts()
        {
            ShowMessage("正在获取常用联系人...",false);
            int count = 0;
            int pageSize = 7;
            int pageIndex = 0;
            ;
            do
            {
                listContact.AddRange(_12306Class.GetFavoriteContacts());

                checkedListBox1.DataSource = listContact;

            } while ((pageIndex + 1) * pageSize < count);

            ShowMessage("完成");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            GetFavoriteContacts();
        }

        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            //new CreateFavoriteContactFrom().ShowDialog();
           System.Diagnostics.Process.Start("https://kyfw.12306.cn/otn/passengers/init");
        }

        private void button5_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < checkedListBox2.Items.Count; i++)
            {
                checkedListBox2.Items.RemoveAt(i);
                i--;
            }
            Trains.GetTrains().Clear();
            new GetTrainsFrom().ShowDialog();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < checkedListBox2.Items.Count; i++)
            {
                checkedListBox2.SetItemCheckState(i, CheckState.Checked);
            }
        }


        public void ShowMessage(string message, bool isNewLine = true)
        {
            try
            {

                this.Invoke(new Action<string, bool>((m, n) => { _ShowMessage(m, n); }), message, isNewLine);
            }
            catch (Exception)
            {
                
            }

        }
        public void _ShowMessage(string message, bool isNewLine = true)
        {
            try
            {
                richTextBox1.ForeColor = Color.White;
                richTextBox1.AppendText(string.Format("{0},{1}", DateTime.Now.ToString("HH:mm:ss"), message));
                if (isNewLine)
                {
                    richTextBox1.AppendText("\r\n");
                }

                richTextBox1.SelectionStart = richTextBox1.TextLength;
                richTextBox1.ScrollToCaret();
            }
            catch (Exception)
            {

            }
        }

        public void ShowMessage(List<Message> messages)
        {
            this.Invoke(new Action<List<Message>>((m) => { _ShowMessage(m); }), messages);

        }
        public void _ShowMessage(List<Message> messages)
        {
            try
            {
                richTextBox1.ForeColor = Color.White;
                richTextBox1.AppendText(string.Format("{0},", DateTime.Now.ToString("HH:mm:ss")));
                foreach (var message in messages)
                {
                    int p1 = richTextBox1.TextLength;  //取出未添加时的字符串长度。   
                    richTextBox1.AppendText(string.Format("{0},", message.Msg));  //保留每行的所有颜色。 //  rtb.Text += strInput + "/n";  //添加时，仅当前行有颜色。   
                    int p2 = string.Format("{0},", message.Msg).Length;  //取出要添加的文本的长度   
                    richTextBox1.Select(p1, p2);        //选中要添加的文本   
                    richTextBox1.SelectionColor = message.ForeColor;  //设置要添加的文本的字体色  
                }

                richTextBox1.AppendText("\r\n");

                richTextBox1.SelectionStart = richTextBox1.TextLength;
                richTextBox1.ScrollToCaret();
            }
            catch (Exception)
            {

            }
        }


        public void button7_Click(object sender, EventArgs e)
        {
            Thread12306.Stop();
            ConfigStore.yesFailedTimes = 0;
            if (checkedListBox1.CheckedItems.Count <= 0)
            {
                MessageBox.Show(Program.mainForm, "请乘车人员");
                return;
            }

            if (checkedListBox2.CheckedItems.Count <= 0)
            {
                MessageBox.Show(Program.mainForm, "请选择列车");
                return;
            }

            if (listBox2.Items.Count <= 0)
            {
                MessageBox.Show(Program.mainForm, "请选择乘车时间");
                return;
            }

            if (listBox1.Items.Count <= 0)
            {
                MessageBox.Show(Program.mainForm, "请选择坐席");
                return;
            }

            if (checkedListBox2.CheckedItems.Count > 1 && listBox2.Items.Count > 1)
            {
                MessageBox.Show(Program.mainForm, "选择多辆列车时，仅能选择一天乘车日期。");
                return;
            }

            List<string> times = new List<string>();

            object[] objs = new object[5];
            objs[0] = TranTime.GetTimes();
            objs[1] = checkedListBox2.CheckedItems;
            objs[2] = Seat.GetSeats();
            objs[3] = this;
            objs[4] = checkedListBox1.CheckedItems;

            button7.Enabled = false;
            checkedListBox1.Enabled = false;

            ThreadPool.QueueUserWorkItem((m) =>
                {
                    bool b;
                    do
                    {
                        b = Thread12306.Start(objs);
                        Thread.Sleep(500);
                    } while (!b);

                    button12.Enabled = true;
                });
        }

        public bool GetRunStatus()
        {
            return !button7.Enabled;
        }

        private void button8_Click(object sender, EventArgs e)
        {
            new SelectTrainSeatForm().ShowDialog();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            Seat.ClearSelectSeats();
        }

        private void button11_Click(object sender, EventArgs e)
        {
            TranTime.Clear();
        }

        private void button10_Click(object sender, EventArgs e)
        {
            new TranTimeForm().ShowDialog();
        }

        public void button12_Click(object sender, EventArgs e)
        {
            button12.Enabled = false;
            checkedListBox1.Enabled = true;
            Thread12306.Stop();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (!iscancel)
                {
                    //DialogResult dialogResult = MessageBox.Show("确定退出吗?", "隐心提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);

                    ComfirmDialog cd = new ComfirmDialog();
                    cd.SetMessage("确定退出吗?", "隐心提示");
                    cd.ShowDialog(this);
                    if (ComfirmDialog.isOk)
                    {
                        iscancel = true;
                        if (firstExit)
                        {
                            e.Cancel = true;
                            Process.GetInstance().StartCloseThread();
                        }
                        else
                        {
                            Thread.Sleep(1000);
                        }
                    }
                    else
                    {
                        e.Cancel = true;//就不退了
                    }
                }
                else
                {
                    Thread.Sleep(1000);
                }
            }
            catch { }           
            
        }


        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            checkedListBox1.ItemCheck += checkedListBox1_ItemCheck;
            //checkedListBox1.SelectedIndexChanged += checkedListBox1_SelectedIndexChanged;
            timer1.Interval = 1000;
            timer1.Tick += timer1_Tick;
            timer1.Start();

        }

        void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                // 初始化皮肤
                /*if (!String.IsNullOrEmpty(ConfigStore.skin))
                {
                    skin.SkinFile = ConfigStore.appDir + "\\skin\\" + ConfigStore.skin;
                }
                timer1.Stop();
                 * */
                if (验证码学习ToolStripMenuItem.Text == "验证码学习   <-")
                {
                    验证码学习ToolStripMenuItem.Text = "验证码学习  <-";
                }
                else if (验证码学习ToolStripMenuItem.Text == "验证码学习  <-")
                {
                    验证码学习ToolStripMenuItem.Text = "验证码学习 <-";
                }
                else if (验证码学习ToolStripMenuItem.Text == "验证码学习 <-")
                {
                    验证码学习ToolStripMenuItem.Text = "验证码学习   <-";
                }
            }
            catch { }
        }

        void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
                
        }

        void checkedListBox1_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            int index = checkedListBox1.SelectedIndex;
            //Console.WriteLine("当前选择行====" + index + "====" + e.CurrentValue);
            ThreadPool.QueueUserWorkItem((a) =>
            {
                Thread.Sleep(300);
                int count = checkedListBox1.CheckedItems.Count;
                person_count.Text = "" + count;
                // 获取选择的用户数
                if (count > TickCount)
                {
                    button7.Enabled = false;
                    checkedListBox1.SetItemCheckState(index, CheckState.Unchecked);
                    MessageBox.Show(Program.mainForm, "选择的人员数超过了购票卷数量，无法购买！");
                }
                else
                {
                    button7.Enabled = true;
                }
                count = checkedListBox1.CheckedItems.Count; 
                person_count.Text = "" + count;
            });
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void groupBox5_Enter(object sender, EventArgs e)
        {

        }

        private void button13_Click(object sender, EventArgs e)
        {
            Console.WriteLine(MyEncrypt.GetMD5(MyEncrypt.GetInfo()));

            Console.WriteLine(MyEncrypt.GetMd5_16(MyEncrypt.GetInfo(), true));
            
        }

        private void btnCgAcc_Click(object sender, EventArgs e)
        {

        }

        private void 登录ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (isLogin)
            {
                DialogResult dr;
                dr = MessageBox.Show(Program.mainForm, "您确定要更换帐号吗？", "提醒", MessageBoxButtons.YesNoCancel,
                         MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                if (dr != DialogResult.Yes)
                {
                    return;
                }
                //登录ToolStripMenuItem.Text = "登录";
            }
            //Process.GetInstance().StartShowLoginThread();
            FrmLogin login = new FrmLogin();
            login.ShowFrm();
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Clipboard.SetData(DataFormats.Text, MachineCode);
            MessageBox.Show(Program.mainForm, "机器码已复制到剪切板！");
        }

        private void btnDuihuan_Click(object sender, EventArgs e)
        {
            FrmDuiHuan fdh = new FrmDuiHuan();
            fdh.ShowDialog(Program.mainForm);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            GenretorMa gm = new GenretorMa();
            gm.ShowDialog(Program.mainForm);
        }

        private void 验证码学习ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Process.GetInstance().StartShowStudyThread();
            VerifyStudy gm = new VerifyStudy();
            Program.mainForm.HideProcess();
            gm.ShowDialog(Program.mainForm);
        }

        private void 设置ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Setting set = new Setting();
            set.ShowDialog(Program.mainForm);
        }

        private void 论坛ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://yisin.88448.com");
        }

        public bool isAutoBaojing()
        {
            return cb_baojing.Checked;
        }

        public void SetAutoBaojing(bool chck)
        {
            cb_baojing.Checked = chck;
        }

        public bool isAutoWriterVerify()
        {
            return cbAutoCode.Checked;
        }

        public void SetAutoWriterVerify(bool chck)
        {
            cbAutoCode.Checked = chck;
            ConfigStore.isAutoWriterVerify = chck;
        }

        public int getFailedTimes()
        {
            var text = tb_failedTimes.Text;
            return text.Equals("")? 0 : Convert.ToInt32(text);
        }

        public void setFailedTimes(int times)
        {
            tb_failedTimes.Text = Convert.ToString(times);
        }

        private void cbAutoCode_CheckedChanged(object sender, EventArgs e)
        {
            tb_failedTimes.Enabled = !cbAutoCode.Checked;
            ConfigStore.isAutoWriterVerify = cbAutoCode.Checked;
            ConfigStore.yesFailedTimes = 0;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < checkedListBox2.Items.Count; i++)
            {
                checkedListBox2.Items.RemoveAt(i);
                i--;
            }
            Trains.GetTrains().Clear();
        }

        private void cb_baojing_CheckedChanged(object sender, EventArgs e)
        {
            ConfigStore.isAutoBaojing = cb_baojing.Checked;
        }

        private void 关于ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            AboutUS aus = new AboutUS();
            aus.ShowDialog(Program.mainForm);
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://kyfw.12306.cn/otn/queryOrder/initNoComplete");
            linkLabel1.Text = "";
        }

        public void SetLinkLabelText(String text)
        {
            linkLabel1.Text = text;
        }

        private void btn_luntan_Click(object sender, EventArgs e)
        {
            //Process.GetInstance().StartShowLoginBBSThread();
            Login88448.Get88448LoginForm().ShowWindow();
        }

        public void SetProcessMessage(String msg)
        {
            processMessage.Text = msg;
        }

        public void ShowProcess()
        {
            panel1.Show();
        }

        public void HideProcess()
        {
            panel1.Hide();
        }

        public static void CloseApplication()
        {
            Application.Exit();
        }

        public void ChangeLuntan(int n)
        {
            if(n == 1){
                btn_luntan.ForeColor = Color.Black;
                btn_luntan.Text = "登录论坛";
            } else {
                btn_luntan.ForeColor = Color.Green;
                btn_luntan.Text = "已登录";
            }            
        }

        private void 作者隐心QQ214175590ToolStripMenuItem_Click(object sender, EventArgs e)
        {
                        
        }
    }



    /// <summary>
    /// Thread12306线程类
    /// </summary>
    class Thread12306
    {
        static bool start = true;
        static bool end = true;

        static Thread thread = null;

        public static void Stop()
        {
            start = false;

            if (thread != null && ShowMessage != null)
            {
                ShowMessage(null, new List<Message>() { new Message(string.Format("线程{0}正在停止...", thread.ManagedThreadId, Color.Red)) });
            }
        }

        public static bool Start(object obj)
        {
            if (!end)
            {
                return end;
            }

            start = true;

            thread = new Thread(ThreadStart());
            ShowMessage(null, new List<Message>() { new Message(string.Format("线程{0}开始...", thread.ManagedThreadId)) });
            thread.Start(obj);


            end = false;

            return true;
        }

        public static event EventHandler<List<Message>> ShowMessage;
        public static event EventHandler ThreadClosed;

        private static ParameterizedThreadStart ThreadStart()
        {
            //int m = 6 * 100;
            //int n = 0;
            return (o) =>
            {
                object[] objs = o as object[];
                var times = objs[0] as List<string>;
                var checkedItemCollection = objs[1] as CheckedListBox.CheckedItemCollection;
                var Seats = objs[2] as List<SeatsType>;
                var from1 = objs[3] as Form1;
                var checkedPeople = objs[4] as CheckedListBox.CheckedItemCollection;
                List<Train> selectTrainList = new List<Train>();
                foreach (Train v in checkedItemCollection)
                {
                    selectTrainList.Add(v);
                }
                List<Contact> selectContactList = new List<Contact>();
                foreach (Contact v in checkedPeople)
                {
                    selectContactList.Add(v);
                }
                ConfigStore.lastFailedTime = DateTime.Now;
                while (start)
                {
                    foreach (var time in times)
                    {
                        //n = 0;
                        var tempTrainList = _12306Class.GetTrains(time
                                         , _12306Class.From.Code
                                         , _12306Class.To.Code);
                        if (tempTrainList == null || tempTrainList.Count == 0)
                        {
                            ShowMessage(null, new List<Message>() { new Message("系统异常", Color.Red) });
                            break;
                        }
                        bool hasTask = false;
                        bool GetTickTrue = false;
                        foreach (Train train in selectTrainList)
                        {
                            if (hasTask)
                            {
                                break;
                            }
                            var tempTrain = tempTrainList.SingleOrDefault(fun => fun.TrainValue == train.TrainValue);
                            if (tempTrain == null)
                            {
                                ShowMessage(null,
                                    new List<Message>() {new Message("车次（" + train.TrainValue + "）不存在", Color.Yellow)});
                                continue;
                            }
                            List<Message> listMessage = new List<Message>();
                            listMessage.Add(new Message("正在检查"));
                            listMessage.Add(new Message(train.TrainValue, Color.Yellow));
                            listMessage.Add(new Message(".."));

                            ShowMessage(null, listMessage);
                            listMessage.Clear();
                            foreach (SeatsType trainSeat in Seats)
                            {
                                if (hasTask)
                                {
                                    break;
                                }
                                switch (trainSeat)
                                {
                                    case SeatsType.商务座:
                                        hasTask = HasTask(train.SWZ);
                                        break;
                                    case SeatsType.特等座:
                                        hasTask = HasTask(train.TZ);
                                        break;
                                    case SeatsType.一等座:
                                        hasTask = HasTask(train.ZY);
                                        break;
                                    case SeatsType.二等座:
                                        hasTask = HasTask(train.ZE);
                                        break;
                                    case SeatsType.高级软卧:
                                        hasTask = HasTask(train.GR);
                                        break;
                                    case SeatsType.软卧:
                                        hasTask = HasTask(train.RW);
                                        break;
                                    case SeatsType.硬卧:
                                        hasTask = HasTask(train.YW);
                                        break;
                                    case SeatsType.软座:
                                        hasTask = HasTask(train.RZ);
                                        break;
                                    case SeatsType.硬座:
                                        hasTask = HasTask(train.YZ);
                                        break;
                                    case SeatsType.无座:
                                        hasTask = HasTask(train.WZ);
                                        break;
                                }


                                if (hasTask)
                                {
                                    ShowMessage(null, new List<Message>() {new Message(trainSeat.ToString() + ":" + "有..")});

                                    if (checkedPeople.Count <= 0)
                                    {
                                        ShowMessage(null,
                                            new List<Message>() { new Message("未选择常用联系人，不进行预订。",Color.Yellow) });
                                        //如果未选择常用联系人，则不订票
                                        hasTask = false;
                                    }
                                }
                                else
                                {
                                    ShowMessage(null,
                                        new List<Message>() {new Message(trainSeat.ToString() + ":" + "无..")});
                                }

                                if (hasTask)
                                {
                                   // from1.button12_Click(null, null);
                                    GetTickTrue = GetTask(train, time, trainSeat, selectContactList);
                                    if (GetTickTrue)
                                    {
                                        from1.button12_Click(null, null);
                                    }
                                }
                            }
                            ShowMessage(null, listMessage);
                        }

                        //foreach (SeatsType trainSeat in selectTrainList)
                        //{
                        //    var seat = _trainSeat.List.SingleOrDefault(fun => fun.Name == trainSeat.ToString() && fun.Count != "无");
                        //    if (seat != null)
                        //    {
                        //        HasTask(_trainSeat, time, seat);

                        //        break;
                        //    }
                        //}
                        /*while (start && n < m)
                        {
                            n++;
                            Thread.Sleep(10);
                        }*/
                        Thread.Sleep(200);
                        if (hasTask && GetTickTrue)
                        {
                            if (ConfigStore.yesFailedTimes < ConfigStore.failedTimes)
                            {
                                DateTime dt1 = ConfigStore.lastFailedTime;
                                int cha = TranTime.DateDiff(DateTime.Now, dt1)/1000;
                                if(cha < 20){
                                    ConfigStore.yesFailedTimes++;
                                    Program.mainForm.ShowMessage(String.Format("自动输入验证码，失败{0}次", ConfigStore.yesFailedTimes));
                                }
                                else
                                {
                                    ConfigStore.yesFailedTimes = 0;
                                }
                                ConfigStore.lastFailedTime = DateTime.Now;
                            }
                            else
                            {
                                Program.mainForm.ShowMessage(String.Format("自动输入验证码失败次数超过{0}次，改为手动输入！", ConfigStore.failedTimes));
                                Program.mainForm.SetAutoWriterVerify(false);
                            }
                        }                                    
                    }
                }


                end = true;
                ShowMessage(null, new List<Message>() { new Message(string.Format("线程{0}已经停止...", thread.ManagedThreadId)) });
                if (ThreadClosed != null)
                {
                    ThreadClosed(null, null);
                }
            };
        }

        private static bool HasTask(string num)
        {
            try
            {
                var i = Convert.ToInt32(num);
                return true;
            }
            catch (Exception)
            {
                if (num == "有")
                {
                    return true;
                }
            }

            return false;
        }

        private static bool GetTask(Train train, string date, SeatsType seat, List<Contact> selectContactList)
        {
            ShowMessage(null, new List<Message>() { new Message("开始抢票...",Color.Yellow) });
            var v = _12306Class.GetTask(train, date, seat,selectContactList);

            if (v != null && !v.IsCreate)
            {
               // MessageBox.Show(Program.mainForm, v.Message);
                ShowMessage(null,new List<Message>() { new Message("抢票失败..."+v.Message,Color.Red) });
            }
            else
            {
                ShowMessage(null, new List<Message>() { new Message("抢票  ...", Color.Yellow), new Message(v.Message, Color.Red) });
            }
            return v.IsCreate;
        }

        private static void HasTask(TrainSeat train, string data, Seat seat)
        {
            if (ShowMessage != null)
            {
                ShowMessage(null, new List<Message>() { new Message(string.Format("{0},{1},{2}有票", train.ToString(), data, seat.Name), Color.Yellow) });

                MessageBox.Show(Program.mainForm, "有票啦。。。。" + seat.Name);


                ThreadPool.QueueUserWorkItem((m) =>
                {
                    SpeechSynthesizer synthesizer = new SpeechSynthesizer();

                    PromptBuilder promptBuilder = new PromptBuilder();

                    promptBuilder.AppendText("买到票啦，我能回家啦！");

                    synthesizer.SpeakAsync(promptBuilder);

                });
            }
        }

    }

    public class Message
    {
        private Color _ForeColor;
        public Color ForeColor
        {
            get { return _ForeColor; }
        }

        private string _Msg;
        public string Msg
        {
            get { return _Msg; }
        }

        public Message(string message)
            : this(message, Color.White)
        {
        }

        public Message(string message, Color color)
        {
            _Msg = message;
            _ForeColor = color;
        }
    }
}
