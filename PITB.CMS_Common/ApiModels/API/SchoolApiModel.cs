using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PITB.CMS_Common.ApiModels.Custom;

namespace PITB.CMS_Common.ApiModels.API
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