using PITB.CMS_Common.Handler.Authentication;
using PITB.CMS_Common.Handler.DataTableJquery;
using PITB.CMS_Common.Handler.Messages;
using PITB.CMS_Common.Helper.Database;
using PITB.CMS_Common.Models.Custom;
using PITB.CMS_Common.Models.Custom.DataTable;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using PITB.CMS_Common.Models.View.ClientMesages;
using PITB.CMS_Common.Models.View.Message;
using System.Threading.Tasks;
using PITB.CMS_Common.Models;

namespace PITB.CMS_Common.Handler.Business
{
    public class BlMessages
    {
        public static VmMessageReply GetMessageReplyVm(int messageId)
        {
            VmMessageReply vmTagEdit = new VmMessageReply();
            DbIncomingMessages incomingMsg = DbIncomingMessages.GetByMessageId(messageId);

            vmTagEdit.IncomingMsgId = incomingMsg.Id;
            vmTagEdit.CallerNo = incomingMsg.Caller_No;
            vmTagEdit.IncomingMsgStr = incomingMsg.Msg_Text;
            vmTagEdit.CampaignId = Convert.ToInt32(AuthenticationHandler.GetCookie().Campaigns);

            return vmTagEdit;
        }

        public static ClientMessage OnReplySubmit(VmMessageReply vmMessageReply)
        {
            try
            {
                DBContextHelperLinq db = new DBContextHelperLinq();
                DbReplyMessages dbReplyMessages = new DbReplyMessages();
                db.DbReplyMessages.Add(dbReplyMessages);
                dbReplyMessages.Created_DateTime = DateTime.Now;
                dbReplyMessages.Incoming_Message_Id = vmMessageReply.IncomingMsgId;
                dbReplyMessages.Campaign_Id = vmMessageReply.CampaignId;
                dbReplyMessages.Caller_No = vmMessageReply.CallerNo;
                dbReplyMessages.Msg_Text = vmMessageReply.ReplyMsgStr;
                dbReplyMessages.Status = (int) Config.MessageStatus.New;
                dbReplyMessages.Created_DateTime = DateTime.Now;
                dbReplyMessages.Group_Id = -1;
                db.SaveChanges();
                return new ClientMessage("Successfully replied", true);
            }
            catch (Exception ex)
            {
                return new ClientMessage("Some error has occured", false);
            }

        }

        public static ClientMessage OnMassReplySubmit(VmMassMessageReply vmMessageReply)
        {
            try
            {
                if (vmMessageReply.CommaSeperatedIds != null)
                {
                    List<int> listIncomingIds = vmMessageReply.CommaSeperatedIds.Split(',').Select(int.Parse).ToList();
                    List<DbIncomingMessages> listDbIncomingMessages =
                        DbIncomingMessages.GetByIncomingMessageIdsList(listIncomingIds);

                    DBContextHelperLinq db = new DBContextHelperLinq();
                    DbReplyMessages dbReplyMessages;
                    foreach (DbIncomingMessages dbIncomingMessage in listDbIncomingMessages)
                    {
                        dbReplyMessages = new DbReplyMessages();
                        db.DbReplyMessages.Add(dbReplyMessages);
                        dbReplyMessages.Created_DateTime = DateTime.Now;
                        dbReplyMessages.Incoming_Message_Id = dbIncomingMessage.Id;
                        dbReplyMessages.Campaign_Id = dbIncomingMessage.Campaign_Id;
                        dbReplyMessages.Caller_No = dbIncomingMessage.Caller_No;
                        dbReplyMessages.Msg_Text = vmMessageReply.ReplyMessageStr;
                        dbReplyMessages.Status = (int) Config.MessageStatus.New;
                        dbReplyMessages.Created_DateTime = DateTime.Now;
                        dbReplyMessages.Group_Id = -1;

                    }
                    db.SaveChanges();


                    return new ClientMessage("Successfully replied", true);
                }
                else
                {
                    return new ClientMessage("Please select user to send message", false);
                }
               
            }
            catch (Exception ex)
            {
                return new ClientMessage("Some error has occured", false);
            }

        }

