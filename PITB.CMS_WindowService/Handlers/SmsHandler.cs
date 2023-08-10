using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using PITB.CMS;
using PITB.CMS.Handler.Messages;
using PITB.CMS.Models.Custom;
using PITB.CMS.Models.DB;
using PITB.CMS_WindowService.Models.Custom;
using PITB.CMS_WindowService.Models.DB;

namespace PITB.CMS_WindowService.Handlers
{
    public class SmsHandler
    {
        private static List<int> listStatusToSendInSms = new List<int>(){1,8,6}; 
        public static void SendStatusWiseStatsIndividualCampaign()
        {
            try
            {
                List<DbPermissionsAssignment> listPermission =
                    DbPermissionsAssignment.GetListOfPermissionsByTypeAndPermissionId((int)Config.PermissionsType.User,
                        (int)Config.UserPermissions.SendMessageStatsOfSpecificCampaign);

                List<int> listUniqueCampIds = HelperFunctions.GetUniqueCampaignIds(listPermission);
                List<ComplaintPartial> listComplaintPartial = DbComplaint.GetComplaintPartialListByCampaignId(listUniqueCampIds);

                List<ComplaintPartial> listComplaintPartialTemp = null;
                List<StatusWiseComplaintCount> listStatusWiseComplaintCount = null;
                DbUsers dbUser = null;
                int campaignId = -1;
                foreach (DbPermissionsAssignment permissionAssignment in listPermission)
                {
                    dbUser = DbUsers.GetUser((int)permissionAssignment.Type_Id);

                    campaignId = Convert.ToInt32(permissionAssignment.Permission_Value);
                    listComplaintPartialTemp = listComplaintPartial.Where(n => n.CampaignId == campaignId).ToList();
                    listStatusWiseComplaintCount = HelperFunctions.GetStatusWiseComplaintCount(listComplaintPartialTemp, listStatusToSendInSms);
                    SendMessageOfComplaintStatsIndividualCampaign(dbUser.Phone, listStatusWiseComplaintCount, campaignId);
                }
            }
            catch (Exception ex)
            {
                DbWindowServiceError.SaveErrorLog(1, Environment.StackTrace, ex, CMS.Config.ServiceType.SendMsgToDCO);
                //throw;
            }

        }

        public static void SendStatusWiseStatsCumulativeCampaign()
        {
            try
            {
                List<DbPermissionsAssignment> listPermission =
                    DbPermissionsAssignment.GetListOfPermissionsByTypeAndPermissionId((int)Config.PermissionsType.User,
                        (int) Config.UserPermissions.SendMessageStatsOfAllCumulativeCampaign);

                List<int> listUniqueCampIds = HelperFunctions.GetUniqueCampaignIds(listPermission);
                List<ComplaintPartial> listComplaintPartial =
                    DbComplaint.GetComplaintPartialListByCampaignId(listUniqueCampIds);

                List<ComplaintPartial> listComplaintPartialTemp = null;
                List<StatusWiseComplaintCount> listStatusWiseComplaintCount = null;
                DbUsers dbUser = null;
                List<int> listCampaignId = null;
                foreach (DbPermissionsAssignment permissionAssignment in listPermission)
                {
                    dbUser = DbUsers.GetUser((int) permissionAssignment.Type_Id);

                    listCampaignId = Utility.GetIntList(permissionAssignment.Permission_Value);
                    listComplaintPartialTemp =
                        listComplaintPartial.Where(n => listCampaignId.Contains(n.CampaignId)).ToList();
                    listStatusWiseComplaintCount = HelperFunctions.GetStatusWiseComplaintCount(
                        listComplaintPartialTemp, listStatusToSendInSms);
                    SendMessageOfComplaintStatsCumulativeCampaign(dbUser.Phone, listStatusWiseComplaintCount);
                }
            }
            catch (Exception ex)
            {
                DbWindowServiceError.SaveErrorLog(2, Environment.StackTrace, ex, CMS.Config.ServiceType.SendMsgToDCO);
                //throw;
            }

        }



