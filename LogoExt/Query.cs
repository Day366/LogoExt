using System;
using System.IO;
using System.Data.SqlClient;
using System.Data;
using System.Diagnostics;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Globalization;

namespace LogoExt
{
    class Query
    {
        private DataTable QueryDB(string sqlCommand) {
            SqlConnection con;
            SqlCommand cmd;
            SqlDataAdapter da;
            DataTable dt;

            try {
                con = new SqlConnection(Global.sqlConnection);
                cmd = new SqlCommand(sqlCommand, con);
                con.Open();
                da = new SqlDataAdapter(cmd);
                dt = new DataTable();
                da.Fill(dt);
                
                con.Close();
                return dt;
            }
            catch (Exception ex) {
                Console.WriteLine(ex.Message + ": " + ex.StackTrace);
                LogWriter.Instance.LogWrite(ex.Message + ": " + ex.StackTrace);
                Global.Instance.ErrorNotification("Data base sorgu hatası: --- " + ex.Message);
                return null;
            }
        }
        public void QueryGTIB(Form1 form1)
        {
            SqlConnection con;
            SqlCommand cmd;
            SqlDataAdapter da;
            DataSet ds;
            DataTable dt;
            DateTime date1 = form1.DateTimePicker1.Value;
            DateTime date2 = form1.DateTimePicker2.Value;
     
            string fileName;
            string directoryPath;
            string fullPath;

            try
            {
                con = new SqlConnection(Global.sqlConnection);
                cmd = new SqlCommand("SELECT Items.GTIPCODE, Clcard.TCKNO, Clcard.TAXNR, Clcard.DEFINITION_, convert(varchar, Invoice.DATE_, 104) as DATE, Invoice.FICHENO, Stline.AMOUNT, Stline.ADDTAXAMOUNT, Stline.TOTAL, Invoice.TOTALDISCOUNTED - Invoice.TOTALDISCOUNTS As TOTALDISCOUNTED, Mark.CODE FROM LG_" + Global.firmCode + "_01_INVOICE Invoice, LG_" + Global.firmCode + "_01_STLINE Stline, LG_" + Global.firmCode + "_ITEMS Items, LG_" + Global.firmCode + "_CLCARD Clcard, LG_" + Global.firmCode + "_MARK Mark WHERE Invoice.DATE_ >= '" + date1.ToString("yyyy") + "/" + date1.ToString("MM") + "/" + date1.ToString("dd") + "' and Invoice.DATE_ <= '" + date2.ToString("yyyy") + "/" + date2.ToString("MM") + "/" + date2.ToString("dd") + " 23:59:59.998' and Invoice.LOGICALREF = Stline.INVOICEREF and Items.GTIPCODE <> '' and Items.LOGICALREF = Stline.STOCKREF and Clcard.LOGICALREF = Invoice.CLIENTREF and Invoice.GRPCODE = 2 and Items.MARKREF = Mark.LOGICALREF and Invoice.CANCELLED = 0 ORDER BY Invoice.DATE_, Invoice.FICHENO, Items.GTIPCODE", con);
                con.Open();

                da = new SqlDataAdapter(cmd);
                ds = new DataSet();
                dt = new DataTable();

                da.Fill(ds);
                con.Close();
                directoryPath = Global.exePath + "\\gtip";
                fileName = "\\GTIP " + DateTime.Now.ToString("yyyy-MM-dd hh-mm-ss") + ".xls";
                fullPath = directoryPath + fileName;

                if (!Directory.Exists(directoryPath)) {         //if directory doesn't exists create the "gtip" folder
                    Directory.CreateDirectory(directoryPath);
                }

                dt = ds.Tables["Table"];

                AmountCorrection(dt);
                LineMerge(dt);
                TotalCollumnFix(dt);
                ChangeCollumnName(dt);                

                ds.WriteXml(fullPath);
                Process.Start(fullPath);

                form1.PictureBox1.Image = form1.CheckImage;
                form1.PictureBox1.Show();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + ": " + ex.StackTrace);
                LogWriter.Instance.LogWrite(ex.Message + ": " + ex.StackTrace);

                Global.Instance.ErrorNotification("GTIP Kodlarını alamadı: --- " + ex.Message);
                form1.PictureBox1.Image = form1.ErrorImage;
                form1.PictureBox1.Show();
            }
        }

