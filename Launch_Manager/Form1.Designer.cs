using System;
using System.IO;
using System.Windows.Forms;

namespace Launch_Manager
{
    partial class Form1
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
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.cleanAndSaveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dESTROYEVERYTHINGToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.ContextMenuStrip = this.contextMenuStrip1;
            this.notifyIcon1.Text = "notifyIcon1";
            this.notifyIcon1.Visible = true;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cleanAndSaveToolStripMenuItem,
            this.dESTROYEVERYTHINGToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(195, 70);
            // 
            // cleanAndSaveToolStripMenuItem
            // 
            this.cleanAndSaveToolStripMenuItem.Name = "cleanAndSaveToolStripMenuItem";
            this.cleanAndSaveToolStripMenuItem.Size = new System.Drawing.Size(194, 22);
            this.cleanAndSaveToolStripMenuItem.Text = "Clean and Save";
            this.cleanAndSaveToolStripMenuItem.Click += new System.EventHandler(this.cleanAndSaveToolStripMenuItem_Click);
            // 
            // dESTROYEVERYTHINGToolStripMenuItem
            // 
            this.dESTROYEVERYTHINGToolStripMenuItem.Name = "dESTROYEVERYTHINGToolStripMenuItem";
            this.dESTROYEVERYTHINGToolStripMenuItem.Size = new System.Drawing.Size(194, 22);
            this.dESTROYEVERYTHINGToolStripMenuItem.Text = "DESTROY EVERYTHING";
            this.dESTROYEVERYTHINGToolStripMenuItem.Click += new System.EventHandler(this.dESTROYEVERYTHINGToolStripMenuItem_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);

        }



        #endregion

        private NotifyIcon notifyIcon1;
        private ContextMenuStrip contextMenuStrip1;
        private ToolStripMenuItem cleanAndSaveToolStripMenuItem;
        private ToolStripMenuItem dESTROYEVERYTHINGToolStripMenuItem;
    }
}

