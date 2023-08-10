using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PITB.CMS.Models.Custom
{
    public class UsersHierarchyMapping
    {
        public string ProvinceId { get; set; }

        public string DivisionId { get; set; }

        public string DistrictId { get; set; }

        public string TehsilId { get; set; }

        public string UcId { get; set; }

        public string WardId { get; set; }

        public int UserId { get; set; }
        public Config.Hierarchy? Hierarchy { get; set; }
        public int? UsersHierarchy { get; set; }

    }
}