﻿using System;
using System.Drawing;
using System.Windows.Forms;

namespace LogoExt
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.button8 = new System.Windows.Forms.Button();
            this.button7 = new System.Windows.Forms.Button();
            this.button6 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.tabForms = new System.Windows.Forms.TabControl();
            this.panelNotification = new System.Windows.Forms.Panel();
            this.button3 = new System.Windows.Forms.Button();
            this.labelWarningBody = new System.Windows.Forms.Label();
            this.timerSlideIn = new System.Windows.Forms.Timer(this.components);
            this.timerSlideOut = new System.Windows.Forms.Timer(this.components);
            this.timerPanel = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.panelNotification.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.button8);
            this.splitContainer1.Panel1.Controls.Add(this.button7);
            this.splitContainer1.Panel1.Controls.Add(this.button6);
            this.splitContainer1.Panel1.Controls.Add(this.button5);
            this.splitContainer1.Panel1.Controls.Add(this.button4);
            this.splitContainer1.Panel1.Controls.Add(this.button2);
            this.splitContainer1.Panel1.Controls.Add(this.button1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.tabForms);
            this.splitContainer1.Size = new System.Drawing.Size(1109, 644);
            this.splitContainer1.SplitterDistance = 186;
            this.splitContainer1.TabIndex = 3;
            // 
            // button8
            // 
            this.button8.Dock = System.Windows.Forms.DockStyle.Top;
            this.button8.Location = new System.Drawing.Point(0, 92);
            this.button8.Name = "button8";
            this.button8.Size = new System.Drawing.Size(186, 23);
            this.button8.TabIndex = 6;
            this.button8.Text = "Satış Faturaları";
            this.button8.UseVisualStyleBackColor = true;
            this.button8.Click += new System.EventHandler(this.button8_Click);
            // 
            // button7
            // 
            this.button7.Dock = System.Windows.Forms.DockStyle.Top;
            this.button7.Location = new System.Drawing.Point(0, 69);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(186, 23);
            this.button7.TabIndex = 5;
            this.button7.Text = "M. Hareketleri";
            this.button7.UseVisualStyleBackColor = true;
            this.button7.Click += new System.EventHandler(this.button7_Click);
            // 
            // button6
            // 
            this.button6.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.button6.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.button6.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button6.Location = new System.Drawing.Point(0, 598);
            this.button6.Name = "button6";
            this.button6.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.button6.Size = new System.Drawing.Size(186, 23);
            this.button6.TabIndex = 4;
            this.button6.Text = "Üste Sabitle";
            this.button6.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Click += new System.EventHandler(this.button6_Click);
            // 
            // button5
            // 
            this.button5.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.button5.Location = new System.Drawing.Point(0, 621);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(186, 23);
            this.button5.TabIndex = 3;
            this.button5.Text = "Ayarlar";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // button4
            // 
            this.button4.Dock = System.Windows.Forms.DockStyle.Top;
            this.button4.Location = new System.Drawing.Point(0, 46);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(186, 23);
            this.button4.TabIndex = 2;
            this.button4.Text = "Ekstre";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // button2
            // 
            this.button2.Dock = System.Windows.Forms.DockStyle.Top;
            this.button2.Location = new System.Drawing.Point(0, 23);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(186, 23);
            this.button2.TabIndex = 1;
            this.button2.Text = "M. Birim Fiyatı";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button1
            // 
            this.button1.Dock = System.Windows.Forms.DockStyle.Top;
            this.button1.Location = new System.Drawing.Point(0, 0);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(186, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "GTİP Kodlu Ürünler";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // tabForms
            // 
            this.tabForms.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabForms.ItemSize = new System.Drawing.Size(145, 18);
            this.tabForms.Location = new System.Drawing.Point(0, 0);
            this.tabForms.Name = "tabForms";
            this.tabForms.SelectedIndex = 0;
            this.tabForms.Size = new System.Drawing.Size(919, 644);
            this.tabForms.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.tabForms.TabIndex = 0;
            this.tabForms.SelectedIndexChanged += new EventHandler(tabForms_SelectedIndexChanged);
            // 
            // panelNotification
            // 
            this.panelNotification.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.panelNotification.Controls.Add(this.button3);
            this.panelNotification.Controls.Add(this.labelWarningBody);
            this.panelNotification.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelNotification.Location = new System.Drawing.Point(0, 644);
            this.panelNotification.Name = "panelNotification";
            this.panelNotification.Size = new System.Drawing.Size(1109, 0);
            this.panelNotification.TabIndex = 5;
            this.panelNotification.MouseClick += new System.Windows.Forms.MouseEventHandler(this.panelNotification_MouseClick);
            // 
            // button3
            // 
            this.button3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button3.BackgroundImage = global::LogoExt.Properties.Resources.close;
            this.button3.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.button3.Location = new System.Drawing.Point(1066, 3);
            this.button3.Name = "button3";
            this.button3.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.button3.Size = new System.Drawing.Size(40, 40);
            this.button3.TabIndex = 2;
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // labelWarningBody
            // 
            this.labelWarningBody.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.labelWarningBody.Font = new System.Drawing.Font("Arial Rounded MT Bold", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelWarningBody.Location = new System.Drawing.Point(0, -35);
            this.labelWarningBody.Name = "labelWarningBody";
            this.labelWarningBody.Size = new System.Drawing.Size(1060, 70);
            this.labelWarningBody.TabIndex = 1;
            this.labelWarningBody.Text = "label2";
            this.labelWarningBody.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.labelWarningBody.MouseClick += new System.Windows.Forms.MouseEventHandler(this.panelNotification_MouseClick);
            // 
            // timerSlideIn
            // 
            this.timerSlideIn.Interval = 1;
            this.timerSlideIn.Tick += new System.EventHandler(this.timerSlideIn_Tick);
            // 
            // timerSlideOut
            // 
            this.timerSlideOut.Interval = 1;
            this.timerSlideOut.Tick += new System.EventHandler(this.timerSlideOut_Tick);
            // 
            // timerPanel
            // 
            this.timerPanel.Interval = 1;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1109, 644);
            this.Controls.Add(this.panelNotification);
            this.Controls.Add(this.splitContainer1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.IsMdiContainer = true;
            this.Name = "MainForm";
            this.Text = "Logo/Eral";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.panelNotification.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TabControl tabForms;
        private System.Windows.Forms.Panel panelNotification;
        private System.Windows.Forms.Timer timerSlideIn;
        private System.Windows.Forms.Timer timerSlideOut;
        private System.Windows.Forms.Label labelWarningBody;
        private Button button3;
        private Button button4;
        private Button button5;
        private Timer timerPanel;
        private Button button6;
        private Image pinImage;
        private Button button7;
        private Button button8;

        public Label LabelWarningBody { get => labelWarningBody; set => labelWarningBody = value; }

        public Timer TimerSlideIn { get => timerSlideIn; set => timerSlideIn = value; }
        public Timer TimerSlideOut { get => timerSlideOut; set => timerSlideOut = value; }
        public Image PinImage { get => pinImage; set => pinImage = value; }
    }
}