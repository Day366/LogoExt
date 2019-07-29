using System;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer;

namespace LogoExt
{
    public partial class EkstreForm : Form
    {
        private int vadeStartRow = -1;
        private int vadeFinishRow = -1;
        private Timer BackColorTimer = new Timer();
        private Timer BackColorTimer2 = new Timer();
        public EkstreForm()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            panel1.Height = 130;

            dataGridView1.DataBindingComplete += dataGridView1_DataBindingComplete;
            dataGridView2.DataBindingComplete += dataGridView2_DataBindingComplete;
            


            dataGridView1.DoubleBuffered(true);
            label4.BringToFront();
            label5.BringToFront();
            label6.BringToFront();
            textBox2.BringToFront();
            dataGridView1.ColumnHeadersDefaultCellStyle.Font = dataGridView1.DefaultCellStyle.Font = new Font((string)Global.Instance.settings.FontFamily, (float)Global.Instance.settings.TextSize);
            dataGridView2.ColumnHeadersDefaultCellStyle.Font = dataGridView2.DefaultCellStyle.Font = new Font((string)Global.Instance.settings.FontFamily, (float)Global.Instance.settings.TextSize);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            ResizePanels();
        }

        //Focus textBox1 when form is shown
        private void EkstreFormShown(object sender, EventArgs e)
        {
            this.ActiveControl = textBox1;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Length > 0) {
                textBox1.Clear();
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Ekstre(-1);
            DataTable dt = Global.Instance.query.QueryFirmDetails(listBox1.SelectedItem.ToString());
            if (dt == null) { return; }
            if (dt.Rows[0] != null) {
                label10.Text = dt.Rows[0][0].ToString();
                label10.Visible = true;
                label11.Text = dt.Rows[0][1].ToString() + " " + dt.Rows[0][2].ToString();
                label11.Visible = true;
                label7.Text = dt.Rows[0][3].ToString();
                label7.Visible = true;
            }
        }

