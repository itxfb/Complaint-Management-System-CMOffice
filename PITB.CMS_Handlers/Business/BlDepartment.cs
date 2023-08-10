using PITB.CMS_Common.Handler.Authentication;
using PITB.CMS_Common.Models.Custom;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PITB.CMS_Common.Helper;
using PITB.CMS_Common.Models;

namespace PITB.CMS_Handlers.Business
{
    public class BlDepartment
    {
        public static object GetDepartmentsOfCampaignOfUser(string[] campaignIds)
        {
            List<DbDepartment> listDepartmentType = DbDepartment.GetByCampaignIds(campaignIds.ToIntList()).Where(x => !string.IsNullOrWhiteSpace(x.Name)).ToList();

            var valueTextPair = from sb in listDepartmentType
                                select new { Value = sb.Id, Text = sb.Name };
            return valueTextPair;
        }
        public static List<SelectListItem> GetDepartmentsOfCampaignOfUser(List<SelectListItem> campaignList)
        {
            List<string> campaingIdsList = new List<string>();
            foreach (SelectListItem item in campaignList)
            {
                campaingIdsList.Add(item.Value);
            }
            int? groupId = DbCategoryGroupMapping.GetModelByCampaignIdAndTypeId(campaingIdsList.ToIntList()[0], Config.ComplaintType.Complaint);
            List<DbDepartment> listDepartmentType = DbDepartment.GetByCampaignIds(campaingIdsList.ToIntList()).Where(x=>  !string.IsNullOrWhiteSpace(x.Name)).ToList();

            var valueTextPair = from sb in listDepartmentType
                                select new { Value = sb.Id, Text = sb.Name };

            return listDepartmentType.Select(c => new SelectListItem() { Text = c.Name, Value = c.Id.ToString() }).ToList();
        }

    }
}