        public static DataTable GetAllThreadListingForExport()
        {
            List<VmStakeholderMessageThreadListing> listThread = new List<VmStakeholderMessageThreadListing>();
            List<string> listPhoneNo = DbIncomingMessages.GetUniquePhoneNoList();

            foreach (string phoneNo in listPhoneNo)
            {
                listThread.AddRange(GetThreadListingOfPhoneNo(phoneNo));
            }

            foreach (VmStakeholderMessageThreadListing thread in listThread)
            {
                if (thread.Is_Incoming_Message == "True")
                {
                    thread.Is_Incoming_Message = "Incoming";
                }
                else
                {
                    thread.Is_Incoming_Message = "Outgoing";
                }
            }

            return listThread.ToDataTable();
        }

        public static List<VmStakeholderMessageThreadListing> GetThreadListingOfPhoneNo(string phoneNo)
        {
            List<DbIncomingMessages> listIncomingMessages = DbIncomingMessages.GetMessagesByPhoneNo(phoneNo);
            List<DbReplyMessages> listReplyMessages = DbReplyMessages.GetMessagesByPhoneNo(phoneNo);

            return MergeMessagesAsSingleThread(listIncomingMessages, listReplyMessages);
        }

        public static List<VmStakeholderMessageThreadListing> GetMassMessagePopupList()
        {
            List<VmStakeholderMessageThreadListing> listVmStakeholder = new List<VmStakeholderMessageThreadListing>();
            List<DbIncomingMessages> listIncomingMessages = DbIncomingMessages.GetAllMessagesGroupByPhoneNoOrderByDateTime();
            DbIncomingMessages dbIncomingMessages = null;
            var phoneGroup = listIncomingMessages.GroupBy(n => n.Caller_No);
            foreach (var ph in phoneGroup)
            {
                dbIncomingMessages = listIncomingMessages.Where(n => n.Caller_No == ph.Key).First();
                listVmStakeholder.Add(new VmStakeholderMessageThreadListing(dbIncomingMessages,true));
            }
            return listVmStakeholder;
        }
        public static void SendTextMessageToHeadTeachers()
        {
            string smsText = @"
تمام  AEOs اور   ہیڈ ٹیچرز کو مطلع کیا جاتا ہے کہ Revised LND کا تعارف اور  اس کے لیسن پلانز https://sis.punjab.gov.pk/user/login  پربھی upload بھی   کردیے گئے ہیں ۔  لیسن پلانز کے ساتھ کہانی یا نظم بھی مہیا کی گئی ہے۔ آپ سے گذارش ہے کہ ان  کو ڈاؤن لوڈ کرکے  کلاس  3کے ٹیچرز کو فراہم کیے جائیں۔ ہمیں امید ہے کہ یہ تدریسی مواد طلبہ کی لٹریسی اور نیومیریسی سکلز کو بہتر بنانے میں اہم کردار ادا کرے گا۔ 
";
            using (DBContextHelperLinq db = new DBContextHelperLinq())
            {
                var data = db.DbUsers.Where(x => x.Campaign_Id == 47 && x.IsActive == true && x.UserCategoryId1 == 2 && x.Hierarchy_Id == Config.Hierarchy.UnionCouncil).Select(z => new { Phone = z.Phone});
                

                Parallel.ForEach(data, (x) => 
                {
                    string phoneNo = x.Phone;
                    if(!string.IsNullOrEmpty(phoneNo))
                        TextMessageHandler.SendMessageToPhoneNo(phoneNo, smsText);
                    return;
                });
                
            }
        }
        private static List<VmStakeholderMessageThreadListing> MergeMessagesAsSingleThread(List<DbIncomingMessages> listIncomingMessages, List<DbReplyMessages> listReplyMessages)
        {
            VmStakeholderMessageThreadListing thread = null;
            List<DbReplyMessages> listTempReplyMessages = null;
            List<VmStakeholderMessageThreadListing> listThread = new List<VmStakeholderMessageThreadListing>();

            foreach (DbIncomingMessages dbIncomingMessages in listIncomingMessages)
            {
                thread = new VmStakeholderMessageThreadListing();
                thread.Caller_No = dbIncomingMessages.Caller_No;
                thread.Created_DateTime = dbIncomingMessages.Message_Created_DateTime.ToString();
                thread.Id = dbIncomingMessages.Id;
                thread.Is_Incoming_Message = true.ToString();
                thread.Msg_Text = dbIncomingMessages.Msg_Text;

                listThread.Add(thread);

                listTempReplyMessages = listReplyMessages.Where(n => n.Incoming_Message_Id==dbIncomingMessages.Id).OrderBy(n=>n.Created_DateTime).ToList();
                foreach (DbReplyMessages dbReplyMessages in listTempReplyMessages)
                {
                    thread = new VmStakeholderMessageThreadListing();
                    thread.Caller_No = dbReplyMessages.Caller_No;
                    thread.Created_DateTime = dbReplyMessages.Created_DateTime.ToString();
                    thread.Is_Incoming_Message = false.ToString();
                    thread.Msg_Text = dbReplyMessages.Msg_Text;

                    listThread.Add(thread);
                }    
            }

            return listThread;
        }

