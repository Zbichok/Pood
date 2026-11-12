namespace Tooted
{
    partial class Peavorm
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
        private Button btnRull;

        private void InitializeComponent()
        {
            btnTooted = new Button();
            btnPood = new Button();
            btnArve = new Button();
            github = new Button();
            btnLogout = new Button();
            btnRull = new Button();
            SuspendLayout();
            btnTooted.Location = new Point(137, 71);
            btnTooted.Name = "btnTooted";
            btnTooted.Size = new Size(200, 40);
            btnTooted.TabIndex = 0;
            btnTooted.Text = "Tootehaldus";
            btnTooted.UseVisualStyleBackColor = true;
            btnTooted.Click += btnTooted_Click;
            btnPood.Location = new Point(137, 121);
            btnPood.Name = "btnPood";
            btnPood.Size = new Size(200, 40);
            btnPood.TabIndex = 1;
            btnPood.Text = "Pood";
            btnPood.UseVisualStyleBackColor = true;
            btnPood.Click += btnPood_Click;
            btnArve.Location = new Point(137, 171);
            btnArve.Name = "btnArve";
            btnArve.Size = new Size(200, 40);
            btnArve.TabIndex = 2;
            btnArve.Text = "Arve";
            btnArve.UseVisualStyleBackColor = true;
            btnArve.Click += btnArve_Click;
            btnRull.Location = new Point(137, 221);
            btnRull.Name = "btnRull";
            btnRull.Size = new Size(200, 40);
            btnRull.TabIndex = 3;
            btnRull.Text = "Päevane rull";
            btnRull.UseVisualStyleBackColor = true;
            btnRull.Click += btnRull_Click;
            btnLogout.Location = new Point(137, 271);
            btnLogout.Name = "btnLogout";
            btnLogout.Size = new Size(200, 40);
            btnLogout.TabIndex = 5;
            btnLogout.Text = "Logi välja";
            btnLogout.UseVisualStyleBackColor = true;
            btnLogout.Click += btnLogout_Click;
            github.BackgroundImage = Properties.Resources._25231;
            github.BackgroundImageLayout = ImageLayout.Stretch;
            github.Location = new Point(211, 335);
            github.Name = "github";
            github.Size = new Size(53, 51);
            github.TabIndex = 4;
            github.UseVisualStyleBackColor = true;
            github.Cursor = Cursors.Hand;
            github.Click += github_Click;
            ClientSize = new Size(493, 400);
            Controls.Add(github);
            Controls.Add(btnTooted);
            Controls.Add(btnPood);
            Controls.Add(btnArve);
            Controls.Add(btnLogout);
            Controls.Add(btnRull);
            Name = "Peavorm";
            Text = "Peamenüü";
            ResumeLayout(false);
        }

        #endregion

        private Button github;
    }
}