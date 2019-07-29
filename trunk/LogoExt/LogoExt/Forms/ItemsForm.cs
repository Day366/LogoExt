using System;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Threading;
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
            dataGridView1.DataBindingComplete += dataGridView1_DataBindingComplete;
            dataGridView2.DataBindingComplete += dataGridView2_DataBindingComplete;
            dataGridView1.DoubleBuffered(true);
            dataGridView2.DoubleBuffered(true);

            dataGridView1.ColumnHeadersDefaultCellStyle.Font = dataGridView1.DefaultCellStyle.Font = new Font((string)Global.Instance.settings.FontFamily, (float)Global.Instance.settings.TextSize);
            dataGridView2.ColumnHeadersDefaultCellStyle.Font = dataGridView2.DefaultCellStyle.Font = new Font((string)Global.Instance.settings.FontFamily, (float)Global.Instance.settings.TextSize);

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

            panel2.Width = 13 + dataGridView1.Columns["Kodu"].Width + dataGridView1.Columns["Açıklaması"].Width + dataGridView1.Columns["Stok"].Width + SystemInformation.VerticalScrollBarWidth;
          //  dataGridView2.Location = new Point(dataGridView1.Location.X + dataGridView1.Width + 5, dataGridView1.Location.Y);           //dgv2'yi dgv1'e göre konumlandır
          //  dataGridView2.Width = this.Width - (dataGridView1.Width + 35);                                                              //dgv2'nin genişliğini belirle. üst satırla birlikte
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            panel1.Height = 85;
            ResizePanels();
        }

        //Focus textBox1 when form is shown
        private void ItemsFormShown(object sender, EventArgs e)
        {
            this.ActiveControl = textBox1;
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
            if (dataGridView1.SelectedRows.Count == 1) {
                QueryItemMovements();
            }
        }

        private void dataGridView1_CellMouseUp(object sender, DataGridViewCellMouseEventArgs e)
        {
            QueryItemMovements();
        }

        //dataGridView'da bir satır seçili ise ItemMovementsı getir. 
        private void dataGridView1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) {
                if (dataGridView1.SelectedRows.Count == 1) {
                    QueryItemMovements();
                    e.Handled = e.SuppressKeyPress = true;          //Enter'a altsatıra inmesini engellemek için şart
                }
            }
        }

        private void dataGridView1_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            dataGridView1.EvenRowColoring(Color.Azure);
        }

        private void dataGridView2_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            dataGridView2.EvenRowColoring(Color.Azure);
            dataGridView2.Columns["Giriş Miktar"].DisplayIndex = 5;
            dataGridView2.Columns["Giriş Birim Fiyat"].DisplayIndex = 6;
            dataGridView2.Columns["Giriş Tutar"].DisplayIndex = 7;
            dataGridView2.Columns["Çıkış Miktar"].DisplayIndex = 8;
            dataGridView2.Columns["Çıkış Birim Fiyat"].DisplayIndex = 9;
            dataGridView2.Columns["Çıkış Tutar"].DisplayIndex = 10;

            //dataGridView2.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            dataGridView2.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            //dataGridView2.Columns[5].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            //dataGridView2.Columns[6].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            //dataGridView2.Columns[7].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            //dataGridView2.Columns[8].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            //dataGridView2.Columns[9].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            //dataGridView2.Columns[10].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;

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

            //color the bottom row if there is a sum row at the end
            if (dataGridView2.Rows.Count > 1) {
                dataGridView2.Rows[dataGridView2.Rows.Count - 1].DefaultCellStyle.BackColor = Color.LightSteelBlue;
            }

            if (dataGridView2.Rows.Count > 10) {
                dataGridView2.FirstDisplayedScrollingRowIndex = dataGridView2.Rows.Count - 1;
            }
        }

        private void dataGridView2_MouseUp(object sender, MouseEventArgs e)
        {
            int columnIndex = 0;
            decimal rowTotal = 0;
            decimal result = 0;
            for (int i = 0; i < dataGridView2.SelectedCells.Count; i++) {
                if (i == 0) {
                    columnIndex = dataGridView2.SelectedCells[i].ColumnIndex;
                }
                if (columnIndex == dataGridView2.SelectedCells[i].ColumnIndex) {
                    if (dataGridView2.SelectedCells[i].Value != null && dataGridView2.SelectedCells[i].Value.ToString() != "") {
                        if (dataGridView2.RowCount > 1 && dataGridView2.SelectedCells[i].RowIndex == dataGridView2.RowCount - 1) { continue; }
                        if (Decimal.TryParse(dataGridView2.SelectedCells[i].Value.ToString(), NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out result)) {
                            rowTotal += result;
                        }
                    }
                }
                else {
                    label1.Text = "Toplam: Farklı sütünları topladın!";
                    return;
                }
            }

            label1.Text = "Toplam: " + rowTotal.ToString("#,###0.00");
            label1.Visible = true;
        }

        //find the item in "dataGridView1" by searching from "textBox1"
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

        //when "Enter" Key is Pressed call Item Movements Query
        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) {
                if (dataGridView1.SelectedRows.Count == 1) {
                    QueryItemMovements();
                    e.Handled = e.SuppressKeyPress = true;          //Enter'a basınca windows sesini kesmek için şart
                }
            }
        }

        //Fills the "dataGridView1" with items stock
        private void QueryItems()
        {
            DataTable dt = Global.Instance.query.QueryItems();
            if (dt == null) { return; }     //we probably got an exception from query. Do nothing.            
            dataGridView1.DataSource = new BindingSource(dt, null);
        }

        //Fills the "dataGridView2" with item movements
        private void QueryItemMovements()
        {
            if (dataGridView1.SelectedCells.Count == 0) { return; }
            DataTable dt = Global.Instance.query.QueryItemMovements(this, dataGridView1.SelectedCells[0].Value.ToString());
            if (dt == null) { return; }     //we probably got an exception from query. Do nothing.
            AddNewCollumns(dt);
            TRCodeConverter(dt);
            SumCollumns(dt);                        
            dataGridView2.DataSource = new BindingSource(dt, null);
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
            dr["Giriş Miktar"] = Math.Round((double)dr["Miktar"], 2).ToString("#,###0.00", CultureInfo.CreateSpecificCulture("en-US"));
            dr["Giriş Birim Fiyat"] = Math.Round((double)dr["Birim Fiyat"], 2).ToString("#,###0.00", CultureInfo.CreateSpecificCulture("en-US"));
            dr["Giriş Tutar"] = Math.Round((double)dr["Tutar"], 2).ToString("#,###0.00", CultureInfo.CreateSpecificCulture("en-US"));
        }

        private void AssingNegativeCollumns(DataRow dr)
        {
            dr["Çıkış Miktar"] = Math.Round((double)dr["Miktar"], 2).ToString("#,###0.00", CultureInfo.CreateSpecificCulture("en-US"));
            dr["Çıkış Birim Fiyat"] = Math.Round((double)dr["Birim Fiyat"], 2).ToString("#,###0.00", CultureInfo.CreateSpecificCulture("en-US"));
            dr["Çıkış Tutar"] = Math.Round((double)dr["Tutar"], 2).ToString("#,###0.00", CultureInfo.CreateSpecificCulture("en-US"));
        }

        private void SumCollumns(DataTable dt) {
            if (dt.Rows.Count > 1) {
                decimal gm = 0;
                decimal gt = 0;
                decimal cm = 0;
                decimal ct = 0;
                decimal result = 0;
                string dash = "---";
                foreach (DataRow dr in dt.Rows) {
                    if (Decimal.TryParse(dr["Giriş Miktar"].ToString(), NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out result)) {
                        gm += result;
                    }
                    if (Decimal.TryParse(dr["Giriş Tutar"].ToString(), NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out result)) {
                        gt += result;
                    }
                    if (Decimal.TryParse(dr["Çıkış Miktar"].ToString(), NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out result)) {
                        cm += result;
                    }
                    if (Decimal.TryParse(dr["Çıkış Tutar"].ToString(), NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out result)) {
                        ct += result;
                    }
                }
                object[] rowArray = { dash, dash, dash, dash, "TOPLAM:", gm.ToString("#,###0.00"), dash, gt.ToString("#,###0.00"), cm.ToString("#,###0.00"), dash, ct.ToString("#,###0.00") };
                dt.Rows.Add(rowArray);
            }
        }

        private void ResizePanels()
        {
            panel3.Height = (int)((this.Height - panel1.Height) * 0.5);
        }
    }
}