        public List<String> QueryFirmCodes(ItemPriceForm itemPriceForm)
        {
            SqlConnection con;
            SqlCommand cmd;
            SqlDataAdapter da;
            DataSet ds;

            try {
                con = new SqlConnection(Global.sqlConnection);
                cmd = new SqlCommand("SELECT CODE FROM LG_0" + itemPriceForm.GetFirmCode(0) + "_CLCARD", con);

                con.Open();

                da = new SqlDataAdapter(cmd);
                ds = new DataSet();

                da.Fill(ds);

                List<String> columnData = new List<String>();
                using (SqlDataReader reader = cmd.ExecuteReader()) {
                    while (reader.Read()) {
                        columnData.Add(reader.GetString(0));
                    }
                }

                con.Close();
                return columnData;
            }
            catch (Exception ex) {
                Console.WriteLine(ex.Message + ": " + ex.StackTrace);
                LogWriter.Instance.LogWrite(ex.Message + ": " + ex.StackTrace);
                Global.Instance.ErrorNotification("Firma kodlarını alamadı: --- " + ex.Message);
                return new List<string>();
            }
        }

        public List<String> QueryItemCodes(ItemPriceForm itemPriceForm)
        {
            SqlConnection con;
            SqlCommand cmd;
            SqlDataAdapter da;
            DataSet ds;  

            try {
                con = new SqlConnection(Global.sqlConnection);
                cmd = new SqlCommand("SELECT CODE FROM LG_0" + itemPriceForm.GetFirmCode(0) + "_ITEMS", con);

                con.Open();

                da = new SqlDataAdapter(cmd);
                ds = new DataSet();

                da.Fill(ds);

                List<String> columnData = new List<String>();
                using (SqlDataReader reader = cmd.ExecuteReader()) {
                    while (reader.Read()) {
                        columnData.Add(reader.GetString(0));
                    }
                }

                con.Close();
                return columnData;
            }
            catch (Exception ex) {
                Console.WriteLine(ex.Message + ": " + ex.StackTrace);
                LogWriter.Instance.LogWrite(ex.Message + ": " + ex.StackTrace);
                Global.Instance.ErrorNotification("Malzeme kodlarını alamadı: --- " + ex.Message);
                return new List<string>();
            }
        }

