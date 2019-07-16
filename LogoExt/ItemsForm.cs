using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
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
            this.ActiveControl = textBox1;                                                  //focus will be on textBox1 on start
            dataGridView1.CellMouseUp += OnCellMouseUp;
            textBox1.KeyPress += new KeyPressEventHandler(TextBox1_KeyPress);
            dataGridView1.DataBindingComplete += dataGridView1_DataBindingComplete;
            dataGridView2.DataBindingComplete += dataGridView2_DataBindingComplete;

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
            dataGridView2.Location = new Point(dataGridView1.Location.X + dataGridView1.Width + 10, dataGridView1.Location.Y);          //dgv2'yi dgv1'e göre konumlandır
            dataGridView2.Width = this.Width - dataGridView1.Width - Global.Instance.settings.SplitterDistance + 25;                    //dgv2'nin genişliğini belirle. üst satırla birlikte
        }

        //Focus textBox1 when form is shown
        private void ItemsFormShown(object sender, EventArgs e)
        {
            this.ActiveControl = textBox1;
        }

        //Focus textBox1 when form is shown
        private void ItemsFormResize(object sender, EventArgs e)
        {

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
            if (dataGridView1.SelectedRows.Count != 0) {
                QueryItemMovements();
            }
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
        }

        //color the table's odd and even lines
        private void DataGridLineColoring(DataGridView dt)
        {
            foreach (DataGridViewRow row in dt.Rows) {
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
            foreach (DataGridViewRow row in dataGridView1.Rows) {
                if (row.Index % 2 == 0) {
                    row.DefaultCellStyle.BackColor = Color.Beige;
                }
                else {
                    row.DefaultCellStyle.BackColor = Color.Azure;
                }
            }
        }

        private void dataGridView2_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            foreach (DataGridViewRow row in dataGridView2.Rows) {
                if (row.Index % 2 == 0) {
                    row.DefaultCellStyle.BackColor = Color.Beige;
                }
                else {
                    row.DefaultCellStyle.BackColor = Color.Azure;
                }
            }
        }

        private void OnCellMouseUp(object sender, MouseEventArgs e)
        {
            QueryItemMovements();
        }

        private void QueryItemMovements()
        {
            if (dataGridView1.SelectedCells.Count == 0) { return; }
            DataTable dt = Global.Instance.query.QueryItemMovements(this, dataGridView1.SelectedCells[0].Value.ToString());
            if (dt == null) { return; }     //we probably got an exception from query. Do nothing.
            dataGridView2.DataSource = new BindingSource(dt, null);
        }

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

        private void TextBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13) {
                if (dataGridView1.SelectedRows.Count == 1) {
                    QueryItemMovements();
                }
            }
        }


    }
}
