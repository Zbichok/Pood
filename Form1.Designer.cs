using Microsoft.EntityFrameworkCore;

namespace Tooted
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            tabpood = new TabControl();
            tabPage1 = new TabPage();
            tabPage2 = new TabPage();
            lblKorv = new Label();
            lblMarkus = new Label();
            tabpood.SuspendLayout();
            SuspendLayout();
            tabpood.Controls.Add(tabPage1);
            tabpood.Controls.Add(tabPage2);
            tabpood.Location = new Point(10, 10);
            tabpood.Name = "tabpood";
            tabpood.SelectedIndex = 0;
            tabpood.Size = new Size(900, 532);
            tabpood.TabIndex = 0;
            tabPage1.Location = new Point(4, 30);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new Padding(3);
            tabPage1.Size = new Size(892, 498);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "tabPage1";
            tabPage1.Click += tabPage1_Click;
            tabPage2.Location = new Point(4, 24);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new Padding(3);
            tabPage2.Size = new Size(892, 572);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "tabPage2";
            lblKorv.AutoSize = true;
            lblKorv.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            lblKorv.Location = new Point(10, 555);
            lblKorv.Name = "lblKorv";
            lblKorv.Text = "Ostukorv: 0 tk – 0.00 €";
            lblMarkus.AutoSize = true;
            lblMarkus.ForeColor = Color.Firebrick;
            lblMarkus.Location = new Point(10, 585);
            lblMarkus.Name = "lblMarkus";
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(919, 624);
            Controls.Add(tabpood);
            Controls.Add(lblKorv);
            Controls.Add(lblMarkus);
            Name = "Form1";
            Text = "Form1";
            tabpood.ResumeLayout(false);
            ResumeLayout(false);

        }
        #endregion
        private TabControl tabpood;
        private TabPage tabPage1;
        private TabPage tabPage2;
        private Label lblKorv;
        private Label lblMarkus;
    }
}