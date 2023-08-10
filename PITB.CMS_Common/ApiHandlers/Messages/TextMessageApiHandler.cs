using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PITB.CMS_Common.ApiModels.Custom;
using PITB.CMS_Common.Helper.Database;
using PITB.CMS_Common.Models.Custom;

namespace PITB.CMS_Common.ApiHandlers.Messages
{
    public class TextMessageApiHandler
    {
        /*public string MobileNumber { get; set; }
        public string MessageText { get; set; }
        public int MessageSentBy { get; set; }
        public int CampaignId { get; set; }
        public int ProfileId { get; set; }
        private bool IsSuccessfull { get; set; }
        */
        /*public TextMessageHandler(string mobileNumber,string messageText,int sentBy,int campaignId,int profileId)
        {
            MobileNumber = mobileNumber;
            MessageText = messageText;
            MessageSentBy = sentBy;
            CampaignId = campaignId;
            ProfileId = profileId;
        }
        */
        /*
        public async static void SendMessage(string mobileNo, string messageTxt)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var url =
                        string.Format(
                            "http://smsrouter.punjab.gov.pk/send_sms_from_smsrouter.json?number={0}&message={1}",
                            mobileNo, messageTxt);
                    await client.GetAsync(url);
                    
                }
                IsSuccessfull = true;
            }
            catch (Exception)
            {

                IsSuccessfull = false;
            }
            finally
            {
                SaveMessageLog(this);
            }
           
        }
        */

        public static async Task<HttpResponseMessage> SendMessage(List<SmsModel> list, String smsLanguage = "english"/*, string messageTxt*/)
        {
            try
            {
                const string secretKey = "6ddd1bfd360fb10ed8edb0f96af2e341";
                //string smsLanguage = "english";

                foreach (SmsModel smsModel in list)
                {

                    string message = "";
                    using (var client = new HttpClient())
                    {

                        var pairs = new List<KeyValuePair<string, string>>
                        {
                            new KeyValuePair<string, string>("sec_key", secretKey),
                            new KeyValuePair<string, string>("sms_text", smsModel.MessageText),
                            new KeyValuePair<string, string>("phone_no",smsModel.MobileNumber),
                            new KeyValuePair<string, string>("sms_language", smsLanguage)

                        };


                        var content = new FormUrlEncodedContent(pairs);


                        HttpResponseMessage responseMessage = await client.PostAsync("https://smsgateway.pitb.gov.pk/api/send_sms", content);
                        if (responseMessage.IsSuccessStatusCode)
                        {

                        }

                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        private void SaveMessageLog(SmsModel smsHistoryModel)
        {
            var paramDict = new Dictionary<string, object>();
            paramDict.Add("@CampaignId", smsHistoryModel.CampaignId);
            paramDict.Add("@MessageText", smsHistoryModel.MessageText);
            paramDict.Add("@MessageType", smsHistoryModel.MessageType);
            paramDict.Add("@SentBySrcId", smsHistoryModel.MessageSentBySrcId);
            paramDict.Add("@MobileNumber", smsHistoryModel.MobileNumber);
            paramDict.Add("@SentDateTime", DateTime.Now);
            paramDict.Add("@SentStatus", smsHistoryModel.SentStatus);
            DBHelper.CrudOperation("[PITB].[Create_Log_AllSent_Messages]", paramDict);
        }



    }
}