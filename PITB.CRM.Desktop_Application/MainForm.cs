using PITB.CMS_Common.Handler.Escalation;
using PITB.CMS_Common.Models.Custom;
using System;
using System.ServiceModel;
using System.Windows.Forms;
//using PITB.CRM_WCF_Service;

namespace PITB.CRM.Desktop_Application
{
    public partial class MainForm : Form
    {
        private DelegateModel delegateModel;
        private string prevCtrlName;

        private const string BTN_START_ESCALATION = "BtnStartEscalation";
        private const string BTN_STOP_ESCALATION = "BtnStopEscalation";
        private const string BTN_ENCRYPTION_DECRYPTION = "enc_dec_Btn";

        private EscalationHandler escalationHandler;
        private ServiceHost host;

        public MainForm()
        {
            InitializeComponent();
            InitializeVariables();
        }

        private void InitializeVariables()
        {
            try
            {
                delegateModel = new DelegateModel();
                delegateModel.callbackPopup = PopupCallBack;
                escalationHandler = new EscalationHandler();

                //host = new ServiceHost(typeof(CRM_WCF_Service.ServiceEscalation));
                //host.Open();

                //InstanceContext instanceContext = new InstanceContext(new CallbackHandler());
                //ServiceReference.ServiceEscalationClient serviceEscalation =
                //    new ServiceReference.ServiceEscalationClient(instanceContext);
                //serviceEscalation.GetData2(10);





                //serviceEscalation.get(instanceContext);

                //host = new ServiceHost(typeof(CRM_WCF_Service.ServiceEscalation));
                //host.Open();
                //var uri = new Uri("net.tcp://localhost");
                //var binding = new NetTcpBinding();
                //var callback = new MyCallbackClient();
                //var client = new MyContractClient(callback, binding, new EndpointAddress(uri));
                //var proxy = client.ChannelFactory.CreateChannel();
                //proxy.DoSomething();



            }
            catch (Exception ex)
            {
                ConfirmationPopup confirmationPopup = new ConfirmationPopup(ex.Message, ex.Message);
                confirmationPopup.Show();
                int i = 0;
            }
        }


        //public class CallbackHandler : ServiceReference.IServiceEscalationCallback
        //{
        //    public void OnCallback()
        //    {
        //        ConfirmationPopup confirmationPopup = new ConfirmationPopup("Start Escalation", "Please confirm if you want to start escalation",null);
        //        confirmationPopup.Show();
        //        Console.WriteLine("Hi from client!");
        //    }
        //}

        private void OnButtonClick(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            prevCtrlName = btn.Name;
            switch (btn.Name)
            {
                case BTN_START_ESCALATION:
                    ConfirmationPopup confirmationPopup = new ConfirmationPopup("Start Escalation", "Please confirm if you want to start escalation", delegateModel.callbackPopup);
                    confirmationPopup.Show();
                    break;
                case BTN_STOP_ESCALATION:
                    //host.Close();
                    break;
                case BTN_ENCRYPTION_DECRYPTION:
                    EncryptionDecryption encryptionDecryption = new EncryptionDecryption();
                    encryptionDecryption.Show();
                    break;
            }
        }

        public void PopupCallBack(string popupName, string message)
        {
            if (popupName == ConfirmationPopup.POPUP_NAME)
            {
                if (prevCtrlName == BTN_START_ESCALATION)
                {
                    escalationHandler.StartEscalation();
                }
            }
        }
    }
}
