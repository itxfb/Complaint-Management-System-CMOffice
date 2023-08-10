using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PITB.CMS_Common.Models.View.Table
{
    public class VmTableStatusHistory
    {
        //public string ComplaintId { get; set; }
        public string Id { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string UserHierarchy { get; set; }

        public string UserHierarchyValue { get; set; }
        public string Status { get; set; }
        public string StatusChangeDateTime { get; set; }
        public string Comments { get; set; }
        public string IsCurrentlyActive { get; set; }

    }
}