﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Web.Helpers;
using System.Windows.Forms;

namespace LogoExt
{
    class Global
    {
        private static Global GlobalVariableInstance = null;

        public Global()
        {
        }

        public static Global Instance
        {
            get
            {
                if (GlobalVariableInstance == null)
                {
                    GlobalVariableInstance = new Global();
                }
                return GlobalVariableInstance;
            }
        }

        public static string exePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        public static string firmCode = "017";

        private static string strDBUser = "tolga";                     // SQL Server kullanıcı adı
        private static string strDBPass = "1234";                      // SQL Server kullanıcısının şifresi
        private static string strDBServer = "MHSB";                    // SQL Server
        private static string strDBName = "TGER";                      // Veritabanı adı 
        public static string sqlConnection = "Data Source=localhost; Persist Security Info = False; User ID = " + strDBUser + "; Password = " + strDBPass + "; Initial Catalog = " + strDBName + "; Data Source = " + strDBServer;

        public dynamic settings;
        public static string GTIPFORM = "GtipForm";
        public static string ITEMPRICEFORM = "ItemPriceForm";
        public static string EKSTREFORM = "EkstreForm";
        public static string ITEMSFORM = "ItemsForm";
        public static string INVOICEFORM = "InvoiceForm";

        public static string DIRECTORYPATH = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\LogoExt";
        public static string FILENAME = "\\settings.json";
        public static string FULLPATH = DIRECTORYPATH + FILENAME;

        public static int PADDING5 = 5;

        public MainForm mainForm = new MainForm();
        public Query query = new Query();
        public List<string> FirmCodeList { get; set; }
        public List<string> ItemCodeList { get; set; }

        public string GetFirmCodeCurrentYear()
        {            
            int year = Int32.Parse(DateTime.Today.Year.ToString().Substring(2, 2)) - 2;
            if (year >= 10) {
                return year.ToString();
            }
            return "0" + year;
        }

        public string GetFirmCodeAnotherYear(int previousYear)
        {
            int year = Int32.Parse(DateTime.Today.Year.ToString().Substring(2, 2)) - 2 - previousYear;
            if (year >= 10) {
                return year.ToString();
            }
            return "0" + year;
        }

        public void ErrorNotification(string errorBody)
        {
            mainForm.LabelWarningBody.Text = errorBody;
            mainForm.TimerSlideIn.Enabled = true;
        }

        public void WriteSettings()
        {
            if (!Directory.Exists(DIRECTORYPATH)) {        //if directory doesn't exists create the "LogoExt" folder
                Directory.CreateDirectory(DIRECTORYPATH);
            }

            File.WriteAllText(FULLPATH, Json.Encode(Global.Instance.settings));
        }
    }

    public static class DatatGridViewExtensions
    {
        /// <summary>
        /// Disable sorting of collumns in DataGridView
        /// </summary>
        public static void SetColumnSortMode(this DataGridView dataGridView, DataGridViewColumnSortMode sortMode)
        {
            foreach (DataGridViewColumn column in dataGridView.Columns) {
                column.SortMode = sortMode;
            }
        }

        /// <summary>
        /// Color the table's even rows with the given color
        /// </summary>
        public static void EvenRowColoring(this DataGridView dt, Color even)
        {
            foreach (DataGridViewRow row in dt.Rows) {
                if (row.Index % 2 == 0) {
                    row.DefaultCellStyle.BackColor = even;
                }
            }
        }

        /// <summary>
        /// Color the table's odd and even rows with the given colors
        /// </summary>
        /// <param name="even">Color for even rows</param>
        /// <param name="odd">Color for odd rows</param>
        public static void EvenOddRowColoring(this DataGridView dt, Color even, Color odd)
        {
            foreach (DataGridViewRow row in dt.Rows) {
                if (row.Index % 2 == 0) {
                    row.DefaultCellStyle.BackColor = even;
                }
                else {
                    row.DefaultCellStyle.BackColor = odd;
                }
            }
        }

        /// <summary>
        /// Default scroll behaviour is jumpy. In order to fix it call this function OnLoad()
        /// </summary>
        public static void DoubleBuffered(this DataGridView dgv, bool setting)
        {
            Type dgvType = dgv.GetType();
            PropertyInfo pi = dgvType.GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            pi.SetValue(dgv, setting, null);
        }
    }

    public static class DataTableExtensions
    {
        /// <summary>
        /// Set collumn order by giving header names as an array
        /// </summary>
        public static void SetColumnsOrder(this DataTable table, params String[] columnNames)
        {
            int columnIndex = 0;
            foreach (var columnName in columnNames) {
                table.Columns[columnName].SetOrdinal(columnIndex);
                columnIndex++;
            }
        }
    }
}
