using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using PITB.CMS.Handler.Authentication;
using PITB.CMS.Helper.Database;
using PITB.CMS.Models.Custom;
using PITB.CMS.Helper.Database;

namespace PITB.CMS.Handler.Business
{
    public class BlDashboard
    {
        public enum Flag
        {
            CategoryAndSubCategory = 0,
            //SubCategory = 1,
            CategoryWiseResolvedPending = 1,
            CategoryWiseLineChart = 2
        }

        public static DataSet GetChartData(string startDate, string endDate, Flag flag, string campaignId)
        {
            CMSCookie cookie = new AuthenticationHandler().CmsCookie;
            Dictionary<string, object> parameter = new Dictionary<string, object>();
            DataSet ds = new DataSet();

            parameter.Add("@From", startDate.ToDbObj());
            parameter.Add("@To", endDate.ToDbObj());
            parameter.Add("@Campaign", campaignId/*cookie.Campaigns.ToDbObj()*/);
            //parameter.Add("@Category", cookie.CategoryIdCommaSep.ToDbObj());
            parameter.Add("@Category", BlComplaintType.GetCommaSepCatAgainstCampaign(campaignId).ToDbObj());
            parameter.Add("@Status", BlCommon.GetStatusStrCommaSepByCampaignId(campaignId).ToDbObj());
            parameter.Add("@UserHierarchyId", cookie.Hierarchy_Id.ToDbObj());
            parameter.Add("@ProvinceId", cookie.ProvinceId.ToDbObj());
            parameter.Add("@DivisionId", cookie.DivisionId.ToDbObj());
            parameter.Add("@DistrictId", cookie.DistrictId.ToDbObj());
            parameter.Add("@Tehsil", cookie.TehsilId.ToDbObj());
            parameter.Add("@UcId", cookie.UcId.ToDbObj());
            parameter.Add("@WardId", cookie.WardId.ToDbObj());
            parameter.Add("@flag", Convert.ToInt32(flag).ToDbObj());


            ds = DBHelper.GetDataSetByStoredProcedure("[pitb].[Get_Stakeholder_Complaints_Dashboard]", parameter);
            return ds;

        }
        
    }
}