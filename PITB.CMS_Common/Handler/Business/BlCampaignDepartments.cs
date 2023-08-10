using PITB.CMS_Common.Helper.Database;
using PITB.CMS_Common.Models;
using PITB.CMS_Common.Models.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PITB.CMS_Common.Handler.Business
{
    public static class BlCampaignDepartments
    {
        public static List<VmCampaignDepartment> GetCampaignsDepartmentByCampaignId(List<int> ids)
        {
            try
            {
                string query = "select * from PITB.Campiagn_Departments where Id IN(" + string.Join(",", ids) + ")";
                return DBHelper.GetDataTableByQueryString(query, null).ToList<VmCampaignDepartment>();
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public static string[] GetTagsbyCampaignId(int campaignId)
        {
            return DbCampaign.AllCampaignTags(campaignId);

        }
        public static void UpdateTagsbyCampaign(string Tags,int CampaignId,int UserId)
        {
            try
            {
                string[] TagsArr = Tags.Split(',');
                List<DbSearchCampaign> listCampaigns = new List<DbSearchCampaign>();
                foreach (var item in TagsArr)
                {
                    listCampaigns.Add(new DbSearchCampaign()
                    {
                        Tag_Name = item,
                        Campaign_Id = CampaignId,
                        Created_By = UserId,
                        Created_At = DateTime.Now,
                        Is_Active = true
                    });
                }
                DbCampaign.UpdateTagsbyCampaign(listCampaigns.ToDataTable<DbSearchCampaign>(), CampaignId);
                
            }
            catch (Exception e)
            {

                throw e;
            }
            

        }
        public static List<VmSearchCampaign> GetCampaignTags(string searchParam,int userId)
        {
            try
            {
                Dictionary<string, object> paramDict = new Dictionary<string, object>();
                paramDict.Add("@searchParam", searchParam);
                paramDict.Add("@userID", userId);
                return DBHelper.GetDataTableByStoredProcedure("[PITB].[GetCampaigns_by_Tag]", paramDict).ToList<VmSearchCampaign>();
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
