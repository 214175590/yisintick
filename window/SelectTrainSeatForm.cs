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
    public partial class SelectTrainSeatForm : Form
    {
        public SelectTrainSeatForm()
        {
            InitializeComponent();

            comboBox1.Items.Clear();

            foreach (int myCode in Enum.GetValues(typeof(SeatsType)))
            {
                comboBox1.Items.Add((SeatsType)myCode);
            }


        }

        private void button1_Click(object sender, EventArgs e)
        {

            if (comboBox1.SelectedItem != null)
            {
                Seat.AddSelectSeats((SeatsType)comboBox1.SelectedItem);
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
