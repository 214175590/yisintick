using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections;

namespace YisinTick
{
    public partial class GetTrainsFrom : Form
    {
        private String fromText = "";
        private String toText = "";
        private String fromTextStr = null;
        private String toTextStr = null;
        public GetTrainsFrom()
        {
            InitializeComponent();
            Control.CheckForIllegalCrossThreadCalls = false;
        }

        private void GetTrainsFrom_Load(object sender, EventArgs e)
        {
            //webBrowser1.Url = new Uri("https://kyfw.12306.cn/otn/leftTicket/init");
            System.Diagnostics.Process.Start("https://kyfw.12306.cn/otn/leftTicket/init");

            cb_from.Text = ConfigStore.fromCity;
            cb_to.Text = ConfigStore.toCity;
            dateTimePicker1.Value = ConfigStore.tempDate;
            dateTimePicker1.Focus();
            cb_from.MouseClick += cb_from_MouseClick;
            cb_from.KeyUp += cb_from_KeyUp;
            cb_to.MouseClick += cb_to_MouseClick;
            cb_to.KeyUp += cb_to_KeyUp;
            timer1.Interval = 500;
            timer1.Tick += timer1_Tick;
            timer2.Interval = 500;
            timer2.Tick += timer2_Tick;
        }

        void timer1_Tick(object sender, EventArgs e)
        {
            ShowFromToList(toTextStr, 1);
            timer1.Stop();
        }       

        void cb_to_KeyUp(object sender, KeyEventArgs e)
        {
            timer1.Stop();
            if (!String.IsNullOrEmpty(cb_to.Text) && !toText.Equals(cb_to.Text))
            {
                toTextStr = cb_to.Text;
                timer1.Start();
            }
            else if (String.IsNullOrEmpty(cb_to.Text))
            {
                toTextStr = null;
                timer1.Start();            
            }
        }

        void cb_to_MouseClick(object sender, MouseEventArgs e)
        {
            timer1.Stop();
            if (String.IsNullOrEmpty(cb_to.Text))
            {
                toTextStr = null;
                timer1.Start(); 
            }
            else
            {
                toTextStr = cb_to.Text;
                timer1.Start();
            }
        }

        void timer2_Tick(object sender, EventArgs e)
        {
            ShowFromToList(fromTextStr, 2);
            timer2.Stop();
        }

        void cb_from_KeyUp(object sender, KeyEventArgs e)
        {
            timer2.Stop();
            if (!String.IsNullOrEmpty(cb_from.Text) && !fromText.Equals(cb_from.Text))
            {
                fromTextStr = cb_from.Text;
                timer2.Start(); 
            }
            else if(String.IsNullOrEmpty(cb_from.Text))
            {
                fromTextStr = null;
                timer2.Start(); 
            }
        }

        void cb_from_MouseClick(object sender, MouseEventArgs e)
        {
            timer2.Stop();
            if (String.IsNullOrEmpty(cb_from.Text))
            {
                fromTextStr = null;
                timer2.Start();
            }
            else
            {
                fromTextStr = cb_from.Text;
                timer2.Start();
            }
        }

        public void ShowFromToList(String text, int type)
        {
            ArrayList list = null;
            String[] staArray = null;
            if (text != null)
            {
                list = StationList.searchStation(text);
                staArray = new String[list.Count];
            }
            else
            {
                list = StationList.GetVaterStationList();
                staArray = new String[list.Count];                
            }
            int i = 0;
            foreach (StationEntity se in list)
            {
                staArray[i] = se.name2;
                i++;
            }
            if (type == 1)
            {
                this.cb_to.AutoCompleteCustomSource.AddRange(staArray);
            }
            else if (type == 2)
            {
                this.cb_from.AutoCompleteCustomSource.AddRange(staArray);
            }
            //this.cb_from.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            //this.cb_from.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
        }



        private void button1_Click(object sender, EventArgs e)
        {
            button3.Enabled = false;
            button2.Enabled = false;
            pictureBox1.Show();
            System.Threading.ThreadPool.QueueUserWorkItem((m) =>
            {
                checkedListBox1.Items.Clear();

                var startStation = Stations.List.FirstOrDefault(fun => fun.Name == cb_from.Text.Trim());
                if (startStation == null)
                {
                    MessageBox.Show(string.Format("找不到站点：{0}，请重新输入！", cb_from.Text.Trim()));
                    return;
                }

                var endStation = Stations.List.FirstOrDefault(fun => fun.Name == cb_to.Text.Trim());

                if (endStation == null)
                {
                    MessageBox.Show(string.Format("找不到站点：{0}，请重新输入！", cb_to.Text.Trim()));

                    return;
                }

                list = _12306Class.GetTrains(dateTimePicker1.Value.ToString("yyyy-MM-dd"), startStation.Code, endStation.Code);
                pictureBox1.Hide();
                if (list == null)
                {
                    MessageBox.Show("网络异常，请稍后重试");
                    return;
                }

                if (list.Count <= 0)
                {
                    MessageBox.Show(string.Format("找不到车次从{0}开往{1}的车次", cb_from.Text, cb_to.Text));
                    return;
                }
                _12306Class.From = startStation;
                _12306Class.To = endStation;
                foreach (var train in list)
                {
                    checkedListBox1.Items.Add(train.ToString());
                }
                ConfigStore.fromCity = startStation.Name;
                ConfigStore.toCity = endStation.Name;
                ConfigStore.tempDate = dateTimePicker1.Value;
                button2.Enabled = true;
                button3.Enabled = true;
            });
        }

        private List<Train> list;

        private void button2_Click(object sender, EventArgs e)
        {
            List<Train> addList = new List<Train>();
            foreach (var index in checkedListBox1.CheckedIndices)
            {
                addList.Add(list[(int)index]);
            }
            Trains.AddTrains(addList);
            TranTime.Clear();
            TranTime.AddTime(dateTimePicker1.Value.ToString("yyyy-MM-dd"));
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }


    }
}
