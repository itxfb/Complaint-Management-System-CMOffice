using PITB.CMS.Handler.Authentication;
using PITB.CMS.Models.DB;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using PITB.CMS.Helper.Database;

namespace PITB.CMS.Handler.Business
{
    public  class BlStakeholderUserParam
    {
        public int Campaign { get; set; }
        public string Category
        {
            get{
                var CategoryList = DbComplaintType.GetByCampaignId(Campaign);
                string catList = "";
                for (int i = 0; i < CategoryList.Count; i++ )
                {
                    catList = catList + "," + CategoryList[i].Complaint_Category.ToString();
                }
                catList = catList.Substring(1);
                return catList;
            }
        }
        public string Status
        {
            get{
                var DbStatusList = DbStatus.GetByCampaignId(Campaign);
                string StatusList = "";
                for (int i = 0; i < DbStatusList.Count; i++ )
                {
                    StatusList = StatusList + "," + DbStatusList[i].Complaint_Status_Id.ToString();
                }
                StatusList = StatusList.Substring(1);
                return StatusList;
            }
        }

        public string TransferedStatus
        {
            get{
                return "0,1";
            }
        }
        public int ComplaintType
        {
            get
            {
                return Convert.ToInt32(Config.ComplaintType.Complaint);
            }
        }
        public int UserHierarchyId
        {
            get
            {
                return Convert.ToInt32(AuthenticationHandler.GetCookie().Hierarchy_Id);
            }
        }

        public int ListingType
        {
            get
            {
                return Convert.ToInt32(Config.StakeholderComplaintListingType.AssignedToMe);
            }
        }

        
        public int UserDesignationHierarchyId
        {
            get
            {
                return Convert.ToInt32(AuthenticationHandler.GetCookie().User_Hierarchy_Id);
            }
        }

        public int ProvinceId
        {
            get
            {
                return Convert.ToInt32(AuthenticationHandler.GetCookie().ProvinceId);
            }
        }
        public int DivisionId
        {
            get
            {
                return Convert.ToInt32(AuthenticationHandler.GetCookie().DivisionId);
            }
        }
        public int DistrictId
        {
            get
            {
                return Convert.ToInt32(AuthenticationHandler.GetCookie().DistrictId);
            }
        }
        public int Tehsil
        {
            get
            {
                return Convert.ToInt32(AuthenticationHandler.GetCookie().TehsilId);
            }
        }
        public int UcId
        {
            get
            {
                return Convert.ToInt32(AuthenticationHandler.GetCookie().UcId);
            }
        }
        public int WardId
        {
            get
            {
                return Convert.ToInt32(AuthenticationHandler.GetCookie().WardId);
            }
        }
        public int UserId
        {
            get
            {
                return Convert.ToInt32(AuthenticationHandler.GetCookie().UserId);
            }
        }
        public string SpType
        {
            get
            {
                return "GraphList";
            }
        }
        public string fromDate{get; set;}
        public string toDate { get; set; }

        public  DataTable GetComplaintsOfStakeholderServerSide()
        {
            Dictionary<string, object> paramDict = new Dictionary<string, object>();
           
            paramDict.Add("@From", fromDate.ToDbObj());
            paramDict.Add("@To", toDate.ToDbObj());
            paramDict.Add("@Campaign", Campaign.ToDbObj());
            paramDict.Add("@Category", Category.ToDbObj());
            paramDict.Add("@Status", Status.ToDbObj());
            paramDict.Add("@TransferedStatus", TransferedStatus.ToDbObj());
            paramDict.Add("@ComplaintType", ComplaintType.ToDbObj());
            paramDict.Add("@UserHierarchyId", UserHierarchyId.ToDbObj());
            paramDict.Add("@UserDesignationHierarchyId", UserDesignationHierarchyId.ToDbObj());
            paramDict.Add("@ListingType", ListingType.ToDbObj());
            paramDict.Add("@ProvinceId", ProvinceId.ToDbObj());
            paramDict.Add("@DivisionId", DivisionId.ToDbObj());
            paramDict.Add("@DistrictId", DistrictId.ToDbObj());

            paramDict.Add("@Tehsil", Tehsil.ToDbObj());
            paramDict.Add("@UcId", UcId.ToDbObj());
            paramDict.Add("@WardId", WardId.ToDbObj());

            paramDict.Add("@UserId", UserId.ToDbObj());

            paramDict.Add("@SpType", SpType);

            return DBHelper.GetDataTableByStoredProcedure("[PITB].[Get_Stakeholder_Complaints_ServerSide_Graphs]", paramDict);
        }


        //Status

        //@From VARCHAR(10)= '',
        //@To VARCHAR(10)= '',
        //@Campaign VARCHAR(MAX)= '',
        //@Category VARCHAR(Max)= '',
        //@Status VARCHAR(50)= '',
        //@TransferedStatus VARCHAR(50)= '',
        //@ComplaintType INT=0,
        //@UserHierarchyId INT=0,
        //@UserDesignationHierarchyId INT=0,
        //@ListingType INT=0,
        //@ProvinceId VARCHAR(MAX)='',
        //@DivisionId VARCHAR(MAX)='',
        //@DistrictId VARCHAR(MAX)='',
        //@Tehsil VARCHAR(MAX)='',
        //@UcId VARCHAR(MAX)='',
        //@WardId VARCHAR(MAX)='',

        //@UserId INT=0,--Agent Id
        //@SpType VARCHAR(100)=''
    }
}