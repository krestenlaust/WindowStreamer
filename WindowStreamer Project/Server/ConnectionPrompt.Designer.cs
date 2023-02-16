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
            this.labelConnect.Location = new System.Drawing.Point(77, 23);
            this.labelConnect.Name = "labelConnect";
            this.labelConnect.Size = new System.Drawing.Size(267, 17);
            this.labelConnect.TabIndex = 0;
            this.labelConnect.Text = "??? is connecting to you, do you accept?";
            // 
            // buttonDeny
            // 
            this.buttonDeny.Location = new System.Drawing.Point(12, 72);
            this.buttonDeny.Name = "buttonDeny";
            this.buttonDeny.Size = new System.Drawing.Size(138, 71);
            this.buttonDeny.TabIndex = 1;
            this.buttonDeny.Text = "Deny";
            this.buttonDeny.UseVisualStyleBackColor = true;
            this.buttonDeny.Click += new System.EventHandler(this.buttonDeny_Click);
            // 
            // buttonAccept
            // 
            this.buttonAccept.Location = new System.Drawing.Point(156, 72);
            this.buttonAccept.Name = "buttonAccept";
            this.buttonAccept.Size = new System.Drawing.Size(87, 71);
            this.buttonAccept.TabIndex = 2;
            this.buttonAccept.Text = "Accept";
            this.buttonAccept.UseVisualStyleBackColor = true;
            this.buttonAccept.Click += new System.EventHandler(this.buttonAccept_Click);
            // 
            // buttonIgnore
            // 
            this.buttonIgnore.Location = new System.Drawing.Point(296, 162);
            this.buttonIgnore.Name = "buttonIgnore";
            this.buttonIgnore.Size = new System.Drawing.Size(138, 34);
            this.buttonIgnore.TabIndex = 3;
            this.buttonIgnore.Text = "Ignore";
            this.buttonIgnore.UseVisualStyleBackColor = true;
            this.buttonIgnore.Click += new System.EventHandler(this.buttonIgnore_Click);
            // 
            // buttonBlock
            // 
            this.buttonBlock.Location = new System.Drawing.Point(187, 162);
            this.buttonBlock.Name = "buttonBlock";
            this.buttonBlock.Size = new System.Drawing.Size(103, 34);
            this.buttonBlock.TabIndex = 4;
            this.buttonBlock.Text = "Block";
            this.buttonBlock.UseVisualStyleBackColor = true;
            this.buttonBlock.Click += new System.EventHandler(this.buttonBlock_Click);
            // 
            // ConnectionPrompt
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(446, 208);
            this.Controls.Add(this.buttonBlock);
            this.Controls.Add(this.buttonIgnore);
            this.Controls.Add(this.buttonAccept);
            this.Controls.Add(this.buttonDeny);
            this.Controls.Add(this.labelConnect);
            this.Name = "ConnectionPrompt";
            this.Text = "ConnectionPrompt";
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