        private void dataGridView1_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            dataGridView1.FirstDisplayedScrollingRowIndex = dataGridView1.RowCount - 1;
        }

        private void dataGridView2_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            ResizePanels();
            dataGridView2.EvenRowColoring(Color.Azure);
            //dataGridView2 açılınca tıklı olan hücrenin üstüne açabilir. tıklı olan hücreye kaydırıyor
            if (dataGridView1.SelectedCells.Count != 0 && !dataGridView1.SelectedCells[0].Displayed) {
                dataGridView1.FirstDisplayedScrollingRowIndex = dataGridView1.SelectedCells[0].RowIndex;
            }

            dataGridView2.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dataGridView2.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dataGridView2.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dataGridView2.Columns[3].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dataGridView2.Columns[4].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dataGridView2.Columns[5].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dataGridView2.Columns[6].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dataGridView2.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView2.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView2.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView2.Columns[10].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView2.Columns[11].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView2.Columns[12].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
        }

        private void dataGridView1_CellMouseUp(object sender, DataGridViewCellMouseEventArgs e)
        {
            QueryInvoiceDetails();
        }

        private void dataGridView1_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Enter) {
                if (dataGridView1.SelectedCells.Count == 1) {
                    QueryInvoiceDetails();
                    e.Handled = e.SuppressKeyPress = true;          //Enter'a altsatıra inmesini engellemek için şart
                }
            }
        }

        private void dataGridView1_MouseUp(object sender, MouseEventArgs e)
        {
            SumCellValues(dataGridView1);
        }

        private void dataGridView2_MouseUp(object sender, MouseEventArgs e)
        {
            SumCellValues(dataGridView2);
        }

        private void QueryInvoiceDetails()
        {
            if (dataGridView1.SelectedCells.Count == 1 && dataGridView1.SelectedCells[0].ColumnIndex == 2) {
                //if (dataGridView1.SelectedCells.Count == 1 ) {
                DateTime dateTime = DateTime.Parse(dataGridView1.Rows[dataGridView1.SelectedCells[0].RowIndex].Cells[0].Value.ToString());
                int year = Int32.Parse(dateTime.ToString("yy")) - 2;

                DataTable dt = Global.Instance.query.QueryInvoiceDetails(year.ToString(), dataGridView1.Rows[dataGridView1.SelectedCells[0].RowIndex].Cells[2].Value.ToString());

                if (dt == null) { return; }     //we probably got an exception from query. Do nothing.
                dataGridView2.DataSource = new BindingSource(dt, null);
            }
        }

        //When "Enter" Key is pressed and there is one item in listBox2 run query
        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) {
                if (listBox1.Items.Count > 0) {
                    listBox1.SelectedItem = listBox1.Items[0];
                    e.Handled = e.SuppressKeyPress = true;          //Enter'a basınca windows sesini kesmek için şart
                }
            }
        }

        //When "Enter" Key is pressed and there is one item in listBox2 run query
        private void textBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && listBox1.SelectedItem != null) {
                if (textBox2.Text.Length > 0 && Int32.TryParse(textBox2.Text, out int result)) {
                    if (result >= 0 && result <= 120) {
                        Ekstre(result);
                        e.Handled = e.SuppressKeyPress = true;      //Enter'a basınca windows sesini kesmek için şart
                    }
                }
            }
        }

        //When Label5 is clicked, copy the value and give a indication with backColor change
        private void label5_MouseDown(object sender, MouseEventArgs e)
        {
            label5.BackColor = Color.LightSkyBlue;
            Clipboard.SetText(label5.Text);
            BackColorTimer.Interval = 50;       //to make the fade transition more smoother change Interval from 100 to 50
            BackColorTimer.Tick += new EventHandler(TimerRemoveBackColorLabel5);
            BackColorTimer.Start();
        }

        //When Label5 is clicked, copy the value and give a indication with backColor change
        private void label10_MouseDown(object sender, MouseEventArgs e)
        {
            label10.BackColor = Color.LightSkyBlue;
            Clipboard.SetText(label10.Text);
            BackColorTimer2.Interval = 50;       //to make the fade transition more smoother change Interval from 100 to 50
            BackColorTimer2.Tick += new EventHandler(TimerRemoveBackColorLabel10);
            BackColorTimer2.Start();
        }

        //fade the alpha value of BackColor 
        private void TimerRemoveBackColorLabel5(object sender, EventArgs eArgs)
        {
            int fadingSpeed = 10;

            if (label5.BackColor.A - fadingSpeed < 0) {
                BackColorTimer.Stop();
                BackColorTimer.Tick -= new EventHandler(TimerRemoveBackColorLabel5);
            }
            else {
                label5.BackColor = Color.FromArgb(label5.BackColor.A - fadingSpeed, label5.BackColor.R, label5.BackColor.G, label5.BackColor.B);
            }
        }

        //fade the alpha value of BackColor 
        private void TimerRemoveBackColorLabel10(object sender, EventArgs eArgs)
        {
            int fadingSpeed = 10;

            if (label10.BackColor.A - fadingSpeed < 0) {
                BackColorTimer2.Stop();
                BackColorTimer2.Tick -= new EventHandler(TimerRemoveBackColorLabel10);
            }
            else {
                label10.BackColor = Color.FromArgb(label10.BackColor.A - fadingSpeed, label10.BackColor.R, label10.BackColor.G, label10.BackColor.B);
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (textBox1.TextLength > 1) {
                Thread.CurrentThread.CurrentCulture = new CultureInfo("tr-TR"); ;
                listBox1.Items.Clear();
                string upper = textBox1.Text.ToUpper();

                for (int i = 0; i < Global.Instance.FirmCodeList.Count(); i++) {
                    string temp = Global.Instance.FirmCodeList[i];

                    if (temp.ToUpper().Contains(upper)) {
                        listBox1.Items.Add(temp);
                    }
                }
            }
            else {
                listBox1.Items.Clear();
            }
        }
        
        private void Ekstre(int vade)
        {
            DataTable payPlanDT;
            Decimal bakiye = 0;
            Decimal vadesiDolan = 0;
            string alacakStr = " (A)";
            string bakiyeStr = " (B)";

            string vadeStr;
            int latestVadeRow = -1;
            if (listBox1.SelectedItem != null) {                
                label1.Text = label2.Text = label3.Text = label4.Text = label5.Text = label6.Text = label8.Text = label10.Text = label11.Text = label12.Text = label13.Text = "";
                label12.Visible = label13.Visible = false;
                if (vade == -1) { //if vade == -1 listBox tan bir firma seçilerek çağrılmış demek. o durumda elle girilen vade kutusunu boşalt
                    textBox2.Text = "";
                }
                label9.Visible = false;
                textBox2.Visible = false;

                DataTable dt = Global.Instance.query.QueryEkstre(listBox1.SelectedItem.ToString());
                if (dt == null) { return; }
                if (dt.Rows.Count == 0) {
                    Global.Instance.ErrorNotification("Hareket yok");
                    dataGridView1.DataSource = null;
                    return;
                }            
                         
                dt.Columns.Add("Bakiye", typeof(string));
                dt.Columns.Add("Fiş Türü", typeof(string));

                payPlanDT = Global.Instance.query.QueryVade(dt.Rows[dt.Rows.Count - 1]["PAYMENTREF"].ToString());
                if (payPlanDT.Rows.Count != 0) {
                    vadeStr = Regex.Match(payPlanDT.Rows[0]["DAY_"].ToString(), @"\d+").Value;
                    if (vade == -1) {
                        vade = Int32.Parse(vadeStr);
                    }
                    label1.Text = "Vade: ";
                    label4.Text = vadeStr;
                    label1.Visible = true;
                    label4.Visible = true;
                }                

                dt.Rows[0]["Bakiye"] = "0";
                foreach (DataRow row in dt.Rows) {
                    row["Fiş Türü"] = ProcessFicheType(row["TRCODE"].ToString());
             //       row["Vade Tarihi"] = ProcessVadeDate(row, vade);

                    if (row["Borç"] != null && row["Borç"].ToString().Length > 0) {
                        bakiye += Decimal.Parse(row["Borç"].ToString(), CultureInfo.InvariantCulture);
                        if (bakiye >= 0) {
                            row["Bakiye"] = string.Format("{0:n}", Math.Floor(Math.Abs(bakiye) * 100) / 100) + bakiyeStr;
                        }
                        else {
                            row["Bakiye"] = string.Format("{0:n}", Math.Floor(Math.Abs(bakiye) * 100) / 100) + alacakStr;
                        }
                    }
                    else if (row["Alacak"] != null && row["Alacak"].ToString().Length > 0) {
                        bakiye -= Decimal.Parse(row["Alacak"].ToString(), CultureInfo.InvariantCulture);
                        if (bakiye >= 0) {
                            row["Bakiye"] = string.Format("{0:n}", Math.Floor(Math.Abs(bakiye) * 100) / 100) + bakiyeStr;
                        }
                        else {
                            row["Bakiye"] = string.Format("{0:n}", Math.Floor(Math.Abs(bakiye) * 100) / 100) + alacakStr;
                        }
                    }

                    DateTime faturaTarihi = DateTime.ParseExact(row["Tarih"].ToString(), "dd.MM.yyyy", CultureInfo.CreateSpecificCulture("de-DE"));
                    double difference = (DateTime.Today - faturaTarihi).TotalDays;
                    if (vade != 0) {
                        if (difference >= vade && row["Bakiye"].ToString() != "") {
                            latestVadeRow += 1;
                            vadesiDolan = bakiye;
                        }
                        else if (difference < vade && row["Alacak"].ToString() != "") {
                            vadesiDolan -= Decimal.Parse(row["Alacak"].ToString(), CultureInfo.InvariantCulture);
                        }
                    }                    
                }
                if (vade == 0) {
                    vadesiDolan = Decimal.Parse(Regex.Match(dt.Rows[dt.Rows.Count-1]["Bakiye"].ToString().Replace(".", string.Empty), @"\d+.+\d").Value, CultureInfo.GetCultureInfo("fr-FR"));
                }

                dt.Columns.Add("Geçen Gün Sayısı");                                 
                VadeAverageCalculation(ref latestVadeRow, vadesiDolan, dt.Rows, vade);
                VadePassedRemainingCalculation(latestVadeRow, dt.Rows, vade);
                label13.Text = RealVadeCalculation(bakiye, dt.Rows, vade).ToString("#,###0.00");

                dt.Columns.Remove("TRCODE");
                dt.Columns.Remove("PAYMENTREF");
                dt.Columns.Remove("SIGN");
                dt.SetColumnsOrder("Tarih", "Vade Tarihi", "Fiş No", "Fiş Türü", "Açıklama", "Borç", "Alacak");
                DataGridViewFormat(dt, latestVadeRow, vade);
                

                label2.Text = "Vadesi Dolan: ";
                label12.Text = "Çekli Bakiye: ";
                label5.Text = vadesiDolan.ToString("#,###0.00");
                textBox2.Visible = label2.Visible = label5.Visible = label9.Visible = true;
            }
        }

        //Format "Tarih" Column, set visible to true, color the odd and even lines, and allign some of the rows
        private void DataGridViewFormat(DataTable dt, int latestVadeRow, int vade)
        {
            DataTableExt.ConvertColumnType(dt, "Tarih");
            dataGridView1.DataSource = new BindingSource(dt, null);

            dataGridView1.EvenRowColoring(Color.WhiteSmoke);            
            
            if (vade > 0) {
                foreach (DataGridViewRow row in dataGridView1.Rows) {
                    DateTime vadeDateTime = (DateTime)row.Cells[0].Value;
                    double difference = (DateTime.Today - vadeDateTime).TotalDays;

                    if (vadeStartRow != -1 && vadeFinishRow != -1 && row.Index <= vadeFinishRow && row.Index >= vadeStartRow) {
                        if (row.Index % 2 == 0) {
                            row.DefaultCellStyle.BackColor = Color.FromArgb(255, 140, 140);
                        }
                        else {
                            row.DefaultCellStyle.BackColor = Color.FromArgb(255, 115, 115);
                        }
                        continue;
                    }

                    if (row.Index >= latestVadeRow) {
                        if (row.Index % 2 == 0) {
                            row.DefaultCellStyle.BackColor = Color.FromArgb(230, 229, 238);
                        }
                        else {
                            row.DefaultCellStyle.BackColor = Color.FromArgb(216, 229, 228);
                        }
                    }
                }
                vadeStartRow = -1;
                vadeFinishRow = -1;
            }                                             
            DataGridLineAlignment();
            dataGridView1.SetColumnSortMode(DataGridViewColumnSortMode.NotSortable);
        }

        private string ProcessFicheType(string ficheType) {
            switch (ficheType) {
                case "1":
                    ficheType = "Nakit tahsilat";
                    break;
                case "2":
                    ficheType = "Nakit ödeme";
                    break;
                case "3":
                    ficheType = "Borç dekontu";
                    break;
                case "4":
                    ficheType = "Alacak dekontu";
                    break;
                case "5":
                    ficheType = "Virman Işlemi";
                    break;
                case "6":
                    ficheType = "Kur farkı işlemi";
                    break;
                case "12":
                    ficheType = "Özel işlem";
                    break;
                case "14":
                    ficheType = "Açılış Fişi";
                    break;
                case "20":
                    ficheType = "Gelen havaleler";
                    break;
                case "21":
                    ficheType = "Gönderilen havaleler";
                    break;
                case "31":
                    ficheType = "Mal alım faturası";
                    break;
                case "32":
                    ficheType = "Perakende satış iade faturası";
                    break;
                case "33":
                    ficheType = "Toptan satış iade faturası";
                    break;
                case "34":
                    ficheType = "Alınan hizmet faturası";
                    break;
                case "35":
                    ficheType = "Alınan proforma faturası";
                    break;
                case "36":
                    ficheType = "Alım iade faturası";
                    break;
                case "37":
                    ficheType = "Perakende satış faturası";
                    break;
                case "38":
                    ficheType = "Toptan satış faturası";
                    break;
                case "39":
                    ficheType = "Verilen hizmet faturası";
                    break;
                case "40":
                    ficheType = "Verilen proforma faturası";
                    break;
                case "41":
                    ficheType = "Verilen vade farkı faturası";
                    break;
                case "42":
                    ficheType = "Alınan Vade farkı faturası";
                    break;
                case "43":
                    ficheType = "Alınan fiyat farkı faturası";
                    break;
                case "44":
                    ficheType = "Verilen fiyat farkı faturası";
                    break;
                case "45":
                    ficheType = "Verilen Serbest Meslek Makbuzu";
                    break;
                case "46":
                    ficheType = "Alınan Serbest Meslek Makbuzu";
                    break;
                case "56":
                    ficheType = "Müstahsil makbuzu";
                    break;
                case "61":
                    ficheType = "Çek girişi";
                    break;
                case "62":
                    ficheType = "Senet girişi";
                    break;
                case "63":
                    ficheType = "Çek çıkış cari hesaba";
                    break;
                case "64":
                    ficheType = "Senet çıkış cari hesaba";
                    break;
                case "70":
                    ficheType = "Kredi Kartı Fişi";
                    break;
                case "71":
                    ficheType = "Kredi Kartı İade Fişi";
                    break;
                case "72":
                    ficheType = "Firma Kredi Kartı Fişi";
                    break;
                case "73":
                    ficheType = "Firma Kredi Kartı İade Fişi";
                    break;
                default:
                    ficheType += " <----HATA VAR---->";
                    break;
            }
            return ficheType;
        }

        private DateTime ProcessVadeDate(DataRow row, int vade) {
            int sign = int.Parse(row["SIGN"].ToString());
            int trcode = int.Parse(row["TRCODE"].ToString());
            if (sign == 1 || trcode == 14) {
                return DateTime.ParseExact(row["Tarih"].ToString(), "dd.MM.yyyy", CultureInfo.CreateSpecificCulture("de-DE"));
            }
            return DateTime.ParseExact(row["Tarih"].ToString(), "dd.MM.yyyy", CultureInfo.CreateSpecificCulture("de-DE")).AddDays(vade);
        }

        private void VadeAverageCalculation(ref int latestVadeRow, decimal vadesiDolan, DataRowCollection rows, int vade) {
            decimal borcTotal = 0, multTotal = 0;
            bool once = false;
            if (vadesiDolan > 1) {
                for (int i = latestVadeRow; i >= 0; i--) {
                    DataRow row = rows[i];
                    if (row["Borç"].ToString() != "") {
                        Decimal faturaTarihi = (Decimal)DateTime.ParseExact(row["Tarih"].ToString(), "dd.MM.yyyy", CultureInfo.CreateSpecificCulture("de-DE")).ToOADate();
                        Decimal alacak = Decimal.Parse(row["Borç"].ToString(), CultureInfo.InvariantCulture);
                        row["Geçen Gün Sayısı"] = (DateTime.Today.Subtract(DateTime.ParseExact(row["Tarih"].ToString(), "dd.MM.yyyy", CultureInfo.CreateSpecificCulture("de-DE"))).TotalDays - vade).ToString();
                        if (!once) {
                            once = true;
                            vadeFinishRow = i;
                        }
                        //artık vadesi dolan son satırdasın gerekli bilgileri ekrana yaz ve çık
                        if (vadesiDolan - alacak <= 1) {
                            borcTotal += vadesiDolan;
                            multTotal += faturaTarihi * vadesiDolan;
                            label3.Text = "V. Geçen Ort. Hsb: ";
                            label6.Text = DateTime.FromOADate((double)(multTotal / borcTotal)).ToString("dd.MM.yyyy");
                            label3.Visible = true;
                            label6.Visible = true;
                            if (vadeStartRow == -1) {
                                vadeStartRow = i;
                            }
                            break;
                        }
                        else {
                            vadesiDolan -= alacak;
                            borcTotal += alacak;
                            multTotal += faturaTarihi * alacak;
                        }
                    }
                    else {
                        if (!once) {
                            once = true;
                            vadeFinishRow = i;
                        }
                    }
                }
            }
            else {
                // vade negative ise latestVadeRow vadesi dolmuş kısımlarıda gösteriyor. Negative ise adamlar fazladan ödemiş demek, 
                // bu kısım normal renkte kalmalı ve "Geçen Gün Sayısı" artı sayılar yazılmamalı
                if (rows.Count > (latestVadeRow + 1)) {
                    latestVadeRow += 1;
                }
            }
        }

        //"Geçen Gün Sayısı" kısmına eksi değerleri giriyor. artı değerler "VadeAverageCalculation(...)" fonksiyonunda giriliyor. 
        //Vade sonuna kaç gün kaldığını gösteriyor
        private void VadePassedRemainingCalculation(int latestVadeRow, DataRowCollection rows, int vade)
        {
            for (int i = latestVadeRow; i >= 0 && i < rows.Count; i++) {
                DataRow row = rows[i];
                if (row["Borç"].ToString() != "") {
                        row["Geçen Gün Sayısı"] = (DateTime.Today.Subtract(DateTime.ParseExact(row["Tarih"].ToString(), "dd.MM.yyyy", CultureInfo.CreateSpecificCulture("de-DE"))).TotalDays - vade).ToString();                                  
                }
            }
        }

        //allign some of the rows to right
        private void DataGridLineAlignment()
        {
            dataGridView1.Columns["Tarih"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView1.Columns["Borç"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView1.Columns["Alacak"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView1.Columns["Vade Tarihi"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView1.Columns["Bakiye"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            //açıklama satırı çok uzun gelebiliyor. Genişliğini 130 a sabitle
            //if (dataGridView1.Columns["Açıklama"].Width > 130) {
            //    dataGridView1.Columns["Açıklama"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            //    dataGridView1.Columns["Açıklama"].Width = 130;
            //}
        }

        private decimal RealVadeCalculation(decimal bakiye, DataRowCollection rows, int vade) {
            foreach (DataRow row in rows) {
                int trcode = int.Parse(row["TRCODE"].ToString());
                if (trcode == 61) {
                    DateTime faturaTarihi = DateTime.ParseExact(row["Vade Tarihi"].ToString(), "dd.MM.yyyy", CultureInfo.CreateSpecificCulture("de-DE"));
                    double difference = (DateTime.Today - faturaTarihi).TotalDays;

                    if (difference < 0) {
                        bakiye = bakiye + Decimal.Parse(row["Alacak"].ToString(), CultureInfo.InvariantCulture);
                        label12.Visible = label13.Visible = true;
                    }
                }
            }
            return bakiye;
        }
               
        private void SumCellValues(DataGridView dt)
        {
            int columnIndex = 0;
            decimal rowTotal = 0;
            decimal result = 0;
            NumberStyles style = NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands;
            for (int i = 0; i < dt.SelectedCells.Count; i++) {
                if (i == 0) {
                    columnIndex = dt.SelectedCells[i].ColumnIndex;
                }

                if (columnIndex == dt.SelectedCells[i].ColumnIndex) {
                    if (dt.SelectedCells[i].Value != null && dt.SelectedCells[i].Value.ToString() != "") {
                        if (Decimal.TryParse(dt.SelectedCells[i].Value.ToString(), style, CultureInfo.InvariantCulture, out result)) {
                            rowTotal += result;
                        }
                    }
                }
                else {
                    label8.Text = "Toplam: Farklı sütünları topladın!";
                    return;
                }
            }

            label8.Text = "Toplam: " + rowTotal.ToString("#,###0.00");
            label8.Visible = true;
        }

        private void ResizePanels()
        {
            if (dataGridView2.RowCount > 0) {
                panel3.Height = (int)((this.Height - panel1.Height) * 0.3);
            }
            else {
                panel3.Height = 0;
            }
        }
    }
}
