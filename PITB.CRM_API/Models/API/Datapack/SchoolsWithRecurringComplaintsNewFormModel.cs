using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PITB.CRM_API.Models.API.Datapack
{
    public class SchoolsWithRecurringComplaintsNewFormModel
    {
        public string DistrictName { get; set; }
        public string DistrictId { get; set; }
        public List<SchoolIdentification> schools { get; set; }
    }
    public class SchoolIdentification{
        public string DistrictName { get; set; }
        public string DistrictId { get; set; }
        public string SchoolEmisCode{get;set;}
        public string SchoolName { get; set; }
        public string MarkazName { get; set; }
        public string MarkazId { get; set; }
        public string MaxTypeName { get; set; }
        public string TypeId { get; set; }
        public string MaxSubTypeName { get; set; }
        public string SubTypeId { get; set; }
        public string CountOfTotalComplaints{get;set;}
        public string CountOfMaxSubTypeNameComplaint{get;set;}

    }
}