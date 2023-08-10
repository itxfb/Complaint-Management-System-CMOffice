using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PITB.CMS_Models.Custom
{
    public class ListingParamsSchoolEducation : ListingParamsModelBase
    {
        public int? UserCategoryId1 { get; set; }
        public int? UserCategoryId2 { get; set; }
        public int? CheckIfExistInSrcId { get; set; }
        public int? CheckIfExistInUserSrcId { get; set; }
        public string SelectionFields { get; set; }
    }
}