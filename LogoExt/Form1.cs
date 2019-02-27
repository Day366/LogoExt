using System;
using System.Drawing;
using System.Windows.Forms;

namespace LogoExt
{
    public partial class Form1 : Form
    {
        /*******************************************************************
        //Singleton example
           private static readonly Form1 instance = new Form1();

             static Form1()
             {
             }

             private Form1()
             {
                 InitializeComponent();
                 this.Load += Form1_Load;
             }

             public static Form1 Instance
             {
                 get { return instance; }
             }
     **********************************************************************/

        public Form1()
        {
            InitializeComponent();
            this.Load += Form1_Load;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            dateTimePicker1.Value = DateTime.Now.AddDays(-15);
            dateTimePicker2.Value = DateTime.Now;
            pictureBox1.Hide();
            checkImage = LogoExt.Properties.Resources.check;
            errorImage = LogoExt.Properties.Resources.error;

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Global.Instance.query.QueryGTIB(this);
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void Form1_Load_1(object sender, EventArgs e)
        {

        }

        private void dateTimePicker2_ValueChanged(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
