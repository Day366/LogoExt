using System;
using System.Drawing;
using System.IO;
using System.Web.Helpers;
using System.Windows.Forms;

namespace LogoExt
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            tabForms.MouseClick += tabControl_MouseClick;
            labelWarningBody.MouseClick += panelNotification_MouseClick;
            this.Load += MainFormOnLoad;  
        }

        private void MainFormOnLoad(object sender, EventArgs e)
        {
            Global.Instance.FirmCodeList = Global.Instance.query.QueryFirmCodes();      //Get all firm codes
            Global.Instance.ItemCodeList = Global.Instance.query.QueryItemCodes();      //Get all items codes
                        
            splitContainer1.SplitterMoved += new SplitterEventHandler(splitContainer1_SplitterMoved);
            ParseSettings();
            splitContainer1.SplitterDistance = Global.Instance.settings.SplitterDistance;

            if (Global.Instance.settings.DefaultForm == Global.ITEMPRICEFORM) {
                OpenItemPriceForm();
            }
            else if (Global.Instance.settings.DefaultForm == Global.GTIPFORM) {
                OpenGtipForm();
            }
            else if (Global.Instance.settings.DefaultForm == Global.EKSTREFORM) {
                OpenEkstreForm();
            }
        }
        
        private void ParseSettings()
        {
            if (!Directory.Exists(Global.DIRECTORYPATH)) {        //if directory doesn't exists create the "LogoExt" folder
                Directory.CreateDirectory(Global.DIRECTORYPATH);
            }

            if (!File.Exists(Global.FULLPATH)) {
                Global.Instance.settings = AddNonExistingSettingDefaultValue("{ }");
                Global.Instance.WriteSettings();
                return;
            }
            else {
                string readSettingsStr = File.ReadAllText(Global.FULLPATH);
                Global.Instance.settings = AddNonExistingSettingDefaultValue(readSettingsStr);
                Global.Instance.WriteSettings();
            }
        }

        /*
         * "{'SplitterDistance': 100, 'TextSize': 12, 'DefaultForm': 'ItemPriceForm', 'FontFamily': 'Tahoma' }"
         * Eğer dosyanın içinde satırları bulamazsa tek tek default değerlerini ekle varsa olduğu gibi kalcak
         */
        private dynamic AddNonExistingSettingDefaultValue(string settingsStr)
        {
            dynamic settings = Json.Decode(settingsStr);
            if(settings.SplitterDistance == null)
                settings.SplitterDistance = 100;
            if (settings.FontFamily == null)
                settings.FontFamily = "Tahoma";
            if (settings.TextSize == null)
                settings.TextSize = 12;
            if (settings.DefaultForm == null)
                settings.DefaultForm = "ItemPriceForm";

            return settings;
        }

        private void splitContainer1_SplitterMoved(object sender, SplitterEventArgs e)
        {
            Global.Instance.settings.SplitterDistance = splitContainer1.SplitterDistance;
            Global.Instance.WriteSettings();
        }

        private void tabForms_SelectedIndexChanged(object sender, EventArgs e)
        {
            if ((tabForms.SelectedTab != null) && (tabForms.SelectedTab.Tag != null)) {
                (tabForms.SelectedTab.Tag as Form).Select();
            }
        }

        private void ActiveMdiChild_FormClosed(object sender, FormClosedEventArgs e)
        {
            ((sender as Form).Tag as TabPage).Dispose();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenGtipForm();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenItemPriceForm();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.timerSlideOut.Enabled = true;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            OpenEkstreForm();
        }

        private void tabControl_MouseClick(object sender, MouseEventArgs e)
        {
            // check if the right mouse button was pressed
            if (e.Button == MouseButtons.Middle) {
                TabPage tabPageCurrent = null;
                // iterate through all the tab pages
                for (int i = 0; i < tabForms.TabCount; i++) {
                    // get their rectangle area and check if it contains the mouse cursor
                    Rectangle r = tabForms.GetTabRect(i);
                    if (r.Contains(e.Location)) {
                        // show the context menu here
                        tabPageCurrent = tabForms.TabPages[i];
                        tabForms.TabPages.Remove(tabPageCurrent);
                    }
                }
            }
        }

        private void panelNotification_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Middle) {
                this.timerSlideOut.Enabled = true;
            }
        }

        //to slide the notification panel call "this.timerSlideIn.Enabled = true;"
        private void timerSlideIn_Tick(object sender, EventArgs e)
        {
            if (this.panelNotification.Height >= 70) {
                this.timerSlideIn.Enabled = false;
            }
            else if (this.panelNotification.Height < 60) {
                this.panelNotification.Height += 6;
            }
            else if (this.panelNotification.Height >= 60) {
                this.panelNotification.Height += 1;
            }
        }

        //to slide the notification panel call "this.timerSlideOut.Enabled = true;"
        private void timerSlideOut_Tick(object sender, EventArgs e)
        {
            if (this.panelNotification.Height <= 0) {
                this.timerSlideOut.Enabled = false;
            }
            else if (this.panelNotification.Height > 10) {
                this.panelNotification.Height -= 6;
            }
            else if (this.panelNotification.Height <= 10) {
                this.panelNotification.Height -= 1;
            }
        }

        private void OpenGtipForm() {
            GtipForm gtipForm = new GtipForm();
            gtipForm.MdiParent = this;
            gtipForm.Dock = DockStyle.Fill;
            gtipForm.TopLevel = false;
            gtipForm.FormBorderStyle = FormBorderStyle.None;
            gtipForm.Show();

            TabPage tp = new TabPage(this.ActiveMdiChild.Text);
            tp.Tag = this.ActiveMdiChild;
            tp.Parent = tabForms;
            tabForms.SelectedTab = tp;

            this.ActiveMdiChild.Tag = tp;
            this.ActiveMdiChild.FormClosed += new FormClosedEventHandler(ActiveMdiChild_FormClosed);
            tp.Controls.Add(gtipForm);

            if (!tabForms.Visible) {
                tabForms.Visible = true;
            }
        }

        private void OpenItemPriceForm()
        {
            ItemPriceForm itemPriceForm = new ItemPriceForm();
            itemPriceForm.MdiParent = this;
            itemPriceForm.Dock = DockStyle.Fill;
            itemPriceForm.TopLevel = false;
            itemPriceForm.FormBorderStyle = FormBorderStyle.None;
            itemPriceForm.Show();

            TabPage tp = new TabPage(this.ActiveMdiChild.Text) {
                Tag = this.ActiveMdiChild,
                Parent = tabForms
            };
            tabForms.SelectedTab = tp;

            this.ActiveMdiChild.Tag = tp;
            this.ActiveMdiChild.FormClosed += new FormClosedEventHandler(ActiveMdiChild_FormClosed);
            tp.Controls.Add(itemPriceForm);

            if (!tabForms.Visible) {
                tabForms.Visible = true;
            }
        }

        private void OpenEkstreForm()
        {
            EkstreForm ekstreForm = new EkstreForm();
            ekstreForm.MdiParent = this;
            ekstreForm.Dock = DockStyle.Fill;
            ekstreForm.TopLevel = false;
            ekstreForm.FormBorderStyle = FormBorderStyle.None;
            ekstreForm.Show();

            TabPage tp = new TabPage(this.ActiveMdiChild.Text) {
                Tag = this.ActiveMdiChild,
                Parent = tabForms
            };
            tabForms.SelectedTab = tp;

            this.ActiveMdiChild.Tag = tp;
            this.ActiveMdiChild.FormClosed += new FormClosedEventHandler(ActiveMdiChild_FormClosed);
            tp.Controls.Add(ekstreForm);

            if (!tabForms.Visible) {
                tabForms.Visible = true;
            }
        }


        private void OpenSettingsForm()
        {
            SettingsForm settingsForm = new SettingsForm();
            settingsForm.MdiParent = this;
            settingsForm.Dock = DockStyle.Fill;
            settingsForm.TopLevel = false;
            settingsForm.FormBorderStyle = FormBorderStyle.None;
            settingsForm.Show();

            TabPage tp = new TabPage(this.ActiveMdiChild.Text) {
                Tag = this.ActiveMdiChild,
                Parent = tabForms
            };
            tabForms.SelectedTab = tp;

            this.ActiveMdiChild.Tag = tp;
            this.ActiveMdiChild.FormClosed += new FormClosedEventHandler(ActiveMdiChild_FormClosed);
            tp.Controls.Add(settingsForm);

            if (!tabForms.Visible) {
                tabForms.Visible = true;
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            OpenSettingsForm();
        }

        public void ChangeFontsOfAllDataGridViews()
        {
            for (int i = 0; i < tabForms.Controls.Count; i++) {
                if (tabForms.Controls[i].Text == "EkstreForm") {
                    EkstreForm ekstreForm = (EkstreForm)tabForms.Controls[i].Tag;
                    ekstreForm.DataGridView1.DefaultCellStyle.Font = new Font((string)Global.Instance.settings.FontFamily, (float)Global.Instance.settings.TextSize);
                }
                else if(tabForms.Controls[i].Text == "ItemPriceForm") {
                    ItemPriceForm itemPriceForm = (ItemPriceForm)tabForms.Controls[i].Tag;
                    itemPriceForm.DataGridView1.DefaultCellStyle.Font = new Font((string)Global.Instance.settings.FontFamily, (float)Global.Instance.settings.TextSize);
                }
            }            
        }
    }
}
