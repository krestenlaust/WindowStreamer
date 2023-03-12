namespace Server
{
    partial class ConnectionPrompt
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConnectionPrompt));
            this.labelConnect = new System.Windows.Forms.Label();
            this.buttonDeny = new System.Windows.Forms.Button();
            this.buttonAccept = new System.Windows.Forms.Button();
            this.buttonIgnore = new System.Windows.Forms.Button();
            this.buttonBlock = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // labelConnect
            // 
            resources.ApplyResources(this.labelConnect, "labelConnect");
            this.labelConnect.Name = "labelConnect";
            // 
            // buttonDeny
            // 
            resources.ApplyResources(this.buttonDeny, "buttonDeny");
            this.buttonDeny.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(159)))), ((int)(((byte)(159)))), ((int)(((byte)(255)))));
            this.buttonDeny.Name = "buttonDeny";
            this.buttonDeny.UseVisualStyleBackColor = false;
            // 
            // buttonAccept
            // 
            resources.ApplyResources(this.buttonAccept, "buttonAccept");
            this.buttonAccept.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.buttonAccept.Name = "buttonAccept";
            this.buttonAccept.UseVisualStyleBackColor = false;
            // 
            // buttonIgnore
            // 
            resources.ApplyResources(this.buttonIgnore, "buttonIgnore");
            this.buttonIgnore.BackColor = System.Drawing.Color.DarkSalmon;
            this.buttonIgnore.Name = "buttonIgnore";
            this.buttonIgnore.UseVisualStyleBackColor = false;
            // 
            // buttonBlock
            // 
            resources.ApplyResources(this.buttonBlock, "buttonBlock");
            this.buttonBlock.BackColor = System.Drawing.Color.Red;
            this.buttonBlock.Name = "buttonBlock";
            this.buttonBlock.UseVisualStyleBackColor = false;
            // 
            // ConnectionPrompt
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.buttonBlock);
            this.Controls.Add(this.buttonIgnore);
            this.Controls.Add(this.buttonAccept);
            this.Controls.Add(this.buttonDeny);
            this.Controls.Add(this.labelConnect);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "ConnectionPrompt";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelConnect;
        private System.Windows.Forms.Button buttonDeny;
        private System.Windows.Forms.Button buttonAccept;
        private System.Windows.Forms.Button buttonIgnore;
        private System.Windows.Forms.Button buttonBlock;
    }
}