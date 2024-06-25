namespace DCB.Client
{
    partial class LoadForm
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
            components = new System.ComponentModel.Container();
            LoadProgressBar = new ProgressBar();
            LoadPictureBox = new PictureBox();
            LoadTextBox = new TextBox();
            LoadTimer = new System.Windows.Forms.Timer(components);
            ((System.ComponentModel.ISupportInitialize)LoadPictureBox).BeginInit();
            SuspendLayout();
            // 
            // LoadProgressBar
            // 
            LoadProgressBar.Location = new Point(14, 599);
            LoadProgressBar.Name = "LoadProgressBar";
            LoadProgressBar.Size = new Size(986, 23);
            LoadProgressBar.TabIndex = 0;
            // 
            // LoadPictureBox
            // 
            LoadPictureBox.BackgroundImageLayout = ImageLayout.Stretch;
            LoadPictureBox.Image = Properties.Resources.Load1;
            LoadPictureBox.Location = new Point(12, 12);
            LoadPictureBox.Name = "LoadPictureBox";
            LoadPictureBox.Size = new Size(988, 526);
            LoadPictureBox.TabIndex = 1;
            LoadPictureBox.TabStop = false;
            // 
            // LoadTextBox
            // 
            LoadTextBox.Location = new Point(12, 558);
            LoadTextBox.Name = "LoadTextBox";
            LoadTextBox.ReadOnly = true;
            LoadTextBox.Size = new Size(988, 23);
            LoadTextBox.TabIndex = 2;
            LoadTextBox.Text = "Загружаем мир";
            // 
            // LoadForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1012, 635);
            ControlBox = false;
            Controls.Add(LoadTextBox);
            Controls.Add(LoadPictureBox);
            Controls.Add(LoadProgressBar);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Name = "LoadForm";
            Text = "Загрузка...";
            ((System.ComponentModel.ISupportInitialize)LoadPictureBox).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ProgressBar LoadProgressBar;
        private PictureBox LoadPictureBox;
        private TextBox LoadTextBox;
        private System.Windows.Forms.Timer LoadTimer;
    }
}