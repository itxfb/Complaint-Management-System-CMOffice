using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PITB.CRM_API.Models.API
{
    public class GetStakeholderComplaintPostModel
    {
        public string UserName { get; set; }

        public string Statuses { get; set; }

        public int StartRowIndex { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }

        public string fcm_key { get; set; }
    }
}