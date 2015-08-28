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
    public partial class AddLibaray : Form
    {
        public AddLibaray()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            String name = textBox1.Text;
            if(String.IsNullOrEmpty(name)){
                MessageBox.Show("请输入特征库名称！例如“12306验证码2014年版”");
            }
            else
            {
                Libaray lib = new Libaray();
                lib.id = ConfigStore.libarays.Count < 10 ? "lib_0" + ConfigStore.libarays.Count : "lib_" + ConfigStore.libarays.Count;
                lib.name = name;
                lib.status = 0;
                ConfigStore.libarays.Add(lib);
                CommonUtil.CreateDir(System.Environment.CurrentDirectory + "\\data\\" + lib.id + "\\");
                XmlUtils.CreateEmptyXml(System.Environment.CurrentDirectory + "\\data\\" + lib.id + "\\data.xml", "datas");

                this.Dispose();
            }
        }
    }
}
