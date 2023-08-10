using System.Linq;
using PITB.CRM_API.Helper.Database;
using PITB.CRM_API.Models.Custom;

namespace PITB.CRM_API.Models.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("PITB.Campaign")]
    public class DbCampaign
    {
        [Key]
        public int Id { get; set; }

        [StringLength(100)]
        public string Campaign_Name { get; set; }

        public int? Province_Id { get; set; }

        [StringLength(10)]
        public string Campaign_HelpLine { get; set; }

        public int? District_Id { get; set; }

        public int? Campaign_Type { get; set; }
        public string LogoUrl { get; set; }

        public string LayoutImageUrl { get; set; }

        public string UrlSuffix { get; set; }

        public bool? IsCustomUrlAllowed { get; set; }

        public string StakeholderLogoUrl { get; set; }

        public string LayoutPopupImageBg { get; set; }

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
