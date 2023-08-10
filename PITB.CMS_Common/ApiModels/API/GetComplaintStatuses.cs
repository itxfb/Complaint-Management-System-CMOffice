using PITB.CMS_Common.ApiModels.Custom;
using PITB.CMS_Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace PITB.CMS_Common.ApiModels.API
{
    public class GetComplaintStatuses : ApiStatus
    {
        public List<DbStatus> listDbStatuses { get; set; }
    }
}