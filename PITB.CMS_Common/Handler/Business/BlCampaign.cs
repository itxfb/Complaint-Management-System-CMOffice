using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using PITB.CMS_Common.Handler.Authentication;
using PITB.CMS_Common.Models;
using PITB.CMS_Common.Models.View;

namespace PITB.CMS_Common.Handler.Business
{
    public class BlCampaign
    {
        public static List<SelectListItem> GetCampaignSelectListItemsFromCookie()
        {
            var listOfCampaigns = DbCampaign.GetByUserId(new AuthenticationHandler().CmsCookie.UserId);
            return listOfCampaigns.Select(c => new SelectListItem() { Text = c.Campaign_Name, Value = c.Id.ToString() }).ToList();
        }

        public static List<VmCampaign> GetCampaignsByDepartmentId(int DepartmentId)
        {
            Mapper.CreateMap<DbCampaign, VmCampaign>();
            return Mapper.Map<List<VmCampaign>>(DbCampaign.GetCampaignsByDepartmentId(DepartmentId));
        }

        public static List<SelectListItem> GetCampaignSelectListItemsFromDbUser(DbUsers dbUser)
        {
            var listOfCampaigns = DbCampaign.GetByUserId(dbUser.User_Id);
            return listOfCampaigns.Select(c => new SelectListItem() { Text = c.Campaign_Name, Value = c.Id.ToString() }).ToList();
        }

        public static VmLogin GetLoginVmAgainstCampaignUrlSuffix(string urlSuffix)
        {
            VmLogin vmLogin = new VmLogin();
            DbCampaign dbCampaign = DbCampaign.GetByUrlSuffix(urlSuffix);
            if (dbCampaign != null && Convert.ToBoolean(dbCampaign.IsCustomUrlAllowed))
            {
                vmLogin.LayoutImageUrl = dbCampaign.LayoutImageUrl;
            }
            if (dbCampaign != null && dbCampaign.LayoutPopupImageBg != null)
            {
                vmLogin.LayoutPopupImageUrl = dbCampaign.LayoutPopupImageBg;
            }
            return vmLogin;
        }

        public static string GetCampaignUrlSuffixAgainstCampaignId(string campaignId)
        {
            //DbCampaign dbCampaign = DbCampaign.GetById(Convert.ToInt32(campaignId));
            DbCampaign dbCampaign = DbCampaign.GetById(Utility.GetIntByCommaSepStr(campaignId));
            if (dbCampaign != null && Convert.ToBoolean(dbCampaign.IsCustomUrlAllowed))
            {
                return dbCampaign.UrlSuffix;
            }
            return null;
        }
    }
}