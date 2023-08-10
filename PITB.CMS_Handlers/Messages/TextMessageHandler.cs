using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PITB.CMS_Common.Handler.Authentication;
using PITB.CMS_Common.Helper.Database;
using PITB.CMS_Common.Models;
using PITB.CMS_Common.Models.Custom;
using PITB.CMS_Common.Models.View;
using PITB.CMS_Common.Models.View.Dynamic;

namespace PITB.CMS_Handlers.Messages
{

    public class TextMessageHandler
    {

        #region SendMessages


        public static void SendSms(Dictionary<string, object> dictSmsParams)
        {
            DbComplaint dbComplaint = (DbComplaint) dictSmsParams["dbComplaint"];
            int campaignId = (int)dictSmsParams["campaignId"];
            string smsText = (string) dictSmsParams["smsText"];
            string tagId = (string)dictSmsParams["ModuleId"];
         
            if (tagId == "FollowUp")
            {
                if (campaignId == (int) Config.Campaign.DcoOffice)
                {
                    List<DbUsers> listDbUsers = TextMessageHandler.PrepareAndSendMessageToStakeholdersOnComplaintLaunch(dbComplaint, smsText);
                }
                
            }
        }

        public static List<DbUsers> PrepareAndSendMessageToStakeholdersOnComplaintLaunch(DbComplaint dbComplaint, string msgToSend = null, bool canSaveMessage = false, SmsModel smsModel = null)
        {

            //DbComplaint dbComplaint = DbComplaint.GetByComplaintId(complaintId);
            int hierarchyVal = DbComplaint.GetHierarchyIdValueAgainstComplaint(dbComplaint);
            List<DbUsers> listUsersToSendMsg = DbUsers.GetByCampIdH_IdUserH_Id2(Convert.ToInt32(dbComplaint.Compaign_Id), (Config.Hierarchy)dbComplaint.Complaint_Computed_Hierarchy_Id,
                hierarchyVal, dbComplaint.Complaint_Computed_User_Hierarchy_Id, Convert.ToInt32(dbComplaint.Complaint_Category), dbComplaint.UserCategoryId1, dbComplaint.UserCategoryId2);
            List<string> phoneNoList = null;
            if (listUsersToSendMsg != null)
            {
                foreach (DbUsers dbUser in listUsersToSendMsg)
                {
                    if (!string.IsNullOrEmpty(dbUser.Phone))
                    {
                        phoneNoList = new List<string>(GetPhoneNumbersFromPhoneNoString(dbUser.Phone));
                        foreach (string phoneNo in phoneNoList)
                        {
                            SendMessageToStakeholdersOnComplaintLaunch(phoneNo, dbComplaint, msgToSend, canSaveMessage, smsModel);
                        }
                    }
                }
            }
            return listUsersToSendMsg;
        }

        public static void SendMessageOnComplaintLaunch(string phoneNo, int campaignId, int complaintId, int categoryId, string personName = null, List<VmDynamicField> listVmDynamic = null)
        {
            if (campaignId == (int)Config.Campaign.DcoOffice || campaignId == (int)Config.Campaign.Police)
            {
                DbCampaign dbCampaign = DbCampaign.GetById(campaignId);
                DbComplaintType dbComplaintType = DbComplaintType.GetById(categoryId);
                //string sms = "Your complaint has been successfully registered in campaign (" + dbCampaign.Campaign_Name + ")\n\nCategory : " +
                //             dbComplaintType.Name + "\nComplaint Id : " + campaignId + "-" + complaintId;

                //                string sms =
                //                    @"Dear {0}, We have received your complaint for '{1}'. The complaint no. is '{2}' for your reference. Concerned authority will respond to your complaint.\n \n
                //                Thank you for calling at Raabta Helpline, GoP, 0800-02345";
                string sms = "Dear " + personName + ", We have received your complaint for '" + dbComplaintType.Name + "'. The complaint no. is '" + campaignId + "-" + complaintId + "' for your reference. Concerned authority will respond to your complaint.\n\nThank you for calling Chief Minister's Complaint Center, 0800-02345";
                CallSendSmsAPI(phoneNo, sms);
            }
            else
            {
                DbCampaign dbCampaign = DbCampaign.GetById(campaignId);
                DbComplaintType dbComplaintType = DbComplaintType.GetById(categoryId);
                string sms = "Your complaint has been successfully registered in campaign (" + dbCampaign.Campaign_Name + ")\n\nCategory : " +
                             dbComplaintType.Name + "\nComplaint Id : " + campaignId + "-" + complaintId;
                CallSendSmsAPI(phoneNo, sms);
            }
        }

        public static void SendMessageToPhoneNoList(IEnumerable<string> listPhoneNo, string msg, bool canSaveMessage = false, SmsModel smsModel = null)
        {
            if (listPhoneNo != null)
            {
                foreach (string sphoneNo in listPhoneNo)
                {
                    if (!string.IsNullOrEmpty(sphoneNo))
                    {
                        /*updated by usman on 10/22/2019*/
                        IEnumerable<string> sphoneNos = sphoneNo.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                        foreach (string phoneNo in sphoneNos)
                        {
                            if (!string.IsNullOrEmpty(phoneNo))
                            {
                                if (smsModel != null)
                                {
                                    smsModel.MobileNumber = phoneNo;
                                }
                                SendMessageToPhoneNo(phoneNo, msg, canSaveMessage, smsModel);
                            }
                        }
                    }
                }
            }
        }

