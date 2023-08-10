using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PITB.CRM_API.Models.API.FixitNew.CFNP.API
{
    public class RequestCNFPComplaintsModel
    {
        public string to { get; set; }

        public string from { get; set; }

        public DateTime ToDate { get; set; }

        public DateTime FromDate { get; set; }
    }
}