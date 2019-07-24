using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace LogoExt
{
    public partial class InvoiceForm : Form
    {
        public InvoiceForm()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            panel1.Height = 120;
            panel2.Height = (int)((this.Height - 120) * 0.60);
            panel3.Height = (int)((this.Height - 120) * 0.40);
            dataGridView1.DataBindingComplete += dataGridView1_DataBindingComplete;
            dataGridView2.DataBindingComplete += dataGridView2_DataBindingComplete;
            dataGridView1.DoubleBuffered(true);
            dataGridView2.DoubleBuffered(true);
            dataGridView1.DefaultCellStyle.Font = new Font((string)Global.Instance.settings.FontFamily, (float)Global.Instance.settings.TextSize);
            dataGridView2.DefaultCellStyle.Font = new Font((string)Global.Instance.settings.FontFamily, (float)Global.Instance.settings.TextSize);

            int year = Int32.Parse(DateTime.Now.ToString("yy"));
            for (; year > 2; year--) {
                if (year < 10) {
                    listBox1.Items.Add("200" + year);
                }
                else {
                    listBox1.Items.Add("20" + year);
                }
            }            
            listBox1.SetSelected(0, true);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            panel1.Height = 85;
            panel2.Height = (int)((this.Height - panel1.Height) * 0.60);
            panel3.Height = (int)((this.Height - panel1.Height) * 0.40);
        }

        public string GetFirmCode(int pastYears)
        {
            int year = Int32.Parse(listBox1.SelectedItem.ToString().Substring(2, 2)) - 2 - pastYears;
            if (year >= 10) {
                return year.ToString();
            }
            return "0" + year;
        }

        public int GetFirmCodeInt(int pastYears)
        {
            return Int32.Parse(listBox1.SelectedItem.ToString().Substring(2, 2)) - 2 - pastYears;
        }

        private void dataGridView1_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            dataGridView1.EvenRowColoring(Color.Azure);
            dataGridView1.FirstDisplayedScrollingRowIndex = dataGridView1.RowCount - 1;
            dataGridView1.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            dataGridView1.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            dataGridView1.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            dataGridView1.Columns[3].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dataGridView1.Columns[4].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dataGridView1.Columns[5].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dataGridView1.Columns[6].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            dataGridView1.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView1.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;       
        }

        private void dataGridView2_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            dataGridView2.EvenRowColoring(Color.Azure);
            dataGridView2.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
        }

        private void dataGridView1_CellMouseUp(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 1) {
                DataTable dt = Global.Instance.query.QueryInvoiceDetails(this, dataGridView1.SelectedCells[1].Value.ToString());
                if (dt == null) { return; }     //we probably got an exception from query. Do nothing.
                dataGridView2.DataSource = new BindingSource(dt, null);
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            QueryInvoices();
        }
        
        private void QueryInvoices()
        {
            DataTable dt = Global.Instance.query.QueryInvoices(this);
            if (dt == null) { return; }     //we probably got an exception from query. Do nothing.
            TRCodeConverter(dt);
            dataGridView1.DataSource = new BindingSource(dt, null);
        }

        private void TRCodeConverter(DataTable dt)
        {
            foreach (DataRow dr in dt.Rows) {
                switch (dr["Türü"].ToString()) {
                    case "2":
                        dr["Türü"] = "Perakende Şatış İade Faturası";
                        break;
                    case "3":
                        dr["Türü"] = "Toptan Şatış İade Faturası";
                        break;
                    case "7":
                        dr["Türü"] = "Perakende Şatış Faturası";
                        break;
                    case "8":
                        dr["Türü"] = "Toptan Şatış Faturası";
                        break;
                    case "9":
                        dr["Türü"] = "Verilen Hizmet Faturası";
                        break;
                    case "10":
                        dr["Türü"] = "Verilen Proforma Fatura";
                        break;
                    case "14":
                        dr["Türü"] = "Şatış Fiyat Farkı Faturası";
                        break;
                    default:
                        dr["Türü"] = "********HATALI********";
                        break;
                }
            }

        }


    }
}
