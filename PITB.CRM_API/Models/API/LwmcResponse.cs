using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PITB.CMS.Models.DB;
using PITB.CRM_API.Models.Custom;

namespace PITB.CRM_API.Models.API
{
    public class LwmcResponse
    {
        public class ComplaintantMessages : ApiStatus
        {
            public List<MessageModel> ListMessages { get; set; }

            public class MessageModel
            {
                public int OrderId { get; set; }
                public string MessageStr { get; set; }
            } 
        }
    }
}