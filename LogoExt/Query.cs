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

            catch (SqlException ex) {
                Console.WriteLine(ex.Message + ": " + ex.StackTrace);
                LogWriter.Instance.LogWrite(ex.Message + ": " + ex.StackTrace);
                Global.Instance.ErrorNotification("Data base sorgu hatası: --- " + ex.Message);
                return null;

            }
            catch (Exception ex) {
                Console.WriteLine(ex.Message + ": " + ex.StackTrace);
                LogWriter.Instance.LogWrite(ex.Message + ": " + ex.StackTrace);
                Global.Instance.ErrorNotification("Data base sorgu hatası: --- " + ex.Message);
                return null;
            }
        }
        //Vergi bildirimi bölümü B cetveli teslimi
        public DataTable QueryBCetveliTeslimGTIP(GtipForm gtipForm, DateTime date1, DateTime date2)
        {
            DataTable dt = QueryDB("SELECT Items.CODE AS 'Malzeme Kodu', Items.GTIPCODE, Clcard.TCKNO, Clcard.TAXNR, Clcard.DEFINITION_, convert(varchar, Invoice.DATE_, 104) as DATE, Invoice.FICHENO, Stline.AMOUNT, Stline.ADDTAXAMOUNT, Stline.TOTAL, Invoice.TOTALDISCOUNTED - Invoice.TOTALDISCOUNTS As TOTALDISCOUNTED, Mark.CODE, Items.NAME4 AS 'Beyanname No' FROM LG_" + Global.firmCode + "_01_INVOICE Invoice, LG_" + Global.firmCode + "_01_STLINE Stline, LG_" + Global.firmCode + "_ITEMS Items, LG_" + Global.firmCode + "_CLCARD Clcard, LG_" + Global.firmCode + "_MARK Mark WHERE Invoice.DATE_ >= '" + date1.ToString("yyyy") + "/" + date1.ToString("MM") + "/" + date1.ToString("dd") + "' and Invoice.DATE_ <= '" + date2.ToString("yyyy") + "/" + date2.ToString("MM") + "/" + date2.ToString("dd") + " 23:59:59.998' and Invoice.LOGICALREF = Stline.INVOICEREF and Items.GTIPCODE <> '' and Items.LOGICALREF = Stline.STOCKREF and Clcard.LOGICALREF = Invoice.CLIENTREF and Invoice.GRPCODE = 2 and Items.MARKREF = Mark.LOGICALREF and Invoice.CANCELLED = 0 ORDER BY Invoice.DATE_, Invoice.FICHENO, Items.GTIPCODE");
            if (dt == null) { return null; }
            dt.TableName = "Vergi bildirimi bölümü B cetveli teslimi";
            return dt;
        }

        //B Cetvelindeki Ürünler
        public DataTable QueryBCetveliUrunlerGTIP(GtipForm gtipForm, DateTime date1, DateTime date2)
        {
            DataTable dt = QueryDB("SELECT a.DESCR as 'Teslim Edilen Mal G.T.İ.P No.', a.[Toplam Teslim Edilen Mal Miktarı], a.[Hesaplanan ÖTV], a.[Teslim Bedeli(ÖTV ve KDV Hariç)] FROM (SELECT Mark.DESCR, SUM(CASE WHEN Mark.CODE = '03100' THEN Stline.AMOUNT / 10 WHEN Mark.CODE = '04200' THEN Stline.AMOUNT / 2 WHEN Mark.CODE = '05500' THEN Stline.AMOUNT / 5  ELSE Stline.AMOUNT END) as 'Toplam Teslim Edilen Mal Miktarı', SUM(Stline.ADDTAXAMOUNT) as 'Hesaplanan ÖTV', SUM(Stline.TOTAL) as 'Teslim Bedeli(ÖTV ve KDV Hariç)' FROM LG_" + Global.firmCode + "_ITEMS Items, LG_" + Global.firmCode + "_01_STLINE Stline, LG_" + Global.firmCode + "_01_INVOICE Invoice, LG_" + Global.firmCode + "_MARK Mark wHERE Invoice.DATE_ >= '" + date1.ToString("yyyy") + "/" + date1.ToString("MM") + "/" + date1.ToString("dd") + "' and Invoice.DATE_ <= '" + date2.ToString("yyyy") + "/" + date2.ToString("MM") + "/" + date2.ToString("dd") + " 23:59:59.998' and Invoice.LOGICALREF = Stline.INVOICEREF and Items.MARKREF = Mark.LOGICALREF and Items.LOGICALREF = Stline.STOCKREF and Invoice.CANCELLED = 0 and Invoice.GRPCODE = 2 and Invoice.TRCODE = 8 and Items.NAME4<> '' GROUP BY Mark.DESCR UNION all SELECT Mark.DESCR, SUM(Stline.Amount) as 'Toplam Teslim Edilen Mal Miktarı', SUM(Stline.ADDTAXAMOUNT) as 'Hesaplanan ÖTV', SUM(Stline.TOTAL) as 'Teslim Bedeli(ÖTV ve KDV Hariç)' FROM LG_" + Global.firmCode + "_ITEMS Items, LG_" + Global.firmCode + "_01_STLINE Stline, LG_" + Global.firmCode + "_01_STFICHE Stfiche, LG_" + Global.firmCode + "_MARK Mark WHERE Stfiche.DATE_ >= '" + date1.ToString("yyyy") + "/" + date1.ToString("MM") + "/" + date1.ToString("dd") + "' and Stfiche.DATE_ <= '" + date2.ToString("yyyy") + "/" + date2.ToString("MM") + "/" + date2.ToString("dd") + " 23:59:59.998' and Stfiche.LOGICALREF = Stline.STFICHEREF and Items.LOGICALREF = Stline.STOCKREF and Items.MARKREF = Mark.LOGICALREF and Stline.TRCODE = 12 and Items.GTIPCODE<> '' GROUP BY Mark.DESCR)a ORDER BY a.DESCR");
            if (dt == null) { return null; }
            dt.TableName = "B Cetvelindeki Ürünler";
            return dt;
        }

        //B cetvelindeki mallar
        public DataTable QueryBCetveliMallarGTIP(GtipForm gtipForm, DateTime date1, DateTime date2)
        {
            DataTable dt = QueryDB("SELECT Mark.DESCR as 'G.T.İ.P No', Items.NAME4 as 'Gümrük Beyannamesi Tescil No', SUM(CASE WHEN Mark.CODE = '03100' THEN Stline.AMOUNT / 10 WHEN Mark.CODE = '04200' THEN Stline.AMOUNT / 2 WHEN Mark.CODE = '05500' THEN Stline.AMOUNT / 5 ELSE Stline.AMOUNT END) as 'Teslim Edilen Mal Miktarı' FROM  LG_" + Global.firmCode + "_ITEMS Items, LG_" + Global.firmCode + "_MARK Mark, LG_" + Global.firmCode + "_01_STLINE Stline, LG_" + Global.firmCode + "_01_INVOICE Invoice WHERE Invoice.DATE_ >= '" + date1.ToString("yyyy") + "/" + date1.ToString("MM") + "/" + date1.ToString("dd") + "' and Invoice.DATE_ <= '" + date2.ToString("yyyy") + "/" + date2.ToString("MM") + "/" + date2.ToString("dd") + " 23:59:59.998' and Invoice.LOGICALREF = Stline.INVOICEREF and Items.MARKREF = Mark.LOGICALREF and Items.LOGICALREF = Stline.STOCKREF and Invoice.CANCELLED = 0 and Invoice.GRPCODE = 2 and Invoice.TRCODE = 8 and Items.NAME4 <> '' GROUP BY Items.NAME4, Mark.DESCR ORDER BY Items.NAME4");
            if (dt == null) { return null; }
            dt.TableName = "B cetvelindeki mallar";
            return dt;
        }


        //Ayrıntılı satış raporu
        public DataTable QueryAyrintiliRaporGTIP(GtipForm gtipForm, DateTime date1, DateTime date2)
        {
            DataTable dt = QueryDB("SELECT Items.CODE as'Malzeme(Sınıfı)Kodu', Items.NAME as 'Malzeme(Sınıfı)Açıklaması', SUM(CASE WHEN Mark.CODE = '03100' THEN Stline.AMOUNT / 10 WHEN Mark.CODE = '04200' THEN Stline.AMOUNT / 2 WHEN Mark.CODE = '05500' THEN Stline.AMOUNT / 5 ELSE Stline.AMOUNT END) as 'Satış', ISNULL((SELECT Stline.Amount FROM  LG_" + Global.firmCode + "_ITEMS Items, LG_" + Global.firmCode + "_MARK Mark, LG_" + Global.firmCode + "_01_STLINE Stline, LG_" + Global.firmCode + "_01_INVOICE Invoice wHERE Invoice.DATE_ >= '" + date1.ToString("yyyy") + "/" + date1.ToString("MM") + "/" + date1.ToString("dd") + "' and Invoice.DATE_ <= '" + date2.ToString("yyyy") + "/" + date2.ToString("MM") + "/" + date2.ToString("dd") + " 23:59:59.998' and Invoice.LOGICALREF = Stline.INVOICEREF and Items.MARKREF = Mark.LOGICALREF and Items.LOGICALREF = Stline.STOCKREF and Invoice.CANCELLED = 0 and Invoice.GRPCODE = 2 and Invoice.TRCODE = 3 and Items.NAME4 <> '' ), 0) as 'İade', SUM(Stline.TOTAL) as 'Satış Tutarı', SUM (ROUND(Stline.TOTAL - ROUND(Stline.LINENET, 2), 2)) as 'İndirim Tutarı', Items.NAME4 as 'Beyanname No', Mark.DESCR as 'G.T.İ.P No' FROM  LG_" + Global.firmCode + "_ITEMS Items, LG_" + Global.firmCode + "_MARK Mark, LG_" + Global.firmCode + "_01_STLINE Stline, LG_" + Global.firmCode + "_01_INVOICE Invoice wHERE Invoice.DATE_ >= '" + date1.ToString("yyyy") + "/" + date1.ToString("MM") + "/" + date1.ToString("dd") + "' and Invoice.DATE_ <= '" + date2.ToString("yyyy") + "/" + date2.ToString("MM") + "/" + date2.ToString("dd") + " 23:59:59.998' and Invoice.LOGICALREF = Stline.INVOICEREF and Items.MARKREF = Mark.LOGICALREF and Items.LOGICALREF = Stline.STOCKREF and Invoice.CANCELLED = 0 and Invoice.GRPCODE = 2 and Invoice.TRCODE = 8 and Items.NAME4 <> '' GROUP BY Items.NAME4, Mark.DESCR, Items.NAME, Items.CODE ORDER BY Items.CODE, Items.NAME4, Items.NAME");
            if (dt == null) { return null; }
            dt.TableName = "Ayrıntılı satış raporu";
            return dt;
        }


        //Sarf fişleri
        public DataTable QuerySarfFisleriGTIP(GtipForm gtipForm, DateTime date1, DateTime date2)
        {
            DataTable dt = QueryDB("SELECT Items.CODE as'Kodu', Items.NAME as 'Açıklaması', Stline.Amount as 'Miktar', convert(varchar, Stline.DATE_, 104) as 'Fiş Tarihi', Mark.DESCR, Items.NAME4 as 'Beyanname No' FROM LG_" + Global.firmCode + "_01_STFICHE Stfiche, LG_" + Global.firmCode + "_01_STLINE Stline, LG_" + Global.firmCode + "_ITEMS Items, LG_" + Global.firmCode + "_MARK Mark WHERE Stfiche.DATE_ >= '" + date1.ToString("yyyy") + "/" + date1.ToString("MM") + "/" + date1.ToString("dd") + "' and Stfiche.DATE_ <= '" + date2.ToString("yyyy") + "/" + date2.ToString("MM") + "/" + date2.ToString("dd") + " 23:59:59.998' and Stfiche.LOGICALREF = Stline.STFICHEREF and Items.LOGICALREF = Stline.STOCKREF and Items.MARKREF = Mark.LOGICALREF and Stline.TRCODE = 12 and Items.GTIPCODE <> ''");
            if (dt == null) { return null; }
            dt.TableName = "Sarf fişleri";
            return dt;
        }

        //Firma kodlarını DB'den al
        public List<String> QueryFirmCodes()
        {
            DataTable dt = QueryDB("SELECT CODE FROM LG_0" + Global.Instance.GetFirmCodeCurrentYear() + "_CLCARD");
            if (dt == null) {
                return null;
            }
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
            if (dt == null) {
                return null;
            }
            List<String> columnData = new List<String>();
            foreach (DataRow row in dt.Rows) {
                columnData.Add(row[0].ToString());
            }
            return columnData;
        }

        //Firma detaylarını DB'den al
        public DataTable QueryFirmDetails(string selectedFirm)
        {
            DataTable dt = QueryDB("SELECT EMAILADDR, TELCODES1 , TELNRS1 FROM LG_0" + Global.Instance.GetFirmCodeCurrentYear() + "_CLCARD WHERE CODE = '" + selectedFirm + "'");
            return dt;
        }

        //Malzeme Stoğunu DB'den al
        public DataTable QueryItems()
        {
            DataTable dt = QueryDB("SELECT Items.CODE as 'Kodu', Items.NAME 'Açıklaması', convert(DOUBLE PRECISION, convert(numeric(38,3),cast(SUM(CASE WHEN (Stline.TRCODE = 13 or Stline.TRCODE = 14) and Stline.SOURCELINK <> 0  THEN 0 WHEN (Stline.TRCODE = 8 or Stline.TRCODE = 12) and Stline.CANCELLED = 0 and Stline.LINETYPE <> 4 and Stline.UINFO1 <> 0 THEN Stline.amount/Stline.UINFO1 * -1 WHEN (Stline.TRCODE = 6 or Stline.TRCODE = 8 or Stline.TRCODE = 9 or Stline.TRCODE = 11 or Stline.TRCODE = 12) and Stline.CANCELLED = 0 and Stline.LINETYPE <> 4 THEN Stline.amount * -1 WHEN (Stline.TRCODE = 1 or Stline.TRCODE = 3 or Stline.TRCODE = 13 or Stline.TRCODE = 14 or Stline.TRCODE = 15 or Stline.TRCODE = 50) and Stline.CANCELLED = 0 and Stline.LINETYPE <> 4 THEN Stline.amount ELSE 0 END) AS FLOAT))) as 'Stok' FROM LG_0" + Global.Instance.GetFirmCodeAnotherYear(0) + "_01_STLINE Stline, LG_0" + Global.Instance.GetFirmCodeAnotherYear(0) + "_ITEMS Items WHERE Stline.STOCKREF = Items.LOGICALREF and Items.ACTIVE = 0 Group By Items.CODE, Items.NAME order by Items.CODE");
            return dt;
        }

        //Malzeme hareketleri
        public DataTable QueryItemMovements(ItemsForm itemsForm, string selectedItem)
        {
            DataTable dt = QueryDB("SELECT format(Stline.DATE_, 'dd.MM.yyyy') as 'Tarih', ISNULL((SELECT Invoice.DOCODE FROM LG_0" + itemsForm.GetFirmCode(0) + "_01_INVOICE Invoice WHERE Invoice.LOGICALREF = Stline.INVOICEREF), '') as 'Fiş Belge No.', CASE WHEN Stline.CANCELLED = 0 and Stline.LINETYPE <> 4 THEN CONVERT(varchar(10), Stline.TRCODE) END as 'Tür', ISNULL((SELECT Clcard.DEFINITION_ FROM LG_0" + itemsForm.GetFirmCode(0) + "_CLCARD Clcard WHERE Clcard.LOGICALREF = Stline.CLIENTREF), '') as 'Cari Hesap Ünvanı', Stline.LINEEXP as 'Satır Açıklaması',  Stline.AMOUNT as 'Miktar', Stline.PRICE as 'Birim Fiyat',Stline.TOTAL as 'Tutar' FROM LG_0" + itemsForm.GetFirmCode(0) + "_01_STLINE Stline, LG_0" + itemsForm.GetFirmCode(0) + "_ITEMS Items WHERE Stline.STOCKREF = Items.LOGICALREF and Items.CODE = '" + selectedItem + "' and not ((Stline.TRCODE = 13 or Stline.TRCODE = 14) and Stline.SOURCELINK <> 0) and Stline.LINETYPE IN (0, 1, 5, 6, 7, 9, 10) and Stline.CANCELLED = 0 ORDER BY Stline.DATE_");
            return dt;
        }

        //Satış faturaları
        public DataTable QueryInvoices(InvoiceForm invoiceForm)
        {
            DataTable dt = QueryDB("SELECT format(Invoice.DATE_, 'dd.MM.yyyy') as 'Tarih', Invoice.FICHENO as 'Fiş No.', Invoice.DOCODE as 'Belge No.', CASE WHEN Invoice.TRCODE = 2 THEN 'Perakende Şatış iade Faturası' WHEN Invoice.TRCODE = 3 THEN 'Toptan Şatış İade Faturası' WHEN Invoice.TRCODE = 7 THEN 'Perakende Şatış Faturası' WHEN Invoice.TRCODE = 8 THEN 'Toptan Şatış Faturası' WHEN Invoice.TRCODE = 9 THEN 'Verilen Hizmet Faturası' WHEN Invoice.TRCODE = 10 THEN 'Verilen Proforma Fatura' WHEN Invoice.TRCODE = 14 THEN 'Şatış Fiyat Farkı Faturası' END as 'Türü', Clcard.DEFINITION_ as 'Müşteri Ünvanı', FORMAT(Invoice.NETTOTAL, '####,##0.00') as 'Turar', FORMAT(Invoice.REPORTNET, '####,##0.00') as 'Dövizli Turar', CASE WHEN Invoice.EINVOICE = 1 THEN (CASE WHEN Invoice.Estatus = 0 THEN 'GİB''e Gönderilecek' WHEN Invoice.Estatus = 10 THEN 'Alıcıda İşlendi - Başarıyla Tamamlandı' WHEN Invoice.Estatus = 12 THEN 'Kabul Edildi' WHEN Invoice.Estatus = 13 THEN 'Reddedildi' ELSE '********HATALI********' END) WHEN Invoice.EINVOICE = 2 THEN (CASE WHEN Invoice.Estatus = 0 THEN 'E-arşiv Fatura Oluşturulacak' WHEN Invoice.Estatus = 2 THEN 'Rapor Dosyasına Yazıldı' ELSE '********HATALI********' END) END as 'E-Fatura Statüsü' FROM LG_0" + invoiceForm.GetFirmCode(0) + "_01_INVOICE Invoice, LG_0" + invoiceForm.GetFirmCode(0) + "_CLCARD Clcard WHERE Invoice.TRCODE in (2, 3, 7, 8, 9, 10, 14) and Clcard.LOGICALREF = Invoice.CLIENTREF ORDER BY Invoice.DATE_, Invoice.FICHENO");
            return dt;
        }

        //Malzeme fiyatlarını malzeme koduyla sorgula
        public DataTable QueryItemPriceByItem(ItemPriceForm itemPriceForm, string selectedItem)
        {
            DataTable dt; 
            if (itemPriceForm.GetFirmCodeInt(0) > 16) {
                dt = QueryDB("SELECT Clcard.DEFINITION_ as 'Firma İsmi', Items.CODE as 'Ürün Kodu', Items.NAME as 'Ürün İsmi', Stline.LINEEXP as Detaylar, Stline.AMOUNT as Miktar, format(Stline.DATE_, 'dd.MM.yyyy') as 'Tarih', (CASE WHEN Stline.PRCURR = 0 and Stline.REPORTRATE != 0 THEN ROUND(Stline.PRICE / Stline.REPORTRATE, 2) END) AS EURO, (CASE WHEN Stline.PRCURR = 11 THEN ROUND(Stline.PRPRICE, 2) END) AS CHF, (CASE WHEN Stline.PRCURR = 1 THEN ROUND(Stline.PRPRICE, 2) END) AS DOLAR FROM LG_0" + itemPriceForm.GetFirmCode(0) + "_ITEMS Items, LG_0" + itemPriceForm.GetFirmCode(0) + "_01_STLINE Stline, LG_0" + itemPriceForm.GetFirmCode(0) + "_01_INVOICE Invoice, LG_0" + itemPriceForm.GetFirmCode(0) + "_CLCARD Clcard WHERE Items.CODE = '" + selectedItem + "' and Items.LOGICALREF = Stline.STOCKREF and Clcard.LOGICALREF = Invoice.CLIENTREF and Invoice.LOGICALREF = Stline.INVOICEREF and Invoice.GRPCODE = 2 and Invoice.CANCELLED = 0 and Stline.LINETYPE = 0 ORDER BY Stline.DATE_ DESC");
            }
            else {
                dt = QueryDB("SELECT Clcard.DEFINITION_ as 'Firma İsmi', Items.CODE as 'Ürün Kodu', Items.NAME as 'Ürün İsmi', Stline.LINEEXP as Detaylar, Stline.AMOUNT as Miktar, format(Stline.DATE_, 'dd.MM.yyyy') as 'Tarih', (CASE WHEN Stline.PRCURR = 0 and Stline.TRRATE != 0 THEN ROUND(Stline.PRICE / Stline.TRRATE, 2) END) AS EURO, (CASE WHEN Stline.PRCURR = 11 THEN ROUND(Stline.PRPRICE, 2) END) AS CHF, (CASE WHEN Stline.PRCURR = 1 THEN ROUND(Stline.PRPRICE, 2) END) AS DOLAR FROM LG_0" + itemPriceForm.GetFirmCode(0) + "_ITEMS Items, LG_0" + itemPriceForm.GetFirmCode(0) + "_01_STLINE Stline, LG_0" + itemPriceForm.GetFirmCode(0) + "_01_INVOICE Invoice, LG_0" + itemPriceForm.GetFirmCode(0) + "_CLCARD Clcard WHERE Items.CODE = '" + selectedItem + "' and Items.LOGICALREF = Stline.STOCKREF and Clcard.LOGICALREF = Invoice.CLIENTREF and Invoice.LOGICALREF = Stline.INVOICEREF and Invoice.GRPCODE = 2 and Invoice.CANCELLED = 0 and Stline.LINETYPE = 0 ORDER BY Stline.DATE_ DESC");
            }
            return dt;
        }

        //Malzeme fiyatlarını firmaya göre sorgula
        public DataTable QueryItemPriceByFirm(ItemPriceForm itemPriceForm, string selectedFirm)
        {
            DataTable dt;   
            if (itemPriceForm.GetFirmCodeInt(0) > 16) {
                dt = QueryDB("SELECT Clcard.DEFINITION_ as 'Firma İsmi', Items.CODE as 'Ürün Kodu', Items.NAME as 'Ürün İsmi', Stline.LINEEXP as Detaylar, Stline.AMOUNT as Miktar, format(Stline.DATE_, 'dd.MM.yyyy') as 'Tarih', (CASE WHEN Stline.PRCURR = 0 and Stline.REPORTRATE != 0 THEN ROUND(Stline.PRICE / Stline.REPORTRATE, 2) END) AS EURO, (CASE WHEN Stline.PRCURR = 11 THEN ROUND(Stline.PRPRICE, 2) END) AS CHF, (CASE WHEN Stline.PRCURR = 1 THEN ROUND(Stline.PRPRICE, 2) END) AS DOLAR FROM LG_0" + itemPriceForm.GetFirmCode(0) + "_ITEMS Items, LG_0" + itemPriceForm.GetFirmCode(0) + "_01_STLINE Stline, LG_0" + itemPriceForm.GetFirmCode(0) + "_01_INVOICE Invoice, LG_0" + itemPriceForm.GetFirmCode(0) + "_CLCARD Clcard WHERE Clcard.LOGICALREF = (SELECT LOGICALREF FROM LG_0" + Global.Instance.GetFirmCodeAnotherYear(0) + "_CLCARD ClcardThisYear WHERE ClcardThisYear.CODE = '" + selectedFirm + "') and Items.LOGICALREF = Stline.STOCKREF and Clcard.LOGICALREF = Invoice.CLIENTREF and Invoice.LOGICALREF = Stline.INVOICEREF and Invoice.GRPCODE = 2 and Invoice.CANCELLED = 0 and Stline.LINETYPE = 0 ORDER BY Stline.DATE_ DESC");
            }
            else {
                dt = QueryDB("SELECT Clcard.DEFINITION_ as 'Firma İsmi', Items.CODE as 'Ürün Kodu', Items.NAME as 'Ürün İsmi', Stline.LINEEXP as Detaylar, Stline.AMOUNT as Miktar, format(Stline.DATE_, 'dd.MM.yyyy') as 'Tarih', (CASE WHEN Stline.PRCURR = 0 and Stline.TRRATE != 0 THEN ROUND(Stline.PRICE / Stline.TRRATE, 2) END) AS EURO, (CASE WHEN Stline.PRCURR = 11 THEN ROUND(Stline.PRPRICE, 2) END) AS CHF, (CASE WHEN Stline.PRCURR = 1 THEN ROUND(Stline.PRPRICE, 2) END) AS DOLAR FROM LG_0" + itemPriceForm.GetFirmCode(0) + "_ITEMS Items, LG_0" + itemPriceForm.GetFirmCode(0) + "_01_STLINE Stline, LG_0" + itemPriceForm.GetFirmCode(0) + "_01_INVOICE Invoice, LG_0" + itemPriceForm.GetFirmCode(0) + "_CLCARD Clcard WHERE Clcard.LOGICALREF = (SELECT LOGICALREF FROM LG_0" + Global.Instance.GetFirmCodeAnotherYear(0) + "_CLCARD ClcardThisYear WHERE ClcardThisYear.CODE = '" + selectedFirm + "') and Items.LOGICALREF = Stline.STOCKREF and Clcard.LOGICALREF = Invoice.CLIENTREF and Invoice.LOGICALREF = Stline.INVOICEREF and Invoice.GRPCODE = 2 and Invoice.CANCELLED = 0 and Stline.LINETYPE = 0 ORDER BY Stline.DATE_ DESC");
            }
            return dt;
        }

        //Malzeme fiyatlarını firmaya göre sorgula
        public DataTable QueryItemPriceByDetail(ItemPriceForm itemPriceForm, string detail)
        {
            DataTable dt;
            if (itemPriceForm.GetFirmCodeInt(0) > 16) {
                dt = QueryDB("SELECT Clcard.DEFINITION_ as 'Firma İsmi', Items.CODE as 'Ürün Kodu', Items.NAME as 'Ürün İsmi', Stline.LINEEXP as Detaylar, Stline.AMOUNT as Miktar, format(Stline.DATE_, 'dd.MM.yyyy') as 'Tarih', (CASE WHEN Stline.PRCURR = 0 and Stline.REPORTRATE != 0 THEN ROUND(Stline.PRICE / Stline.REPORTRATE, 2) END) AS EURO, (CASE WHEN Stline.PRCURR = 11 THEN ROUND(Stline.PRPRICE, 2) END) AS CHF, (CASE WHEN Stline.PRCURR = 1 THEN ROUND(Stline.PRPRICE, 2) END) AS DOLAR FROM LG_0" + itemPriceForm.GetFirmCode(0) + "_ITEMS Items, LG_0" + itemPriceForm.GetFirmCode(0) + "_01_STLINE Stline, LG_0" + itemPriceForm.GetFirmCode(0) + "_01_INVOICE Invoice, LG_0" + itemPriceForm.GetFirmCode(0) + "_CLCARD Clcard WHERE Stline.LINEEXP like '" + "%" + detail + "%" + "' and Items.LOGICALREF = Stline.STOCKREF and Clcard.LOGICALREF = Invoice.CLIENTREF and Invoice.LOGICALREF = Stline.INVOICEREF and Invoice.GRPCODE = 2 and Invoice.CANCELLED = 0 and Stline.LINETYPE = 0 ORDER BY Stline.DATE_ DESC");
            }
            else {
                dt = QueryDB("SELECT Clcard.DEFINITION_ as 'Firma İsmi', Items.CODE as 'Ürün Kodu', Items.NAME as 'Ürün İsmi', Stline.LINEEXP as Detaylar, Stline.AMOUNT as Miktar, format(Stline.DATE_, 'dd.MM.yyyy') as 'Tarih', (CASE WHEN Stline.PRCURR = 0 and Stline.TRRATE != 0 THEN ROUND(Stline.PRICE / Stline.TRRATE, 2) END) AS EURO, (CASE WHEN Stline.PRCURR = 11 THEN ROUND(Stline.PRPRICE, 2) END) AS CHF, (CASE WHEN Stline.PRCURR = 1 THEN ROUND(Stline.PRPRICE, 2) END) AS DOLAR FROM LG_0" + itemPriceForm.GetFirmCode(0) + "_ITEMS Items, LG_0" + itemPriceForm.GetFirmCode(0) + "_01_STLINE Stline, LG_0" + itemPriceForm.GetFirmCode(0) + "_01_INVOICE Invoice, LG_0" + itemPriceForm.GetFirmCode(0) + "_CLCARD Clcard WHERE Stline.LINEEXP like '" + "%" + detail + "%" + "' and Items.LOGICALREF = Stline.STOCKREF and Clcard.LOGICALREF = Invoice.CLIENTREF and Invoice.LOGICALREF = Stline.INVOICEREF and Invoice.GRPCODE = 2 and Invoice.CANCELLED = 0 and Stline.LINETYPE = 0 ORDER BY Stline.DATE_ DESC");
            }
            return dt;
        }

        //Malzeme fiyatlarını firmaya göre sorgula
        public DataTable QueryItemPriceByDetailAndFirm(ItemPriceForm itemPriceForm, string detail, string selectedFirm)
        {
            DataTable dt;
            if (itemPriceForm.GetFirmCodeInt(0) > 16) {
                dt = QueryDB("SELECT Clcard.DEFINITION_ as 'Firma İsmi', Items.CODE as 'Ürün Kodu', Items.NAME as 'Ürün İsmi', Stline.LINEEXP as Detaylar, Stline.AMOUNT as Miktar, format(Stline.DATE_, 'dd.MM.yyyy') as 'Tarih', (CASE WHEN Stline.PRCURR = 0 and Stline.REPORTRATE != 0 THEN ROUND(Stline.PRICE / Stline.REPORTRATE, 2) END) AS EURO, (CASE WHEN Stline.PRCURR = 11 THEN ROUND(Stline.PRPRICE, 2) END) AS CHF, (CASE WHEN Stline.PRCURR = 1 THEN ROUND(Stline.PRPRICE, 2) END) AS DOLAR FROM LG_0" + itemPriceForm.GetFirmCode(0) + "_ITEMS Items, LG_0" + itemPriceForm.GetFirmCode(0) + "_01_STLINE Stline, LG_0" + itemPriceForm.GetFirmCode(0) + "_01_INVOICE Invoice, LG_0" + itemPriceForm.GetFirmCode(0) + "_CLCARD Clcard WHERE Stline.LINEEXP like '" + "%" + detail + "%" + "' and Clcard.LOGICALREF = (SELECT LOGICALREF FROM LG_0" + Global.Instance.GetFirmCodeAnotherYear(0) + "_CLCARD ClcardThisYear WHERE ClcardThisYear.CODE = '" + selectedFirm + "') and Items.LOGICALREF = Stline.STOCKREF and Clcard.LOGICALREF = Invoice.CLIENTREF and Invoice.LOGICALREF = Stline.INVOICEREF and Invoice.GRPCODE = 2 and Invoice.CANCELLED = 0 and Stline.LINETYPE = 0 ORDER BY Stline.DATE_ DESC");
            }
            else {
                dt = QueryDB("SELECT Clcard.DEFINITION_ as 'Firma İsmi', Items.CODE as 'Ürün Kodu', Items.NAME as 'Ürün İsmi', Stline.LINEEXP as Detaylar, Stline.AMOUNT as Miktar, format(Stline.DATE_, 'dd.MM.yyyy') as 'Tarih', (CASE WHEN Stline.PRCURR = 0 and Stline.TRRATE != 0 THEN ROUND(Stline.PRICE / Stline.TRRATE, 2) END) AS EURO, (CASE WHEN Stline.PRCURR = 11 THEN ROUND(Stline.PRPRICE, 2) END) AS CHF, (CASE WHEN Stline.PRCURR = 1 THEN ROUND(Stline.PRPRICE, 2) END) AS DOLAR FROM LG_0" + itemPriceForm.GetFirmCode(0) + "_ITEMS Items, LG_0" + itemPriceForm.GetFirmCode(0) + "_01_STLINE Stline, LG_0" + itemPriceForm.GetFirmCode(0) + "_01_INVOICE Invoice, LG_0" + itemPriceForm.GetFirmCode(0) + "_CLCARD Clcard WHERE Stline.LINEEXP like '" + "%" + detail + "%" + "' and Clcard.LOGICALREF = (SELECT LOGICALREF FROM LG_0" + Global.Instance.GetFirmCodeAnotherYear(0) + "_CLCARD ClcardThisYear WHERE ClcardThisYear.CODE = '" + selectedFirm + "') and Items.LOGICALREF = Stline.STOCKREF and Clcard.LOGICALREF = Invoice.CLIENTREF and Invoice.LOGICALREF = Stline.INVOICEREF and Invoice.GRPCODE = 2 and Invoice.CANCELLED = 0 and Stline.LINETYPE = 0 ORDER BY Stline.DATE_ DESC");
            }
            return dt;
        }

        //Bir malzemenin verilen firmadaki o yılki bütün satışları
        public DataTable QueryItemPriceByFirmAndItem(ItemPriceForm itemPriceForm, string selectedItem, string selectedFirm)
        {
            DataTable dt;
            if (itemPriceForm.GetFirmCodeInt(0) > 16) {
                dt = QueryDB("SELECT Clcard.DEFINITION_ as 'Firma İsmi', Items.CODE as 'Ürün Kodu', Items.NAME as 'Ürün İsmi', Stline.LINEEXP as Detaylar, Stline.AMOUNT as Miktar, format(Stline.DATE_, 'dd.MM.yyyy') as 'Tarih', (CASE WHEN Stline.PRCURR = 0 and Stline.REPORTRATE != 0 THEN ROUND(Stline.PRICE / Stline.REPORTRATE, 2) END) AS EURO, (CASE WHEN Stline.PRCURR = 11 THEN ROUND(Stline.PRPRICE, 2) END) AS CHF, (CASE WHEN Stline.PRCURR = 1 THEN ROUND(Stline.PRPRICE, 2) END) AS DOLAR FROM LG_0" + itemPriceForm.GetFirmCode(0) + "_ITEMS Items, LG_0" + itemPriceForm.GetFirmCode(0) + "_01_STLINE Stline, LG_0" + itemPriceForm.GetFirmCode(0) + "_01_INVOICE Invoice, LG_0" + itemPriceForm.GetFirmCode(0) + "_CLCARD Clcard WHERE Items.CODE = '" + selectedItem + "' and Clcard.LOGICALREF = (SELECT LOGICALREF FROM LG_0" + Global.Instance.GetFirmCodeAnotherYear(0) + "_CLCARD ClcardThisYear WHERE ClcardThisYear.CODE = '" + selectedFirm + "') and Items.LOGICALREF = Stline.STOCKREF and Clcard.LOGICALREF = Invoice.CLIENTREF and Invoice.LOGICALREF = Stline.INVOICEREF and Invoice.GRPCODE = 2 and Invoice.CANCELLED = 0 and Stline.LINETYPE = 0 ORDER BY Stline.DATE_ DESC");
            }
            else {
                dt = QueryDB("SELECT Clcard.DEFINITION_ as 'Firma İsmi', Items.CODE as 'Ürün Kodu', Items.NAME as 'Ürün İsmi', Stline.LINEEXP as Detaylar, Stline.AMOUNT as Miktar, format(Stline.DATE_, 'dd.MM.yyyy') as 'Tarih', (CASE WHEN Stline.PRCURR = 0 and Stline.TRRATE != 0 THEN ROUND(Stline.PRICE / Stline.TRRATE, 2) END) AS EURO, (CASE WHEN Stline.PRCURR = 11 THEN ROUND(Stline.PRPRICE, 2) END) AS CHF, (CASE WHEN Stline.PRCURR = 1 THEN ROUND(Stline.PRPRICE, 2) END) AS DOLAR FROM LG_0" + itemPriceForm.GetFirmCode(0) + "_ITEMS Items, LG_0" + itemPriceForm.GetFirmCode(0) + "_01_STLINE Stline, LG_0" + itemPriceForm.GetFirmCode(0) + "_01_INVOICE Invoice, LG_0" + itemPriceForm.GetFirmCode(0) + "_CLCARD Clcard WHERE Items.CODE = '" + selectedItem + "' and Clcard.LOGICALREF = (SELECT LOGICALREF FROM LG_0" + Global.Instance.GetFirmCodeAnotherYear(0) + "_CLCARD ClcardThisYear WHERE ClcardThisYear.CODE = '" + selectedFirm + "') and Items.LOGICALREF = Stline.STOCKREF and Clcard.LOGICALREF = Invoice.CLIENTREF and Invoice.LOGICALREF = Stline.INVOICEREF and Invoice.GRPCODE = 2 and Invoice.CANCELLED = 0 and Stline.LINETYPE = 0 ORDER BY Stline.DATE_ DESC");
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


        //Bir malzemenin detaylara göre son 10 yıldaki fiyat hareketleri
        public DataTable QueryPastTenYearsPriceByDetails(ItemPriceForm itemPriceForm, string details)
        {
            DataTable dt = QueryDB(CreateQueryDetailsDataSince2010(itemPriceForm, details));
            return dt;
        }

        //Bir malzemenin detaylara göre son 10 yıldaki fiyat hareketleri
        public DataTable QueryPastTenYearsPriceByDetailAndFirm(ItemPriceForm itemPriceForm, string details, string selectedFirm)
        {
            DataTable dt = QueryDB(CreateQueryDetailsAndFirmDataSince2010(itemPriceForm, details, selectedFirm));
            return dt;
        }

        //Firmanın ekstresini sorgula
        public DataTable QueryEkstre(string selectedFirm)
        {
            DataTable dt = QueryDB("SELECT format(a.Tarih, 'dd.MM.yyyy') as 'Tarih', a.[Fiş No], a.TRCODE, format(a.[Vade Tarihi], 'dd.MM.yyyy') as 'Vade Tarihi', a.Açıklama, a.Borç, a.Alacak, a.SIGN, a.PAYMENTREF FROM (SELECT Clfline.DATE_ as 'Tarih', DATEADD(day, Cast((SELECT DAY_ FROM LG_0" + Global.Instance.GetFirmCodeAnotherYear(0) + "_PAYLINES Where PAYPLANREF = Clcard.PAYMENTREF) as int), Clfline.DATE_) AS 'Vade Tarihi', Clfline.DOCODE as 'Fiş No', Clfline.TRCODE, Clcard.PAYMENTREF, Clfline.LINEEXP as 'Açıklama', Clfline.SIGN, Cast((SELECT DAY_ FROM LG_0" + Global.Instance.GetFirmCodeAnotherYear(0) + "_PAYLINES Where PAYPLANREF = Clcard.PAYMENTREF) as int) as 'VadeDay', (CASE WHEN Clfline.SIGN = 0 THEN FORMAT(Clfline.AMOUNT, '####,###.00') END) AS 'Borç', (CASE WHEN Clfline.SIGN = 1 THEN FORMAT(Clfline.AMOUNT, '####,###.00') END) AS 'Alacak' FROM LG_0" + Global.Instance.GetFirmCodeAnotherYear(1) + "_01_CLFLINE Clfline, LG_0" + Global.Instance.GetFirmCodeAnotherYear(1) + "_CLCARD Clcard WHERE Clcard.LOGICALREF = (SELECT LOGICALREF FROM LG_0" + Global.Instance.GetFirmCodeAnotherYear(0) + "_CLCARD ClcardThisYear WHERE ClcardThisYear.CODE = '" + selectedFirm + "') and Clcard.LOGICALREF = Clfline.CLIENTREF and Clfline.CANCELLED = 0 and Clfline.TRCODE != 61 UNION ALL SELECT Cscard.SETDATE as 'Tarih', Cscard.DUEDATE as 'Vade Tarih', Clfline.DOCODE as 'Fiş No', Clfline.TRCODE, Cscard.OPSTAT, Clfline.LINEEXP as 'Açıklama',Clfline.SIGN, Cscard.OPSTAT, (CASE WHEN Clfline.SIGN = 0 THEN FORMAT(Cscard.AMOUNT, '####,###.00') END) AS 'Borç', (CASE WHEN Clfline.SIGN = 1 THEN FORMAT(Cscard.AMOUNT, '####,###.00') END) AS 'Alacak' FROM LG_0" + Global.Instance.GetFirmCodeAnotherYear(1) + "_01_CLFLINE Clfline, LG_0" + Global.Instance.GetFirmCodeAnotherYear(1) + "_01_CSTRANS Cstrans, LG_0" + Global.Instance.GetFirmCodeAnotherYear(1) + "_01_CScard Cscard, LG_0" + Global.Instance.GetFirmCodeAnotherYear(1) + "_CLCARD Clcard WHERE Clcard.LOGICALREF = (SELECT LOGICALREF FROM LG_0" + Global.Instance.GetFirmCodeAnotherYear(0) + "_CLCARD ClcardThisYear WHERE ClcardThisYear.CODE = '" + selectedFirm + "') and Clcard.LOGICALREF = Clfline.CLIENTREF and Clfline.SOURCEFREF = Cstrans.ROLLREF and Cstrans.CSREF = Cscard.LOGICALREF and Clfline.trcode = 61 UNION all SELECT Clfline.DATE_ as 'Tarih', DATEADD(day, Cast((SELECT DAY_ FROM LG_0" + Global.Instance.GetFirmCodeAnotherYear(0) + "_PAYLINES Where PAYPLANREF = Clcard.PAYMENTREF) as int), Clfline.DATE_) AS 'Vade Tarihi', Clfline.DOCODE as 'Fiş No', Clfline.TRCODE, Clcard.PAYMENTREF, Clfline.LINEEXP as 'Açıklama', Clfline.SIGN, Cast((SELECT DAY_ FROM LG_0" + Global.Instance.GetFirmCodeAnotherYear(0) + "_PAYLINES Where PAYPLANREF = Clcard.PAYMENTREF) as int) as 'VadeDay', (CASE WHEN Clfline.SIGN = 0 THEN FORMAT(Clfline.AMOUNT, '####,###.00') END) AS 'Borç', (CASE WHEN Clfline.SIGN = 1 THEN FORMAT(Clfline.AMOUNT, '####,###.00') END) AS 'Alacak' FROM LG_0" + Global.Instance.GetFirmCodeAnotherYear(0) + "_01_CLFLINE Clfline, LG_0" + Global.Instance.GetFirmCodeAnotherYear(0) + "_CLCARD Clcard WHERE Clcard.CODE = '" + selectedFirm + "' and Clcard.LOGICALREF = Clfline.CLIENTREF and Clfline.CANCELLED = 0 and Clfline.TRCODE != 14 and Clfline.TRCODE != 61 UNION ALL SELECT Cscard.SETDATE as 'Tarih', Cscard.DUEDATE as 'Vade Tarih', Clfline.DOCODE as 'Fiş No', Clfline.TRCODE, Cscard.OPSTAT, Clfline.LINEEXP as 'Açıklama',Clfline.SIGN, Cscard.OPSTAT, (CASE WHEN Clfline.SIGN = 0 THEN FORMAT(Cscard.AMOUNT, '####,###.00') END) AS 'Borç', (CASE WHEN Clfline.SIGN = 1 THEN FORMAT(Cscard.AMOUNT, '####,###.00') END) AS 'Alacak' FROM LG_0" + Global.Instance.GetFirmCodeAnotherYear(0) + "_01_CLFLINE Clfline, LG_0" + Global.Instance.GetFirmCodeAnotherYear(0) + "_01_CSTRANS Cstrans, LG_0" + Global.Instance.GetFirmCodeAnotherYear(0) + "_01_CScard Cscard, LG_0" + Global.Instance.GetFirmCodeAnotherYear(0) + "_CLCARD Clcard WHERE Clcard.CODE = '" + selectedFirm + "' and Clcard.LOGICALREF = Clfline.CLIENTREF and Clfline.SOURCEFREF = Cstrans.ROLLREF and Cstrans.CSREF = Cscard.LOGICALREF and Clfline.trcode = 61 ) a ORDER BY a.Tarih");                    
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
                    finalSql += "SELECT Clcard.DEFINITION_ as 'Firma İsmi', Items.CODE as 'Ürün Kodu', Items.NAME as 'Ürün İsmi', Stline.LINEEXP as Detaylar, Stline.AMOUNT as Miktar, Stline.DATE_ as Tarih, (CASE WHEN Stline.PRCURR = 0 and Stline.REPORTRATE != 0 THEN ROUND(Stline.PRICE / Stline.REPORTRATE, 2) END) AS EURO, (CASE WHEN Stline.PRCURR = 11 THEN ROUND(Stline.PRPRICE, 2) END) AS CHF, (CASE WHEN Stline.PRCURR = 1 THEN ROUND(Stline.PRPRICE, 2) END) AS DOLAR FROM LG_0" + itemPriceForm.GetLatestFirmCode(i) + "_ITEMS Items, LG_0" + itemPriceForm.GetLatestFirmCode(i) + "_01_STLINE Stline, LG_0" + itemPriceForm.GetLatestFirmCode(i) + "_01_INVOICE Invoice, LG_0" + itemPriceForm.GetLatestFirmCode(i) + "_CLCARD Clcard WHERE Items.CODE = '" + selectedItem + "' and Items.LOGICALREF = Stline.STOCKREF and Clcard.LOGICALREF = Invoice.CLIENTREF and Invoice.LOGICALREF = Stline.INVOICEREF and Invoice.GRPCODE = 2 and Invoice.CANCELLED = 0 and Stline.LINETYPE = 0 ";
                }
                else {
                    finalSql += "SELECT Clcard.DEFINITION_ as 'Firma İsmi', Items.CODE as 'Ürün Kodu', Items.NAME as 'Ürün İsmi', Stline.LINEEXP as Detaylar, Stline.AMOUNT as Miktar, Stline.DATE_ as Tarih, (CASE WHEN Stline.PRCURR = 0 and Stline.TRRATE != 0 THEN ROUND(Stline.PRICE / Stline.TRRATE, 2) END) AS EURO, (CASE WHEN Stline.PRCURR = 11 THEN ROUND(Stline.PRPRICE, 2) END) AS CHF, (CASE WHEN Stline.PRCURR = 1 THEN ROUND(Stline.PRPRICE, 2) END) AS DOLAR FROM LG_0" + itemPriceForm.GetLatestFirmCode(i) + "_ITEMS Items, LG_0" + itemPriceForm.GetLatestFirmCode(i) + "_01_STLINE Stline, LG_0" + itemPriceForm.GetLatestFirmCode(i) + "_01_INVOICE Invoice, LG_0" + itemPriceForm.GetLatestFirmCode(i) + "_CLCARD Clcard WHERE Items.CODE = '" + selectedItem + "' and Items.LOGICALREF = Stline.STOCKREF and Clcard.LOGICALREF = Invoice.CLIENTREF and Invoice.LOGICALREF = Stline.INVOICEREF and Invoice.GRPCODE = 2 and Invoice.CANCELLED = 0 and Stline.LINETYPE = 0 ";
                }

                if (i != pastYears) {
                    finalSql += "UNION all ";
                }
            }
            finalSql += ") a ORDER BY a.Tarih DESC";

            return finalSql;
        }

        private string CreateQueryDetailsDataSince2010(ItemPriceForm itemPriceForm, string details)
        {
            string finalSql = "SELECT a.[Firma İsmi], a.[Ürün Kodu], a.[Ürün İsmi], format(a.Tarih, 'dd.MM.yyyy') as 'Tarih', a.Detaylar, a.Miktar, a.EURO, a.CHF, a.DOLAR FROM (";

            int pastYears = DateTime.Today.Year - 2010;
            for (int i = 0; i <= pastYears; i++) {
                if (itemPriceForm.GetLatestFirmCodeInt(i) > 16) {
                    finalSql += "SELECT Clcard.DEFINITION_ as 'Firma İsmi', Items.CODE as 'Ürün Kodu', Items.NAME as 'Ürün İsmi', Stline.LINEEXP as Detaylar, Stline.AMOUNT as Miktar, Stline.DATE_ as Tarih, (CASE WHEN Stline.PRCURR = 0 and Stline.REPORTRATE != 0 THEN ROUND(Stline.PRICE / Stline.REPORTRATE, 2) END) AS EURO, (CASE WHEN Stline.PRCURR = 11 THEN ROUND(Stline.PRPRICE, 2) END) AS CHF, (CASE WHEN Stline.PRCURR = 1 THEN ROUND(Stline.PRPRICE, 2) END) AS DOLAR FROM LG_0" + itemPriceForm.GetLatestFirmCode(i) + "_ITEMS Items, LG_0" + itemPriceForm.GetLatestFirmCode(i) + "_01_STLINE Stline, LG_0" + itemPriceForm.GetLatestFirmCode(i) + "_01_INVOICE Invoice, LG_0" + itemPriceForm.GetLatestFirmCode(i) + "_CLCARD Clcard WHERE Stline.LINEEXP like '" + "%" + details + "%" + "' and Items.LOGICALREF = Stline.STOCKREF and Clcard.LOGICALREF = Invoice.CLIENTREF and Invoice.LOGICALREF = Stline.INVOICEREF and Invoice.GRPCODE = 2 and Invoice.CANCELLED = 0 and Stline.LINETYPE = 0 ";
                }
                else {
                    finalSql += "SELECT Clcard.DEFINITION_ as 'Firma İsmi', Items.CODE as 'Ürün Kodu', Items.NAME as 'Ürün İsmi', Stline.LINEEXP as Detaylar, Stline.AMOUNT as Miktar, Stline.DATE_ as Tarih, (CASE WHEN Stline.PRCURR = 0 and Stline.TRRATE != 0 THEN ROUND(Stline.PRICE / Stline.TRRATE, 2) END) AS EURO, (CASE WHEN Stline.PRCURR = 11 THEN ROUND(Stline.PRPRICE, 2) END) AS CHF, (CASE WHEN Stline.PRCURR = 1 THEN ROUND(Stline.PRPRICE, 2) END) AS DOLAR FROM LG_0" + itemPriceForm.GetLatestFirmCode(i) + "_ITEMS Items, LG_0" + itemPriceForm.GetLatestFirmCode(i) + "_01_STLINE Stline, LG_0" + itemPriceForm.GetLatestFirmCode(i) + "_01_INVOICE Invoice, LG_0" + itemPriceForm.GetLatestFirmCode(i) + "_CLCARD Clcard WHERE Stline.LINEEXP like '" + "%" + details + "%" + "' and Items.LOGICALREF = Stline.STOCKREF and Clcard.LOGICALREF = Invoice.CLIENTREF and Invoice.LOGICALREF = Stline.INVOICEREF and Invoice.GRPCODE = 2 and Invoice.CANCELLED = 0 and Stline.LINETYPE = 0 ";
                }

                if (i != pastYears) {
                    finalSql += "UNION all ";
                }
            }
            finalSql += ") a ORDER BY a.Tarih DESC";

            return finalSql;
        }

        private string CreateQueryDetailsAndFirmDataSince2010(ItemPriceForm itemPriceForm, string details, string selectedFirm)
        {
            string finalSql = "SELECT a.[Firma İsmi], a.[Ürün Kodu], a.[Ürün İsmi], format(a.Tarih, 'dd.MM.yyyy') as 'Tarih', a.Detaylar, a.Miktar, a.EURO, a.CHF, a.DOLAR FROM (";

            int pastYears = DateTime.Today.Year - 2010;
            for (int i = 0; i <= pastYears; i++) {
                if (itemPriceForm.GetLatestFirmCodeInt(i) > 16) {
                    finalSql += "SELECT Clcard.DEFINITION_ as 'Firma İsmi', Items.CODE as 'Ürün Kodu', Items.NAME as 'Ürün İsmi', Stline.LINEEXP as Detaylar, Stline.AMOUNT as Miktar, Stline.DATE_ as Tarih, (CASE WHEN Stline.PRCURR = 0 and Stline.REPORTRATE != 0 THEN ROUND(Stline.PRICE / Stline.REPORTRATE, 2) END) AS EURO, (CASE WHEN Stline.PRCURR = 11 THEN ROUND(Stline.PRPRICE, 2) END) AS CHF, (CASE WHEN Stline.PRCURR = 1 THEN ROUND(Stline.PRPRICE, 2) END) AS DOLAR FROM LG_0" + itemPriceForm.GetLatestFirmCode(i) + "_ITEMS Items, LG_0" + itemPriceForm.GetLatestFirmCode(i) + "_01_STLINE Stline, LG_0" + itemPriceForm.GetLatestFirmCode(i) + "_01_INVOICE Invoice, LG_0" + itemPriceForm.GetLatestFirmCode(i) + "_CLCARD Clcard WHERE Stline.LINEEXP like '" + "%" + details + "%" + "' and Clcard.LOGICALREF = (SELECT LOGICALREF FROM LG_0" + Global.Instance.GetFirmCodeAnotherYear(0) + "_CLCARD ClcardThisYear WHERE ClcardThisYear.CODE = '" + selectedFirm + "') and Items.LOGICALREF = Stline.STOCKREF and Clcard.LOGICALREF = Invoice.CLIENTREF and Invoice.LOGICALREF = Stline.INVOICEREF and Invoice.GRPCODE = 2 and Invoice.CANCELLED = 0 and Stline.LINETYPE = 0 ";
                }
                else {
                    finalSql += "SELECT Clcard.DEFINITION_ as 'Firma İsmi', Items.CODE as 'Ürün Kodu', Items.NAME as 'Ürün İsmi', Stline.LINEEXP as Detaylar, Stline.AMOUNT as Miktar, Stline.DATE_ as Tarih, (CASE WHEN Stline.PRCURR = 0 and Stline.TRRATE != 0 THEN ROUND(Stline.PRICE / Stline.TRRATE, 2) END) AS EURO, (CASE WHEN Stline.PRCURR = 11 THEN ROUND(Stline.PRPRICE, 2) END) AS CHF, (CASE WHEN Stline.PRCURR = 1 THEN ROUND(Stline.PRPRICE, 2) END) AS DOLAR FROM LG_0" + itemPriceForm.GetLatestFirmCode(i) + "_ITEMS Items, LG_0" + itemPriceForm.GetLatestFirmCode(i) + "_01_STLINE Stline, LG_0" + itemPriceForm.GetLatestFirmCode(i) + "_01_INVOICE Invoice, LG_0" + itemPriceForm.GetLatestFirmCode(i) + "_CLCARD Clcard WHERE Stline.LINEEXP like '" + "%" + details + "%" + "' and Clcard.LOGICALREF = (SELECT LOGICALREF FROM LG_0" + Global.Instance.GetFirmCodeAnotherYear(0) + "_CLCARD ClcardThisYear WHERE ClcardThisYear.CODE = '" + selectedFirm + "') and Items.LOGICALREF = Stline.STOCKREF and Clcard.LOGICALREF = Invoice.CLIENTREF and Invoice.LOGICALREF = Stline.INVOICEREF and Invoice.GRPCODE = 2 and Invoice.CANCELLED = 0 and Stline.LINETYPE = 0 ";
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
                    finalSql += "SELECT Clcard.DEFINITION_ as 'Firma İsmi', Items.CODE as 'Ürün Kodu', Items.NAME as 'Ürün İsmi', Stline.LINEEXP as Detaylar, Stline.AMOUNT as Miktar, Stline.DATE_ as Tarih, (CASE WHEN Stline.PRCURR = 0 and Stline.REPORTRATE != 0 THEN ROUND(Stline.PRICE / Stline.REPORTRATE, 2) END) AS EURO, (CASE WHEN Stline.PRCURR = 11 THEN ROUND(Stline.PRPRICE, 2) END) AS CHF, (CASE WHEN Stline.PRCURR = 1 THEN ROUND(Stline.PRPRICE, 2) END) AS DOLAR FROM LG_0" + itemPriceForm.GetLatestFirmCode(i) + "_ITEMS Items, LG_0" + itemPriceForm.GetLatestFirmCode(i) + "_01_STLINE Stline, LG_0" + itemPriceForm.GetLatestFirmCode(i) + "_01_INVOICE Invoice, LG_0" + itemPriceForm.GetLatestFirmCode(i) + "_CLCARD Clcard WHERE Clcard.LOGICALREF = (SELECT LOGICALREF FROM LG_0" + Global.Instance.GetFirmCodeAnotherYear(0) + "_CLCARD ClcardThisYear WHERE ClcardThisYear.CODE = '" + selectedFirm + "') and Items.LOGICALREF = Stline.STOCKREF and Clcard.LOGICALREF = Invoice.CLIENTREF and Invoice.LOGICALREF = Stline.INVOICEREF and Invoice.GRPCODE = 2 and Invoice.CANCELLED = 0 and Stline.LINETYPE = 0 ";
                }
                else {
                    finalSql += "SELECT Clcard.DEFINITION_ as 'Firma İsmi', Items.CODE as 'Ürün Kodu', Items.NAME as 'Ürün İsmi', Stline.LINEEXP as Detaylar, Stline.AMOUNT as Miktar, Stline.DATE_ as Tarih, (CASE WHEN Stline.PRCURR = 0 and Stline.TRRATE != 0 THEN ROUND(Stline.PRICE / Stline.TRRATE, 2) END) AS EURO, (CASE WHEN Stline.PRCURR = 11 THEN ROUND(Stline.PRPRICE, 2) END) AS CHF, (CASE WHEN Stline.PRCURR = 1 THEN ROUND(Stline.PRPRICE, 2) END) AS DOLAR FROM LG_0" + itemPriceForm.GetLatestFirmCode(i) + "_ITEMS Items, LG_0" + itemPriceForm.GetLatestFirmCode(i) + "_01_STLINE Stline, LG_0" + itemPriceForm.GetLatestFirmCode(i) + "_01_INVOICE Invoice, LG_0" + itemPriceForm.GetLatestFirmCode(i) + "_CLCARD Clcard WHERE Clcard.LOGICALREF = (SELECT LOGICALREF FROM LG_0" + Global.Instance.GetFirmCodeAnotherYear(0) + "_CLCARD ClcardThisYear WHERE ClcardThisYear.CODE = '" + selectedFirm + "') and Items.LOGICALREF = Stline.STOCKREF and Clcard.LOGICALREF = Invoice.CLIENTREF and Invoice.LOGICALREF = Stline.INVOICEREF and Invoice.GRPCODE = 2 and Invoice.CANCELLED = 0 and Stline.LINETYPE = 0 ";
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
                    finalSql += "SELECT Clcard.DEFINITION_ as 'Firma İsmi', Items.CODE as 'Ürün Kodu', Items.NAME as 'Ürün İsmi', Stline.LINEEXP as Detaylar, Stline.AMOUNT as Miktar, Stline.DATE_ as Tarih, (CASE WHEN Stline.PRCURR = 0 and Stline.REPORTRATE != 0 THEN ROUND(Stline.PRICE / Stline.REPORTRATE, 2) END) AS EURO, (CASE WHEN Stline.PRCURR = 11 THEN ROUND(Stline.PRPRICE, 2) END) AS CHF, (CASE WHEN Stline.PRCURR = 1 THEN ROUND(Stline.PRPRICE, 2) END) AS DOLAR FROM LG_0" + itemPriceForm.GetLatestFirmCode(i) + "_ITEMS Items, LG_0" + itemPriceForm.GetLatestFirmCode(i) + "_01_STLINE Stline, LG_0" + itemPriceForm.GetLatestFirmCode(i) + "_01_INVOICE Invoice, LG_0" + itemPriceForm.GetLatestFirmCode(i) + "_CLCARD Clcard WHERE Items.CODE = '" + selectedItem + "' and Clcard.LOGICALREF = (SELECT LOGICALREF FROM LG_0" + Global.Instance.GetFirmCodeAnotherYear(0) + "_CLCARD ClcardThisYear WHERE ClcardThisYear.CODE = '" + selectedFirm + "') and Items.LOGICALREF = Stline.STOCKREF and Clcard.LOGICALREF = Invoice.CLIENTREF and Invoice.LOGICALREF = Stline.INVOICEREF and Invoice.GRPCODE = 2 and Invoice.CANCELLED = 0 and Stline.LINETYPE = 0 ";
                }
                else {
                    finalSql += "SELECT Clcard.DEFINITION_ as 'Firma İsmi', Items.CODE as 'Ürün Kodu', Items.NAME as 'Ürün İsmi', Stline.LINEEXP as Detaylar, Stline.AMOUNT as Miktar, Stline.DATE_ as Tarih, (CASE WHEN Stline.PRCURR = 0 and Stline.TRRATE != 0 THEN ROUND(Stline.PRICE / Stline.TRRATE, 2) END) AS EURO, (CASE WHEN Stline.PRCURR = 11 THEN ROUND(Stline.PRPRICE, 2) END) AS CHF, (CASE WHEN Stline.PRCURR = 1 THEN ROUND(Stline.PRPRICE, 2) END) AS DOLAR FROM LG_0" + itemPriceForm.GetLatestFirmCode(i) + "_ITEMS Items, LG_0" + itemPriceForm.GetLatestFirmCode(i) + "_01_STLINE Stline, LG_0" + itemPriceForm.GetLatestFirmCode(i) + "_01_INVOICE Invoice, LG_0" + itemPriceForm.GetLatestFirmCode(i) + "_CLCARD Clcard WHERE Items.CODE = '" + selectedItem + "' and Clcard.LOGICALREF = (SELECT LOGICALREF FROM LG_0" + Global.Instance.GetFirmCodeAnotherYear(0) + "_CLCARD ClcardThisYear WHERE ClcardThisYear.CODE = '" + selectedFirm + "') and Items.LOGICALREF = Stline.STOCKREF and Clcard.LOGICALREF = Invoice.CLIENTREF and Invoice.LOGICALREF = Stline.INVOICEREF and Invoice.GRPCODE = 2 and Invoice.CANCELLED = 0 and Stline.LINETYPE = 0 ";
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
