using System;
using System.Windows.Forms;

namespace LogoExt
{
    public partial class GtipForm : Form
    {
        /*******************************************************************
        //Singleton example
           private static readonly GtipForm instance = new GtipForm();

             static GtipForm()
             {
             }

             private GtipForm()
             {
                 InitializeComponent();
                 this.Load += Gtip_Load;
             }

             public static GtipForm Instance
             {
                 get { return instance; }
             }
     **********************************************************************/

        public GtipForm()
        {
            InitializeComponent();
            this.Load += Gtip_Load;
        }

        private void Gtip_Load(object sender, EventArgs e)
        {
            dateTimePicker1.Value = DateTime.Now.AddDays(-15);
            dateTimePicker2.Value = DateTime.Now;
            pictureBox1.Hide();
            checkImage = Properties.Resources.check;
            errorImage = Properties.Resources.error;

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Global.Instance.query.QueryGTIB(this);
        }
    }
}
