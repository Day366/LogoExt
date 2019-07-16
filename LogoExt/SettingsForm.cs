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
    public partial class SettingsForm : Form
    {
        public SettingsForm()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            string[] forms = new string[] { "GTİP Kodlu Ürünler", "Malzeme Birim Fiyatı", "Ekstre", "Malzeme Hareketleri" };
            fontDialog1.Font = new Font((string)Global.Instance.settings.FontFamily, (float)Global.Instance.settings.TextSize);            
            comboBox1.Items.AddRange(forms);

            if (Global.Instance.settings.DefaultForm == Global.GTIPFORM) {
                comboBox1.SelectedIndex = 0;
            }
            else if (Global.Instance.settings.DefaultForm == Global.ITEMPRICEFORM) {
                comboBox1.SelectedIndex = 1;
            }
            else if (Global.Instance.settings.DefaultForm == Global.EKSTREFORM) {
                comboBox1.SelectedIndex = 2;
            }
            else if (Global.Instance.settings.DefaultForm == Global.ITEMSFORM) {
                comboBox1.SelectedIndex = 3;
            }
        }

        private void FontStyleButton_Click(object sender, EventArgs e)
        {
            DialogResult fontResult = fontDialog1.ShowDialog();
            if (fontResult == DialogResult.OK) {
                Global.Instance.settings.FontFamily = fontDialog1.Font.FontFamily.Name;
                Global.Instance.settings.TextSize = fontDialog1.Font.Size;
                Global.Instance.WriteSettings();
                Global.Instance.mainForm.ChangeFontsOfAllDataGridViews();
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBox1.SelectedIndex) {
                case 0:
                    Global.Instance.settings.DefaultForm = Global.GTIPFORM;
                    break;
                case 1:
                    Global.Instance.settings.DefaultForm = Global.ITEMPRICEFORM;
                    break;
                case 2:
                    Global.Instance.settings.DefaultForm = Global.EKSTREFORM;
                    break;
                case 3:
                    Global.Instance.settings.DefaultForm = Global.ITEMSFORM;
                    break;
            }
            Global.Instance.WriteSettings();
        }
    }
}
