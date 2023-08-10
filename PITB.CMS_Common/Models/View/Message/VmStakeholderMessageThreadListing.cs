using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace PITB.CMS_Common.Models.View.Message
{
    public class VmStakeholderMessageThreadListing
    {
        public int Id { get; set; }
        public string Caller_No { get; set; }
        public string Msg_Text { get; set; }
        public string Created_DateTime { get; set; }
        public string Is_Incoming_Message { get; set; }

        public VmStakeholderMessageThreadListing()
        {
            
        }

        public VmStakeholderMessageThreadListing(DbIncomingMessages dbIncomingMessages, bool isIncomingMsg)
        {
            this.Id = dbIncomingMessages.Id;
            this.Caller_No = dbIncomingMessages.Caller_No.ToString();
            this.Msg_Text = dbIncomingMessages.Msg_Text.ToString();
            this.Created_DateTime = dbIncomingMessages.Message_Created_DateTime.ToString();
            this.Is_Incoming_Message = isIncomingMsg.ToString();
        }

        public static List<VmStakeholderMessageThreadListing> GetVmFromDbIncomingMsgs(List<DbIncomingMessages> listDbIncomingMessages)
        {
            List<VmStakeholderMessageThreadListing> listVmStakeholderMsg = new List<VmStakeholderMessageThreadListing>();

            foreach (DbIncomingMessages dbIncomingMessages in listDbIncomingMessages)
            {
                listVmStakeholderMsg.Add(new VmStakeholderMessageThreadListing(dbIncomingMessages,true));
            }

            return listVmStakeholderMsg;
        }

    }
}