        public static void SendMessageToUsersList(List<DbUsers> listDbUsers, string msg, bool canSaveMessage = false, SmsModel smsModel = null)
        {
            if (listDbUsers != null)
            {
                foreach (DbUsers dbUser in listDbUsers)
                {
                    if (!string.IsNullOrEmpty(dbUser.Phone))
                    {
                        SendMessageToPhoneNo(dbUser.Phone, msg, canSaveMessage, smsModel);
                    }
                }
            }
        }

        public static void SendMessageToPhoneNo(string phoneNo, string msg)
        {
            CallSendSmsAPI(phoneNo, msg);
        }

        public static void SendMessageToPhoneNo(string phoneNo, string msg, bool canSaveMessage, SmsModel smsModel)
        {
            CallSendSmsAPI(phoneNo, msg, canSaveMessage, smsModel);
        }

        public static void SendMessageOnStatusChange(string phoneNo, int campaignId, int complaintId, int categoryId, int statusId, string statusComments)
        {
            //if (statusId == 8)

            DbCampaign dbCampaign = DbCampaign.GetById(campaignId);
            DbComplaintType dbComplaintType = DbComplaintType.GetById(categoryId);
            List<DbDynamicComplaintFields> dynaimcFields = DbDynamicComplaintFields.GetByComplaintId(complaintId);
            string alert = "";
            if (dynaimcFields != null)
            {
                var callSource = dynaimcFields.SingleOrDefault(x => x.ControlId == 20 && x.CategoryTypeId == 6123);
                if (callSource != null)
                {
                    //alert = callSource.FieldValue;
                    alert = "Chief Minister's Complaint Center";
                }

            }
            string sms = null;
            if (campaignId.Equals((int)Config.Campaign.DcoOffice))
            {
                sms = string.Format("Dear Citizen,\n\nYour complaint no. {0} has been resolved. You will receive a call from the Chief Minister's office requesting your feedback.\n\nThank you for contacting Chief Minister's Complaint Center 0800-02345", complaintId);
            }
            else if (campaignId.Equals((int)Config.Campaign.CombinedCampaign))
            {
                sms = string.Format("Your feedback no. {0} has been resolved.\n\nThank you for using 'Hamara Lodhran' for feedback");
            }
            else
            {
                sms = string.Format("Your complaint no. {0} has been resolved.\n\nThank you for contacting {1} 0800-02345", complaintId, alert);
            }


            //"Thank you for contacting " + alert + ", 0800-02345";

            /*
            if (campaignId == 70) // dc office
            {
                sms = alert + "Your complaint no. " + complaintId + " has been resolved.\'s\n" +
                      "Thank you for contacting Chief Minister's Complaint Center, 0800-02345";
            }
            else
            {
                sms = alert + "Your complaint no. " + complaintId + " has been resolved.\n\n" +
                      "Thank you for contacting " + alert + " , 0800-02345";
            }*/
            CallSendSmsAPI(phoneNo, sms);

        }

        #endregion

        private static string[] GetPhoneNumbersFromPhoneNoString(string phoneNoStr)
        {
            phoneNoStr = phoneNoStr.Replace(" ", "");
            phoneNoStr = phoneNoStr.Replace("-", "");
            return phoneNoStr.Split(',');
        }

        private static void SendMessageToStakeholdersOnComplaintLaunch(string phoneNo, DbComplaint dbComplaint, string msgToSend = null, bool canSaveMessage = false, SmsModel smsModel = null)
        {
            DbCampaign dbCampaign = DbCampaign.GetById(Convert.ToInt32(dbComplaint.Compaign_Id));
            DbComplaintType dbComplaintType = DbComplaintType.GetById(Convert.ToInt32(dbComplaint.Complaint_Category));

            string sms = "A complaint has been recieved in campaign (" + dbCampaign.Campaign_Name + ")\n\nCategory : " +
                         dbComplaintType.Name + "\nComplaint Id : " + dbComplaint.Compaign_Id + "-" + dbComplaint.Id;

            if (msgToSend == null)
            {
                msgToSend = sms;
            }
            CallSendSmsAPI(phoneNo, msgToSend, canSaveMessage, smsModel);
        }




        private static void CallSendSmsAPI(string phoneNo, string msg, bool canSaveMessage = false, SmsModel smsModel = null)
        {
            phoneNo = phoneNo.Trim();
            if (!string.IsNullOrEmpty(phoneNo))
            {
                //phoneNo = phoneNo.Replace("-", "").Trim();
                List<SmsModel> listModel = new List<SmsModel>();
                if (smsModel == null)
                {
                    listModel.Add(new SmsModel(1, phoneNo, msg, (int)Config.MsgType.ToComplainant,
                        (int)Config.MsgSrcType.Web, DateTime.Now, 1, -1));
                }
                else
                {
                    listModel.Add(smsModel);
                }

                if (canSaveMessage)
                {
                    SaveMessageLog(smsModel);
                }

                TextMessageApiHandler.SendMessage(listModel);
            }
        }

        private static void SaveMessageLog(SmsModel smsHistoryModel)
        {
            var paramDict = new Dictionary<string, object>();
            paramDict.Add("@CampaignId", smsHistoryModel.CampaignId);
            paramDict.Add("@MessageText", smsHistoryModel.MessageText);
            paramDict.Add("@MessageType", smsHistoryModel.MessageType);
            paramDict.Add("@SentBySrcId", smsHistoryModel.MessageSentBySrcId);
            paramDict.Add("@MobileNumber", smsHistoryModel.MobileNumber);
            paramDict.Add("@SentDateTime", DateTime.Now);
            paramDict.Add("@SentStatus", smsHistoryModel.SentStatus);
            paramDict.Add("@MessageTagId", smsHistoryModel.MessageTagId);
            DBHelper.CrudOperation("[PITB].[Create_Log_AllSent_Messages]", paramDict);
        }

    }
}