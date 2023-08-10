using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PITB.CRM_API.Models.DB;

namespace PITB.CRM_API.Models.API
{

    public class StatusHistory
    {
        public List<StatusHistoryModel> ListStatusHistoryModel { get; set; }

        public StatusHistory()
        {
            this.ListStatusHistoryModel = new List<StatusHistoryModel>();
        }
    }

    public class StatusHistoryModel
    {
        public int Id { get; set; }

        public int StatusChangedByPersonId { get; set; }

        public string StatusChangedByPersonName { get; set; }

        public string StatusChangedByPersonDesignationName { get; set; }

        public string StatusChangedByPersonDesignationAbbr { get; set; }

        public int? StatusId { get; set; }

        public string StatusName { get; set; }

        public string StatusComment { get; set; }

        public DateTime StatusChangeDateTime { get; set; }

        public int OrderId { get; set; }

        public List<DbAttachments> ListAttachment { get; set; }
    }
}