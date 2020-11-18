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
    public partial class msg : Form
    {
        //public int distance { get; set; }
        //public double angle { get; set; }

        public static string assym = "";
        public static string support = "";



        public msg()
        {
            InitializeComponent();
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {

            if (assym == "" || support == "")
            {

                btnPrint.Enabled = false;
                lblError.Text = "Please Answer all questions!!!!";

            }
            else
            {
                Form1.answerAssym = msg.assym;
                Form1.answerSupport = msg.support;
                this.Hide();

            }


        }

        private void cboxDOA_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (assym != "" || support != "") btnPrint.Enabled = true;
            
            switch (cboxDOA.SelectedIndex)
            {
                case 0:assym = "None"; break;
                case 1: assym = "Average";break;
                case 2: assym = "Significant"; break;

                default:break;
            }



        }

        private void txtBoxSupport_TextChanged(object sender, EventArgs e)
        {
            msg.support = txtBoxSupport.Text;
            if (assym != "" || support != "") btnPrint.Enabled = true;
            
        }

        private void msg_Load(object sender, EventArgs e)
        {
            cboxDOA.SelectedText = "--select--";
        }
    }
}