        private static void SendMessageOfComplaintStatsIndividualCampaign(string phoneNos, List<StatusWiseComplaintCount> listStatusWiseComplaintCount, int campaignId)
        {
            DbCampaign dbCampaign = DbCampaign.GetById(campaignId);

            string[] phoneNoArr = phoneNos.Split(',');
            string sms = "";
            
            foreach (string phoneNo in phoneNoArr)
            {
                sms = "Complaint stats for campaign " + dbCampaign.Campaign_Name + "\nDate : " + DateTime.Now.ToShortDateString();
            
                foreach (StatusWiseComplaintCount statusWiseComplaintCount in listStatusWiseComplaintCount)
                {
                    sms = sms + "\n" + statusWiseComplaintCount.StatusName + " : " + statusWiseComplaintCount.ComplaintCount;
                }
                CallSendSmsAPI(phoneNo, sms);
            }
        }

        private static void SendMessageOfComplaintStatsCumulativeCampaign(string phoneNos, List<StatusWiseComplaintCount> listStatusWiseComplaintCount)
        {
            //DbCampaign dbCampaign = DbCampaign.GetById(campaignId);

            string[] phoneNoArr = phoneNos.Split(',');
            string sms = "";

            foreach (string phoneNo in phoneNoArr)
            {
                sms = "Complaint stats for (Smart Lahore)\nDate : " + DateTime.Now.ToShortDateString();

                foreach (StatusWiseComplaintCount statusWiseComplaintCount in listStatusWiseComplaintCount)
                {
                    sms = sms + "\n" + statusWiseComplaintCount.StatusName + " : " + statusWiseComplaintCount.ComplaintCount;
                }
                CallSendSmsAPI(phoneNo, sms);
            }
        }

        private static void CallSendSmsAPI(string phoneNo, string msg)
        {
            phoneNo = phoneNo.Replace("-", "").Trim();
            List<SmsModel> listModel = new List<SmsModel>();
            listModel.Add(new SmsModel(1, phoneNo, msg, (int)Config.MsgType.ToComplainant, (int)Config.MsgSrcType.Web, DateTime.Now, 1,-1));
            TextMessageApiHandler.SendMessage(listModel);
        }

        /// <summary>
        /// SendStatusWiseStatsforCampaignList
        /// </summary>
        /// <param name="Dictionary<int, string> dictCampIdsAndNumbers">Dictionary with camoign id and phone number(comma seperated for multiple numbers) to send summary message</param>
        /// <param name="List<int> listStatusToSendInSms">List of campaign-statuses whose report is to send in message</param>
        /// <returns></returns>
        public static void SendStatusWiseStatsforCampaignList(Dictionary<int, string> dictCampIdsAndNumbers, List<int> listStatusToSendInSms)
        {
            try
            {
                //dictCampIdsAndNumbers = new Dictionary<int, string>() { { 83, "03344657460" } };//special education camppign id
                //listStatusToSendInSms = new List<int>() { 1, 8, 6 };

                List<int> listCampIds = dictCampIdsAndNumbers.Keys.ToList();
                List<ComplaintPartial> listComplaintPartial = DbComplaint.GetComplaintPartialListByCampaignId(listCampIds);

                List<ComplaintPartial> listComplaintPartialTemp = null;
                List<StatusWiseComplaintCount> listStatusWiseComplaintCount = null;
                string phonenumber = null;
                int campaignId = -1;

                foreach (var campi in dictCampIdsAndNumbers)
                {
                    campaignId = campi.Key;
                    phonenumber = campi.Value;

                    listComplaintPartialTemp = listComplaintPartial.Where(n => n.CampaignId == campaignId).ToList();
                    listStatusWiseComplaintCount = HelperFunctions.GetStatusWiseComplaintCount(listComplaintPartialTemp, listStatusToSendInSms);
                    //SendMessageOfComplaintStatsIndividualCampaign(phonenumber, listStatusWiseComplaintCount, campaignId);
                    
                    //send message
                    string[] phoneNoArr = phonenumber.Split(',');
                    string sms = "";

                    foreach (string phoneNo in phoneNoArr)
                    {
                        sms = "Date : " + DateTime.Now.ToShortDateString();

                        foreach (StatusWiseComplaintCount statusWiseComplaintCount in listStatusWiseComplaintCount)
                        {
                            sms = sms + "\n" + statusWiseComplaintCount.StatusName + " : " + statusWiseComplaintCount.ComplaintCount;
                        }
                        CallSendSmsAPI(phoneNo, sms);
                    }

                }
            }
            catch (Exception ex)
            {
                DbWindowServiceError.SaveErrorLog(1, Environment.StackTrace, ex, CMS.Config.ServiceType.SendMsgToDCO);
                //throw;
            }

        }
    }
}
