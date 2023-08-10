using PITB.CMS_Common.Models.Custom;
using PITB.CMS_DB.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using PITB.CMS_Common;
using System.Text;
using System.Threading.Tasks;

namespace PITB.CMS_DB.Modules.Repository.Db
{
    public class RepDbCampaign : GenericRepository<DbCampaign>
    {
        public RepDbCampaign(CMSDbContext cmsDbContext = null ) : base(cmsDbContext)
        {
          
        }

        public List<DbCampaign> GetAll()
        {
            try
            {
                return Get().ToList();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public String GetCampaignShortNameById(int campaignId)
        {
            return GetById(campaignId).Campaign_ShortName;
        }

        public List<DbCampaign> GetByIds(List<int> listCampaignId)
        {
            return Get(where: m => listCampaignId.Contains(m.Id)).ToList();
        }

        public DbCampaign GetByProvinceId(int provinceId)
        {
            return Get(where: m => m.Province_Id == provinceId).FirstOrDefault();
        }

        public List<DbCampaign> GetByUserId(int userId, List<int> listCampaigns)
        {
            return (from d in dbContext.DbUsers
                    from c in dbContext.DbCampaigns
                    where listCampaigns.Contains(c.Id) && d.User_Id == userId
                    select c).ToList();
        }

        public string GetSingleOrCumulativeCampaignName(string campaignIds)
        {
            string campaignName = null;
            List<int> campIds = Utility.GetIntList(campaignIds);
            if (campIds.Count == 1)
            {
                campaignName = GetCampaignShortNameById(campIds[0]);
            }
            else if (campIds.Count > 1)
            {
                RepDbPermissionsAssignment repDbPermission = new RepDbPermissionsAssignment();
                List<DbPermissionsAssignment> lst = repDbPermission.GetListOfPermissionsByTypeTypeIdAndPermissionId((int)Config.PermissionsType.General, 1, (int)Config.GeneralPermissions.CumulativeCampaignName);

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
            return campaignName;
        }

        public string GetLogoUrlForCumulativeCampaignIds(string campaignIds)
        {
            string LogoUrl = null;
            List<int> campIds = Utility.GetIntList(campaignIds);
            for (int i = 0; i < campIds.Count; i++)
            {
                LogoUrl = GetLogoUrlByCampaignId(campIds[i]);
                if (LogoUrl != null)
                {
                    break;
                }
            }
            return LogoUrl;
        }

        public string GetLogoUrlByCampaignId(int campaignId)
        {
            return GetSingle(where: m => m.Id == campaignId).LogoUrl;
        }

        public List<string> GetCampaignNameByUserId(int userId, List<int> listCampaign)
        {
            var firstOrDefault = dbContext.DbUsers.AsNoTracking().FirstOrDefault(m => m.User_Id == userId);
            if (firstOrDefault != null)
            {
                string userCampaignsString = firstOrDefault.Campaigns;
                List<int> userCampaignsIntList = userCampaignsString.Split(',').Select(int.Parse).ToList();

                return (from d in dbContext.DbUsers
                        from c in dbContext.DbCampaigns
                        where userCampaignsIntList.Contains(c.Id) && d.User_Id == userId
                        where listCampaign.Contains(c.Id) && d.User_Id == userId
                        select c.Campaign_Name).ToList();
            }
            return null;
        }
        public DbCampaign GetByUrlSuffix(string urlSuffix)
        {
            return GetSingle(where: m => m.UrlSuffix == urlSuffix);
        }


        //protected void Dispose(bool isDisposing)
        //{
        //    if (!isDisposed)
        //    {
        //        if (isDisposing)
        //        {
        //            dbContext.Dispose();
        //        }
        //    }
        //    isDisposed = true;
        //}

        //public void Dispose()
        //{
        //    DisposeMemory(true);
        //    GC.SuppressFinalize(this);
        //}
    }
}
