using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PITB.CMS.Models.API
{
    public class PostSmsModel
    {
        public string sec_key { get; set; }

        public string sms_text { get; set; }

        public string phone_no { get; set; }

        public string sms_language { get; set; }
    }
}