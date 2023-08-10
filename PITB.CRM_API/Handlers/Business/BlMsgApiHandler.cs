using PITB.CRM_API.Helper.Database;
using PITB.CRM_API.Models.API;
using PITB.CRM_API.Models.Custom;
using PITB.CRM_API.Models.DB;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace PITB.CRM_API.Handlers.Business
{
    public class BlMsgApiHandler
    {
        public static ApiStatus SubmitIncomingMsgs(IncomingMsgGroupModel submitIncomingMsgs, Int64 apiRequestId)
        {
            try
            {
                DateTime currDateTime = DateTime.Now;
                foreach (IncomingMsgModel incomingModel in submitIncomingMsgs.listIncomingMsgModel)
                {
                    DBContextHelperLinq db = new DBContextHelperLinq();
                    DbIncomingMessages dbIncomingMessage = new DbIncomingMessages();
                    db.DbIncomingMessages.Add(dbIncomingMessage);

                    dbIncomingMessage.Caller_No = incomingModel.Caller_No;
                    dbIncomingMessage.Campaign_Id = incomingModel.Campaign_Id;
                    dbIncomingMessage.Msg_Text = incomingModel.Msg_Text;
                    dbIncomingMessage.Message_Created_DateTime = incomingModel.Message_Created_DateTime;
                    dbIncomingMessage.Created_DateTime = currDateTime;
                    dbIncomingMessage.Src_Id = (int) Config.RequestType.Mobile;
                    dbIncomingMessage.Status = (byte) Config.MessageStatus.New;
                    db.SaveChanges();
                    SaveMobileRequest(incomingModel, dbIncomingMessage.Id, apiRequestId, currDateTime);
                }
                return new ApiStatus(Config.ResponseType.Success.ToString(), "Your messages has been posted");
            }
            catch (Exception exception)
            {
                return new ApiStatus(Config.ResponseType.Failure.ToString(), exception.Message);
            }

        }


        
        public static ApiStatus SubmitSentMessageStatus(int groupId)
        {
            try
            {
                DBContextHelperLinq db = new DBContextHelperLinq();
                List<DbReplyMessages> listDbReplyMessages = DbReplyMessages.GetMessageWithGroupAndCampaign(db, groupId);
                foreach (DbReplyMessages dbReplyMessages in listDbReplyMessages)
                {
                    db.DbReplyMessages.Attach(dbReplyMessages);
                    
                    dbReplyMessages.Status = (int) Config.MessageStatus.Sent;
                    db.Entry(dbReplyMessages).Property(n => n.Status).IsModified = true;
                    
                    dbReplyMessages.Group_Id = -1;
                    db.Entry(dbReplyMessages).Property(n => n.Group_Id).IsModified = true;
                }
                if (listDbReplyMessages != null && listDbReplyMessages.Count > 0)
                {
                    db.SaveChanges();
                }
                return new ApiStatus(Config.ResponseType.Success.ToString(), "Message group id and status has been updated");
            }
            catch (Exception exception)
            {
                return new ApiStatus(Config.ResponseType.Failure.ToString(), exception.Message);
            }

        }
        

        public static ReplyGroupModel GetReplyMessages(int campaignId)
        {
            try
            {
                int maxGroupId = DbLinearIncrement.GetMaxCount((int) Config.IncrementalType.GetReplyMsg);
                int replyMsgCount = Convert.ToInt32(ConfigurationManager.AppSettings["ReplyMsgThreshold"]);
                int msgStatus = (int) Config.MessageStatus.New;

                DBContextHelperLinq db = new DBContextHelperLinq();
                List<DbReplyMessages> listReplyMsgs = DbReplyMessages.GetTopMsgs(db, campaignId, msgStatus, replyMsgCount);
                List<ReplyModel> listReplyModel = new List<ReplyModel>();
                foreach (DbReplyMessages dbReplyMessages in listReplyMsgs)
                {
                    db.DbReplyMessages.Attach(dbReplyMessages);

                    dbReplyMessages.Group_Id = maxGroupId;
                    db.Entry(dbReplyMessages).Property(n => n.Group_Id).IsModified = true;

                    dbReplyMessages.Status = (int) Config.MessageStatus.Sending;
                    db.Entry(dbReplyMessages).Property(n => n.Status).IsModified = true;

                    listReplyModel.Add(new ReplyModel()
                    {
                        CallerNo = dbReplyMessages.Caller_No,
                        CreaterDateTime = (DateTime) dbReplyMessages.Created_DateTime,
                        MsgTxt = dbReplyMessages.Msg_Text,
                        GroupId = (int)dbReplyMessages.Group_Id,
                    });
                }
                if (listReplyMsgs != null && listReplyModel.Count > 0)
                {
                    DbLinearIncrement.IncrementValue(db, (int) Config.IncrementalType.GetReplyMsg, 1);
                    db.SaveChanges();
                }
                return new ReplyGroupModel() {ListGroupModel = listReplyModel};
            }
            catch (Exception exception)
            {
                return null;
            }
        }

        private static void SaveMobileRequest(IncomingMsgModel submitMsg, int messageId, Int64 apiRequestId, DateTime currDateTime)
        {
            DBContextHelperLinq db = new DBContextHelperLinq();
            DbMobileRequest dbMobileRequest = new DbMobileRequest();
            dbMobileRequest.ComplaintId = null;
            dbMobileRequest.Latitude = (string.IsNullOrEmpty(submitMsg.lattitude)) ? (double?)null : double.Parse(submitMsg.lattitude);
            dbMobileRequest.Longitude = (string.IsNullOrEmpty(submitMsg.longitude)) ? (double?)null : double.Parse(submitMsg.longitude);
            dbMobileRequest.RequestType = (int)Config.MobileUserRequestType.MessageSubmit;
            dbMobileRequest.RequestTypeId = messageId;
            dbMobileRequest.Imei = submitMsg.imei_number;
            dbMobileRequest.CreatedDateTime = currDateTime;
            dbMobileRequest.ApiRequestId = apiRequestId;
            db.DbMobileRequest.Add(dbMobileRequest);
            db.SaveChanges();
            //db.db
        }
    }
}