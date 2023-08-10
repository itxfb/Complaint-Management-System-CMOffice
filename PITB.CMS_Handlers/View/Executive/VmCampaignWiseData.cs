using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PITB.CMS_Common.Handler.Complaint;

namespace PITB.CMS_Handlers.View.Executive
{
    /* public class VmCampaignWiseData
     {
         public int FormId { get; set; }
         public int? CampaignId { get; set; }
         public string CampaignName { get; set; }
         public string CampaignLogoSrc { get; set; }
         public Dictionary<int, string> StatusListName { get; set; }
         public Dictionary<int, int> StatusListCount { get; set; }

         public VmCampaignWiseData()
         {

         }

     }*/

    public class VmCampaignWiseData
    {
        public int FormId { get; set; }
        public int? CampaignId { get; set; }
        public string CampaignName { get; set; }
        public string CampaignLogoSrc { get; set; }
        public List<StatusCount> ListStatusWiseCount { get; set; }
        public List<FeedbackCategoryWiseCount> ListfeedbackCategoryWiseCounts { get; set; }
        public int FeedbackTotalCount { get; set; }

        public dynamic CallsLandedCount { get; set; }

        public VmCampaignWiseData()
        {
            ListStatusWiseCount = new List<StatusCount>();
        }

        public VmCampaignWiseData(List<DbStatus> listDbStatus)
        {
            ListStatusWiseCount = new List<StatusCount>();

            foreach (var dbStatus in listDbStatus)
            {

                var status = new StatusCount();
                status.StatusId = dbStatus.Complaint_Status_Id;
                if (dbStatus.Complaint_Status_Id == 18)
                {
                    status.StatusName = "Overdue";
                }
                else
                {
                    status.StatusName = dbStatus.Status;
                }
                status.Count = 0;
                ListStatusWiseCount.Add(status);
            }
        }

        public class StatusCount
        {
            public int StatusId { get; set; }

            public string StatusName { get; set; }

            public int Count { get; set; }

            public float Percentage { get; set; }

            public string CSSClassName { get; set; }
        }
    }

}