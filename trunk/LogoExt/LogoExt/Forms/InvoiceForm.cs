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
            dataGridView1.DataBindingComplete += dataGridView1_DataBindingComplete;
            dataGridView2.DataBindingComplete += dataGridView2_DataBindingComplete;

            dataGridView1.DoubleBuffered(true);
            dataGridView2.DoubleBuffered(true);

            int year = Int32.Parse(DateTime.Now.ToString("yy"));
            for (; year > 2; year--) {
                if (year < 10) {
                    listBox1.Items.Add("200" + year);
                }
                else {
                    listBox1.Items.Add("20" + year);
                }
            }

            dataGridView1.DefaultCellStyle.Font = new Font((string)Global.Instance.settings.FontFamily, (float)Global.Instance.settings.TextSize);
            dataGridView2.DefaultCellStyle.Font = new Font((string)Global.Instance.settings.FontFamily, (float)Global.Instance.settings.TextSize);

            dataGridView1.Width = dataGridView2.Width = this.Width - Global.Instance.settings.SplitterDistance - SystemInformation.VerticalScrollBarWidth;
            dataGridView1.Height = (int)(this.Height * 0.5);
            dataGridView2.Location = new Point(dataGridView1.Location.X, dataGridView1.Location.Y + dataGridView1.Height + 5);
            dataGridView2.Height = this.Height - (dataGridView1.Location.Y + dataGridView1.Height + 30);
            
            listBox1.SetSelected(0, true);
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

        private void QueryInvoices()
        {
            DataTable dt = Global.Instance.query.QueryInvoices(this);
            if (dt == null) { return; }     //we probably got an exception from query. Do nothing.
            TRCodeConverter(dt);
            dataGridView1.DataSource = new BindingSource(dt, null);
        }

        private void dataGridView1_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            DataGridLineColoring(dataGridView1);
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
            DataGridLineColoring(dataGridView2);
        }

        //color the table's odd and even lines
        private void DataGridLineColoring(DataGridView dt)
        {
            foreach (DataGridViewRow row in dt.Rows) {
                if (row.Index % 2 == 0) {
                    row.DefaultCellStyle.BackColor = Color.Azure;
                }
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            QueryInvoices();
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
