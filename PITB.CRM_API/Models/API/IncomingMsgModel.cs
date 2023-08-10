using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PITB.CRM_API.Models.API
{
    public class IncomingMsgModel : BaseMobileApiModel
    {
        public string Caller_No { get; set; }

        public string Msg_Text { get; set; }

        public int Campaign_Id { get; set; }

        public int Src_Id { get; set; }

        public byte Status { get; set; }

        public DateTime Message_Created_DateTime { get; set; }

    }
 
}