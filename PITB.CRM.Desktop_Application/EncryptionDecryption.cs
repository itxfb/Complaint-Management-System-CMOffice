using PITB.CMS_Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PITB.CRM.Desktop_Application
{
    public partial class EncryptionDecryption : Form
    {
        public EncryptionDecryption()
        {
            InitializeComponent();
        }

        private void EncryptionDecryption_Load(object sender, EventArgs e)
        {
            methodDropdown.Focus();
            methodDropdown.Items.Add("Encrypt");
            methodDropdown.Items.Add("Decrypt");
        }

        private void submitInput_Click(object sender, EventArgs e)
        {
            try
            {
                if (methodDropdown.SelectedIndex == -1)
                {
                    methodDropdown.Focus();
                    MessageBox.Show("Please select operation from dropdown");
                }
                else if (string.IsNullOrEmpty(inputTextBox.Text))
                {
                    MessageBox.Show("Provide input text");
                    inputTextBox.Focus();
                }
                else
                {
                    string selectedItem = methodDropdown.Items[methodDropdown.SelectedIndex].ToString();

                    if (selectedItem == "Encrypt")
                    {
                        outputTextBox.Text = Utility.GetEncryptedString(inputTextBox.Text);
                    }
                    else if (selectedItem == "Decrypt")
                    {
                        outputTextBox.Text = Utility.GetDecryptedString(inputTextBox.Text);
                    }
                    outputTextBox.Focus();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error has occurred", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
