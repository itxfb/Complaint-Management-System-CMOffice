using PITB.CMS_Common.ApiModels.Custom;
using PITB.CMS_Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace PITB.CMS_Common.ApiModels.API
{
    public class StatusList : ApiStatus
    {
        public List<DbStatus> ListStatuses { get; set; }

        public StatusList()
        {
            ListStatuses = new List<DbStatus>();
        }

        public StatusList(List<DbStatus> listStatuses )
        {
            ListStatuses = listStatuses;
        }
    } 
}