        public DataTable QueryItemPriceByItem(ItemPriceForm itemPriceForm, string selectedItem)
        {
            DataTable dt; 
            if (itemPriceForm.GetFirmCodeInt(0) > 16) {
                dt = QueryDB("SELECT Clcard.DEFINITION_ as 'Firma İsmi', Items.CODE as 'Firma Kodu', Items.NAME as 'Ürün Kodu', Stline.LINEEXP as Detaylar, Stline.AMOUNT as Miktar, format(Stline.DATE_, 'dd.MM.yyyy') as 'Tarih', (CASE WHEN Stline.PRCURR = 0 and Stline.REPORTRATE != 0 THEN ROUND(Stline.PRICE / Stline.REPORTRATE, 2) END) AS EURO, (CASE WHEN Stline.PRCURR = 11 THEN ROUND(Stline.PRPRICE, 2) END) AS CHF, (CASE WHEN Stline.PRCURR = 1 THEN ROUND(Stline.PRPRICE, 2) END) AS DOLAR FROM LG_0" + itemPriceForm.GetFirmCode(0) + "_ITEMS Items, LG_0" + itemPriceForm.GetFirmCode(0) + "_01_STLINE Stline, LG_0" + itemPriceForm.GetFirmCode(0) + "_01_INVOICE Invoice, LG_0" + itemPriceForm.GetFirmCode(0) + "_CLCARD Clcard WHERE Items.CODE = '" + selectedItem + "' and Items.LOGICALREF = Stline.STOCKREF and Clcard.LOGICALREF = Invoice.CLIENTREF and Invoice.LOGICALREF = Stline.INVOICEREF and Invoice.GRPCODE = 2 and Invoice.CANCELLED = 0 ORDER BY Stline.DATE_ DESC");
            }
            else {
                dt = QueryDB("SELECT Clcard.DEFINITION_ as 'Firma İsmi', Items.CODE as 'Firma Kodu', Items.NAME as 'Ürün Kodu', Stline.LINEEXP as Detaylar, Stline.AMOUNT as Miktar, format(Stline.DATE_, 'dd.MM.yyyy') as 'Tarih', (CASE WHEN Stline.PRCURR = 0 and Stline.TRRATE != 0 THEN ROUND(Stline.PRICE / Stline.TRRATE, 2) END) AS EURO, (CASE WHEN Stline.PRCURR = 11 THEN ROUND(Stline.PRPRICE, 2) END) AS CHF, (CASE WHEN Stline.PRCURR = 1 THEN ROUND(Stline.PRPRICE, 2) END) AS DOLAR FROM LG_0" + itemPriceForm.GetFirmCode(0) + "_ITEMS Items, LG_0" + itemPriceForm.GetFirmCode(0) + "_01_STLINE Stline, LG_0" + itemPriceForm.GetFirmCode(0) + "_01_INVOICE Invoice, LG_0" + itemPriceForm.GetFirmCode(0) + "_CLCARD Clcard WHERE Items.CODE = '" + selectedItem + "' and Items.LOGICALREF = Stline.STOCKREF and Clcard.LOGICALREF = Invoice.CLIENTREF and Invoice.LOGICALREF = Stline.INVOICEREF and Invoice.GRPCODE = 2 and Invoice.CANCELLED = 0 ORDER BY Stline.DATE_ DESC");
            }
            return dt;
        }

        public DataTable QueryItemPriceByFirm(ItemPriceForm itemPriceForm, string selectedFirm)
        {
            DataTable dt;   
            if (itemPriceForm.GetFirmCodeInt(0) > 16) {
                dt = QueryDB("SELECT Clcard.DEFINITION_ as 'Firma İsmi', Items.CODE as 'Firma Kodu', Items.NAME as 'Ürün Kodu', Stline.LINEEXP as Detaylar, Stline.AMOUNT as Miktar, format(Stline.DATE_, 'dd.MM.yyyy') as 'Tarih', (CASE WHEN Stline.PRCURR = 0 and Stline.REPORTRATE != 0 THEN ROUND(Stline.PRICE / Stline.REPORTRATE, 2) END) AS EURO, (CASE WHEN Stline.PRCURR = 11 THEN ROUND(Stline.PRPRICE, 2) END) AS CHF, (CASE WHEN Stline.PRCURR = 1 THEN ROUND(Stline.PRPRICE, 2) END) AS DOLAR FROM LG_0" + itemPriceForm.GetFirmCode(0) + "_ITEMS Items, LG_0" + itemPriceForm.GetFirmCode(0) + "_01_STLINE Stline, LG_0" + itemPriceForm.GetFirmCode(0) + "_01_INVOICE Invoice, LG_0" + itemPriceForm.GetFirmCode(0) + "_CLCARD Clcard WHERE Clcard.CODE = '" + selectedFirm + "' and Items.LOGICALREF = Stline.STOCKREF and Clcard.LOGICALREF = Invoice.CLIENTREF and Invoice.LOGICALREF = Stline.INVOICEREF and Invoice.GRPCODE = 2 and Invoice.CANCELLED = 0 ORDER BY Stline.DATE_ DESC");
            }
            else {
                dt = QueryDB("SELECT Clcard.DEFINITION_ as 'Firma İsmi', Items.CODE as 'Firma Kodu', Items.NAME as 'Ürün Kodu', Stline.LINEEXP as Detaylar, Stline.AMOUNT as Miktar, format(Stline.DATE_, 'dd.MM.yyyy') as 'Tarih', (CASE WHEN Stline.PRCURR = 0 and Stline.TRRATE != 0 THEN ROUND(Stline.PRICE / Stline.TRRATE, 2) END) AS EURO, (CASE WHEN Stline.PRCURR = 11 THEN ROUND(Stline.PRPRICE, 2) END) AS CHF, (CASE WHEN Stline.PRCURR = 1 THEN ROUND(Stline.PRPRICE, 2) END) AS DOLAR FROM LG_0" + itemPriceForm.GetFirmCode(0) + "_ITEMS Items, LG_0" + itemPriceForm.GetFirmCode(0) + "_01_STLINE Stline, LG_0" + itemPriceForm.GetFirmCode(0) + "_01_INVOICE Invoice, LG_0" + itemPriceForm.GetFirmCode(0) + "_CLCARD Clcard WHERE Clcard.CODE = '" + selectedFirm + "' and Items.LOGICALREF = Stline.STOCKREF and Clcard.LOGICALREF = Invoice.CLIENTREF and Invoice.LOGICALREF = Stline.INVOICEREF and Invoice.GRPCODE = 2 and Invoice.CANCELLED = 0 ORDER BY Stline.DATE_ DESC");
            }
            return dt;
        }

