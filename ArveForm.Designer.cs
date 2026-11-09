namespace Tooted
{
    partial class ArveForm
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
            lblOstukorv = new Label();
            listOstukorv = new ListBox();
            lblKokku = new Label();
            lblMarkus = new Label();
            btnEemalda = new Button();
            btnGenPDF = new Button();
            btnSaadaEmail = new Button();
            SuspendLayout();
            lblOstukorv.AutoSize = true;
            lblOstukorv.Location = new Point(20, 15);
            lblOstukorv.Name = "lblOstukorv";
            lblOstukorv.TabIndex = 0;
            lblOstukorv.Text = "Arve read (ostukorv)";
            listOstukorv.ItemHeight = 21;
            listOstukorv.Location = new Point(20, 40);
            listOstukorv.Name = "listOstukorv";
            listOstukorv.Size = new Size(440, 277);
            listOstukorv.TabIndex = 1;
            listOstukorv.DoubleClick += listOstukorv_DoubleClick;
            lblKokku.AutoSize = true;
            lblKokku.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblKokku.Location = new Point(20, 330);
            lblKokku.Name = "lblKokku";
            lblKokku.TabIndex = 2;
            lblKokku.Text = "Kokku: 0 €";
            lblMarkus.AutoSize = true;
            lblMarkus.ForeColor = Color.Firebrick;
            lblMarkus.Location = new Point(20, 370);
            lblMarkus.Name = "lblMarkus";
            lblMarkus.TabIndex = 3;
            btnEemalda.Location = new Point(480, 40);
            btnEemalda.Name = "btnEemalda";
            btnEemalda.Size = new Size(160, 35);
            btnEemalda.TabIndex = 4;
            btnEemalda.Text = "Eemalda valitud";
            btnEemalda.UseVisualStyleBackColor = true;
            btnEemalda.Click += btnEemalda_Click;
            btnGenPDF.Location = new Point(480, 85);
            btnGenPDF.Name = "btnGenPDF";
            btnGenPDF.Size = new Size(160, 35);
            btnGenPDF.TabIndex = 5;
            btnGenPDF.Text = "Genereeri PDF";
            btnGenPDF.UseVisualStyleBackColor = true;
            btnGenPDF.Click += btnGenPDF_Click;
            btnSaadaEmail.Location = new Point(480, 130);
            btnSaadaEmail.Name = "btnSaadaEmail";
            btnSaadaEmail.Size = new Size(160, 35);
            btnSaadaEmail.TabIndex = 6;
            btnSaadaEmail.Text = "Saada emailiga";
            btnSaadaEmail.UseVisualStyleBackColor = true;
            btnSaadaEmail.Click += btnSaadaEmail_Click;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(670, 410);
            Controls.Add(lblOstukorv);
            Controls.Add(listOstukorv);
            Controls.Add(lblKokku);
            Controls.Add(lblMarkus);
            Controls.Add(btnEemalda);
            Controls.Add(btnGenPDF);
            Controls.Add(btnSaadaEmail);
            Name = "ArveForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Arve";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblOstukorv;
        private ListBox listOstukorv;
        private Label lblKokku;
        private Label lblMarkus;
        private Button btnEemalda;
        private Button btnGenPDF;
        private Button btnSaadaEmail;
    }
}
