using PITB.CMS_Common.ApiHandlers.Translation;
using PITB.CMS_Common.ApiModels.Custom;
using PITB.CMS_Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace PITB.CMS_Common.ApiHandlers.Messages
{
    public class TextMessageHandler
    {

        public static void PrepareAndSendMessageToStakeholdersOnComplaintLaunch(int complaintId, string complainantContactInfo)
        {
            DbComplaint dbComplaint = DbComplaint.GetByComplaintId(complaintId);
            int hierarchyVal = DbComplaint.GetHierarchyIdValueAgainstHierarchyId(dbComplaint);
            List<DbUsers> listUsersToSendMsg = DbUsers.GetByCampIdH_IdUserH_Id(Convert.ToInt32(dbComplaint.Compaign_Id), (Config.Hierarchy)dbComplaint.Complaint_Computed_Hierarchy_Id,
                hierarchyVal, dbComplaint.Complaint_Computed_User_Hierarchy_Id, Convert.ToInt32(dbComplaint.Complaint_Category));
            List<string> phoneNoList = null;
            foreach (DbUsers dbUser in listUsersToSendMsg)
            {
                if (!string.IsNullOrEmpty(dbUser.Phone))
                {
                    phoneNoList = new List<string>(GetPhoneNumbersFromPhoneNoString(dbUser.Phone));
                    foreach (string phoneNo in phoneNoList)
                    {
                        SendMessageToStakeholdersOnComplaintLaunch(phoneNo, dbComplaint, complainantContactInfo);
                    }
                }
            }
        }

        private static void SendMessageToStakeholdersOnComplaintLaunch(string phoneNo, DbComplaint dbComplaint, string complainantContactInfo)
        {
            DbCampaign dbCampaign = DbCampaign.GetById(Convert.ToInt32(dbComplaint.Compaign_Id));
            DbComplaintType dbComplaintType = DbComplaintType.GetById(Convert.ToInt32(dbComplaint.Complaint_Category));

            //string sms = "A complaint has been recieved in campaign (" + dbCampaign.Campaign_Name + ")\n\nCategory : " +
            //             dbComplaintType.Name + "\nComplaint Id : " + dbComplaint.Compaign_Id + "-" + dbComplaint.Id+
            //             "\n Complainant Phone No : "+complainantContactInfo;

            Dictionary<string, TranslatedModel> translationDict = DbTranslationMapping.GetTranslationDictionaryFromTranslationMapping_API(DbTranslationMapping.GetAllTranslation());
            dbComplaint.GetTranslatedModel<DbComplaint>(@"Compaign_Id,Name,Campaign_Name", translationDict, Config.Language.Urdu);
            //string sms = complainantContactInfo + "شکایت فون نمبر \n" + dbComplaint.Compaign_Id + "شکایت نمبر" + dbComplaintType.Name + ":قسم \n\n" + dbCampaign.Campaign_Name + ")ایک شکایت درج ہوئ ہے مہم";

            string sms = "ایک شکایت درج ہوئ ہے مہم"+
                         " (" + dbCampaign.Campaign_Name + ") میں\n\n" +
                         " قسم: " + dbComplaintType.Name + " \n" +
                         "شکایت نمبر: " + "" + dbComplaint.Id + "-" + dbComplaint.Compaign_Id + "\n"+
                         "شکایت فون نمبر: " + complainantContactInfo;
            CallSendSmsAPI(phoneNo, sms);
        }

        public static void SendVerificationMessageToStakeholder(DbUsers dbUsers)
        {
            if (!string.IsNullOrEmpty(dbUsers.Phone))
            {
                List<string> phoneNoList = new List<string>(); 
                phoneNoList = new List<string>(GetPhoneNumbersFromPhoneNoString(dbUsers.Phone));
                string sms = "";
                phoneNoList = phoneNoList.Distinct().ToList();
                foreach (string phoneNo in phoneNoList)
                {
                    //sms = "You have been registered to use Punjab Fixit Application";
                    sms = "آپ کو پنجاب فکسٹ کو استعمال کرنے کا اختیار دے دیا ہے";
                    CallSendSmsAPI(phoneNo, sms);
                }
            }
        }

        public static void SendVerificationMessageToComplainant(DbPersonInformation dbPersonalInformation)
        {
            if (!string.IsNullOrEmpty(dbPersonalInformation.Mobile_No))
            {
                List<string> phoneNoList = new List<string>();
                phoneNoList = new List<string>(GetPhoneNumbersFromPhoneNoString(dbPersonalInformation.Mobile_No));
                string sms = "";
                foreach (string phoneNo in phoneNoList)
                {
                    //sms = "You have been registered to use Punjab Fixit Application";
                    sms = "آپ کو پنجاب فکسٹ کو استعمال کرنے کا اختیار دے دیا ہے";
                    CallSendSmsAPI(phoneNo, sms);
                }
            }
        }

        private static string[] GetPhoneNumbersFromPhoneNoString(string phoneNoStr)
        {
            phoneNoStr = phoneNoStr.Replace(" ", "");
            phoneNoStr = phoneNoStr.Replace("-", "");
            return phoneNoStr.Split(',');
        }

        public static void SendMessageOnComplaintLaunch(string phoneNo, int campaignId, int complaintId, int categoryId)
        {
            DbCampaign dbCampaign = DbCampaign.GetById(campaignId);
            DbComplaintType dbComplaintType = DbComplaintType.GetById(categoryId);
            //string sms = "Your complaint has been successfully registered in campaign (" + dbCampaign.Campaign_Name + ")\n\nCategory : " +
            //             dbComplaintType.Name + "\nComplaint Id : " + campaignId + "-" + complaintId;

            Dictionary<string, TranslatedModel> translationDict = DbTranslationMapping.GetTranslationDictionaryFromTranslationMapping_API(DbTranslationMapping.GetAllTranslation());
            dbCampaign.GetTranslatedModel<DbCampaign>(@"Campaign_Name", translationDict, Config.Language.Urdu);
            dbComplaintType.GetTranslatedModel<DbComplaintType>(@"Name", translationDict, Config.Language.Urdu);


            //string sms = "" + campaignId + "-" + complaintId + " : شکایت نمبر\n" + dbComplaintType.Name + " : قسم\n\n(" +
            //      dbCampaign.Campaign_Name + ") آپ کی شکایت کامیابی سے درج کر دی گی ہے مہم";

            //sms = dbComplaintType.Name + " : قسم\n\n(" + dbCampaign.Campaign_Name + ") آپ کی شکایت کامیابی سے درج کر دی گی ہے مہم";

            string sms = "آپ کی شکایت کامیابی سے درج کر دی گی ہے مہم "+
                  " (" + dbCampaign.Campaign_Name + ") میں\n\n" +
                  " قسم: " + dbComplaintType.Name +" \n"+
                  "شکایت نمبر: " + "" + complaintId + "-" + campaignId + "\n";


            List<string> listPhoneNo = GetListOfPhoneNoOfStakeholdersAgainstComplaintId(complaintId);
            string stakeholderphoneNo = "";

            stakeholderphoneNo = string.Join(",", listPhoneNo.Select(n => n.ToString()).ToArray());
            if (!string.IsNullOrEmpty(stakeholderphoneNo))
            {
                //sms = sms + "\nPlease Contact the following people for your complaint follow up.\nContact No:" +
                //      stakeholderphoneNo;
                sms = sms +".آپ کی شکایت پر اپ کی پیروی کے لئے درج ذیل افراد سے رابطہ کریں \n" +
                      "رابطہ نمبر: "+
                      stakeholderphoneNo;
            }
            CallSendSmsAPI(phoneNo, sms);

        }

        public static void SendMessageOnComplaintLaunch(string phoneNo, int campaignId, int complaintId,
            int categoryId, string complaintNo)
        {
            DbCampaign dbCampaign = DbCampaign.GetById(campaignId);
            DbComplaintType dbComplaintType = DbComplaintType.GetById(categoryId);
            //string sms = "Your complaint has been successfully registered in campaign (" + dbCampaign.Campaign_Name + ")\n\nCategory : " +
            //             dbComplaintType.Name + "\nComplaint Id : " + campaignId + "-" + complaintId;

            Dictionary<string, TranslatedModel> translationDict = DbTranslationMapping.GetTranslationDictionaryFromTranslationMapping_API(DbTranslationMapping.GetAllTranslation());
            dbCampaign.GetTranslatedModel<DbCampaign>(@"Campaign_Name", translationDict, Config.Language.Urdu);
            dbComplaintType.GetTranslatedModel<DbComplaintType>(@"Name", translationDict, Config.Language.Urdu);


            //string sms = "" + campaignId + "-" + complaintId + " : شکایت نمبر\n" + dbComplaintType.Name + " : قسم\n\n(" +
            //      dbCampaign.Campaign_Name + ") آپ کی شکایت کامیابی سے درج کر دی گی ہے مہم";

            //sms = dbComplaintType.Name + " : قسم\n\n(" + dbCampaign.Campaign_Name + ") آپ کی شکایت کامیابی سے درج کر دی گی ہے مہم";

            string sms = "آپ کی شکایت کامیابی سے درج کر دی گی ہے مہم " +
                  " (" + dbCampaign.Campaign_Name + ") میں\n\n" +
                  " قسم: " + dbComplaintType.Name + " \n" +
                  "شکایت نمبر: " + complaintNo+ "\n";


            List<string> listPhoneNo = GetListOfPhoneNoOfStakeholdersAgainstComplaintId(complaintId);
            string stakeholderphoneNo = "";

            stakeholderphoneNo = string.Join(",", listPhoneNo.Select(n => n.ToString()).ToArray());
            if (!string.IsNullOrEmpty(stakeholderphoneNo))
            {
                //sms = sms + "\nPlease Contact the following people for your complaint follow up.\nContact No:" +
                //      stakeholderphoneNo;
                sms = sms + ".آپ کی شکایت پر اپ کی پیروی کے لئے درج ذیل افراد سے رابطہ کریں \n" +
                      "رابطہ نمبر: " +
                      stakeholderphoneNo;
            }
            CallSendSmsAPI(phoneNo, sms);
        }
        private static List<string> GetListOfPhoneNoOfStakeholdersAgainstComplaintId(int complaintId)
        {
            DbComplaint dbComplaint = DbComplaint.GetByComplaintId(complaintId);
            int hierarchyVal = DbComplaint.GetHierarchyIdValueAgainstHierarchyId(dbComplaint);
            List<DbUsers> listUsersToSendMsg = DbUsers.GetByCampIdH_IdUserH_Id(Convert.ToInt32(dbComplaint.Compaign_Id), (Config.Hierarchy)dbComplaint.Complaint_Computed_Hierarchy_Id,
                hierarchyVal, dbComplaint.Complaint_Computed_User_Hierarchy_Id, Convert.ToInt32(dbComplaint.Complaint_Category));
            List<string> phoneNoList = new List<string>();
            foreach (DbUsers dbUser in listUsersToSendMsg)
            {
                if (!string.IsNullOrEmpty(dbUser.Phone))
                {
                    phoneNoList.AddRange(GetPhoneNumbersFromPhoneNoString(dbUser.Phone));
                }
            }
            return phoneNoList;
        }

        public static void SendMessageOnStatusChange(string phoneNo, int campaignId, int complaintId, int categoryId, int statusId, string statusComments)
        {
            DbCampaign dbCampaign = DbCampaign.GetById(campaignId);
            DbComplaintType dbComplaintType = DbComplaintType.GetById(categoryId);
            DbStatus dbStatus = DbStatus.GetById(statusId);
            //string sms = "Sir status of complaint (" + campaignId + "-" + complaintId + ") has been changed to (" + dbStatus.Status + "\n\nCategory :  " +
                         //dbComplaintType.Name + "\nCampaign : " + dbCampaign.Campaign_Name + "" + "\nComments : " + statusComments;
            
            //string sms = "Sir status of complaint (" + campaignId + "-" + complaintId + ") has been changed to (" + dbStatus.Status + "\n\nCategory :  " +
            //             dbComplaintType.Name + "\nCampaign : " + dbCampaign.Campaign_Name + "" + "\nComments : " + statusComments;



            Dictionary<string, TranslatedModel> translationDict = DbTranslationMapping.GetTranslationDictionaryFromTranslationMapping_API(DbTranslationMapping.GetAllTranslation());
            dbStatus.GetTranslatedModel<DbStatus>(@"Status", translationDict, Config.Language.Urdu);
            dbComplaintType.GetTranslatedModel<DbComplaintType>(@"Name", translationDict, Config.Language.Urdu);
            dbCampaign.GetTranslatedModel<DbCampaign>(@"Campaign_Name", translationDict, Config.Language.Urdu);
            
            string sms = "آپ کی شکایت"
                  + "(" + complaintId + "-" + campaignId+") "+
                  "کا اسٹیٹس تبدیل کر دیا گیا ہے \n"+
                  "اسٹیٹس: " + dbStatus.Status +" \n"+
                  "قسم: "+dbComplaintType.Name+" \n"+
                  "مہم: " + dbCampaign.Campaign_Name+" \n"+
                  "تبصرہ: " + statusComments;
            CallSendSmsAPI(phoneNo, sms);
        }

        private static void CallSendSmsAPI(string phoneNo, string msg)
        {
            phoneNo = phoneNo.Replace("-", "").Trim();
            List<SmsModel> listModel = new List<SmsModel>();
            listModel.Add(new SmsModel(1, phoneNo, msg, (int)Config.MsgType.ToComplainant, (int)Config.MsgSrcType.Web, DateTime.Now, 1));
            TextMessageApiHandler.SendMessage(listModel, "urdu");
        }
        
    }
}