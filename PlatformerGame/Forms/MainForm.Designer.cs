namespace PlatformerGame.Forms
{
    partial class MainForm
    {
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _backgroundImage?.Dispose(); 
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 600);
            this.Name = "MainForm";
            this.Text = "Platformer Game";
            this.ResumeLayout(false);
        }
    }
}