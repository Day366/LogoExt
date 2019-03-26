using System;
using System.Data;
using System.Diagnostics;
using System.IO;
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
             }

             public static GtipForm Instance
             {
                 get { return instance; }
             }
     **********************************************************************/

        public GtipForm()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            dateTimePicker1.Value = DateTime.Now.AddDays(-15);
            dateTimePicker2.Value = DateTime.Now;
            pictureBox1.Hide();
            checkImage = Properties.Resources.check;
            errorImage = Properties.Resources.error;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DateTime date1 = dateTimePicker1.Value;
            DateTime date2 = dateTimePicker2.Value;
            DataTable dt = Global.Instance.query.QueryGTIB(this, date1, date2);
            if (dt == null) {
                //we probably got an exception from query. Do nothing.
                ShowError("DB sorgusu null döndü");
                return;
            }    
            string fileName = "\\GTIP " + DateTime.Now.ToString("yyyy-MM-dd hh-mm-ss") + ".xls"; 
            string directoryPath = Global.exePath + "\\gtip"; ;
            string fullPath = directoryPath + fileName; 

            try {      
                if (!Directory.Exists(directoryPath)) {         //if directory doesn't exists create the "gtip" folder
                    Directory.CreateDirectory(directoryPath);
                }

                AmountCorrection(dt);
                LineMerge(dt);
                //TotalCollumnFix(dt);  //suatın her satırı hesaplaması gibi ama buna gerek yok dediler "şimdilik"
                ChangeCollumnName(dt);

                dt.WriteXml(fullPath);
                Process.Start(fullPath);

                pictureBox1.Image = checkImage;
                pictureBox1.Show();
            }
            catch (Exception ex) {
                Console.WriteLine(ex.Message + ": " + ex.StackTrace);
                LogWriter.Instance.LogWrite(ex.Message + ": " + ex.StackTrace);
                ShowError(ex.Message);
            }
        }

        /*
         * Devide 'AMOUNT' collumn by 10, 5 or 2 according to its value on 'CODE' collumn
         * 
         * 03100: Devide by 10
         * 04200: Devide by 5
         * 05500: Devide by 2
         * 02: Bu bölünmiyecek ama illaki bişey yazmak lazım. yoksa query o satırı almıyor
         */
        private void AmountCorrection(DataTable dt)
        {
            foreach (DataRow dr in dt.Rows) {
                if (dr["CODE"].ToString().Equals("03100")) {
                    dr["AMOUNT"] = (Convert.ToDouble(dr["AMOUNT"].ToString()) / 10).ToString();
                }
                else if (dr["CODE"].ToString().Equals("04200")) {
                    dr["AMOUNT"] = (Convert.ToDouble(dr["AMOUNT"].ToString()) / 5).ToString();
                }
                else if (dr["CODE"].ToString().Equals("05500")) {
                    dr["AMOUNT"] = (Convert.ToDouble(dr["AMOUNT"].ToString()) / 2).ToString();
                }
            }
        }

        /*
         * Merge lines with same 'GTIP'. 
         * DB'den gelen query sıralı geldiği için aynı fatura numarası olanların GTIPleri alt alta sıralanacak.
         * Merge 'TOTAL', 'AMOUNT' AND 'ADDTAXAMOUNT'
         */
        private void LineMerge(DataTable dt)
        {
            DataRow previousDr = null;
            foreach (DataRow currentDr in dt.Rows) {
                if (previousDr == null) {
                    previousDr = currentDr;
                    continue;
                }

                if (currentDr["FICHENO"].ToString().Equals(previousDr["FICHENO"].ToString())) {
                    if (currentDr["GTIPCODE"].ToString().Equals(previousDr["GTIPCODE"].ToString())) {
                        double newTotal = Convert.ToDouble(previousDr["TOTAL"].ToString()) + Convert.ToDouble(currentDr["TOTAL"].ToString());
                        double newAmount = Convert.ToDouble(previousDr["AMOUNT"].ToString()) + Convert.ToDouble(currentDr["AMOUNT"].ToString());
                        double newAddTaxAmount = Convert.ToDouble(previousDr["ADDTAXAMOUNT"].ToString()) + Convert.ToDouble(currentDr["ADDTAXAMOUNT"].ToString());
                        previousDr["TOTAL"] = newTotal.ToString();
                        previousDr["AMOUNT"] = newAmount.ToString();
                        previousDr["ADDTAXAMOUNT"] = newAddTaxAmount.ToString();
                        currentDr.Delete();
                        continue;
                    }
                }
                previousDr = currentDr;
            }
        }

        /*
         * Tolal satırı Suatın istediği gibi getirmek için
         * Aynı faturadaki kolonların toplamı, KDV ve ÖTV siz toplama eşit olmalı.
         * Fatura No tek satırsa 'TOTAL' = 'TOTALDISCOUNTED'
         * Fatura No aynı birden fazla satır varsa. 2 satır 'TOTAL' toplamını, 'TOTALDISCOUNTED'dan çıkar. 
         */
        private void TotalCollumnFix(DataTable dt)
        {
            DataRow previousDr = null;
            foreach (DataRow currentDr in dt.Rows) {
                if (currentDr.RowState == DataRowState.Deleted) { continue; }
                if (previousDr == null) {
                    previousDr = currentDr;
                    continue;
                }

                if (currentDr["FICHENO"].ToString().Equals(previousDr["FICHENO"].ToString())) {
                    decimal crPlusDrTotal = Convert.ToDecimal(previousDr["TOTAL"]) + Convert.ToDecimal(currentDr["TOTAL"]);
                    if (crPlusDrTotal == Convert.ToDecimal(previousDr["TOTALDISCOUNTED"])) {                                         //gib satırları, fatura satırları ile eşitse kendi değeri kalması lazım
                        currentDr["TOTALDISCOUNTED"] = (Convert.ToDecimal(previousDr["TOTALDISCOUNTED"]) - Convert.ToDecimal(previousDr["TOTAL"])).ToString();
                        continue;
                    }
                    previousDr["TOTAL"] = crPlusDrTotal.ToString();
                    currentDr["TOTAL"] = "0";
                    currentDr["TOTALDISCOUNTED"] = (Convert.ToDecimal(previousDr["TOTALDISCOUNTED"]) - crPlusDrTotal).ToString();
                }
                else {
                    previousDr["TOTAL"] = (Convert.ToDecimal(previousDr["TOTALDISCOUNTED"])).ToString();
                }
                previousDr = currentDr;
            }
        }

        /*
         * Column headerlarının ismini daha okunaklı yap ve son 2 kolonu yok et
         */
        private void ChangeCollumnName(DataTable dt)
        {
            dt.Columns["GTIPCODE"].ColumnName = "G.T.I.P. No";
            dt.Columns["TCKNO"].ColumnName = "Alıcının T.C\nKimlik No";
            dt.Columns["TAXNR"].ColumnName = "Alıcının Vergi\nKimlik No";
            dt.Columns["DEFINITION_"].ColumnName = "Alıcının Adı\nSoyadı / Ünvanı";
            dt.Columns["DATE"].ColumnName = "Fatura Tarihi";
            dt.Columns["FICHENO"].ColumnName = "Fatura Seri -\nSıra No";
            dt.Columns["AMOUNT"].ColumnName = "Teslim Edilen\nMal Miktarı";
            dt.Columns["ADDTAXAMOUNT"].ColumnName = "Toplam ÖTV\nTutarı";
            dt.Columns["TOTAL"].ColumnName = "Teslim Bedeli\n(ÖTV KDV Hariç)";
            dt.Columns.Remove("TOTALDISCOUNTED");
            dt.Columns.Remove("CODE");
        }


        private void ShowError(string message) {
            Global.Instance.ErrorNotification("GTIP Kodlarını alamadı: --- " + message);
            pictureBox1.Image = errorImage;
            pictureBox1.Show();
        }
    }
}
