using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PITB.CMS_Common.ApiModels.API
{
    public class RequestComplaintStatusLog
    {
        public int ComplaintId { get; set; }
        public string Username { get; set; }
        public int LogId { get; set; }
        public int StartingRowIndex { get; set; }
        public int From { get { return (StartingRowIndex - 1); } }
        public int To { get { return From + Config.PageSizeMobileLWMC - 1 ; } }
    }
}