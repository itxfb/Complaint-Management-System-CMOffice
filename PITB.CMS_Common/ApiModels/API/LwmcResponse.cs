using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PITB.CMS_Common.ApiModels.Custom;


namespace PITB.CMS_Common.ApiModels.API
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