using System.Data.SqlClient;
using Z.BulkOperations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using PITB.CMS_Common;
using PITB.CMS_Models.DB;
using PITB.CMS_Models.ApiModels.Response;

namespace PITB.CMS_Handlers.DB.Repository
{

    public class RepoDbTehsil
    {
        #region HelperMethods
        public static List<DbTehsil> GetTehsilList(int districtId)
        {
            try
            {
                DBContextHelperLinq db = new DBContextHelperLinq();
                return (from d in db.DbDistricts
                        join t in db.DbTehsils on
                        d.District_Id equals t.District_Id
                        where d.District_Id == districtId
                        orderby t.Tehsil_Name
                        select t).ToList();

            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public static List<DbTehsil> GetTehsilList(int districtId, int? groupId)
        {
            try
            {
                DBContextHelperLinq db = new DBContextHelperLinq();
                return (from d in db.DbDistricts
                        join t in db.DbTehsils on
                        d.District_Id equals t.District_Id
                        where d.District_Id == districtId && t.Group_Id == groupId
                        orderby t.Tehsil_Name
                        select t).ToList();

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static List<DbTehsil> GetByDistrictIdList(List<int?> listDistrictIds, int? groupId)
        {
            try
            {
                DBContextHelperLinq db = new DBContextHelperLinq();
                return (from d in db.DbDistricts
                        join t in db.DbTehsils on
                        d.District_Id equals t.District_Id
                        where listDistrictIds.Contains(d.District_Id) && t.Group_Id == groupId
                        orderby t.Tehsil_Name
                        select t).ToList();

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static IEnumerable<SelectListItem> GetSelectListofTehsilForCampaignAndDistrictIds(int campaignId, int districtId)
        {
            try
            {
                int groupId = RepoDbHierarchyCampaignGroupMapping.GetGroupIdForCampaignAndHierarchyIds(campaignId, (int)Config.Hierarchy.Tehsil);
                List<DbTehsil> lstTeh = null;
                if (groupId == -1)
                {
                    lstTeh = GetByDistrictAndGroupId(districtId, null);
                }
                else
                {
                    lstTeh = GetByDistrictAndGroupId(districtId, groupId);
                }
                List<SelectListItem> lstSelectList = new List<SelectListItem>();
                //lstSelectList.Add(new SelectListItem()
                //{
                //    Value = "-1",
                //    Text = "--Select--",
                //    Selected = true
                //});
                for (int i = 0; i < lstTeh.Count; i++)
                {
                    lstSelectList.Add(new SelectListItem() { Text = lstTeh[i].Tehsil_Name, Value = lstTeh[i].Tehsil_Id.ToString() });
                }
                return new SelectList(lstSelectList, "Value", "Text");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static List<DbTehsil> GetByDistrictIdList(DBContextHelperLinq db, List<int?> listDistrictIds, int? groupId)
        {
            try
            {
                //DBContextHelperLinq db = new DBContextHelperLinq();
                return (from d in db.DbDistricts
                        join t in db.DbTehsils on
                        d.District_Id equals t.District_Id
                        where listDistrictIds.Contains(d.District_Id) && t.Group_Id == groupId
                        orderby t.Tehsil_Name
                        select t).ToList();

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static DbTehsil GetById(int tehsilId)
        {
            try
            {
                DBContextHelperLinq db = new DBContextHelperLinq();
                return db.DbTehsils.Where(n => n.Tehsil_Id == tehsilId).ToList().First();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static List<DbTehsil> GetByDistrictId(int districtId)
        {
            try
            {
                DBContextHelperLinq db = new DBContextHelperLinq();
                return db.DbTehsils.Where(n => n.District_Id == districtId).ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static List<DbTehsil> GetByDistrictAndGroupId(int districtId, int? groupId)
        {
            try
            {
                DBContextHelperLinq db = new DBContextHelperLinq();
                return db.DbTehsils.Where(n => n.District_Id == districtId && n.Group_Id == groupId).ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public static List<DbTehsil> GetTehsilByProvinceIdAndGroupId(int provinceId, int? groupId)
        {
            try
            {
                DBContextHelperLinq db = new DBContextHelperLinq();
                return db.DbProvinces.Where(x => x.Province_Id == provinceId).Join(db.DbDivisions, p => p.Province_Id, div => div.Province_Id, (p, div) => div).Join(db.DbDistricts, div => div.Division_Id, dis => dis.Division_Id, (div, dis) => dis).Join(db.DbTehsils, dis => dis.District_Id, teh => teh.District_Id, (dis, teh) => teh).Where(q => q.Group_Id == 4).ToList<DbTehsil>();
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public static List<DbTehsil> GetByGroupId(int? groupId)
        {
            try
            {
                DBContextHelperLinq db = new DBContextHelperLinq();
                return db.DbTehsils.Where(x => x.Group_Id == groupId && x.Is_Active == true).ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public static List<DbTehsil> GetTehsilByProvinceAndDivisionIdAndGroupId(int provinceId, int divisionId, int? groupId)
        {
            try
            {
                DBContextHelperLinq db = new DBContextHelperLinq();
                return db.DbProvinces.Where(x => x.Province_Id == provinceId).Join(db.DbDivisions, p => p.Province_Id, div => div.Province_Id, (p, div) => div).Where(a => a.Division_Id == divisionId).Join(db.DbDistricts, div => div.Division_Id, dis => dis.Division_Id, (div, dis) => dis).Join(db.DbTehsils, dis => dis.District_Id, teh => teh.District_Id, (dis, teh) => teh).Where(q => q.Group_Id == 4).ToList<DbTehsil>();
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public static List<DbTehsil> GetTehsilByProvinceAndDivisionAndDistrictIdAndGroupId(int provinceId, int divisionId, int districtId, int? groupId)
        {
            try
            {
                DBContextHelperLinq db = new DBContextHelperLinq();
                return db.DbProvinces.Where(x => x.Province_Id == provinceId).Join(db.DbDivisions, p => p.Province_Id, div => div.Province_Id, (p, div) => div).Where(a => a.Division_Id == divisionId).Join(db.DbDistricts, div => div.Division_Id, dis => dis.Division_Id, (div, dis) => dis).Where(q => q.District_Id == districtId).Join(db.DbTehsils, dis => dis.District_Id, teh => teh.District_Id, (dis, teh) => teh).Where(q => q.Group_Id == 4).ToList<DbTehsil>();
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        //public static List<DbTehsil> GetTehsilByProvinceAndDivisionIdAndGroupId(int provinceId,int divisionId, int? groupId)
        //{
        //    try
        //    {
        //        DBContextHelperLinq db = new DBContextHelperLinq();

        //        var lstDivision = db.DbDivisions.Where(x => x.Province_Id == provinceId).Select(y => new { Id = y.Division_Id,DivisionName=y.Division_Name});
        //        var lstDistricts = db.DbDistricts.Where(x=> x.Division_Id == )
        //    }catch(Exception ex){
        //        throw ex;
        //    }
        //}
        // public static List<DbTehsil> GetTehsilByProvinceAndDivisionAndDistrictIdAndGroupId(int provinceId,int divisionId,int district, int? groupId)
        //{
        //    try
        //    {
        //        DBContextHelperLinq db = new DBContextHelperLinq();

        //        var lstDivision = db.DbDivisions.Where(x => x.Province_Id == provinceId).Select(y => new { Id = y.Division_Id,DivisionName=y.Division_Name});
        //        var lstDistricts = db.DbDistricts.Where(x=> x.Division_Id == )
        //    }catch(Exception ex){
        //        throw ex;
        //    }
        //}
        public static List<DbTehsil> GetByDistrictIdsAndGroupId(string districtIds, int? groupId)
        {
            try
            {
                List<int> listIds = districtIds.Split(',').Select(int.Parse).ToList();

                DBContextHelperLinq db = new DBContextHelperLinq();
                return db.DbTehsils.Where(n => listIds.Contains((int)n.District_Id) && n.Group_Id == groupId).ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static List<DbTehsil> GetByDistrictIdsStr(string districtIds)
        {
            try
            {
                List<int> listIds = districtIds.Split(',').Select(int.Parse).ToList();
                //DBContextHelperLinq db = new DBContextHelperLinq();
                //return db.DbDistricts.Where(n => n.Division_Id == divisionId).ToList();

                using (var db = new DBContextHelperLinq())
                {
                    //return string.Join(",", list.Select(n => n.ToString()).ToArray());
                    return db.DbTehsils.AsNoTracking().Where(m => listIds.Contains((int)m.District_Id)).ToList();
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static string GetByTehsilIdsStr(string idsStr)
        {
            try
            {
                List<int> listIds = idsStr.Split(',').Select(int.Parse).ToList();
                using (var db = new DBContextHelperLinq())
                {
                    //return string.Join(",", list.Select(n => n.ToString()).ToArray());
                    List<string> listNames = db.DbTehsils.AsNoTracking().Where(m => listIds.Contains(m.Tehsil_Id)).Select(n => n.Tehsil_Name).ToList();
                    return string.Join(",", listNames.Select(n => n.ToString()).ToArray());
                }

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static List<DbTehsil> GetByTehsilIdsObject(string idsStr)
        {
            try
            {
                List<int> listIds = idsStr.Split(',').Select(int.Parse).ToList();
                using (var db = new DBContextHelperLinq())
                {
                    return db.DbTehsils.AsNoTracking().Where(m => listIds.Contains(m.Tehsil_Id)).ToList<DbTehsil>();
                }

            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public bool SetUpdated(ApiResponseSyncSEModel.ResponseSEDataTehsil.Tehsil syncRespTehsil, int crmDistrictId)
        {
            if (crmDistrictId != this.District_Id
                || !syncRespTehsil.tehsil_name.Equals(this.Tehsil_Name)
                || syncRespTehsil.is_Active != this.Is_Active)
            {
                this.Tehsil_Name = syncRespTehsil.tehsil_name;
                this.Is_Active = syncRespTehsil.is_Active;
                this.District_Id = crmDistrictId;
                return true;
            }
            else
            {
                return false;
            }
        }

        public static DbTehsil GetDbTeshil(ApiResponseSyncSEModel.ResponseSEDataTehsil.Tehsil syncRespTehsil, int? groupId, int districtId)
        {
            DbTehsil dbTehsil = new DbTehsil();
            dbTehsil.Tehsil_Name = syncRespTehsil.tehsil_name;
            dbTehsil.District_Id = districtId;
            dbTehsil.Group_Id = groupId;
            dbTehsil.Is_Active = syncRespTehsil.is_Active;
            return dbTehsil;
        }

        public static bool BulkMerge(List<DbTehsil> listToMerge, SqlConnection con)
        {
            //SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["PITB.CMS"].ConnectionString);
            //con.Open();
            BulkOperation<DbTehsil> bulkOp = new BulkOperation<DbTehsil>(con);
            bulkOp.BatchSize = 1000;
            bulkOp.ColumnInputExpression = c => new
            {
                c.Tehsil_Name,
                c.District_Id,
                c.Group_Id,
                c.Is_Active
            };
            bulkOp.DestinationTableName = "PITB.Tehsil";
            bulkOp.ColumnOutputExpression = c => c.Id;
            bulkOp.ColumnPrimaryKeyExpression = c => c.Id;
            bulkOp.BulkMerge(listToMerge);
            return true;
        }

        #endregion
    }
}
