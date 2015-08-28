using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Security.Cryptography;

namespace YisinTick
{
    public partial class GenretorMa : Form
    {
        private static String sap = "9eeabf09e8e92a6462883b0f4574d1c5";
        public GenretorMa()
        {
            InitializeComponent();
        }

        private void GenretorMa_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            String pwd = pwd_text.Text;            
            String jqm = jiqima.Text;
            String zh = zh_text.Text;
            pwd = MyEncrypt.GetMd5_16(pwd, true);
            pwd = MyEncrypt.GetMD5(pwd);
            if (pwd.Equals(sap))
            {
                errorInfo.ForeColor = Color.Green;
                errorInfo.Text = "生成成功！";
                String text = TickCute.CreateKey(jqm, zh);
                textBox1.Text = text;
                Clipboard.SetData(DataFormats.Text, text);
            }
            else
            {
                errorInfo.ForeColor = Color.Red;
                errorInfo.Text = "错误！";
            }
        }
    }
}
