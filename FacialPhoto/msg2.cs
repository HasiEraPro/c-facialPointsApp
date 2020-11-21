using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FacialPhoto
{
    public partial class msg2 : Form
    {
        public static string support = "";

        public msg2()
        {
            InitializeComponent();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            if (support == "")
            {

                lblError.Text = "Please input a value between 1 to 5";



            }
            else
            {

                Form1.answerSupport = support;
                this.Hide();


            }



        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            support = textBox1.Text;

        }
    }
}
