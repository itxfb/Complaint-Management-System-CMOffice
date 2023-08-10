
namespace PITB.CRM.Desktop_Application
{
    partial class EncryptionDecryption
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
            this.mainPanel = new System.Windows.Forms.Panel();
            this.outputPanel = new System.Windows.Forms.Panel();
            this.outputTextBox = new System.Windows.Forms.TextBox();
            this.outputLabel = new System.Windows.Forms.Label();
            this.inputPanel = new System.Windows.Forms.Panel();
            this.submitInput = new System.Windows.Forms.Button();
            this.inputTextBox = new System.Windows.Forms.TextBox();
            this.methodDropdown = new System.Windows.Forms.ComboBox();
            this.inputLabel = new System.Windows.Forms.Label();
            this.mainPanel.SuspendLayout();
            this.outputPanel.SuspendLayout();
            this.inputPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainPanel
            // 
            this.mainPanel.Controls.Add(this.outputPanel);
            this.mainPanel.Controls.Add(this.inputPanel);
            this.mainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainPanel.Location = new System.Drawing.Point(0, 0);
            this.mainPanel.Name = "mainPanel";
            this.mainPanel.Size = new System.Drawing.Size(800, 450);
            this.mainPanel.TabIndex = 0;
            // 
            // outputPanel
            // 
            this.outputPanel.Controls.Add(this.outputTextBox);
            this.outputPanel.Controls.Add(this.outputLabel);
            this.outputPanel.Location = new System.Drawing.Point(405, 0);
            this.outputPanel.Name = "outputPanel";
            this.outputPanel.Size = new System.Drawing.Size(383, 327);
            this.outputPanel.TabIndex = 0;
            // 
            // outputTextBox
            // 
            this.outputTextBox.Location = new System.Drawing.Point(6, 61);
            this.outputTextBox.Multiline = true;
            this.outputTextBox.Name = "outputTextBox";
            this.outputTextBox.Size = new System.Drawing.Size(336, 193);
            this.outputTextBox.TabIndex = 1;
            // 
            // outputLabel
            // 
            this.outputLabel.AutoSize = true;
            this.outputLabel.Location = new System.Drawing.Point(3, 9);
            this.outputLabel.Name = "outputLabel";
            this.outputLabel.Size = new System.Drawing.Size(51, 17);
            this.outputLabel.TabIndex = 1;
            this.outputLabel.Text = "Output";
            // 
            // inputPanel
            // 
            this.inputPanel.Controls.Add(this.submitInput);
            this.inputPanel.Controls.Add(this.inputTextBox);
            this.inputPanel.Controls.Add(this.methodDropdown);
            this.inputPanel.Controls.Add(this.inputLabel);
            this.inputPanel.Location = new System.Drawing.Point(0, 0);
            this.inputPanel.Name = "inputPanel";
            this.inputPanel.Size = new System.Drawing.Size(399, 327);
            this.inputPanel.TabIndex = 1;
            // 
            // submitInput
            // 
            this.submitInput.Location = new System.Drawing.Point(12, 260);
            this.submitInput.Name = "submitInput";
            this.submitInput.Size = new System.Drawing.Size(75, 23);
            this.submitInput.TabIndex = 2;
            this.submitInput.Text = "Submit";
            this.submitInput.UseVisualStyleBackColor = true;
            this.submitInput.Click += new System.EventHandler(this.submitInput_Click);
            // 
            // inputTextBox
            // 
            this.inputTextBox.Location = new System.Drawing.Point(12, 61);
            this.inputTextBox.Multiline = true;
            this.inputTextBox.Name = "inputTextBox";
            this.inputTextBox.Size = new System.Drawing.Size(336, 193);
            this.inputTextBox.TabIndex = 1;
            // 
            // methodDropdown
            // 
            this.methodDropdown.FormattingEnabled = true;
            this.methodDropdown.Location = new System.Drawing.Point(15, 31);
            this.methodDropdown.Name = "methodDropdown";
            this.methodDropdown.Size = new System.Drawing.Size(167, 24);
            this.methodDropdown.TabIndex = 0;
            // 
            // inputLabel
            // 
            this.inputLabel.AutoSize = true;
            this.inputLabel.Location = new System.Drawing.Point(12, 9);
            this.inputLabel.Name = "inputLabel";
            this.inputLabel.Size = new System.Drawing.Size(114, 17);
            this.inputLabel.TabIndex = 3;
            this.inputLabel.Text = "Select Operation";
            // 
            // EncryptionDecryption
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.mainPanel);
            this.Name = "EncryptionDecryption";
            this.Text = "EncryptionDecryption";
            this.Load += new System.EventHandler(this.EncryptionDecryption_Load);
            this.mainPanel.ResumeLayout(false);
            this.outputPanel.ResumeLayout(false);
            this.outputPanel.PerformLayout();
            this.inputPanel.ResumeLayout(false);
            this.inputPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel mainPanel;
        private System.Windows.Forms.Panel outputPanel;
        private System.Windows.Forms.Label outputLabel;
        private System.Windows.Forms.Panel inputPanel;
        private System.Windows.Forms.ComboBox methodDropdown;
        private System.Windows.Forms.Label inputLabel;
        private System.Windows.Forms.TextBox outputTextBox;
        private System.Windows.Forms.TextBox inputTextBox;
        private System.Windows.Forms.Button submitInput;
    }
}