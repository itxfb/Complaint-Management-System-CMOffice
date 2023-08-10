using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PITB.CMS.Models.View.Message
{
    public class VmStakeholderMessageListing
    {
        public string Id { get; set; }
        public string Caller_No { get; set; }

        public string Msg_Text { get; set; }
        public string Message_Created_DateTime { get; set; }

        public string ReplyMessages { get; set; }

        public int Total_Rows { get; set; }
    }
}