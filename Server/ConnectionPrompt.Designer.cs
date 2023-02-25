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
            this.labelConnect = new System.Windows.Forms.Label();
            this.buttonDeny = new System.Windows.Forms.Button();
            this.buttonAccept = new System.Windows.Forms.Button();
            this.buttonIgnore = new System.Windows.Forms.Button();
            this.buttonBlock = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // labelConnect
            // 
            this.labelConnect.AutoSize = true;
            this.labelConnect.Location = new System.Drawing.Point(67, 22);
            this.labelConnect.Name = "labelConnect";
            this.labelConnect.Size = new System.Drawing.Size(219, 15);
            this.labelConnect.TabIndex = 0;
            this.labelConnect.Text = "??? is connecting to you, do you accept?";
            // 
            // buttonDeny
            // 
            this.buttonDeny.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(159)))), ((int)(((byte)(159)))), ((int)(((byte)(255)))));
            this.buttonDeny.Location = new System.Drawing.Point(10, 68);
            this.buttonDeny.Name = "buttonDeny";
            this.buttonDeny.Size = new System.Drawing.Size(121, 67);
            this.buttonDeny.TabIndex = 0;
            this.buttonDeny.Text = "&Deny";
            this.buttonDeny.UseVisualStyleBackColor = false;
            this.buttonDeny.Click += new System.EventHandler(this.buttonDeny_Click);
            // 
            // buttonAccept
            // 
            this.buttonAccept.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.buttonAccept.Location = new System.Drawing.Point(136, 68);
            this.buttonAccept.Name = "buttonAccept";
            this.buttonAccept.Size = new System.Drawing.Size(76, 67);
            this.buttonAccept.TabIndex = 1;
            this.buttonAccept.Text = "&Accept";
            this.buttonAccept.UseVisualStyleBackColor = false;
            this.buttonAccept.Click += new System.EventHandler(this.buttonAccept_Click);
            // 
            // buttonIgnore
            // 
            this.buttonIgnore.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonIgnore.BackColor = System.Drawing.Color.DarkSalmon;
            this.buttonIgnore.Location = new System.Drawing.Point(259, 152);
            this.buttonIgnore.Name = "buttonIgnore";
            this.buttonIgnore.Size = new System.Drawing.Size(121, 32);
            this.buttonIgnore.TabIndex = 3;
            this.buttonIgnore.Text = "&Ignore";
            this.buttonIgnore.UseVisualStyleBackColor = false;
            this.buttonIgnore.Click += new System.EventHandler(this.buttonIgnore_Click);
            // 
            // buttonBlock
            // 
            this.buttonBlock.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonBlock.BackColor = System.Drawing.Color.Red;
            this.buttonBlock.Location = new System.Drawing.Point(164, 152);
            this.buttonBlock.Name = "buttonBlock";
            this.buttonBlock.Size = new System.Drawing.Size(90, 32);
            this.buttonBlock.TabIndex = 2;
            this.buttonBlock.Text = "&Block";
            this.buttonBlock.UseVisualStyleBackColor = false;
            this.buttonBlock.Click += new System.EventHandler(this.buttonBlock_Click);
            // 
            // ConnectionPrompt
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(390, 195);
            this.Controls.Add(this.buttonBlock);
            this.Controls.Add(this.buttonIgnore);
            this.Controls.Add(this.buttonAccept);
            this.Controls.Add(this.buttonDeny);
            this.Controls.Add(this.labelConnect);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "ConnectionPrompt";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Incoming transmission";
            this.Load += new System.EventHandler(this.ConnectionPrompt_Load);
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