using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LogoExt
{
    public partial class ItemsForm : Form
    {
        public ItemsForm()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            this.ActiveControl = textBox1;                                                  //focus will be on textBox1 on start
            dataGridView1.CellMouseUp += OnCellMouseUp;
            textBox1.KeyPress += new KeyPressEventHandler(TextBox1_KeyPress);
            dataGridView1.DataBindingComplete += dataGridView1_DataBindingComplete;
            dataGridView2.DataBindingComplete += dataGridView2_DataBindingComplete;

            dataGridView1.DoubleBuffered(true);
            dataGridView2.DoubleBuffered(true);
            int year = Int32.Parse(DateTime.Now.ToString("yy"));
            for (; year > 2; year--) {
                if (year< 10) {
                    listBox1.Items.Add("200" + year);
                }
                else {
                    listBox1.Items.Add("20" + year);
                }
            }
            listBox1.SetSelected(0, true);
            QueryItems();

            dataGridView1.DefaultCellStyle.Font = new Font((string)Global.Instance.settings.FontFamily, (float)Global.Instance.settings.TextSize);
            dataGridView2.DefaultCellStyle.Font = new Font((string)Global.Instance.settings.FontFamily, (float)Global.Instance.settings.TextSize);
            dataGridView1.Width = 2 + dataGridView1.Columns["Kodu"].Width + dataGridView1.Columns["Açıklaması"].Width + dataGridView1.Columns["Stok"].Width + SystemInformation.VerticalScrollBarWidth;
            dataGridView2.Location = new Point(dataGridView1.Location.X + dataGridView1.Width + 5, dataGridView1.Location.Y);           //dgv2'yi dgv1'e göre konumlandır
            dataGridView2.Width = this.Width - (dataGridView1.Width + 35);                                                              //dgv2'nin genişliğini belirle. üst satırla birlikte
        }

        //Focus textBox1 when form is shown
        private void ItemsFormShown(object sender, EventArgs e)
        {
            this.ActiveControl = textBox1;
        }

        //Focus textBox1 when form is shown
        private void ItemsFormResize(object sender, EventArgs e)
        {

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

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count != 0) {
                QueryItemMovements();
            }
        }

        private void QueryItemMovements()
        {
            if (dataGridView1.SelectedCells.Count == 0) { return; }
            DataTable dt = Global.Instance.query.QueryItemMovements(this, dataGridView1.SelectedCells[0].Value.ToString());
            if (dt == null) { return; }     //we probably got an exception from query. Do nothing.
            AddNewCollumns(dt);
            TRCodeConverter(dt);
            dataGridView2.DataSource = new BindingSource(dt, null);
        }

        //Fills the "dataGridView1" with Items Stock
        private void QueryItems()
        {
            DataTable dt = Global.Instance.query.QueryItems();
            if (dt == null) { return; }     //we probably got an exception from query. Do nothing.            
            dataGridView1.DataSource = new BindingSource(dt, null);
        }

        private void AddNewCollumns(DataTable dt)
        {
            dt.Columns.Add("Giriş Miktar");
            dt.Columns.Add("Giriş Birim Fiyat");
            dt.Columns.Add("Giriş Tutar");
            dt.Columns.Add("Çıkış Miktar");
            dt.Columns.Add("Çıkış Birim Fiyat");
            dt.Columns.Add("Çıkış Tutar");
        }

        private void TRCodeConverter(DataTable dt)
        {
            foreach (DataRow dr in dt.Rows) {
                switch (dr["Tür"].ToString()) {
                    case "1":
                        AssingPositiveCollumns(dr);
                        dr["Tür"] = "Satınalma İrsaliyesi";
                        break;
                    case "2":
                        AssingPositiveCollumns(dr);
                        dr["Tür"] = "Perakende Şatış İade İrsaliyesi";                        
                        break;
                    case "3":
                        AssingPositiveCollumns(dr);
                        dr["Tür"] = "Toptan Şatış İade İrsaliyesi";
                        break;
                    case "4":
                        AssingPositiveCollumns(dr);
                        dr["Tür"] = "Konsinye Çıkış İade İrsaliyesi";
                        break;
                    case "5":
                        AssingPositiveCollumns(dr);
                        dr["Tür"] = "Konsinye Giriş İrsaliyesi";
                        break;
                    case "6":
                        AssingNegativeCollumns(dr);
                        dr["Tür"] = "Alım İade İrsaliyesi";
                        break;
                    case "7":
                        AssingNegativeCollumns(dr);
                        dr["Tür"] = "Perakende Şatış İrsaliyesi";
                        break;
                    case "8":
                        AssingNegativeCollumns(dr);
                        dr["Tür"] = "Toptan Şatış İrsaliyesi";
                        break;
                    case "9":
                        AssingNegativeCollumns(dr);
                        dr["Tür"] = "Konsinye Çıkış İrsaliyesi";
                        break;
                    case "10":
                        AssingNegativeCollumns(dr);
                        dr["Tür"] = "Konsinye Giriş İade İrsaliyesi";
                        break;
                    case "11":
                        AssingNegativeCollumns(dr);
                        dr["Tür"] = "Fire Fişi";
                        break;
                    case "12":
                        AssingNegativeCollumns(dr);
                        dr["Tür"] = "Sarf Fişi";
                        break;
                    case "13":
                        AssingPositiveCollumns(dr);
                        dr["Tür"] = "Üretimden Giriş Fişi";                        
                        break;
                    case "14":
                        AssingPositiveCollumns(dr);
                        dr["Tür"] = "Devir Fişi";
                        break;
                    case "25":
                        AssingPositiveCollumns(dr);
                        dr["Tür"] = "Ambar Fişi";
                        break;
                    case "26":
                        AssingNegativeCollumns(dr);
                        dr["Tür"] = "Mustahsil İrsaliyesi";
                        break;
                    case "50":
                        AssingPositiveCollumns(dr);
                        dr["Tür"] = "Sayım Fazlası Fişi";
                        break;
                    case "51":
                        AssingNegativeCollumns(dr);
                        dr["Tür"] = "Sayım Eksiği Fişi";
                        break;
                    default:
                        dr["Tür"] = "********HATALI********";
                        break;
                }
            }
            dt.Columns.Remove("Miktar");
            dt.Columns.Remove("Birim Fiyat");
            dt.Columns.Remove("Tutar");            
        }

        private void AssingPositiveCollumns(DataRow dr)
        {
            dr["Giriş Miktar"] = dr["Miktar"];
            dr["Giriş Birim Fiyat"] = Math.Round((double)dr["Birim Fiyat"], 2).ToString("#,###0.00");
            dr["Giriş Tutar"] = Math.Round((double)dr["Tutar"], 2).ToString("#,###0.00");
        }

        private void AssingNegativeCollumns(DataRow dr)
        {
            dr["Çıkış Miktar"] = dr["Miktar"];
            dr["Çıkış Birim Fiyat"] = Math.Round((double)dr["Birim Fiyat"], 2).ToString("#,###0.00");
            dr["Çıkış Tutar"] = Math.Round((double)dr["Tutar"], 2).ToString("#,###0.00");
        }

        private void dataGridView1_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
             DataGridLineColoring(dataGridView1);
        }

        private void dataGridView2_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            DataGridLineColoring(dataGridView2);
            dataGridView2.Columns["Giriş Miktar"].DisplayIndex = 5;
            dataGridView2.Columns["Giriş Birim Fiyat"].DisplayIndex = 6;
            dataGridView2.Columns["Giriş Tutar"].DisplayIndex = 7;
            dataGridView2.Columns["Çıkış Miktar"].DisplayIndex = 8;
            dataGridView2.Columns["Çıkış Birim Fiyat"].DisplayIndex = 9;
            dataGridView2.Columns["Çıkış Tutar"].DisplayIndex = 10;


            dataGridView2.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            dataGridView2.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            dataGridView2.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            dataGridView2.Columns[4].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            dataGridView2.Columns[5].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            dataGridView2.Columns[6].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            dataGridView2.Columns[7].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            dataGridView2.Columns[8].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            dataGridView2.Columns[9].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            dataGridView2.Columns[10].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            dataGridView2.Columns[3].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dataGridView2.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView2.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView2.Columns[7].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView2.Columns[8].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView2.Columns[9].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView2.Columns[10].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            dataGridView2.Columns["Giriş Miktar"].HeaderCell.Style.BackColor = Color.GreenYellow;
            dataGridView2.Columns["Giriş Birim Fiyat"].HeaderCell.Style.BackColor = Color.GreenYellow;
            dataGridView2.Columns["Giriş Tutar"].HeaderCell.Style.BackColor = Color.GreenYellow;
            dataGridView2.Columns["Çıkış Miktar"].HeaderCell.Style.BackColor = Color.PaleVioletRed;
            dataGridView2.Columns["Çıkış Birim Fiyat"].HeaderCell.Style.BackColor = Color.PaleVioletRed;
            dataGridView2.Columns["Çıkış Tutar"].HeaderCell.Style.BackColor = Color.PaleVioletRed;
            dataGridView2.EnableHeadersVisualStyles = false;
        }

        private void OnCellMouseUp(object sender, MouseEventArgs e)
        {
            QueryItemMovements();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (textBox1.TextLength > 1) {
                Thread.CurrentThread.CurrentCulture = new CultureInfo("tr-TR");
                string upper = textBox1.Text.ToUpper();

                foreach (DataGridViewRow row in dataGridView1.Rows) {
                    string temp = row.Cells[0].Value.ToString();
                    if (temp.ToUpper().Contains(upper)) {
                        row.Selected = true;
                        dataGridView1.FirstDisplayedScrollingRowIndex = row.Index;
                        break;
                    }
                }
            }            
        }

        private void TextBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13) {
                if (dataGridView1.SelectedRows.Count == 1) {
                    QueryItemMovements();
                }
            }
        }

        //color the table's odd and even lines
        private void DataGridLineColoring(DataGridView dt)
        {
            foreach (DataGridViewRow row in dt.Rows) {
                if (row.Index % 2 == 0) {
                    row.DefaultCellStyle.BackColor = Color.Beige;
                }
                else {
                    row.DefaultCellStyle.BackColor = Color.Azure;
                }
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Enter) {
                if (dataGridView1.SelectedRows.Count == 1) {
                    QueryItemMovements();
                    return true;
                }
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

    }
}
