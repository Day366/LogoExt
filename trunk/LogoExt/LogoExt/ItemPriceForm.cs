﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;

namespace LogoExt
{
    public partial class ItemPriceForm : Form
    {

        public List<string> ItemCodeList { get; }
        public List<string> FirmCodeList { get; }

        public ItemPriceForm()
        {
            InitializeComponent();
            listBox1.Items.Clear();
            listBox2.Items.Clear();
            listBox3.Items.Clear();
            textBox1.KeyPress += new KeyPressEventHandler(TextBox1_KeyPress);
            textBox2.KeyPress += new KeyPressEventHandler(TextBox2_KeyPress);
            dataGridView1.ColumnHeaderMouseClick += dataGridView1_SelectionChanged;
            dataGridView1.DoubleBuffered(true);

            int year = Int32.Parse(DateTime.Now.ToString("yy"));

            for (; year > 2; year--) {
                if (year < 10) {
                    listBox1.Items.Add("200" + year);
                }
                else {
                    listBox1.Items.Add("20" + year);
                }
            }
            listBox1.SetSelected(1, true);

            ItemCodeList = Global.Instance.query.QueryItemCodes(this);      //Get all items codes
            FirmCodeList = Global.Instance.query.QueryFirmCodes(this);      //Get all firm codes
        }

        public string GetFirmCode(int pastYears) {
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

        public int GetLatestFirmCodeInt(int pastYears)
        {
            return Int32.Parse(DateTime.Today.Year.ToString().Substring(2, 2)) - 2 - pastYears;
        }

        public string GetLatestFirmCode(int pastYears)
        {
            int year = Int32.Parse(DateTime.Today.Year.ToString().Substring(2, 2)) - 2 - pastYears;
            if (year >= 10) {
                return year.ToString();
            }
            return "0" + year;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (textBox1.TextLength > 1) {
                Thread.CurrentThread.CurrentCulture = new CultureInfo("tr-TR");
                listBox2.Items.Clear();
                string upper = textBox1.Text.ToUpper();

                for (int i = 0; i < ItemCodeList.Count(); i++) {
                    string temp = ItemCodeList[i];

                    if (temp.ToUpper().Contains(upper)) {
                        listBox2.Items.Add(temp);
                    }
                }
            }
            else {
                listBox2.Items.Clear();
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            if (textBox2.TextLength > 1) {
                Thread.CurrentThread.CurrentCulture = new CultureInfo("tr-TR"); ;
                listBox3.Items.Clear();
                string upper = textBox2.Text.ToUpper();

                for (int i = 0; i < FirmCodeList.Count(); i++) {
                    string temp = FirmCodeList[i];

                    if (temp.ToUpper().Contains(upper)) {
                        listBox3.Items.Add(temp);
                    }
                }
            }
            else {
                listBox3.Items.Clear();
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox2.SelectedItem != null && listBox3.SelectedItem != null) {
                QueryItemPriceByFirmAndItem(listBox2, listBox3);
            }
            else if (listBox2.SelectedItem != null) {
                QueryItemPriceByItem(listBox2);
            }
            else if (listBox3.SelectedItem != null) {
                QueryItemPriceByFirm(listBox3);
            }
        }

        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox2.SelectedItem != null && listBox3.SelectedItem != null) {
                QueryItemPriceByFirmAndItem(listBox2, listBox3);
            }
            else if (listBox2.SelectedItem != null) {
                QueryItemPriceByItem(listBox2);
            }
        }

