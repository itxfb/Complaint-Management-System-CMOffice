using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;


namespace PITB.CMS_Handlers.View.Agent
{
    public class VmComplaintZimmedarShehriDetailAgent : VmStakeholderComplaintDetail
    {
        public bool CanShowFollowUpView { get; set; }
        [Required]
        public string FollowupComments { get; set; }
        public VmComplaintZimmedarShehriDetailAgent()
        {
            //this = vmStakeholderComplaintDetail;
            
        }

        //public static VmComplaintZimmedarShehriDetailAgent GetComplaintDetail(DbComplaint dbComplaint, List<DbDynamicComplaintFields> ListDynamicComplaintFields, VmStakeholderComplaintDetail.DetailType detailType)
        //{
        //    GetComplaintDetail(dbComplaint, ListDynamicComplaintFields, detailType);
        //}
       
    }
}