        public DataTable QueryItemPriceByFirmAndItem(ItemPriceForm itemPriceForm, string selectedItem, string selectedFirm)
        {
            DataTable dt;
            if (itemPriceForm.GetFirmCodeInt(0) > 16) {
                dt = QueryDB("SELECT Clcard.DEFINITION_ as 'Firma İsmi', Items.CODE as 'Firma Kodu', Items.NAME as 'Ürün Kodu', Stline.LINEEXP as Detaylar, Stline.AMOUNT as Miktar, format(Stline.DATE_, 'dd.MM.yyyy') as 'Tarih', (CASE WHEN Stline.PRCURR = 0 and Stline.REPORTRATE != 0 THEN ROUND(Stline.PRICE / Stline.REPORTRATE, 2) END) AS EURO, (CASE WHEN Stline.PRCURR = 11 THEN ROUND(Stline.PRPRICE, 2) END) AS CHF, (CASE WHEN Stline.PRCURR = 1 THEN ROUND(Stline.PRPRICE, 2) END) AS DOLAR FROM LG_0" + itemPriceForm.GetFirmCode(0) + "_ITEMS Items, LG_0" + itemPriceForm.GetFirmCode(0) + "_01_STLINE Stline, LG_0" + itemPriceForm.GetFirmCode(0) + "_01_INVOICE Invoice, LG_0" + itemPriceForm.GetFirmCode(0) + "_CLCARD Clcard WHERE Items.CODE = '" + selectedItem + "' and Clcard.CODE = '" + selectedFirm + "' and Items.LOGICALREF = Stline.STOCKREF and Clcard.LOGICALREF = Invoice.CLIENTREF and Invoice.LOGICALREF = Stline.INVOICEREF and Invoice.GRPCODE = 2 and Invoice.CANCELLED = 0 ORDER BY Stline.DATE_ DESC");
            }
            else {
                dt = QueryDB("SELECT Clcard.DEFINITION_ as 'Firma İsmi', Items.CODE as 'Firma Kodu', Items.NAME as 'Ürün Kodu', Stline.LINEEXP as Detaylar, Stline.AMOUNT as Miktar, format(Stline.DATE_, 'dd.MM.yyyy') as 'Tarih', (CASE WHEN Stline.PRCURR = 0 and Stline.TRRATE != 0 THEN ROUND(Stline.PRICE / Stline.TRRATE, 2) END) AS EURO, (CASE WHEN Stline.PRCURR = 11 THEN ROUND(Stline.PRPRICE, 2) END) AS CHF, (CASE WHEN Stline.PRCURR = 1 THEN ROUND(Stline.PRPRICE, 2) END) AS DOLAR FROM LG_0" + itemPriceForm.GetFirmCode(0) + "_ITEMS Items, LG_0" + itemPriceForm.GetFirmCode(0) + "_01_STLINE Stline, LG_0" + itemPriceForm.GetFirmCode(0) + "_01_INVOICE Invoice, LG_0" + itemPriceForm.GetFirmCode(0) + "_CLCARD Clcard WHERE Items.CODE = '" + selectedItem + "' and Clcard.CODE = '" + selectedFirm + "' and Items.LOGICALREF = Stline.STOCKREF and Clcard.LOGICALREF = Invoice.CLIENTREF and Invoice.LOGICALREF = Stline.INVOICEREF and Invoice.GRPCODE = 2 and Invoice.CANCELLED = 0 ORDER BY Stline.DATE_ DESC");
            }
            return dt;
        }

