using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PITB.CMS_Common.ApiModels.API
{
    public class ComplaintsModel
    {
        public int Id { get; set; }
        public string DepartmentName { get; set; }
        public string TypeName { get; set; }
        public string SubTypeName { get; set; }
        public string DistrictName { get; set; }
        public string TehsilName { get; set; }
        public string MarkazName { get; set; }
        public string SchoolEmisCode { get; set; }
        public string SchoolName { get; set; }
        public string SchoolLevel { get; set; }
        public string SchoolGender { get; set; }
        public string Assignee { get; set; }
        public string OriginalAssignee { get; set; }
        public string CreatedDate { get; set; }
        public string Created_Date { get; set; }
        public int Month { get; set; }
        public string MonthName { get; set; }
        public string StatusChangeDateTime { get; set; }
        public string ComplaintComputedStatus { get; set; }
        public int OverdueDays { get; set; }
    }
}