using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PITB.CMS_Models.Custom
{
    public class SmsModel
    {
        public int CampaignId { get; set; }
        public string MobileNumber { get; set; }
        public string MessageText { get; set; }

        public int MessageType { get; set; }
        public int MessageSentBySrcId { get; set; }

        public DateTime SentDateTime { get; set; }

        public int SentStatus { get; set; }

        public int MessageTagId { get; set; }


        public SmsModel()
        {
            
        }
        public SmsModel(int campId, string mobileNumber, string msgTxt, int msgType, int msgSentBySrcId, DateTime sendDateTime, int sentStatus, int messageTagId)
        {
            this.CampaignId = campId;
            this.MobileNumber = mobileNumber;
            this.MessageText = msgTxt;
            this.MessageType = msgType;
            this.MessageSentBySrcId = msgSentBySrcId;
            this.SentDateTime = sendDateTime;
            this.SentStatus = sentStatus;
            this.MessageTagId = messageTagId;
        }

    
    
    }

//     @CampaignId INT,
//@MessageText NVARCHAR(250),
//@MessageType INT,
//@SentBySrcId INT,
//@MobileNumber NVARCHAR(20),
//@SentDateTime DATETIME,
//@SentStatus int
}