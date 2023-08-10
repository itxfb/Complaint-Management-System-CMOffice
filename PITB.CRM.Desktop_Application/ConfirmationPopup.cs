using PITB.CMS_Common.Models.Custom;
using System;
using System.Windows.Forms;

namespace PITB.CRM.Desktop_Application
{
    public partial class ConfirmationPopup : Form
    {
        public const string POPUP_NAME = "ConfirmationPopup";
        private DelegateModel.CallbackPopup callBackConfirmation;
        /*public ConfirmationPopup()
        {
            InitializeComponent();
        }*/

        public ConfirmationPopup(string heading, string message)
        {
            InitializeComponent();
            this.Text = heading;
            this.LabelMessageConfirmation.Text = message;
        }
        public ConfirmationPopup(string heading, string message, DelegateModel.CallbackPopup callbackPopup )
        {
            InitializeComponent();
            this.Text = heading;
            this.LabelMessageConfirmation.Text = message;
            callBackConfirmation = callbackPopup;
        }

       
        private void OnButtonClick(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            switch (btn.Name)
            {
                case "BtnConfirmYes":
                    this.Hide();
                    callBackConfirmation(POPUP_NAME, "Yes");
                    break;
                case "BtnConfirmNo":
                    this.Hide();
                    callBackConfirmation(POPUP_NAME, "No");
                    break;
            }
        }

    }
}
