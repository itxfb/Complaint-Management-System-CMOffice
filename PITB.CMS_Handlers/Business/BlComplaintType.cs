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
    public class BlComplaintType
    {
        public static List<SelectListItem> GetUserCategoriesAgainstCampaign(List<SelectListItem> campaignList, Config.ComplaintType complaintType= Config.ComplaintType.Complaint)
        {
            List<string> campaingIdsList = new List<string>();
            foreach(SelectListItem item in campaignList)
            {
                campaingIdsList.Add(item.Value);
            }
            int? groupId = DbCategoryGroupMapping.GetModelByCampaignIdAndTypeId(campaingIdsList.ToIntList()[0], (Config.ComplaintType)complaintType);
            List<DbComplaintType> listComplaintType = DbComplaintType.GetByCampaignIdsAndGroupId(campaingIdsList.ToIntList(), groupId);
            listComplaintType = GetCookieComplaintType(listComplaintType);
            
            //cookie.CategoryIds.RemoveAll(n=>)

            var valueTextPair = from sb in listComplaintType
                                select new { Value = sb.Complaint_Category, Text = sb.Name };

            return listComplaintType.Select(c => new SelectListItem() { Text = c.Name, Value = c.Complaint_Category.ToString() }).ToList();
        }
        public static List<SelectListItem> GetUserCategoriesAgainstCampaignWithDepartment(List<SelectListItem> campaignList, List<SelectListItem> departmentList)
        {
            List<string> campaignIds = new List<string>();
            foreach (SelectListItem item in campaignList)
            {
                campaignIds.Add(item.Value);
            }
            List<string> departmentIds = new List<string>();
            foreach (SelectListItem item in departmentList)
            {
                departmentIds.Add(item.Value);
            }
            List<DbComplaintType> listComplaintType = DbComplaintType.GetByCampaignIdsAndDepartmentIds(campaignIds.ToIntList(), departmentIds.ToIntList());
            listComplaintType = GetCookieComplaintType(listComplaintType);

            List<DbDepartment> listDepartmentType = DbDepartment.GetByDepartmentIds(departmentIds.ToIntList());
            foreach (DbDepartment department in listDepartmentType)
            {
                var complaintsType = listComplaintType.Where(x => x.DepartmentId == department.Id);
                foreach (var type in complaintsType)
                {
                    type.Name = string.Format("{0} : {1}", department.Name, type.Name);
                }
            }
            var valueTextPair = from sb in listComplaintType
                                select new { Value = sb.Complaint_Category, Text = sb.Name };
            return listComplaintType.Select(c => new SelectListItem() { Text = c.Name, Value = c.Complaint_Category.ToString() }).ToList();
        }
        public static List<SelectListItem> GetUserCategoriesAgainstCampaign(List<SelectListItem> campaignList, DbUsers dbUser)
        {
            List<string> campaingIdsList = new List<string>();
            foreach (SelectListItem item in campaignList)
            {
                campaingIdsList.Add(item.Value);
            }
            List<DbComplaintType> listComplaintType = DbComplaintType.GetByCampaignIds(campaingIdsList.ToIntList());
            listComplaintType = GetDbUserComplaintType(listComplaintType, dbUser);

            //cookie.CategoryIds.RemoveAll(n=>)

            var valueTextPair = from sb in listComplaintType
                                select new { Value = sb.Complaint_Category, Text = sb.Name };

            return listComplaintType.Select(c => new SelectListItem() { Text = c.Name, Value = c.Complaint_Category.ToString() }).ToList();
        }

        public static object GetUserCategoriesAgainstCampaign(string[] campaignIds)
        {
            //var jsonToReturn = from sb in DbComplaintType.GetByCampaignIds(campaignIds.ToIntList()).OrderBy(m => m.Name)
            //                   select new { Value = sb.Complaint_Category, Text = sb.Name };

            List<DbComplaintType> listComplaintType = DbComplaintType.GetByCampaignIds(campaignIds.ToIntList());
            listComplaintType = GetCookieComplaintType(listComplaintType);

            var valueTextPair = from sb in listComplaintType
                                select new { Value = sb.Complaint_Category, Text = sb.Name };
            return valueTextPair;
        }
        public static object GetUserCategoriesAgainstCampaignWithDepartment(string[] campaignIds,string[] departmentIds)
        {
            List<DbComplaintType> listComplaintType = DbComplaintType.GetByCampaignIdsAndDepartmentIds(campaignIds.ToIntList(),departmentIds.ToIntList());
            listComplaintType = GetCookieComplaintType(listComplaintType);

            List<DbDepartment> listDepartmentType = DbDepartment.GetByDepartmentIds(departmentIds.ToIntList());
            foreach (DbDepartment department in listDepartmentType)
            {
                var complaintsType = listComplaintType.Where(x => x.DepartmentId == department.Id);
                foreach (var type in complaintsType)
                {
                    type.Name = string.Format("{0} : {1}", department.Name, type.Name);
                }
            }
            var valueTextPair = from sb in listComplaintType
                                select new { Value = sb.Complaint_Category, Text = sb.Name };                            
            return valueTextPair;
        }
        public static string GetCommaSepCatAgainstCampaign(string campaignIds)
        {
            List<DbComplaintType> listComplaintType = DbComplaintType.GetByCampaignIds(Utility.GetIntList(campaignIds));
            listComplaintType = GetCookieComplaintType(listComplaintType);
            return Utility.GetCommaSepStrFromList(listComplaintType.Select(n=>n.Complaint_Category).ToList());
        }
        public static string GetCommaSepCategoriesForAllComplaintTypesofCampaign(string campaignIds)
        {
            List<DbComplaintType> listComplaintType = DbComplaintType.GetCategoriesByCampaignIdsNotGroupId(Utility.GetIntList(campaignIds));
            //listComplaintType = GetCookieComplaintType(listComplaintType);
            return Utility.GetCommaSepStrFromList(listComplaintType.Select(n => n.Complaint_Category).ToList());
        }

        private static List<DbComplaintType> GetCookieComplaintType(List<DbComplaintType> listComplaintType)
        {
            CMSCookie cookie = AuthenticationHandler.GetCookie();
            if (!cookie.CategoryIds.Contains(-1)) // -1 means all are selected
            {
                listComplaintType.RemoveAll(n => !cookie.CategoryIds.Contains(n.Complaint_Category));
                //cookie.CategoryIds.RemoveAll(n=>)
            }
            var query = from s in listComplaintType
                        join c in cookie.CategoryIds
                        on s.Complaint_Category equals c
                        select s;
            return listComplaintType;
        }

        private static List<DbComplaintType> GetDbUserComplaintType(List<DbComplaintType> listComplaintType, DbUsers dbUser)
        {
            //CMSCookie cookie = AuthenticationHandler.GetCookie();
            List<int> listCategories = dbUser.Categories.Split(',').Select(int.Parse).ToList();
            if (!listCategories.Contains(-1)) // -1 means all are selected
            {
                listComplaintType.RemoveAll(n => !listCategories.Contains(n.Complaint_Category));
                //cookie.CategoryIds.RemoveAll(n=>)
            }
            return listComplaintType;
        }
    }
}