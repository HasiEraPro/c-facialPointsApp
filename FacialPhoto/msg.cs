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
       

     

        public msg()
        {
            InitializeComponent();
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {

            if (assym == "")
            {

                btnPrint.Enabled = false;
                lblError.Text = "Please Answer all questions!!!!";

            }
            else
            {
                Form1.answerAssym = msg.assym;
                msg2 f2 = new msg2();
                f2.ShowDialog();
                    this.Hide();

            }






        }

        private void cboxDOA_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!btnPrint.Enabled) btnPrint.Enabled = true;
            
            switch (cboxDOA.SelectedIndex)
            {
                case 0:assym = "None"; break;
                case 1: assym = "Average";break;
                case 2: assym = "Significant"; break;

                default:break;
            }



        }

       

        private void msg_Load(object sender, EventArgs e)
        {
            cboxDOA.SelectedText = "--select--";
            

        }
    }
}
