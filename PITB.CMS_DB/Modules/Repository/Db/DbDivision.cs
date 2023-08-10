using PITB.CMS_Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web.Mvc;

namespace PITB.CMS_DB.Models
{

    public partial class DbDivision
    {
        public static DbDivision GetById(int id)
        {
            try
            {
                using (var db = new DBContextHelperLinq())
                {
                    return db.DbDivisions.AsNoTracking().Where(m => m.Division_Id == id && m.Is_Active == true).FirstOrDefault();
                }

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static List<DbDivision> GetByProvinceId(int provinceId)
        {
            try
            {
                using (var db = new DBContextHelperLinq())
                {
                    return db.DbDivisions.AsNoTracking().Where(m => m.Province_Id == provinceId && m.Is_Active == true).ToList();
                }

            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public static IEnumerable<SelectListItem> GetSelectListofDivisionForCampaignAndProvinceIds(int campaignId, int provinceId)
        {
            try
            {
                int groupId = DbHierarchyCampaignGroupMapping.GetGroupIdForCampaignAndHierarchyIds(campaignId, (int)Config.Hierarchy.Province);
                List<DbDivision> lstDiv = null;
                if (groupId == -1)
                {
                    lstDiv = DbDivision.GetByProvinceAndGroupId(provinceId, null);
                }
                else
                {
                    lstDiv = DbDivision.GetByProvinceAndGroupId(provinceId, groupId);
                }
                List<SelectListItem> lstSelectList = new List<SelectListItem>();
                //lstSelectList.Add(new SelectListItem()
                //{
                //    Value = "-1",
                //    Text = "--Select--",
                //    Selected = true
                //});
                for (int i = 0; i < lstDiv.Count; i++)
                {
                    lstSelectList.Add(new SelectListItem() { Text = lstDiv[i].Division_Name, Value = lstDiv[i].Division_Id.ToString() });
                }
                return new SelectList(lstSelectList, "Value", "Text");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static List<DbDivision> GetByProvinceAndGroupId(int provinceId, int? groupId)
        {
            try
            {
                using (var db = new DBContextHelperLinq())
                {
                    return db.DbDivisions.AsNoTracking().Where(m => m.Province_Id == provinceId && m.Group_Id == groupId && m.Is_Active == true).ToList();
                }

            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public static IEnumerable<object> GetDivisionsNameAndIDByProvinceId(int provinveId)
        {
            try
            {

                using (var db = new DBContextHelperLinq())
                {
                    var lstDivisions = from prov in db.DbProvinces
                                       join div in db.DbDivisions
                                       on prov.Province_Id equals div.Province_Id
                                       where div.Is_Active == true && prov.Province_Id == provinveId && div.Division_Id != 10 && div.Division_Id != 11 && div.Division_Id != 12 && div.Group_Id == null
                                       select new { DivisionId = div.Division_Id, DivisionName = div.Division_Name };
                    return lstDivisions.OrderBy(x => x.DivisionName).ToList();
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public static List<DbDivision> GetByProvinceIdsStr(string provinceIds)
        {
            try
            {
                List<int> listIds = provinceIds.Split(',').Select(int.Parse).ToList();
                //DBContextHelperLinq db = new DBContextHelperLinq();
                //return db.DbDistricts.Where(n => n.Division_Id == divisionId).ToList();

                using (var db = new DBContextHelperLinq())
                {
                    //return string.Join(",", list.Select(n => n.ToString()).ToArray());
                    return db.DbDivisions.AsNoTracking().Where(m => listIds.Contains((int)m.Province_Id) && m.Is_Active == true).ToList();
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public static List<DbDivision> GetByGroupId(int? groupId)
        {
            try
            {
                using (var db = new DBContextHelperLinq())
                {

                    return db.DbDivisions.AsNoTracking().Where(x => x.Group_Id == groupId && x.Is_Active == true).ToList();
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static List<DbDivision> GetByProvinceIdsStrAndGroupId(string provinceIds, int? groupId)
        {
            try
            {
                List<int> listIds = provinceIds.Split(',').Select(int.Parse).ToList();
                //DBContextHelperLinq db = new DBContextHelperLinq();
                //return db.DbDistricts.Where(n => n.Division_Id == divisionId).ToList();

                using (var db = new DBContextHelperLinq())
                {
                    //return string.Join(",", list.Select(n => n.ToString()).ToArray());
                    return db.DbDivisions.AsNoTracking().Where(m => listIds.Contains((int)m.Province_Id) && m.Group_Id == groupId && m.Is_Active == true).ToList();
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        public static string GetByDivisionIdsStr(string idsStr)
        {
            try
            {
                List<int> listIds = idsStr.Split(',').Select(int.Parse).ToList();
                using (var db = new DBContextHelperLinq())
                {
                    //return string.Join(",", list.Select(n => n.ToString()).ToArray());
                    List<string> listNames = db.DbDivisions.AsNoTracking().Where(m => listIds.Contains(m.Division_Id) && m.Is_Active == true).Select(n => n.Division_Name).ToList();
                    return string.Join(",", listNames.Select(n => n.ToString()).ToArray());
                }

            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public static object GetDivisionsByProvinceId(int provinveId)
        {
            try
            {

                using (var db = new DBContextHelperLinq())
                {

                    var lstDivisions = (from prov in db.DbProvinces join div in db.DbDivisions on prov.Province_Id equals div.Province_Id where prov.Province_Id == provinveId && div.Is_Active == true select new { DivisionId = div.Division_Id, DivisionName = div.Division_Name });
                    return lstDivisions.ToList();
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

    }
}
