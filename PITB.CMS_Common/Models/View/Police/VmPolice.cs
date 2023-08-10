using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PITB.CMS_Common.Models.View.Police
{
    public class VmPolice
    {
        public class VmComplaintAction
        {
            public int ComplaintId { get; set; }

            public string ComplaintIdStr { get; set; }

            //public DbComplaint DbComplaint { get; set; }

            public int CurrentStep { get; set; }

            //public List<DbDynamicComplaintFields> ListDbDynamicComplaintFields { get; set; }

            public DbPoliceAction DbPoliceAction { get; set; }

            public List<DbDynamicComplaintFields> ListDbDynamicComplaintFields { get; set; }

            public List<DbPoliceActionReportLogs> ListDbActionReportLogs { get; set; }



            public List<DbComplaintSubType> ListDbDisposalCategories { get; set; }

            public string DisposalCatId { get; set; }
            public List<SelectListItem> ListSelectDisposalCat { get; set; }

            public bool CanSaveComplaintAction { get; set; }

            public VmComplaintAction()
            {
                ListDbActionReportLogs = new List<DbPoliceActionReportLogs>();
            }
        }
    }
}