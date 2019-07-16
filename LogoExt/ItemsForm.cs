using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
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
            dataGridView1.ColumnHeaderMouseClick += dataGridView1_SelectionChanged;
            dataGridView1.CellMouseUp += OnCellMouseUp;
            dataGridView1.MouseLeave += OnCellMouseLeave;
            dataGridView1.DataBindingComplete += dataGridView1_DataBindingComplete;

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
            dataGridView1.Width = 2 + dataGridView1.Columns["Kodu"].Width + dataGridView1.Columns["Açıklaması"].Width + dataGridView1.Columns["Stok"].Width + dataGridView1.RowHeadersWidth + SystemInformation.VerticalScrollBarWidth;
            dataGridView2.Location = new Point(dataGridView1.Location.X + dataGridView1.Margin.Left + dataGridView1.Width, dataGridView2.Location.Y);
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
            if (listBox1.SelectedItem != null) {
                //TODO one item movement
            }
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            DataGridLineColoring();
        }

        //Fills the "dataGridView1" with Items Stock
        private void QueryItems()
        {
            DataTable dt = Global.Instance.query.QueryItems();
            DataGridViewFormat(dt);
        }

        //Format "Tarih" Column, set visible to true, color the odd and even lines, and allign some of the rows
        private void DataGridViewFormat(DataTable dt)
        {
            if (dt == null) { return; }     //we probably got an exception from query. Do nothing.
            dataGridView1.DataSource = new BindingSource(dt, null);
            DataGridLineColoring();
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

        private void dataGridView1_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            DataGridLineColoring();
        }

            private void OnCellMouseUp(object sender, MouseEventArgs e)
        {
            SumCellValues();
        }

        private void OnCellMouseLeave(object sender, EventArgs e)
        {
            SumCellValues();
        }

        private void SumCellValues()
        {
            if (dataGridView1.SelectedCells.Count == 0) { return; }
            DataTable dt = Global.Instance.query.QueryItemMovements(this, dataGridView1.SelectedCells[0].Value.ToString());
            if (dt == null) { return; }     //we probably got an exception from query. Do nothing.
            dataGridView2.DataSource = new BindingSource(dt, null);
            DataGridLineColoring();
        }
    }
}
