using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PITB.CRM_API.Models.Custom;
using PITB.CRM_API.Models.DB;

namespace PITB.CRM_API.Models.API
{
    public class StatusList : ApiStatus
    {
        public List<DbStatuses> ListStatuses { get; set; }

        public StatusList()
        {
            ListStatuses = new List<DbStatuses>();
        }

        public StatusList(List<DbStatuses> listStatuses )
        {
            ListStatuses = listStatuses;
        }
    } 
}