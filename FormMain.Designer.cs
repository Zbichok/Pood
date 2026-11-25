namespace Tooted
{
    partial class FormMain
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

        private Button btnTooted;
        private Button btnPood;
        private Button btnArve;
        private Button btnLogout;

        private void InitializeComponent()
        {
            btnTooted = new Button();
            btnPood = new Button();
            btnArve = new Button();
            btnLogout = new Button();
            SuspendLayout();
            btnTooted.Location = new Point(137, 71);
            btnTooted.Name = "btnTooted";
            btnTooted.Size = new Size(200, 40);
            btnTooted.TabIndex = 0;
            btnTooted.Text = Keel.T("Tootehaldus", "Управление товарами", "Product management");
            btnTooted.UseVisualStyleBackColor = true;
            btnTooted.Click += btnTooted_Click;
            btnPood.Location = new Point(137, 121);
            btnPood.Name = "btnPood";
            btnPood.Size = new Size(200, 40);
            btnPood.TabIndex = 1;
            btnPood.Text = Keel.T("Pood", "Магазин", "Shop");
            btnPood.UseVisualStyleBackColor = true;
            btnPood.Click += btnPood_Click;
            btnArve.Location = new Point(137, 171);
            btnArve.Name = "btnArve";
            btnArve.Size = new Size(200, 40);
            btnArve.TabIndex = 2;
            btnArve.Text = Keel.T("Arve", "Счёт", "Invoice");
            btnArve.UseVisualStyleBackColor = true;
            btnArve.Click += btnArve_Click;
            btnLogout.Location = new Point(137, 221);
            btnLogout.Name = "btnLogout";
            btnLogout.Size = new Size(200, 40);
            btnLogout.TabIndex = 5;
            btnLogout.Text = Keel.T("Logi välja", "Выйти", "Log out");
            btnLogout.UseVisualStyleBackColor = true;
            btnLogout.Click += btnLogout_Click;
            ClientSize = new Size(493, 290);
            Controls.Add(btnTooted);
            Controls.Add(btnPood);
            Controls.Add(btnArve);
            Controls.Add(btnLogout);
            Name = "FormMain";
            Text = Keel.T("Peamenüü", "Главное меню", "Main menu");
            ResumeLayout(false);
        }

        #endregion
    }
}