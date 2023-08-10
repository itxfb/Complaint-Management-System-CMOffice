using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PITB.CMS_Handlers.Custom
{
    public class ListingParamsModelBase
    {
        public int? StartRow { get; set; }

        public int? EndRow { get; set; } 
        public string OrderByColumnName { get; set; }

        public string OrderByDirection { get; set; }

        public string WhereOfMultiSearch { get; set; }

        public string From { get; set; }
        public string To { get; set; }
	    public string Campaign { get; set; }
	    public string Category { get; set; }
	    public string Status { get; set; }
	    public string TransferedStatus { get; set; }
	    public int? ComplaintType { get; set; }
	    public int? UserHierarchyId { get; set; }
	    public int? UserDesignationHierarchyId { get; set; }
	    public int? ListingType { get; set; }
	    public string ProvinceId { get; set; }
	    public string DivisionId { get; set; }
	    public string DistrictId { get; set; }
	    public string Tehsil { get; set; }
	    public string UcId { get; set; }
	    public string WardId { get; set; }

	    public int? UserId { get; set; }

        public int? UserCategoryId1 { get; set; }
        public int? UserCategoryId2 { get; set; }

        public List<UserCategoryModel> ListUserCategory { get; set; }

        public int? CheckIfExistInSrcId { get; set; }
        public int? CheckIfExistInUserSrcId { get; set; }
        public string SelectionFields { get; set; }

        public string InnerJoinLogic { get; set; }

        public string WhereLogic { get; set; }

        public string GroupByLogic { get; set; }

        public string SpType { get; set; }

        public string DynamicFieldsControlId { get; set; }

        public bool IgnoreComputedHierarchyCheck { get; set; }

        public int regionType { get; set; }

        public int regionTypeId { get; set; }

        //public string RefField1 { get; set; }
    }
}