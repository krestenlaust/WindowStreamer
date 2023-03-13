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
            labelConnect = new System.Windows.Forms.Label();
            buttonDeny = new System.Windows.Forms.Button();
            buttonAccept = new System.Windows.Forms.Button();
            buttonIgnore = new System.Windows.Forms.Button();
            buttonBlock = new System.Windows.Forms.Button();
            SuspendLayout();
            // 
            // labelConnect
            // 
            resources.ApplyResources(labelConnect, "labelConnect");
            labelConnect.Name = "labelConnect";
            // 
            // buttonDeny
            // 
            resources.ApplyResources(buttonDeny, "buttonDeny");
            buttonDeny.BackColor = System.Drawing.Color.FromArgb(159, 159, 255);
            buttonDeny.Name = "buttonDeny";
            buttonDeny.UseVisualStyleBackColor = false;
            buttonDeny.Click += buttonDeny_Click;
            // 
            // buttonAccept
            // 
            resources.ApplyResources(buttonAccept, "buttonAccept");
            buttonAccept.BackColor = System.Drawing.Color.FromArgb(128, 128, 255);
            buttonAccept.Name = "buttonAccept";
            buttonAccept.UseVisualStyleBackColor = false;
            buttonAccept.Click += buttonAccept_Click;
            // 
            // buttonIgnore
            // 
            resources.ApplyResources(buttonIgnore, "buttonIgnore");
            buttonIgnore.BackColor = System.Drawing.Color.DarkSalmon;
            buttonIgnore.Name = "buttonIgnore";
            buttonIgnore.UseVisualStyleBackColor = false;
            buttonIgnore.Click += buttonIgnore_Click;
            // 
            // buttonBlock
            // 
            resources.ApplyResources(buttonBlock, "buttonBlock");
            buttonBlock.BackColor = System.Drawing.Color.Red;
            buttonBlock.Name = "buttonBlock";
            buttonBlock.UseVisualStyleBackColor = false;
            buttonBlock.Click += buttonBlock_Click;
            // 
            // ConnectionPrompt
            // 
            resources.ApplyResources(this, "$this");
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(buttonBlock);
            Controls.Add(buttonIgnore);
            Controls.Add(buttonAccept);
            Controls.Add(buttonDeny);
            Controls.Add(labelConnect);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "ConnectionPrompt";
            Load += ConnectionPrompt_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label labelConnect;
        private System.Windows.Forms.Button buttonDeny;
        private System.Windows.Forms.Button buttonAccept;
        private System.Windows.Forms.Button buttonIgnore;
        private System.Windows.Forms.Button buttonBlock;
    }
}