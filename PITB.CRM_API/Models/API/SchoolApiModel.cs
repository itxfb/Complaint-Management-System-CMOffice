using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PITB.CRM_API.Models.Custom;

namespace PITB.CRM_API.Models.API
{
    public class SchoolApiModel : ApiStatus
    {
        public List<SchoolModel> ListSchoolModel { get; set; } 
    }

    public class SchoolModel
    {
        public string EmisCode { get; set; }
    }
}