        private void listBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox2.SelectedItem != null && listBox3.SelectedItem != null) {
                QueryItemPriceByFirmAndItem(listBox2, listBox3);
            }
            else if (listBox3.SelectedItem != null) {
                QueryItemPriceByFirm(listBox3);
            }
        }


        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Length > 0) {
                textBox1.Clear();
                if (listBox3.SelectedItem != null) {
                    QueryItemPriceByFirm(listBox3);
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (textBox2.Text.Length > 0) {
                textBox2.Clear();
                if (listBox2.SelectedItem != null) {
                    QueryItemPriceByItem(listBox2);
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            DataTable dt;
            if (listBox2.SelectedItem != null && listBox3.SelectedItem != null) {
                dt = Global.Instance.query.QueryPastTenYearsPriceByItemAndFirm(this, listBox2.SelectedItem.ToString(), listBox3.SelectedItem.ToString());
                DataGridViewFormat(dt);
            }
            else if (listBox2.SelectedItem != null) {
                dt = Global.Instance.query.QueryPastTenYearsPriceByItem(this, listBox2.SelectedItem.ToString());
                DataGridViewFormat(dt);
            }
            else if (listBox3.SelectedItem != null) {
                dt = Global.Instance.query.QueryPastTenYearsPriceByFirm(this, listBox3.SelectedItem.ToString());
                DataGridViewFormat(dt);
            }
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            DataGridLineColoring();
        }

        //When "Enter" Key is pressed and there is one item in listBox2 run query
        private void TextBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13) {
                if (listBox2.Items.Count == 1) {
                    listBox2.SelectedItem = listBox2.Items[0];
                }
            }
        }

        //When "Enter" Key is pressed and there is one item in listBox3 run query
        private void TextBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13) {
                if (listBox3.Items.Count == 1) {
                    listBox3.SelectedItem = listBox3.Items[0];
                }
            }
        }


        //Fills the "dataGridView1" according to given "FirmCode" and "ItemCode"
        private void QueryItemPriceByFirmAndItem(ListBox listBox1, ListBox listBox2)
        {
            DataTable dt = Global.Instance.query.QueryItemPriceByFirmAndItem(this, listBox1.SelectedItem.ToString(), listBox2.SelectedItem.ToString());
            if (dt == null) {
                //TODO dataGridVİew Dolmicak ekrana hata ver
                return;
            }
            DataGridViewFormat(dt);
        }

        //Fills the "dataGridView1" according to given "FirmCode"
        private void QueryItemPriceByFirm(ListBox listBox1) {
            DataTable dt = Global.Instance.query.QueryItemPriceByFirm(this, listBox1.SelectedItem.ToString());
            if (dt == null) {
                //TODO dataGridVİew Dolmicak ekrana hata ver
                return;
            }
            DataGridViewFormat(dt);
        }


        //Fills the "dataGridView1" according to given "FirmCode"
        private void QueryItemPriceByItem(ListBox listBox1) {
            DataTable dt = Global.Instance.query.QueryItemPriceByItem(this, listBox1.SelectedItem.ToString());
            if (dt == null) {
                //TODO dataGridVİew Dolmicak ekrana hata ver
                return;
            }
            DataGridViewFormat(dt);
        }

        //Format "Tarih" Column, set visible to true, color the odd and even lines, and allign some of the rows
        private void DataGridViewFormat(DataTable dt)
        {
            DataTableExt.ConvertColumnType(dt, "Tarih");
            dataGridView1.DataSource = new BindingSource(dt, null);
            dataGridView1.Visible = true;
            DataGridLineColoring();
            DataGridLineAlignment();
        }

        //color the table's odd and even lines
        private void DataGridLineColoring()
        {
            foreach (DataGridViewRow row in dataGridView1.Rows) {
                if (row.Index % 2 == 0) {
                    row.DefaultCellStyle.BackColor = Color.Beige;
                }
                else {
                    row.DefaultCellStyle.BackColor = Color.Azure;
                }
            }
        }

        //allign some of the rows to right
        private void DataGridLineAlignment()
        {
            dataGridView1.Columns["EURO"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView1.Columns["CHF"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView1.Columns["DOLAR"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView1.Columns["Tarih"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
        }
    }

    public static class ExtensionMethods
    {        
        //To fix scroll at dataGridView
        public static void DoubleBuffered(this DataGridView dgv, bool setting)
        {
            Type dgvType = dgv.GetType();
            PropertyInfo pi = dgvType.GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            pi.SetValue(dgv, setting, null);
        }
    }
}