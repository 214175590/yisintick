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
    public partial class TranTimeForm : Form
    {
        public TranTimeForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            TranTime.AddTime(dateTimePicker1.Value.ToString("yyyy-MM-dd"));
            this.Dispose();
        }

        private void TranTimeForm_Load(object sender, EventArgs e)
        {
            dateTimePicker1.Value = ConfigStore.tempDate;
        }
    }
}
