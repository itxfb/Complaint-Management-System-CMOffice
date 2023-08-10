using System.Linq;
using System;
using System.Collections.Generic;
using PITB.CMS_Common;
using PITB.CMS_Models.DB;
using PITB.CMS_Models.Custom;

namespace PITB.CMS_Handlers.DB.Repository
{

    public class RepoDbCampaign
    {
        #region Helpers
        public static List<DbCampaign> All()
        {
            try
            {
                using (var db = new DBContextHelperLinq())
                {
                    return db.DbCampaigns.AsNoTracking().ToList();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        public static DbCampaign GetById(int campaignId)
        {
            try
            {
                using (var db = new DBContextHelperLinq())
                {
                    return db.DbCampaigns.AsNoTracking().FirstOrDefault(m => m.Id == campaignId);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public static String GetCampaignShortNameById(int campaignId)
        {
            try
            {
                using (var db = new DBContextHelperLinq())
                {
                    return db.DbCampaigns.AsNoTracking().FirstOrDefault(m => m.Id == campaignId).Campaign_ShortName;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public static List<DbCampaign> GetByIds(List<int> listCampaignId)
        {
            try
            {
                using (var db = new DBContextHelperLinq())
                {
                    return db.DbCampaigns.AsNoTracking().Where(m => listCampaignId.Contains(m.Id)).ToList();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        public static DbCampaign GetByProvinceId(int provinceId)
        {
            try
            {
                using (var db = new DBContextHelperLinq())
                {
                    return db.DbCampaigns.AsNoTracking().FirstOrDefault(m => m.Province_Id == provinceId);
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        public static List<DbCampaign> GetByUserId(int userId)
        {
            try
            {
                CMSCookie cookie = new AuthenticationHandler().CmsCookie;
                List<int> listCampaigns = (cookie.Campaigns).Split(',').Select(int.Parse).ToList();
                using (var db = new DBContextHelperLinq())
                {
                    return (from d in db.DbUsers
                            from c in db.DbCampaigns
                            where listCampaigns.Contains(c.Id) && d.User_Id == userId
                            select c).ToList();


                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        public static string GetSingleOrCumulativeCampaignName(string campaignIds)
        {
            string campaignName = null;
            try
            {
                List<int> campIds = Utility.GetIntList(campaignIds);
                if (campIds.Count == 1)
                {
                    campaignName = RepoDbCampaign.GetCampaignShortNameById(campIds[0]);
                }
                else if (campIds.Count > 1)
                {

                    List<DbPermissionsAssignment> lst = RepoDbPermissionsAssignment.GetListOfPermissionsByTypeTypeIdAndPermissionId((int)Config.PermissionsType.General, 1, (int)Config.GeneralPermissions.CumulativeCampaignName);

                    if (lst != null)
                    {
                        Dictionary<string, string> dict = Utility.ConvertCollonFormatToDict(lst[0].Permission_Value);

                        foreach (var value in dict.Values)
                        {
                            int[] valIds = Utility.GetIntList(value).ToArray();
                            bool flag = campIds.All(x => valIds.Contains(x));
                            if (flag)
                            {
                                campaignName = dict.First(x => x.Value == value).Key;
                                break;
                            }
                            else
                            {
                                campaignName = string.Empty;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return campaignName;
        }
        public static string GetLogoUrlForCumulativeCampaignIds(string campaignIds)
        {
            string LogoUrl = null;
            try
            {
                List<int> campIds = Utility.GetIntList(campaignIds);
                for (int i = 0; i < campIds.Count; i++)
                {
                    LogoUrl = GetLogoUrlByCampaignId(campIds[i]);
                    if (LogoUrl != null)
                    {
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return LogoUrl;
        }
        public static List<string> GetCampaignNameByUserId(int userId, List<int> listCampaign)
        {
            try
            {
                using (var db = new DBContextHelperLinq())
                {
                    var firstOrDefault = db.DbUsers.AsNoTracking().FirstOrDefault(m => m.User_Id == userId);
                    if (firstOrDefault != null)
                    {
                        string userCampaignsString = firstOrDefault.Campaigns;
                        List<int> userCampaignsIntList = userCampaignsString.Split(',').Select(int.Parse).ToList();

                        return (from d in db.DbUsers
                                from c in db.DbCampaigns
                                where userCampaignsIntList.Contains(c.Id) && d.User_Id == userId
                                where listCampaign.Contains(c.Id) && d.User_Id == userId
                                select c.Campaign_Name).ToList();
                    }

                }
                return null;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public static DbCampaign GetByUrlSuffix(string urlSuffix)
        {
            try
            {
                using (var db = new DBContextHelperLinq())
                {
                    return db.DbCampaigns.AsNoTracking().FirstOrDefault(m => m.UrlSuffix == urlSuffix);
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public static string GetLogoUrlByCampaignId(int campaignId)
        {
            try
            {
                using (var db = new DBContextHelperLinq())
                {
                    return db.DbCampaigns.AsNoTracking().FirstOrDefault(m => m.Id == campaignId).LogoUrl;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public static DbCampaign GetStakeholderLogoUrlByCampaignId(int campaignId)
        {
            try
            {
                using (var db = new DBContextHelperLinq())
                {
                    return db.DbCampaigns.AsNoTracking().FirstOrDefault(m => m.Id == campaignId);
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        #endregion
    }
}
