using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LogoExt
{
    public partial class EkstreForm : Form
    {
        private int vadeStartRow = -1;
        private int vadeFinishRow = -1;
        public EkstreForm()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            textBox1.KeyPress += new KeyPressEventHandler(TextBox1_KeyPress);
            dataGridView1.CellMouseUp += OnCellMouseUp;
            dataGridView1.DoubleBuffered(true);
            label4.BringToFront();
            label5.BringToFront();
            label6.BringToFront();
            dataGridView1.DefaultCellStyle.Font = new Font((string)Global.Instance.settings.FontFamily, (float)Global.Instance.settings.TextSize);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Length > 0) {
                textBox1.Clear();
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem != null) {
                label1.Text = label2.Text = label3.Text = label4.Text = label5.Text = label6.Text = label8.Text = "";
                DataTable dt = Global.Instance.query.QueryEkstre(listBox1.SelectedItem.ToString());
                if (dt.Rows.Count == 0) {
                    //TODO hiç row yoksa ekstre de yoktur ona göre bi geri bildirim yap
                    dataGridView1.DataSource = null;
                    dataGridView1.Visible = true;
                    return;
                }
                DataTable payPlanDT;
                Decimal bakiye;
                Decimal vadesiDolan = 0;
                string alacakStr;
                string bakiyeStr;
                int vade;
                string vadeStr;
                int latestVadeRow = -1;


                bakiye = 0;
                alacakStr = " (A)";
                bakiyeStr = " (B)";
                dt.Columns.Add("Bakiye", typeof(string));
                dt.Columns.Add("Fiş türü", typeof(string));
                dt.Columns.Add("Vade Tarihi", typeof(DateTime));

                payPlanDT = Global.Instance.query.QueryVade(dt.Rows[dt.Rows.Count-1]["PAYMENTREF"].ToString());
                vadeStr = Regex.Match(payPlanDT.Rows[0]["DAY_"].ToString(), @"\d+").Value;
                vade = Int32.Parse(vadeStr);
                label1.Text = "Vade: ";
                label4.Text = vadeStr;
                label1.Visible = true;
                label4.Visible = true;
                dt.Rows[0]["Bakiye"] = "0";
                foreach (DataRow row in dt.Rows) {
                    row["Fiş Türü"] = ProcessFicheType(row["TRCODE"].ToString());                    
                    row["Vade Tarihi"] = ProcessVadeDate(row, vade);
                                       

                    if (row["Borç"] != null && row["Borç"].ToString().Length > 0) {
                        bakiye += Decimal.Parse(row["Borç"].ToString(), CultureInfo.InvariantCulture);
                        if (bakiye >= 0) {
                            row["Bakiye"] = string.Format("{0:n}", Math.Floor(Math.Abs(bakiye) * 100) / 100) + bakiyeStr;
                        }
                        else {
                            row["Bakiye"] = string.Format("{0:n}", Math.Floor(Math.Abs(bakiye) * 100) / 100) + alacakStr;
                        }                      
                    }
                    else if(row["Alacak"] != null && row["Alacak"].ToString().Length > 0) {
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
                VadeAvarageCalculation(latestVadeRow, vadesiDolan, dt.Rows);
                dt.Columns.Remove("TRCODE");
                dt.Columns.Remove("PAYMENTREF");
                dt.Columns.Remove("SIGN");
                dt.SetColumnsOrder("Tarih", "Fiş No", "Fiş Türü", "Vade Tarihi", "Açıklama", "Borç", "Alacak");
                DataGridViewFormat(dt, latestVadeRow);
                if (vade != 0) {
                    label2.Text = "Vadesi Dolan: ";
                    label5.Text = vadesiDolan.ToString("#,###0.00");
                    label2.Visible = true;
                    label5.Visible = true;
                }                
            }
        }

        //When "Enter" Key is pressed and there is one item in listBox2 run query
        private void TextBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13) {
                if (listBox1.Items.Count > 0) {
                    listBox1.SelectedItem = listBox1.Items[0];
                }
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

        //Format "Tarih" Column, set visible to true, color the odd and even lines, and allign some of the rows
        private void DataGridViewFormat(DataTable dt, int latestVadeRow)
        {
            DataTableExt.ConvertColumnType(dt, "Tarih");
            dataGridView1.DataSource = new BindingSource(dt, null);
            dataGridView1.Visible = true;

            DataGridLineColoring(Color.White, Color.WhiteSmoke);

            int vade = Int32.Parse(Regex.Match(label4.Text, @"\d+").Value);
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

        //color the table's odd and even lines
        private void DataGridLineColoring(Color even, Color odd)
        {
            foreach (DataGridViewRow row in dataGridView1.Rows) {
                if (row.Index % 2 == 0) {
                    row.DefaultCellStyle.BackColor = even;
                }
                else {
                    row.DefaultCellStyle.BackColor = odd;
                }
            }
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
                    ficheType += " <------------------------------------------HATA VAR*******************************************";
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

        private void VadeAvarageCalculation(int latestVadeRow, decimal vadesiDolan, DataRowCollection rows) {
            decimal borcTotal = 0, multTotal = 0;
            bool once = false;
            if (vadesiDolan > 1) {
                for (int i = latestVadeRow; i >= 0; i--) {
                    DataRow row = rows[i];
                    if (row["Borç"].ToString() != "") {
                        Decimal faturaTarihi = (Decimal)DateTime.ParseExact(row["Tarih"].ToString(), "dd.MM.yyyy", CultureInfo.CreateSpecificCulture("de-DE")).ToOADate();
                        Decimal alacak = Decimal.Parse(row["Borç"].ToString(), CultureInfo.InvariantCulture);

                        if (!once) {
                            once = true;
                            vadeFinishRow = i;
                        }
                        if (vadesiDolan - alacak <= 0) {
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
            if (dataGridView1.Columns["Açıklama"].Width > 130) {
                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                dataGridView1.Columns["Açıklama"].Width = 130;
                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            }
        }

        private void OnCellMouseUp(object sender, MouseEventArgs e)
        {
            int columnIndex = 0;
            decimal rowTotal = 0;
            decimal result = 0;
            NumberStyles style = NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands;
            for (int i = 0; i < dataGridView1.SelectedCells.Count; i++) {
                if (i == 0) {
                    columnIndex = dataGridView1.SelectedCells[i].ColumnIndex;
                }

                if (columnIndex == dataGridView1.SelectedCells[i].ColumnIndex) {
                    if (dataGridView1.SelectedCells[i].Value != null && dataGridView1.SelectedCells[i].Value.ToString() != "") {
                        if (Decimal.TryParse(dataGridView1.SelectedCells[i].Value.ToString(), style, CultureInfo.InvariantCulture, out result)) {
                            rowTotal += result;
                        }                        
                    }
                }
                else {
                    label8.Text = "Toplam: Farklı sütünları topladın!";
                    return;
                }
            }
            
            label8.Text = "Toplam: " + rowTotal.ToString();
            label8.Visible = true;
        }
    }
}
