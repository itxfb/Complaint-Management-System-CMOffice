using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PITB.CRM_API.Models.Custom;
using PITB.CRM_API.Models.DB;

namespace PITB.CRM_API.Models.API
{
    public class GetComplaintStatuses : ApiStatus
    {
        public List<DbStatuses> listDbStatuses { get; set; }
    }
}