using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PITB.CRM_API.Models.API.Datapack
{
    public class SchoolsWithRecurringComplaintsModel
    {
        public string DistrictName { get; set; }
        public string DistrictId { get; set; }
        public int CountOfSchoolEmisCode { get; set; }
        public List<clsSchoolEmisCodeName> SchoolEmisCodeNameList { get; set; }
    }
    public class clsSchoolEmisCodeName
    {
        public string SchoolEmisCodeName { set; get; }
        public int CountOfMarkazName { get; set; }
        public List<clsMarkazName> MarkazNameList { get; set; }

    }
    public class clsMarkazName
    {
        public string MarkazName { get; set; }
        public string MarkazId { get; set; }
        public int CountOfSchoolName { get; set; }
        public List<clsSchoolName> SchoolNameList { get; set; }

    }
    public class clsSchoolName
    {
        public string SchoolName { get; set; }
        public int CountOfTypeName { get; set; }
        public List<clsTypeName> TypeNameList { get; set; }
    }
    public class clsTypeName
    {
        public string TypeName { get; set; }
        public string TypeId { get; set; }
        public int CountOfSubTypeName { get; set; }
        public List<clsSubTypeName> SubTypeNameList { get; set; }

    }
    public class clsSubTypeName
    {
        public string SubTypeName { get; set; }
        public string SubTypeId { get; set; }

        public int SubTypeCount { get; set; }

    }
}