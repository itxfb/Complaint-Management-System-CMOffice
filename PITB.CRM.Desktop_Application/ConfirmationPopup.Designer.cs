namespace PITB.CRM.Desktop_Application
{
    partial class ConfirmationPopup
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
            this.LabelMessageConfirmation = new System.Windows.Forms.Label();
            this.BtnConfirmYes = new System.Windows.Forms.Button();
            this.BtnConfirmNo = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // LabelMessageConfirmation
            // 
            this.LabelMessageConfirmation.AutoSize = true;
            this.LabelMessageConfirmation.Location = new System.Drawing.Point(174, 74);
            this.LabelMessageConfirmation.Name = "LabelMessageConfirmation";
            this.LabelMessageConfirmation.Size = new System.Drawing.Size(133, 17);
            this.LabelMessageConfirmation.TabIndex = 0;
            this.LabelMessageConfirmation.Text = "Enter your text here";
            // 
            // BtnConfirmYes
            // 
            this.BtnConfirmYes.BackColor = System.Drawing.Color.DarkSeaGreen;
            this.BtnConfirmYes.Location = new System.Drawing.Point(114, 145);
            this.BtnConfirmYes.Name = "BtnConfirmYes";
            this.BtnConfirmYes.Size = new System.Drawing.Size(103, 43);
            this.BtnConfirmYes.TabIndex = 1;
            this.BtnConfirmYes.Text = "Yes";
            this.BtnConfirmYes.UseVisualStyleBackColor = false;
            this.BtnConfirmYes.Click += new System.EventHandler(this.OnButtonClick);
            // 
            // BtnConfirmNo
            // 
            this.BtnConfirmNo.BackColor = System.Drawing.Color.PaleVioletRed;
            this.BtnConfirmNo.Location = new System.Drawing.Point(280, 145);
            this.BtnConfirmNo.Name = "BtnConfirmNo";
            this.BtnConfirmNo.Size = new System.Drawing.Size(103, 43);
            this.BtnConfirmNo.TabIndex = 2;
            this.BtnConfirmNo.Text = "No";
            this.BtnConfirmNo.UseVisualStyleBackColor = false;
            this.BtnConfirmNo.Click += new System.EventHandler(this.OnButtonClick);
            // 
            // ConfirmationPopup
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(496, 253);
            this.Controls.Add(this.BtnConfirmNo);
            this.Controls.Add(this.BtnConfirmYes);
            this.Controls.Add(this.LabelMessageConfirmation);
            this.Name = "ConfirmationPopup";
            this.Text = "Heading";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label LabelMessageConfirmation;
        private System.Windows.Forms.Button BtnConfirmYes;
        private System.Windows.Forms.Button BtnConfirmNo;
    }
}