        public static List<VmStakeholderMessageListing> GetMessagingListDataTable(DataTableParamsModel dtModel)
        {
            
            List<string> prefixStrList = new List<string> { "IncomingMessages", "IncomingMessages", "IncomingMessages", "IncomingMessages", "ReplyMessages" };

            Dictionary<string, string> dictOrderQuery = new Dictionary<string, string>();
            dictOrderQuery.Add("ReplyMessages", "ReplyMessages");

            DataTableHandler.ApplyColumnOrderPrefix(dtModel, prefixStrList, dictOrderQuery);

            Dictionary<string, string> dictFilterQuery = new Dictionary<string, string>();
            dictFilterQuery.Add("IncomingMessages.Message_Created_DateTime", "CONVERT(VARCHAR(20),IncomingMessages.Message_Created_DateTime,120) Like '%_Value_%'");
            dictFilterQuery.Add("ReplyMessages.ReplyMessages", "count(b.id) Like '%_Value_%'");

            //dictFilterQuery.Add("complaints.Created_Date", "CONVERT(VARCHAR(10),complaints.Created_Date,120) Like '%_Value_%'");

            DataTableHandler.ApplyColumnFilters(dtModel, new List<string>(), prefixStrList, dictFilterQuery);
            //List<VmAgentTagListing> listOfTagging = GetTaggingList(dtModel);

            if (dtModel.Draw == 1)
            {
                dtModel.ListOrder[0].columnName = "IncomingMessages.Message_Created_DateTime";
                dtModel.ListOrder[0].sortingDirectionStr = "desc";
            }


            // call api
            CMSCookie cookie = new AuthenticationHandler().CmsCookie;
            Dictionary<string, object> paramDict = new Dictionary<string, object>();
            paramDict.Add("@CampaignId", (Convert.ToInt32(cookie.Campaigns)).ToDbObj());
            paramDict.Add("@StartRow", (dtModel.Start).ToDbObj());
            paramDict.Add("@EndRow", (dtModel.End).ToDbObj());
            paramDict.Add("@OrderByColumnName", (dtModel.ListOrder[0].columnName).ToDbObj());
            paramDict.Add("@OrderByDirection", (dtModel.ListOrder[0].sortingDirectionStr).ToDbObj());
            paramDict.Add("@WhereOfMultiSearch", dtModel.WhereOfMultiSearch.ToDbObj());

            //DataTable dt = DBHelper.GetDataTableByStoredProcedure("[PITB].[Get_All_Tagging_List]", paramDict);
            return DBHelper.GetDataTableByStoredProcedure("[PITB].[Get_All_Messaging_List_Temp]", paramDict).ToList<VmStakeholderMessageListing>();


            //return listOfTagging;
        }

        
    }
}