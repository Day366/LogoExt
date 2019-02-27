using System;
using System.Drawing;
using System.Threading;
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
            this.Load += MainForm_Load;          
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            OpenItemPriceForm();
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
            Form1 form1 = new Form1();
            form1.MdiParent = this;
            form1.Dock = DockStyle.Fill;
            form1.TopLevel = false;
            form1.FormBorderStyle = FormBorderStyle.None;
            form1.Show();

            TabPage tp = new TabPage(this.ActiveMdiChild.Text);
            tp.Tag = this.ActiveMdiChild;
            tp.Parent = tabForms;
            tabForms.SelectedTab = tp;

            this.ActiveMdiChild.Tag = tp;
            this.ActiveMdiChild.FormClosed += new FormClosedEventHandler(ActiveMdiChild_FormClosed);
            tp.Controls.Add(form1);

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

        private void button3_Click(object sender, EventArgs e)
        {
            this.timerSlideOut.Enabled = true;
        }
    }
}
