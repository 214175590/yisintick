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
    public partial class FrmDuiHuan : Form
    {
        public FrmDuiHuan()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string code = textBox1.Text;
            if (String.IsNullOrEmpty(code))
            {
                label1.Text = "请输入兑换码！";
            }
            else
            {
                // 先判断是否存在
                List<String> list = TickCute.readHistoryStr();
                String key = "", codeKey = "";// MyEncrypt.EncryptA(code);
                int count = 0;
                for(int i = 0; i < list.Count; i++){
                    key = list[i];
                    codeKey = MyEncrypt.DecryptB(key);
                    if (code.Equals(codeKey))
                    {
                        count++;
                    }
                }
                if (count == 0){
                    count = TickCute.GetUseTick(Form1.MachineCode, code, true);
                    if (count <= 0 && count > 50)
                    {
                        label1.Text = "您输入的兑换码无效！";
                    }
                    else
                    {
                        TickCute.ChangeTickCount(count);
                        TickCute.WriteHistoryToFile(code);
                        this.Dispose();
                    }
                }
                else
                {
                    label1.Text = "您输入的兑换码无效！";
                }                
            }
        }

        private void FrmDuiHuan_Load(object sender, EventArgs e)
        {
            textBox1.TextChanged += textBox1_TextChanged;
        }

        void textBox1_TextChanged(object sender, EventArgs e)
        {
            label1.Text = "";
        }
    }
}
