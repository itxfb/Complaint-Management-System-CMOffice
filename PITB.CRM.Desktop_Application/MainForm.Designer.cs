namespace PITB.CRM.Desktop_Application
{
    partial class MainForm
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
            this.BtnStartEscalation = new System.Windows.Forms.Button();
            this.BtnStopEscalation = new System.Windows.Forms.Button();
            this.enc_dec_Btn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // BtnStartEscalation
            // 
            this.BtnStartEscalation.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.BtnStartEscalation.ForeColor = System.Drawing.Color.Black;
            this.BtnStartEscalation.Location = new System.Drawing.Point(43, 46);
            this.BtnStartEscalation.Name = "BtnStartEscalation";
            this.BtnStartEscalation.Size = new System.Drawing.Size(190, 54);
            this.BtnStartEscalation.TabIndex = 0;
            this.BtnStartEscalation.Text = "Start Escalation Service";
            this.BtnStartEscalation.UseVisualStyleBackColor = false;
            this.BtnStartEscalation.Click += new System.EventHandler(this.OnButtonClick);
            // 
            // BtnStopEscalation
            // 
            this.BtnStopEscalation.BackColor = System.Drawing.Color.HotPink;
            this.BtnStopEscalation.Location = new System.Drawing.Point(308, 46);
            this.BtnStopEscalation.Name = "BtnStopEscalation";
            this.BtnStopEscalation.Size = new System.Drawing.Size(190, 54);
            this.BtnStopEscalation.TabIndex = 1;
            this.BtnStopEscalation.Text = "Stop Escalation Service";
            this.BtnStopEscalation.UseVisualStyleBackColor = false;
            this.BtnStopEscalation.Click += new System.EventHandler(this.OnButtonClick);
            // 
            // enc_dec_Btn
            // 
            this.enc_dec_Btn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(179)))), ((int)(((byte)(157)))));
            this.enc_dec_Btn.Location = new System.Drawing.Point(43, 142);
            this.enc_dec_Btn.Name = "enc_dec_Btn";
            this.enc_dec_Btn.Size = new System.Drawing.Size(190, 49);
            this.enc_dec_Btn.TabIndex = 2;
            this.enc_dec_Btn.Text = "Encryption/Decryption";
            this.enc_dec_Btn.UseVisualStyleBackColor = false;
            this.enc_dec_Btn.Click += new System.EventHandler(this.OnButtonClick);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(712, 503);
            this.Controls.Add(this.enc_dec_Btn);
            this.Controls.Add(this.BtnStopEscalation);
            this.Controls.Add(this.BtnStartEscalation);
            this.Name = "MainForm";
            this.Text = "Main Form Control";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button BtnStartEscalation;
        private System.Windows.Forms.Button BtnStopEscalation;
        private System.Windows.Forms.Button enc_dec_Btn;
    }
}