        public DataTable QueryPastTenYearsPriceByItem(ItemPriceForm itemPriceForm, string selectedItem)
        {
            DataTable dt = QueryDB(ItemDataSince2010(itemPriceForm, selectedItem));
            return dt;
        }

        public DataTable QueryPastTenYearsPriceByFirm(ItemPriceForm itemPriceForm, string selectedFirm)
        {
            DataTable dt = QueryDB(FirmDataSince2010(itemPriceForm, selectedFirm));
            return dt;
        }
        
        public DataTable QueryPastTenYearsPriceByItemAndFirm(ItemPriceForm itemPriceForm, string selectedItem, string selectedFirm)
        {
            DataTable dt = QueryDB(ItemAndFirmDataSince2010(itemPriceForm, selectedItem, selectedFirm));
            return dt;
        }

        /*
         * Devide 'AMOUNT' collumn by 10, 5 or 2 according to its value on 'CODE' collumn
         * 
         * 03100: Devide by 10
         * 04200: Devide by 5
         * 05500: Devide by 2
         */
        private void AmountCorrection(DataTable dt)
        {
            foreach (DataRow dr in dt.Rows)
            {
                if (dr["CODE"].ToString().Equals("03100"))
                {
                    dr["AMOUNT"] = (Convert.ToDouble(dr["AMOUNT"].ToString()) / 10).ToString();
                }
                else if (dr["CODE"].ToString().Equals("04200"))
                {
                    dr["AMOUNT"] = (Convert.ToDouble(dr["AMOUNT"].ToString()) / 5).ToString();
                }
                else if (dr["CODE"].ToString().Equals("05500"))
                {
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
            foreach (DataRow currentDr in dt.Rows)
            {
                if (previousDr == null) {
                    previousDr = currentDr;
                    continue;
                }

                if (currentDr["FICHENO"].ToString().Equals(previousDr["FICHENO"].ToString())) {
                    if (currentDr["GTIPCODE"].ToString().Equals(previousDr["GTIPCODE"].ToString()))
                    {
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
            foreach (DataRow currentDr in dt.Rows)
            {
                if (currentDr.RowState == DataRowState.Deleted) {continue;}
                if (previousDr == null)
                {
                    previousDr = currentDr;
                    continue;
                }

                if (currentDr["FICHENO"].ToString().Equals(previousDr["FICHENO"].ToString()))
                {
                    decimal crPlusDrTotal = Convert.ToDecimal(previousDr["TOTAL"]) + Convert.ToDecimal(currentDr["TOTAL"]);
                    if (crPlusDrTotal == Convert.ToDecimal(previousDr["TOTALDISCOUNTED"])) {                                         //gib satırları, fatura satırları ile eşitse kendi değeri kalması lazım
                        currentDr["TOTALDISCOUNTED"] = (Convert.ToDecimal(previousDr["TOTALDISCOUNTED"]) - Convert.ToDecimal(previousDr["TOTAL"])).ToString();
                        continue;
                    }
                    previousDr["TOTAL"] = crPlusDrTotal.ToString();
                    currentDr["TOTAL"] = "0";
                    currentDr["TOTALDISCOUNTED"] = (Convert.ToDecimal(previousDr["TOTALDISCOUNTED"]) - crPlusDrTotal).ToString() ;
                }
                else {
                    previousDr["TOTAL"] = (Convert.ToDecimal(previousDr["TOTALDISCOUNTED"])).ToString();
                }
                previousDr = currentDr;
            }
        }

        //Get data of a specific item since 2010
        private string ItemDataSince2010(ItemPriceForm itemPriceForm, string selectedItem)
        {
            string finalSql = "SELECT a.[Firma İsmi], a.[Firma Kodu], a.[Ürün Kodu], format(a.Tarih, 'dd.MM.yyyy') as 'Tarih', a.Detaylar, a.Miktar, a.EURO, a.CHF, a.DOLAR FROM (";

            int pastYears = DateTime.Today.Year - 2010;
            for(int i = 0; i <= pastYears; i++) {
                if (itemPriceForm.GetLatestFirmCodeInt(i) > 16) {
                    finalSql += "SELECT Clcard.DEFINITION_ as 'Firma İsmi', Items.CODE as 'Firma Kodu', Items.NAME as 'Ürün Kodu', Stline.LINEEXP as Detaylar, Stline.AMOUNT as Miktar, Stline.DATE_ as Tarih, (CASE WHEN Stline.PRCURR = 0 and Stline.REPORTRATE != 0 THEN ROUND(Stline.PRICE / Stline.REPORTRATE, 2) END) AS EURO, (CASE WHEN Stline.PRCURR = 11 THEN ROUND(Stline.PRPRICE, 2) END) AS CHF, (CASE WHEN Stline.PRCURR = 1 THEN ROUND(Stline.PRPRICE, 2) END) AS DOLAR FROM LG_0" + itemPriceForm.GetLatestFirmCode(i) + "_ITEMS Items, LG_0" + itemPriceForm.GetLatestFirmCode(i) + "_01_STLINE Stline, LG_0" + itemPriceForm.GetLatestFirmCode(i) + "_01_INVOICE Invoice, LG_0" + itemPriceForm.GetLatestFirmCode(i) + "_CLCARD Clcard WHERE Items.CODE = '" + selectedItem + "' and Items.LOGICALREF = Stline.STOCKREF and Clcard.LOGICALREF = Invoice.CLIENTREF and Invoice.LOGICALREF = Stline.INVOICEREF and Invoice.GRPCODE = 2 and Invoice.CANCELLED = 0 ";
                }
                else {
                    finalSql += "SELECT Clcard.DEFINITION_ as 'Firma İsmi', Items.CODE as 'Firma Kodu', Items.NAME as 'Ürün Kodu', Stline.LINEEXP as Detaylar, Stline.AMOUNT as Miktar, Stline.DATE_ as Tarih, (CASE WHEN Stline.PRCURR = 0 and Stline.TRRATE != 0 THEN ROUND(Stline.PRICE / Stline.TRRATE, 2) END) AS EURO, (CASE WHEN Stline.PRCURR = 11 THEN ROUND(Stline.PRPRICE, 2) END) AS CHF, (CASE WHEN Stline.PRCURR = 1 THEN ROUND(Stline.PRPRICE, 2) END) AS DOLAR FROM LG_0" + itemPriceForm.GetLatestFirmCode(i) + "_ITEMS Items, LG_0" + itemPriceForm.GetLatestFirmCode(i) + "_01_STLINE Stline, LG_0" + itemPriceForm.GetLatestFirmCode(i) + "_01_INVOICE Invoice, LG_0" + itemPriceForm.GetLatestFirmCode(i) + "_CLCARD Clcard WHERE Items.CODE = '" + selectedItem + "' and Items.LOGICALREF = Stline.STOCKREF and Clcard.LOGICALREF = Invoice.CLIENTREF and Invoice.LOGICALREF = Stline.INVOICEREF and Invoice.GRPCODE = 2 and Invoice.CANCELLED = 0 ";
                }

                if (i != pastYears) {
                    finalSql += "UNION all ";
                }
            }
            finalSql += ") a ORDER BY a.Tarih DESC";

            return finalSql;
        }

        //Get data of a specific firm since 2010
        private string FirmDataSince2010(ItemPriceForm itemPriceForm, string selectedFirm)
        {
            string finalSql = "SELECT a.[Firma İsmi], a.[Firma Kodu], a.[Ürün Kodu], format(a.Tarih, 'dd.MM.yyyy') as 'Tarih', a.Detaylar, a.Miktar, a.EURO, a.CHF, a.DOLAR FROM (";

            int pastYears = DateTime.Today.Year - 2010;
            for (int i = 0; i <= pastYears; i++) {
                if (itemPriceForm.GetLatestFirmCodeInt(i) > 16) {
                    finalSql += "SELECT Clcard.DEFINITION_ as 'Firma İsmi', Items.CODE as 'Firma Kodu', Items.NAME as 'Ürün Kodu', Stline.LINEEXP as Detaylar, Stline.AMOUNT as Miktar, Stline.DATE_ as Tarih, (CASE WHEN Stline.PRCURR = 0 and Stline.REPORTRATE != 0 THEN ROUND(Stline.PRICE / Stline.REPORTRATE, 2) END) AS EURO, (CASE WHEN Stline.PRCURR = 11 THEN ROUND(Stline.PRPRICE, 2) END) AS CHF, (CASE WHEN Stline.PRCURR = 1 THEN ROUND(Stline.PRPRICE, 2) END) AS DOLAR FROM LG_0" + itemPriceForm.GetLatestFirmCode(i) + "_ITEMS Items, LG_0" + itemPriceForm.GetLatestFirmCode(i) + "_01_STLINE Stline, LG_0" + itemPriceForm.GetLatestFirmCode(i) + "_01_INVOICE Invoice, LG_0" + itemPriceForm.GetLatestFirmCode(i) + "_CLCARD Clcard WHERE Clcard.CODE = '" + selectedFirm + "' and Items.LOGICALREF = Stline.STOCKREF and Clcard.LOGICALREF = Invoice.CLIENTREF and Invoice.LOGICALREF = Stline.INVOICEREF and Invoice.GRPCODE = 2 and Invoice.CANCELLED = 0 ";
                }
                else {
                    finalSql += "SELECT Clcard.DEFINITION_ as 'Firma İsmi', Items.CODE as 'Firma Kodu', Items.NAME as 'Ürün Kodu', Stline.LINEEXP as Detaylar, Stline.AMOUNT as Miktar, Stline.DATE_ as Tarih, (CASE WHEN Stline.PRCURR = 0 and Stline.TRRATE != 0 THEN ROUND(Stline.PRICE / Stline.TRRATE, 2) END) AS EURO, (CASE WHEN Stline.PRCURR = 11 THEN ROUND(Stline.PRPRICE, 2) END) AS CHF, (CASE WHEN Stline.PRCURR = 1 THEN ROUND(Stline.PRPRICE, 2) END) AS DOLAR FROM LG_0" + itemPriceForm.GetLatestFirmCode(i) + "_ITEMS Items, LG_0" + itemPriceForm.GetLatestFirmCode(i) + "_01_STLINE Stline, LG_0" + itemPriceForm.GetLatestFirmCode(i) + "_01_INVOICE Invoice, LG_0" + itemPriceForm.GetLatestFirmCode(i) + "_CLCARD Clcard WHERE Clcard.CODE = '" + selectedFirm + "' and Items.LOGICALREF = Stline.STOCKREF and Clcard.LOGICALREF = Invoice.CLIENTREF and Invoice.LOGICALREF = Stline.INVOICEREF and Invoice.GRPCODE = 2 and Invoice.CANCELLED = 0 ";
                }

                if (i != pastYears) {
                    finalSql += "UNION all ";
                }
            }
            finalSql += ") a ORDER BY a.Tarih DESC";

            return finalSql;
        }

        //Get data of a specific firm and specific item since 2010
        private string ItemAndFirmDataSince2010(ItemPriceForm itemPriceForm, string selectedItem, string selectedFirm)
        {
            string finalSql = "SELECT a.[Firma İsmi], a.[Firma Kodu], a.[Ürün Kodu], format(a.Tarih, 'dd.MM.yyyy') as 'Tarih', a.Detaylar, a.Miktar, a.EURO, a.CHF, a.DOLAR FROM (";

            int pastYears = DateTime.Today.Year - 2010;
            for (int i = 0; i <= pastYears; i++) {
                if (itemPriceForm.GetLatestFirmCodeInt(i) > 16) {
                    finalSql += "SELECT Clcard.DEFINITION_ as 'Firma İsmi', Items.CODE as 'Firma Kodu', Items.NAME as 'Ürün Kodu', Stline.LINEEXP as Detaylar, Stline.AMOUNT as Miktar, Stline.DATE_ as Tarih, (CASE WHEN Stline.PRCURR = 0 and Stline.REPORTRATE != 0 THEN ROUND(Stline.PRICE / Stline.REPORTRATE, 2) END) AS EURO, (CASE WHEN Stline.PRCURR = 11 THEN ROUND(Stline.PRPRICE, 2) END) AS CHF, (CASE WHEN Stline.PRCURR = 1 THEN ROUND(Stline.PRPRICE, 2) END) AS DOLAR FROM LG_0" + itemPriceForm.GetLatestFirmCode(i) + "_ITEMS Items, LG_0" + itemPriceForm.GetLatestFirmCode(i) + "_01_STLINE Stline, LG_0" + itemPriceForm.GetLatestFirmCode(i) + "_01_INVOICE Invoice, LG_0" + itemPriceForm.GetLatestFirmCode(i) + "_CLCARD Clcard WHERE Items.CODE = '" + selectedItem + "' and Clcard.CODE = '" + selectedFirm + "' and Items.LOGICALREF = Stline.STOCKREF and Clcard.LOGICALREF = Invoice.CLIENTREF and Invoice.LOGICALREF = Stline.INVOICEREF and Invoice.GRPCODE = 2 and Invoice.CANCELLED = 0 ";
                }
                else {
                    finalSql += "SELECT Clcard.DEFINITION_ as 'Firma İsmi', Items.CODE as 'Firma Kodu', Items.NAME as 'Ürün Kodu', Stline.LINEEXP as Detaylar, Stline.AMOUNT as Miktar, Stline.DATE_ as Tarih, (CASE WHEN Stline.PRCURR = 0 and Stline.TRRATE != 0 THEN ROUND(Stline.PRICE / Stline.TRRATE, 2) END) AS EURO, (CASE WHEN Stline.PRCURR = 11 THEN ROUND(Stline.PRPRICE, 2) END) AS CHF, (CASE WHEN Stline.PRCURR = 1 THEN ROUND(Stline.PRPRICE, 2) END) AS DOLAR FROM LG_0" + itemPriceForm.GetLatestFirmCode(i) + "_ITEMS Items, LG_0" + itemPriceForm.GetLatestFirmCode(i) + "_01_STLINE Stline, LG_0" + itemPriceForm.GetLatestFirmCode(i) + "_01_INVOICE Invoice, LG_0" + itemPriceForm.GetLatestFirmCode(i) + "_CLCARD Clcard WHERE Items.CODE = '" + selectedItem + "' and Clcard.CODE = '" + selectedFirm + "' and Items.LOGICALREF = Stline.STOCKREF and Clcard.LOGICALREF = Invoice.CLIENTREF and Invoice.LOGICALREF = Stline.INVOICEREF and Invoice.GRPCODE = 2 and Invoice.CANCELLED = 0 ";
                }

                if (i != pastYears) {
                    finalSql += "UNION all ";
                }
            }
            finalSql += ") a ORDER BY a.Tarih DESC";

            return finalSql;
        }

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
            dt.Columns["TOTALDISCOUNTED"].ColumnName = "Toplam(ÖTV\nKDV Hariç)";
            dt.Columns["CODE"].ColumnName = "Kod";
        }
    }


    public static class DataTableExt
    {
        public static void ConvertColumnType(this DataTable dt, string columnName)
        {
            using (DataColumn dc = new DataColumn(columnName + "_new", typeof(DateTime))) {
                // Add the new column which has the new type, and move it to the ordinal of the old column
                int ordinal = dt.Columns[columnName].Ordinal;
                dt.Columns.Add(dc);
                dc.SetOrdinal(ordinal);

                // Get and convert the values of the old column, and insert them into the new
                foreach (DataRow dr in dt.Rows) {
                    dr[dc.ColumnName] = Convert.ToDateTime(DateTime.ParseExact(dr[columnName].ToString(), "dd.MM.yyyy", CultureInfo.CreateSpecificCulture("de-DE")));
                }
                
                // Remove the old column
                dt.Columns.Remove(columnName);

                // Give the new column the old column's name
                dc.ColumnName = columnName;
            }
        }
    }
}
