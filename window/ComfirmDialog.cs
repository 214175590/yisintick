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
    public partial class ComfirmDialog : Form
    {
        public static bool isOk = false;

        public ComfirmDialog()
        {
            InitializeComponent();
        }

        private void ComfirmDialog_Load(object sender, EventArgs e)
        {
            isOk = false;
            this.FormClosing += ComfirmDialog_FormClosing;
        }

        void ComfirmDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            label1.Text = "";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            isOk = true;
            label1.Text = "";
            this.Dispose();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            isOk = false;
            label1.Text = "";
            this.Dispose();
        }

        public void SetMessage(String msg, String title)
        {
            label1.Text = msg;
            this.Text = title;
        }
    }
}
