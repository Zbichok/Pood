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
            lblTooted = new Label();
            listTooted = new ListBox();
            lblOstukorv = new Label();
            listOstukorv = new ListBox();
            lblKokku = new Label();
            btnLisaKorvi = new Button();
            btnGenPDF = new Button();
            btnSaadaEmail = new Button();
            pictureBox1 = new PictureBox();
            toodeNimi = new Label();
            toodeKirjeldus = new Label();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            lblTooted.AutoSize = true;
            lblTooted.Location = new Point(20, 15);
            lblTooted.Name = "lblTooted";
            lblTooted.Size = new Size(56, 19);
            lblTooted.TabIndex = 0;
            lblTooted.Text = "Tooted";
            listTooted.ItemHeight = 21;
            listTooted.Location = new Point(20, 40);
            listTooted.Name = "listTooted";
            listTooted.Size = new Size(220, 277);
            listTooted.TabIndex = 0;
            listTooted.SelectedIndexChanged += listTooted_SelectedIndexChanged;
            lblOstukorv.AutoSize = true;
            lblOstukorv.Location = new Point(260, 15);
            lblOstukorv.Name = "lblOstukorv";
            lblOstukorv.Size = new Size(237, 19);
            lblOstukorv.TabIndex = 1;
            lblOstukorv.Text = "Ostukorv (topeltklõps = eemalda)";
            listOstukorv.ItemHeight = 21;
            listOstukorv.Location = new Point(260, 40);
            listOstukorv.Name = "listOstukorv";
            listOstukorv.Size = new Size(250, 277);
            listOstukorv.TabIndex = 1;
            listOstukorv.DoubleClick += listOstukorv_DoubleClick;
            lblKokku.AutoSize = true;
            lblKokku.Location = new Point(340, 332);
            lblKokku.Name = "lblKokku";
            lblKokku.Size = new Size(88, 21);
            lblKokku.TabIndex = 2;
            lblKokku.Text = "Kokku: 0 €";
            btnLisaKorvi.Location = new Point(530, 40);
            btnLisaKorvi.Name = "btnLisaKorvi";
            btnLisaKorvi.Size = new Size(150, 35);
            btnLisaKorvi.TabIndex = 2;
            btnLisaKorvi.Text = "Lisa toode korvi";
            btnLisaKorvi.UseVisualStyleBackColor = true;
            btnLisaKorvi.Click += btnLisaKorvi_Click;
            btnGenPDF.Location = new Point(530, 85);
            btnGenPDF.Name = "btnGenPDF";
            btnGenPDF.Size = new Size(150, 35);
            btnGenPDF.TabIndex = 3;
            btnGenPDF.Text = "Genereeri PDF";
            btnGenPDF.UseVisualStyleBackColor = true;
            btnGenPDF.Click += btnGenPDF_Click;
            btnSaadaEmail.Location = new Point(530, 130);
            btnSaadaEmail.Name = "btnSaadaEmail";
            btnSaadaEmail.Size = new Size(150, 35);
            btnSaadaEmail.TabIndex = 4;
            btnSaadaEmail.Text = "Saada emailiga";
            btnSaadaEmail.UseVisualStyleBackColor = true;
            btnSaadaEmail.Click += btnSaadaEmail_Click;
            pictureBox1.BorderStyle = BorderStyle.FixedSingle;
            pictureBox1.Location = new Point(530, 180);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(150, 120);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.TabIndex = 5;
            pictureBox1.TabStop = false;
            toodeNimi.AutoSize = true;
            toodeNimi.Location = new Point(530, 305);
            toodeNimi.Name = "toodeNimi";
            toodeNimi.Size = new Size(0, 21);
            toodeNimi.TabIndex = 6;
            toodeKirjeldus.AutoSize = true;
            toodeKirjeldus.Location = new Point(530, 330);
            toodeKirjeldus.Name = "toodeKirjeldus";
            toodeKirjeldus.Size = new Size(0, 15);
            toodeKirjeldus.TabIndex = 7;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(710, 416);
            Controls.Add(lblTooted);
            Controls.Add(listTooted);
            Controls.Add(lblOstukorv);
            Controls.Add(listOstukorv);
            Controls.Add(lblKokku);
            Controls.Add(btnLisaKorvi);
            Controls.Add(btnGenPDF);
            Controls.Add(btnSaadaEmail);
            Controls.Add(pictureBox1);
            Controls.Add(toodeNimi);
            Controls.Add(toodeKirjeldus);
            Name = "ArveForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Arve";
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblTooted;
        private ListBox listTooted;
        private Label lblOstukorv;
        private ListBox listOstukorv;
        private Label lblKokku;
        private Button btnLisaKorvi;
        private Button btnGenPDF;
        private Button btnSaadaEmail;
        private PictureBox pictureBox1;
        private Label toodeNimi;
        private Label toodeKirjeldus;
    }
}
