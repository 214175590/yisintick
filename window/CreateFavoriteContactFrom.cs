using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace YisinTick
{
    public partial class CreateFavoriteContactFrom : Form
    {
        public CreateFavoriteContactFrom()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string name = textBox1.Text.Trim();
            string sexCode = "M";
            string bornDate = dateTimePicker1.Value.ToString("yyyy-MM-dd");
            string cardType = "2";
            string cardNo = textBox4.Text.Trim();
            string mobileNo = textBox5.Text.Trim();

            switch (comboBox1.SelectedItem as string)
            {
                case "男":
                    sexCode = "M";
                    break;
                case "女":
                    sexCode = "F";
                    break;
                default:
                    MessageBox.Show("请选择性别");
                    return;
            }

            switch (comboBox2.SelectedItem as string)
            {
                case "二代身份证":
                    cardType = "2";
                    break;
                case "港澳通行证":
                    cardType = "C";
                    break;
                case "台湾通行证":
                    cardType = "G";
                    break;
                case "护照":
                    cardType = "B";
                    break;
                default:
                    MessageBox.Show("请选择证件类型");
                    return;
            }

            var v = _12306Class.CreateCreateFavoriteContact(name, sexCode, bornDate, cardType, cardNo, mobileNo);

            if (v.IsCreated)
            {
                MessageBox.Show("添加成功");
                this.Close();
            }
            else
            {
                MessageBox.Show(v.Message);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
            textBox4.Text = "";
            textBox5.Text = "";
        }
    }
}
