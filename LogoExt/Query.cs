using System;
using System.IO;
using System.Data.SqlClient;
using System.Data;
using System.Diagnostics;
using System.Collections.Generic;
using System.Globalization;

namespace LogoExt
{
    class Query
    {
        private DataTable QueryDB(string sqlCommand) {
            SqlConnection con = new SqlConnection(Global.sqlConnection);
            SqlCommand cmd = new SqlCommand(sqlCommand, con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();

            try {
                con.Open();
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
        //GTIB Kodlarını DB'den al
        public DataTable QueryGTIB(GtipForm gtipForm, DateTime date1, DateTime date2)
        {
            DataTable dt = QueryDB("SELECT Items.GTIPCODE, Clcard.TCKNO, Clcard.TAXNR, Clcard.DEFINITION_, convert(varchar, Invoice.DATE_, 104) as DATE, Invoice.FICHENO, Stline.AMOUNT, Stline.ADDTAXAMOUNT, Stline.TOTAL, Invoice.TOTALDISCOUNTED - Invoice.TOTALDISCOUNTS As TOTALDISCOUNTED, Mark.CODE FROM LG_" + Global.firmCode + "_01_INVOICE Invoice, LG_" + Global.firmCode + "_01_STLINE Stline, LG_" + Global.firmCode + "_ITEMS Items, LG_" + Global.firmCode + "_CLCARD Clcard, LG_" + Global.firmCode + "_MARK Mark WHERE Invoice.DATE_ >= '" + date1.ToString("yyyy") + "/" + date1.ToString("MM") + "/" + date1.ToString("dd") + "' and Invoice.DATE_ <= '" + date2.ToString("yyyy") + "/" + date2.ToString("MM") + "/" + date2.ToString("dd") + " 23:59:59.998' and Invoice.LOGICALREF = Stline.INVOICEREF and Items.GTIPCODE <> '' and Items.LOGICALREF = Stline.STOCKREF and Clcard.LOGICALREF = Invoice.CLIENTREF and Invoice.GRPCODE = 2 and Items.MARKREF = Mark.LOGICALREF and Invoice.CANCELLED = 0 ORDER BY Invoice.DATE_, Invoice.FICHENO, Items.GTIPCODE");
            dt.TableName = "Table";
            return dt;
        }

        //Firma kodlarını DB'den al
        public List<String> QueryFirmCodes()
        {
            DataTable dt = QueryDB("SELECT CODE FROM LG_0" + Global.Instance.GetFirmCodeCurrentYear() + "_CLCARD");
            List<String> columnData = new List<String>();
            foreach (DataRow row in dt.Rows) {
                columnData.Add(row[0].ToString());
            }
            return columnData;
        }

        //Malzeme kodlarını DB'den al
        public List<String> QueryItemCodes()
        {
            DataTable dt = QueryDB("SELECT CODE FROM LG_0" + Global.Instance.GetFirmCodeCurrentYear() + "_ITEMS");
            List<String> columnData = new List<String>();
            foreach (DataRow row in dt.Rows) {
                columnData.Add(row[0].ToString());
            }
            return columnData;
        }

        //Malzeme kodlarını DB'den al
        public DataTable QueryFirmDetails(string selectedFirm)
        {
            DataTable dt = QueryDB("SELECT EMAILADDR, TELCODES1 , TELNRS1 FROM LG_0" + Global.Instance.GetFirmCodeCurrentYear() + "_CLCARD WHERE CODE = '" + selectedFirm + "'");
            return dt;
        }

        //Malzeme fiyatlarını malzeme koduyla sorgula
        public DataTable QueryItemPriceByItem(ItemPriceForm itemPriceForm, string selectedItem)
        {
            DataTable dt; 
            if (itemPriceForm.GetFirmCodeInt(0) > 16) {
                dt = QueryDB("SELECT Clcard.DEFINITION_ as 'Firma İsmi', Items.CODE as 'Ürün Kodu', Items.NAME as 'Ürün İsmi', Stline.LINEEXP as Detaylar, Stline.AMOUNT as Miktar, format(Stline.DATE_, 'dd.MM.yyyy') as 'Tarih', (CASE WHEN Stline.PRCURR = 0 and Stline.REPORTRATE != 0 THEN ROUND(Stline.PRICE / Stline.REPORTRATE, 2) END) AS EURO, (CASE WHEN Stline.PRCURR = 11 THEN ROUND(Stline.PRPRICE, 2) END) AS CHF, (CASE WHEN Stline.PRCURR = 1 THEN ROUND(Stline.PRPRICE, 2) END) AS DOLAR FROM LG_0" + itemPriceForm.GetFirmCode(0) + "_ITEMS Items, LG_0" + itemPriceForm.GetFirmCode(0) + "_01_STLINE Stline, LG_0" + itemPriceForm.GetFirmCode(0) + "_01_INVOICE Invoice, LG_0" + itemPriceForm.GetFirmCode(0) + "_CLCARD Clcard WHERE Items.CODE = '" + selectedItem + "' and Items.LOGICALREF = Stline.STOCKREF and Clcard.LOGICALREF = Invoice.CLIENTREF and Invoice.LOGICALREF = Stline.INVOICEREF and Invoice.GRPCODE = 2 and Invoice.CANCELLED = 0 ORDER BY Stline.DATE_ DESC");
            }
            else {
                dt = QueryDB("SELECT Clcard.DEFINITION_ as 'Firma İsmi', Items.CODE as 'Ürün Kodu', Items.NAME as 'Ürün İsmi', Stline.LINEEXP as Detaylar, Stline.AMOUNT as Miktar, format(Stline.DATE_, 'dd.MM.yyyy') as 'Tarih', (CASE WHEN Stline.PRCURR = 0 and Stline.TRRATE != 0 THEN ROUND(Stline.PRICE / Stline.TRRATE, 2) END) AS EURO, (CASE WHEN Stline.PRCURR = 11 THEN ROUND(Stline.PRPRICE, 2) END) AS CHF, (CASE WHEN Stline.PRCURR = 1 THEN ROUND(Stline.PRPRICE, 2) END) AS DOLAR FROM LG_0" + itemPriceForm.GetFirmCode(0) + "_ITEMS Items, LG_0" + itemPriceForm.GetFirmCode(0) + "_01_STLINE Stline, LG_0" + itemPriceForm.GetFirmCode(0) + "_01_INVOICE Invoice, LG_0" + itemPriceForm.GetFirmCode(0) + "_CLCARD Clcard WHERE Items.CODE = '" + selectedItem + "' and Items.LOGICALREF = Stline.STOCKREF and Clcard.LOGICALREF = Invoice.CLIENTREF and Invoice.LOGICALREF = Stline.INVOICEREF and Invoice.GRPCODE = 2 and Invoice.CANCELLED = 0 ORDER BY Stline.DATE_ DESC");
            }
            return dt;
        }

        //Malzeme fiyatlarını firmaya göre sorgula
        public DataTable QueryItemPriceByFirm(ItemPriceForm itemPriceForm, string selectedFirm)
        {
            DataTable dt;   
            if (itemPriceForm.GetFirmCodeInt(0) > 16) {
                dt = QueryDB("SELECT Clcard.DEFINITION_ as 'Firma İsmi', Items.CODE as 'Ürün Kodu', Items.NAME as 'Ürün İsmi', Stline.LINEEXP as Detaylar, Stline.AMOUNT as Miktar, format(Stline.DATE_, 'dd.MM.yyyy') as 'Tarih', (CASE WHEN Stline.PRCURR = 0 and Stline.REPORTRATE != 0 THEN ROUND(Stline.PRICE / Stline.REPORTRATE, 2) END) AS EURO, (CASE WHEN Stline.PRCURR = 11 THEN ROUND(Stline.PRPRICE, 2) END) AS CHF, (CASE WHEN Stline.PRCURR = 1 THEN ROUND(Stline.PRPRICE, 2) END) AS DOLAR FROM LG_0" + itemPriceForm.GetFirmCode(0) + "_ITEMS Items, LG_0" + itemPriceForm.GetFirmCode(0) + "_01_STLINE Stline, LG_0" + itemPriceForm.GetFirmCode(0) + "_01_INVOICE Invoice, LG_0" + itemPriceForm.GetFirmCode(0) + "_CLCARD Clcard WHERE Clcard.CODE = '" + selectedFirm + "' and Items.LOGICALREF = Stline.STOCKREF and Clcard.LOGICALREF = Invoice.CLIENTREF and Invoice.LOGICALREF = Stline.INVOICEREF and Invoice.GRPCODE = 2 and Invoice.CANCELLED = 0 ORDER BY Stline.DATE_ DESC");
            }
            else {
                dt = QueryDB("SELECT Clcard.DEFINITION_ as 'Firma İsmi', Items.CODE as 'Ürün Kodu', Items.NAME as 'Ürün İsmi', Stline.LINEEXP as Detaylar, Stline.AMOUNT as Miktar, format(Stline.DATE_, 'dd.MM.yyyy') as 'Tarih', (CASE WHEN Stline.PRCURR = 0 and Stline.TRRATE != 0 THEN ROUND(Stline.PRICE / Stline.TRRATE, 2) END) AS EURO, (CASE WHEN Stline.PRCURR = 11 THEN ROUND(Stline.PRPRICE, 2) END) AS CHF, (CASE WHEN Stline.PRCURR = 1 THEN ROUND(Stline.PRPRICE, 2) END) AS DOLAR FROM LG_0" + itemPriceForm.GetFirmCode(0) + "_ITEMS Items, LG_0" + itemPriceForm.GetFirmCode(0) + "_01_STLINE Stline, LG_0" + itemPriceForm.GetFirmCode(0) + "_01_INVOICE Invoice, LG_0" + itemPriceForm.GetFirmCode(0) + "_CLCARD Clcard WHERE Clcard.CODE = '" + selectedFirm + "' and Items.LOGICALREF = Stline.STOCKREF and Clcard.LOGICALREF = Invoice.CLIENTREF and Invoice.LOGICALREF = Stline.INVOICEREF and Invoice.GRPCODE = 2 and Invoice.CANCELLED = 0 ORDER BY Stline.DATE_ DESC");
            }
            return dt;
        }

        //Bir malzemenin verilen firmadaki o yılki bütün satışları
        public DataTable QueryItemPriceByFirmAndItem(ItemPriceForm itemPriceForm, string selectedItem, string selectedFirm)
        {
            DataTable dt;
            if (itemPriceForm.GetFirmCodeInt(0) > 16) {
                dt = QueryDB("SELECT Clcard.DEFINITION_ as 'Firma İsmi', Items.CODE as 'Ürün Kodu', Items.NAME as 'Ürün İsmi', Stline.LINEEXP as Detaylar, Stline.AMOUNT as Miktar, format(Stline.DATE_, 'dd.MM.yyyy') as 'Tarih', (CASE WHEN Stline.PRCURR = 0 and Stline.REPORTRATE != 0 THEN ROUND(Stline.PRICE / Stline.REPORTRATE, 2) END) AS EURO, (CASE WHEN Stline.PRCURR = 11 THEN ROUND(Stline.PRPRICE, 2) END) AS CHF, (CASE WHEN Stline.PRCURR = 1 THEN ROUND(Stline.PRPRICE, 2) END) AS DOLAR FROM LG_0" + itemPriceForm.GetFirmCode(0) + "_ITEMS Items, LG_0" + itemPriceForm.GetFirmCode(0) + "_01_STLINE Stline, LG_0" + itemPriceForm.GetFirmCode(0) + "_01_INVOICE Invoice, LG_0" + itemPriceForm.GetFirmCode(0) + "_CLCARD Clcard WHERE Items.CODE = '" + selectedItem + "' and Clcard.CODE = '" + selectedFirm + "' and Items.LOGICALREF = Stline.STOCKREF and Clcard.LOGICALREF = Invoice.CLIENTREF and Invoice.LOGICALREF = Stline.INVOICEREF and Invoice.GRPCODE = 2 and Invoice.CANCELLED = 0 ORDER BY Stline.DATE_ DESC");
            }
            else {
                dt = QueryDB("SELECT Clcard.DEFINITION_ as 'Firma İsmi', Items.CODE as 'Ürün Kodu', Items.NAME as 'Ürün İsmi', Stline.LINEEXP as Detaylar, Stline.AMOUNT as Miktar, format(Stline.DATE_, 'dd.MM.yyyy') as 'Tarih', (CASE WHEN Stline.PRCURR = 0 and Stline.TRRATE != 0 THEN ROUND(Stline.PRICE / Stline.TRRATE, 2) END) AS EURO, (CASE WHEN Stline.PRCURR = 11 THEN ROUND(Stline.PRPRICE, 2) END) AS CHF, (CASE WHEN Stline.PRCURR = 1 THEN ROUND(Stline.PRPRICE, 2) END) AS DOLAR FROM LG_0" + itemPriceForm.GetFirmCode(0) + "_ITEMS Items, LG_0" + itemPriceForm.GetFirmCode(0) + "_01_STLINE Stline, LG_0" + itemPriceForm.GetFirmCode(0) + "_01_INVOICE Invoice, LG_0" + itemPriceForm.GetFirmCode(0) + "_CLCARD Clcard WHERE Items.CODE = '" + selectedItem + "' and Clcard.CODE = '" + selectedFirm + "' and Items.LOGICALREF = Stline.STOCKREF and Clcard.LOGICALREF = Invoice.CLIENTREF and Invoice.LOGICALREF = Stline.INVOICEREF and Invoice.GRPCODE = 2 and Invoice.CANCELLED = 0 ORDER BY Stline.DATE_ DESC");
            }
            return dt;
        }

        //Bir malzemenin son 10 yıldaki fiyat hareketleri
        public DataTable QueryPastTenYearsPriceByItem(ItemPriceForm itemPriceForm, string selectedItem)
        {
            DataTable dt = QueryDB(CreateQueryItemDataSince2010(itemPriceForm, selectedItem));
            return dt;
        }

        //Bir firmanın son 10 yıldaki fiyat hareketleri
        public DataTable QueryPastTenYearsPriceByFirm(ItemPriceForm itemPriceForm, string selectedFirm)
        {
            DataTable dt = QueryDB(CreateQueryFirmDataSince2010(itemPriceForm, selectedFirm));
            return dt;
        }
        
        //Bir firmanın ve bir malzemenin son 10 yıldaki fiyat hareketleri
        public DataTable QueryPastTenYearsPriceByItemAndFirm(ItemPriceForm itemPriceForm, string selectedItem, string selectedFirm)
        {
            DataTable dt = QueryDB(CreateQueryItemAndFirmDataSince2010(itemPriceForm, selectedItem, selectedFirm));
            return dt;
        }

        //Firmanın ekstresini sorgula
        public DataTable QueryEkstre(string selectedFirm)
        {
            DataTable dt = QueryDB("SELECT format(a.Tarih, 'dd.MM.yyyy') as 'Tarih', a.[Fiş No], a.TRCODE, a.PAYMENTREF, a.Açıklama, a.Borç, a.Alacak, a.SIGN FROM (SELECT Clfline.DATE_ as 'Tarih', Clfline.DOCODE as 'Fiş No', Clfline.TRCODE, Clcard.PAYMENTREF,  Clfline.LINEEXP as 'Açıklama', Clfline.SIGN, (CASE WHEN Clfline.SIGN = 0 THEN FORMAT(Clfline.AMOUNT, '####,###.00') END) AS 'Borç', (CASE WHEN Clfline.SIGN = 1 THEN FORMAT(Clfline.AMOUNT, '####,###.00') END) AS 'Alacak' FROM LG_0" + Global.Instance.GetFirmCodeAnotherYear(1) + "_01_CLFLINE Clfline, LG_0" + Global.Instance.GetFirmCodeAnotherYear(1) + "_CLCARD Clcard WHERE Clcard.CODE = '" + selectedFirm + "' and Clcard.LOGICALREF = Clfline.CLIENTREF and Clfline.CANCELLED = 0 UNION all SELECT Clfline.DATE_ as 'Tarih', Clfline.DOCODE as 'Fiş No', Clfline.TRCODE, Clcard.PAYMENTREF,  Clfline.LINEEXP as 'Açıklama', Clfline.SIGN, (CASE WHEN Clfline.SIGN = 0 THEN FORMAT(Clfline.AMOUNT, '####,###.00') END) AS 'Borç', (CASE WHEN Clfline.SIGN = 1 THEN FORMAT(Clfline.AMOUNT, '####,###.00') END) AS 'Alacak' FROM LG_0" + Global.Instance.GetFirmCodeAnotherYear(0) + "_01_CLFLINE Clfline, LG_0" + Global.Instance.GetFirmCodeAnotherYear(0) + "_CLCARD Clcard WHERE Clcard.CODE = '" + selectedFirm + "' and Clcard.LOGICALREF = Clfline.CLIENTREF and Clfline.CANCELLED = 0 and Clfline.TRCODE != 14) a");                    
            return dt; 
        }

        //Firmanın Vadesini sorgula
        public DataTable QueryVade(string payplanref)
        {
            DataTable dt = QueryDB("SELECT DAY_ FROM LG_0" + Global.Instance.GetFirmCodeAnotherYear(0) + "_PAYLINES Where PAYPLANREF =" + payplanref);
            return dt;
        }

        //Get data of a specific item since 2010
        private string CreateQueryItemDataSince2010(ItemPriceForm itemPriceForm, string selectedItem)
        {
            string finalSql = "SELECT a.[Firma İsmi], a.[Ürün Kodu], a.[Ürün İsmi], format(a.Tarih, 'dd.MM.yyyy') as 'Tarih', a.Detaylar, a.Miktar, a.EURO, a.CHF, a.DOLAR FROM (";

            int pastYears = DateTime.Today.Year - 2010;
            for(int i = 0; i <= pastYears; i++) {
                if (itemPriceForm.GetLatestFirmCodeInt(i) > 16) {
                    finalSql += "SELECT Clcard.DEFINITION_ as 'Firma İsmi', Items.CODE as 'Ürün Kodu', Items.NAME as 'Ürün İsmi', Stline.LINEEXP as Detaylar, Stline.AMOUNT as Miktar, Stline.DATE_ as Tarih, (CASE WHEN Stline.PRCURR = 0 and Stline.REPORTRATE != 0 THEN ROUND(Stline.PRICE / Stline.REPORTRATE, 2) END) AS EURO, (CASE WHEN Stline.PRCURR = 11 THEN ROUND(Stline.PRPRICE, 2) END) AS CHF, (CASE WHEN Stline.PRCURR = 1 THEN ROUND(Stline.PRPRICE, 2) END) AS DOLAR FROM LG_0" + itemPriceForm.GetLatestFirmCode(i) + "_ITEMS Items, LG_0" + itemPriceForm.GetLatestFirmCode(i) + "_01_STLINE Stline, LG_0" + itemPriceForm.GetLatestFirmCode(i) + "_01_INVOICE Invoice, LG_0" + itemPriceForm.GetLatestFirmCode(i) + "_CLCARD Clcard WHERE Items.CODE = '" + selectedItem + "' and Items.LOGICALREF = Stline.STOCKREF and Clcard.LOGICALREF = Invoice.CLIENTREF and Invoice.LOGICALREF = Stline.INVOICEREF and Invoice.GRPCODE = 2 and Invoice.CANCELLED = 0 ";
                }
                else {
                    finalSql += "SELECT Clcard.DEFINITION_ as 'Firma İsmi', Items.CODE as 'Ürün Kodu', Items.NAME as 'Ürün İsmi', Stline.LINEEXP as Detaylar, Stline.AMOUNT as Miktar, Stline.DATE_ as Tarih, (CASE WHEN Stline.PRCURR = 0 and Stline.TRRATE != 0 THEN ROUND(Stline.PRICE / Stline.TRRATE, 2) END) AS EURO, (CASE WHEN Stline.PRCURR = 11 THEN ROUND(Stline.PRPRICE, 2) END) AS CHF, (CASE WHEN Stline.PRCURR = 1 THEN ROUND(Stline.PRPRICE, 2) END) AS DOLAR FROM LG_0" + itemPriceForm.GetLatestFirmCode(i) + "_ITEMS Items, LG_0" + itemPriceForm.GetLatestFirmCode(i) + "_01_STLINE Stline, LG_0" + itemPriceForm.GetLatestFirmCode(i) + "_01_INVOICE Invoice, LG_0" + itemPriceForm.GetLatestFirmCode(i) + "_CLCARD Clcard WHERE Items.CODE = '" + selectedItem + "' and Items.LOGICALREF = Stline.STOCKREF and Clcard.LOGICALREF = Invoice.CLIENTREF and Invoice.LOGICALREF = Stline.INVOICEREF and Invoice.GRPCODE = 2 and Invoice.CANCELLED = 0 ";
                }

                if (i != pastYears) {
                    finalSql += "UNION all ";
                }
            }
            finalSql += ") a ORDER BY a.Tarih DESC";

            return finalSql;
        }

        //Get data of a specific firm since 2010
        private string CreateQueryFirmDataSince2010(ItemPriceForm itemPriceForm, string selectedFirm)
        {
            string finalSql = "SELECT a.[Firma İsmi], a.[Ürün Kodu], a.[Ürün İsmi], format(a.Tarih, 'dd.MM.yyyy') as 'Tarih', a.Detaylar, a.Miktar, a.EURO, a.CHF, a.DOLAR FROM (";

            int pastYears = DateTime.Today.Year - 2010;
            for (int i = 0; i <= pastYears; i++) {
                if (itemPriceForm.GetLatestFirmCodeInt(i) > 16) {
                    finalSql += "SELECT Clcard.DEFINITION_ as 'Firma İsmi', Items.CODE as 'Ürün Kodu', Items.NAME as 'Ürün İsmi', Stline.LINEEXP as Detaylar, Stline.AMOUNT as Miktar, Stline.DATE_ as Tarih, (CASE WHEN Stline.PRCURR = 0 and Stline.REPORTRATE != 0 THEN ROUND(Stline.PRICE / Stline.REPORTRATE, 2) END) AS EURO, (CASE WHEN Stline.PRCURR = 11 THEN ROUND(Stline.PRPRICE, 2) END) AS CHF, (CASE WHEN Stline.PRCURR = 1 THEN ROUND(Stline.PRPRICE, 2) END) AS DOLAR FROM LG_0" + itemPriceForm.GetLatestFirmCode(i) + "_ITEMS Items, LG_0" + itemPriceForm.GetLatestFirmCode(i) + "_01_STLINE Stline, LG_0" + itemPriceForm.GetLatestFirmCode(i) + "_01_INVOICE Invoice, LG_0" + itemPriceForm.GetLatestFirmCode(i) + "_CLCARD Clcard WHERE Clcard.CODE = '" + selectedFirm + "' and Items.LOGICALREF = Stline.STOCKREF and Clcard.LOGICALREF = Invoice.CLIENTREF and Invoice.LOGICALREF = Stline.INVOICEREF and Invoice.GRPCODE = 2 and Invoice.CANCELLED = 0 ";
                }
                else {
                    finalSql += "SELECT Clcard.DEFINITION_ as 'Firma İsmi', Items.CODE as 'Ürün Kodu', Items.NAME as 'Ürün İsmi', Stline.LINEEXP as Detaylar, Stline.AMOUNT as Miktar, Stline.DATE_ as Tarih, (CASE WHEN Stline.PRCURR = 0 and Stline.TRRATE != 0 THEN ROUND(Stline.PRICE / Stline.TRRATE, 2) END) AS EURO, (CASE WHEN Stline.PRCURR = 11 THEN ROUND(Stline.PRPRICE, 2) END) AS CHF, (CASE WHEN Stline.PRCURR = 1 THEN ROUND(Stline.PRPRICE, 2) END) AS DOLAR FROM LG_0" + itemPriceForm.GetLatestFirmCode(i) + "_ITEMS Items, LG_0" + itemPriceForm.GetLatestFirmCode(i) + "_01_STLINE Stline, LG_0" + itemPriceForm.GetLatestFirmCode(i) + "_01_INVOICE Invoice, LG_0" + itemPriceForm.GetLatestFirmCode(i) + "_CLCARD Clcard WHERE Clcard.CODE = '" + selectedFirm + "' and Items.LOGICALREF = Stline.STOCKREF and Clcard.LOGICALREF = Invoice.CLIENTREF and Invoice.LOGICALREF = Stline.INVOICEREF and Invoice.GRPCODE = 2 and Invoice.CANCELLED = 0 ";
                }

                if (i != pastYears) {
                    finalSql += "UNION all ";
                }
            }
            finalSql += ") a ORDER BY a.Tarih DESC";

            return finalSql;
        }

        //Get data of a specific firm and specific item since 2010
        private string CreateQueryItemAndFirmDataSince2010(ItemPriceForm itemPriceForm, string selectedItem, string selectedFirm)
        {
            string finalSql = "SELECT a.[Firma İsmi], a.[Ürün Kodu], a.[Ürün İsmi], format(a.Tarih, 'dd.MM.yyyy') as 'Tarih', a.Detaylar, a.Miktar, a.EURO, a.CHF, a.DOLAR FROM (";

            int pastYears = DateTime.Today.Year - 2010;
            for (int i = 0; i <= pastYears; i++) {
                if (itemPriceForm.GetLatestFirmCodeInt(i) > 16) {
                    finalSql += "SELECT Clcard.DEFINITION_ as 'Firma İsmi', Items.CODE as 'Ürün Kodu', Items.NAME as 'Ürün İsmi', Stline.LINEEXP as Detaylar, Stline.AMOUNT as Miktar, Stline.DATE_ as Tarih, (CASE WHEN Stline.PRCURR = 0 and Stline.REPORTRATE != 0 THEN ROUND(Stline.PRICE / Stline.REPORTRATE, 2) END) AS EURO, (CASE WHEN Stline.PRCURR = 11 THEN ROUND(Stline.PRPRICE, 2) END) AS CHF, (CASE WHEN Stline.PRCURR = 1 THEN ROUND(Stline.PRPRICE, 2) END) AS DOLAR FROM LG_0" + itemPriceForm.GetLatestFirmCode(i) + "_ITEMS Items, LG_0" + itemPriceForm.GetLatestFirmCode(i) + "_01_STLINE Stline, LG_0" + itemPriceForm.GetLatestFirmCode(i) + "_01_INVOICE Invoice, LG_0" + itemPriceForm.GetLatestFirmCode(i) + "_CLCARD Clcard WHERE Items.CODE = '" + selectedItem + "' and Clcard.CODE = '" + selectedFirm + "' and Items.LOGICALREF = Stline.STOCKREF and Clcard.LOGICALREF = Invoice.CLIENTREF and Invoice.LOGICALREF = Stline.INVOICEREF and Invoice.GRPCODE = 2 and Invoice.CANCELLED = 0 ";
                }
                else {
                    finalSql += "SELECT Clcard.DEFINITION_ as 'Firma İsmi', Items.CODE as 'Ürün Kodu', Items.NAME as 'Ürün İsmi', Stline.LINEEXP as Detaylar, Stline.AMOUNT as Miktar, Stline.DATE_ as Tarih, (CASE WHEN Stline.PRCURR = 0 and Stline.TRRATE != 0 THEN ROUND(Stline.PRICE / Stline.TRRATE, 2) END) AS EURO, (CASE WHEN Stline.PRCURR = 11 THEN ROUND(Stline.PRPRICE, 2) END) AS CHF, (CASE WHEN Stline.PRCURR = 1 THEN ROUND(Stline.PRPRICE, 2) END) AS DOLAR FROM LG_0" + itemPriceForm.GetLatestFirmCode(i) + "_ITEMS Items, LG_0" + itemPriceForm.GetLatestFirmCode(i) + "_01_STLINE Stline, LG_0" + itemPriceForm.GetLatestFirmCode(i) + "_01_INVOICE Invoice, LG_0" + itemPriceForm.GetLatestFirmCode(i) + "_CLCARD Clcard WHERE Items.CODE = '" + selectedItem + "' and Clcard.CODE = '" + selectedFirm + "' and Items.LOGICALREF = Stline.STOCKREF and Clcard.LOGICALREF = Invoice.CLIENTREF and Invoice.LOGICALREF = Stline.INVOICEREF and Invoice.GRPCODE = 2 and Invoice.CANCELLED = 0 ";
                }

                if (i != pastYears) {
                    finalSql += "UNION all ";
                }
            }
            finalSql += ") a ORDER BY a.Tarih DESC";

            return finalSql;
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
