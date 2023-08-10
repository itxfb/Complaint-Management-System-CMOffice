using Z.BulkOperations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.SqlClient;
using System.Web.Mvc;
using PITB.CMS_Common;
using PITB.CMS_Models.DB;
using PITB.CMS_Models.ApiModels.Response;

namespace PITB.CMS_Handlers.DB.Repository
{

    public class RepoDbDistrict
    {
        #region HelperMethods


        public static List<DbDistrict> GetAll()
        {
            try
            {
                using (DBContextHelperLinq db = new DBContextHelperLinq())
                {
                    //DBContextHelperLinq db = new DBContextHelperLinq();
                    return db.DbDistricts.Where(n => n.Is_Active == true).Select(n => n).ToList();

                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static List<DbDistrict> GetAll(DBContextHelperLinq db)
        {
            try
            {
                return db.DbDistricts.Select(n => n).ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static List<DbDistrict> GetDistrictList(int ProvinceId)
        {
            try
            {
                DBContextHelperLinq db = new DBContextHelperLinq();
                return (from p in db.DbProvinces
                        join d in db.DbDivisions on
                        p.Province_Id equals d.Province_Id
                        join c in db.DbDistricts on
                        d.Division_Id equals c.Division_Id
                        where d.Province_Id == ProvinceId && (c.Is_Active == true)
                        orderby c.District_Name
                        select c).ToList();

            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public static IEnumerable<SelectListItem> GetSelectListofDistrictForCampaignAndDivisionIds(int campaignId, int divisionId)
        {
            try
            {
                int groupId = DbHierarchyCampaignGroupMapping.GetGroupIdForCampaignAndHierarchyIds(campaignId, (int)Config.Hierarchy.Division);
                List<DbDistrict> lstDis = null;
                if (groupId == -1)
                {
                    lstDis = GetByDivisionAndGroupId(divisionId, null);
                }
                else
                {
                    lstDis = GetByDivisionAndGroupId(divisionId, groupId);
                }
                List<SelectListItem> lstSelectList = new List<SelectListItem>();
                //lstSelectList.Add(new SelectListItem()
                //{
                //    Value = "-1",
                //    Text = "--Select--",
                //    Selected = true
                //});
                for (int i = 0; i < lstDis.Count; i++)
                {
                    lstSelectList.Add(new SelectListItem() { Text = lstDis[i].District_Name, Value = lstDis[i].District_Id.ToString() });
                }
                return new SelectList(lstSelectList, "Value", "Text");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static List<DbDistrict> GetByGroupId(int? groupId)
        {
            try
            {
                using (DBContextHelperLinq db = new DBContextHelperLinq())
                {
                    return db.DbDistricts.AsNoTracking().Where(n => n.Group_Id == groupId && n.Is_Active == true).ToList();
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public static List<DbDistrict> GetByProvinceIdsList(List<int?> listProvinceIds, int? groupId)
        {
            try
            {
                DBContextHelperLinq db = new DBContextHelperLinq();
                return (from p in db.DbProvinces
                        join d in db.DbDivisions on
                        p.Province_Id equals d.Province_Id
                        join c in db.DbDistricts on
                        d.Division_Id equals c.Division_Id
                        where listProvinceIds.Contains(d.Province_Id) && (c.Is_Active == true) && d.Group_Id == groupId
                        orderby c.District_Name
                        select c).ToList();

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static List<DbDistrict> GetDistrictByProvinceAndGroup(int ProvinceId, int? groupId)
        {
            try
            {
                DBContextHelperLinq db = new DBContextHelperLinq();
                return (from p in db.DbProvinces
                        join d in db.DbDivisions on
                        p.Province_Id equals d.Province_Id
                        join c in db.DbDistricts on
                        d.Division_Id equals c.Division_Id
                        where (d.Province_Id == ProvinceId)
                        && (c.Group_Id == groupId) && (c.Is_Active == true)
                        orderby c.District_Name
                        select c).ToList();

            }
            catch (Exception ex)
            {
                return null;
            }
        }


        public static DbDistrict GetById(int districtId)
        {
            try
            {
                DBContextHelperLinq db = new DBContextHelperLinq();
                return db.DbDistricts.Where(n => n.District_Id == districtId).ToList().First();
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        public static List<DbDistrict> GetByDivisionId(int divisionId)
        {
            try
            {
                DBContextHelperLinq db = new DBContextHelperLinq();
                return db.DbDistricts.Where(n => n.Division_Id == divisionId).ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static List<DbDistrict> GetByDivisionAndGroupId(int divisionId, int? groupId)
        {
            try
            {
                DBContextHelperLinq db = new DBContextHelperLinq();
                return db.DbDistricts.Where(n => n.Division_Id == divisionId && n.Group_Id == groupId && n.Is_Active == true).ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        public static List<DbDistrict> GetByDivisionIdsStr(string divisionIds)
        {
            try
            {
                List<int> listIds = divisionIds.Split(',').Select(int.Parse).ToList();
                //DBContextHelperLinq db = new DBContextHelperLinq();
                //return db.DbDistricts.Where(n => n.Division_Id == divisionId).ToList();

                using (var db = new DBContextHelperLinq())
                {
                    //return string.Join(",", list.Select(n => n.ToString()).ToArray());
                    return db.DbDistricts.AsNoTracking().Where(m => listIds.Contains((int)m.Division_Id) && m.Group_Id == null).ToList();
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public static object GetDistrictsByProvinceId(int provinveId)
        {
            try
            {

                using (var db = new DBContextHelperLinq())
                {
                    var lstDivisions = (from prov in db.DbProvinces join div in db.DbDivisions on prov.Province_Id equals div.Province_Id where prov.Province_Id == provinveId select new { DivisionId = div.Division_Id });
                    var lstDistricts = from div in lstDivisions join dis in db.DbDistricts on div.DivisionId equals dis.Division_Id where dis.Group_Id == null select new { DistrictId = dis.District_Id };
                    var obj = lstDistricts.ToList();
                    var d = string.Join(",", obj.Select(x => x.DistrictId).ToList());
                    return d;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public static object GetDistrictsByProvinceIdForZavoreTaleem(int provinveId)
        {
            try
            {

                using (var db = new DBContextHelperLinq())
                {
                    var lstDivisions = (from prov in db.DbProvinces join div in db.DbDivisions on prov.Province_Id equals div.Province_Id where prov.Province_Id == provinveId select new { DivisionId = div.Division_Id });
                    var lstDistricts = from div in lstDivisions join dis in db.DbDistricts on div.DivisionId equals dis.Division_Id select new { DistrictId = dis.District_Id };
                    var obj = lstDistricts.ToList();
                    var excludedList = new[] { 7, 12, 9, 3, 15, 11, 26, 8, 10, 23, 29, 6, 36, 27, 31, 13 };
                    var d = string.Join(",", excludedList);
                    return d;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public static IEnumerable<object> GetDistrictsNameAndIDByProvinceId(int provinveId)
        {
            try
            {

                using (var db = new DBContextHelperLinq())
                {
                    var lstDivisions = (from prov in db.DbProvinces join div in db.DbDivisions on prov.Province_Id equals div.Province_Id where prov.Province_Id == provinveId select new { DivisionId = div.Division_Id });
                    var lstDistricts = from div in lstDivisions join dis in db.DbDistricts on div.DivisionId equals dis.Division_Id where dis.Is_Active == true && dis.Group_Id == null select new { DistrictId = dis.District_Id, DistrictName = dis.District_Name };
                    return lstDistricts.OrderBy(x => x.DistrictName).ToList();
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static IEnumerable<object> GetDistrictsByProvinceAndDivisionId(int provinveId, int divisionId)
        {
            try
            {

                using (var db = new DBContextHelperLinq())
                {
                    var lstDivisions = (from prov in db.DbProvinces join div in db.DbDivisions on prov.Province_Id equals div.Province_Id where prov.Province_Id == provinveId select new { DivisionId = div.Division_Id });
                    var lstDistricts = from div in lstDivisions join dis in db.DbDistricts on div.DivisionId equals dis.Division_Id where div.DivisionId == divisionId && dis.Is_Active == true && dis.Group_Id == null select new { DistrictId = dis.District_Id, DistrictName = dis.District_Name };
                    return lstDistricts.OrderBy(x => x.DistrictName).ToList();
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public static object GetDistrictsByDivisionId(int divisionId)
        {
            try
            {

                using (var db = new DBContextHelperLinq())
                {
                    var lstDistricts = from div in db.DbDivisions join dis in db.DbDistricts on div.Division_Id equals dis.Division_Id where div.Division_Id == divisionId && dis.Group_Id == null select new { DistrictId = dis.District_Id };
                    return lstDistricts.ToList();
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public static List<DbDistrict> GetByDivisionIdsStrAndGroupId(string divisionIds, int? groupId)
        {
            try
            {
                List<int> listIds = divisionIds.Split(',').Select(int.Parse).ToList();
                //DBContextHelperLinq db = new DBContextHelperLinq();
                //return db.DbDistricts.Where(n => n.Division_Id == divisionId).ToList();

                using (var db = new DBContextHelperLinq())
                {
                    //return string.Join(",", list.Select(n => n.ToString()).ToArray());
                    return db.DbDistricts.AsNoTracking().Where(m => listIds.Contains((int)m.Division_Id) && m.Group_Id == groupId && (m.Is_Active == true)).ToList();
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static string GetByDistrictIdsStr(string idsStr)
        {
            try
            {
                List<int> listIds = idsStr.Split(',').Select(int.Parse).ToList();
                using (var db = new DBContextHelperLinq())
                {
                    //return string.Join(",", list.Select(n => n.ToString()).ToArray());
                    List<string> listNames = db.DbDistricts.AsNoTracking().Where(m => listIds.Contains(m.District_Id) && (m.Is_Active == true)).Select(n => n.District_Name).ToList();
                    return string.Join(",", listNames.Select(n => n.ToString()).ToArray());
                }

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public bool SetUpdated(ApiResponseSyncSEModel.ResponseSEDataDistrict.District syncRespDistrict)
        {
            if (!syncRespDistrict.district_name.Equals(this.District_Name) || syncRespDistrict.is_Active != this.Is_Active)
            {
                this.District_Name = syncRespDistrict.district_name;
                this.Is_Active = syncRespDistrict.is_Active;
                return true;
            }
            else
            {
                return false;
            }
        }

        public static DbDistrict GetDbDistrict(ApiResponseSyncSEModel.ResponseSEDataDistrict.District syncRespDistrict)
        {
            DbDistrict dbDistrict = new DbDistrict();
            dbDistrict.District_Name = syncRespDistrict.district_name;
            dbDistrict.Is_Active = syncRespDistrict.is_Active;
            return dbDistrict;
        }

        public static bool BulkMerge(List<DbDistrict> listToMerge, SqlConnection con)
        {
            //SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["PITB.CMS"].ConnectionString);
            //con.Open();
            BulkOperation<DbDistrict> bulkOp = new BulkOperation<DbDistrict>(con);
            bulkOp.BatchSize = 1000;
            bulkOp.ColumnInputExpression = c => new
            {
                c.District_Name,
                c.Is_Active
            };
            bulkOp.DestinationTableName = "PITB.Districts";
            bulkOp.ColumnOutputExpression = c => c.id;
            bulkOp.ColumnPrimaryKeyExpression = c => c.id;
            bulkOp.BulkMerge(listToMerge);
            return true;
        }

        #endregion
    }
}


