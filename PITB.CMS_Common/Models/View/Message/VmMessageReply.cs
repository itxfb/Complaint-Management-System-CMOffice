using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PITB.CMS_Common.Models.View.Message
{
    public class VmMessageReply
    {
        public int IncomingMsgId { get; set; }

        public int CampaignId { get; set; }

        public string CallerNo { get; set; }

        public string IncomingMsgStr { get; set; }

        [Required]
        public string ReplyMsgStr { get; set; }
    }
}