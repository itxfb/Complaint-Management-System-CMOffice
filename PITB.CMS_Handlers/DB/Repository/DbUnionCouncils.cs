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


    //public class RepoDbUnionCouncils2
    //{
    //    [Key]
    //    public int Id { get; set; }
    //    public string Councils_Name { get; set; }

    //    public int? Tehsil_Id { get; set; }

    //    public int? Campaign_Id { get; set; }

    //    public int? Group_Id { get; set; }

    //    public int? UcNo { get; set; }

    //    public bool? Is_Active { get; set; }
    //}


    public class RepoDbUnionCouncils
    {
        #region HelperMethods
        public static List<DbUnionCouncils> GetUnionCouncilList(int tehsilId, int? groupId)
        {
            try
            {
                DBContextHelperLinq db = new DBContextHelperLinq();

                return (from uc in db.DbUnionCouncils
                        join t in db.DbTehsils on
                        uc.Tehsil_Id equals t.Tehsil_Id
                        where t.Tehsil_Id == tehsilId && uc.Group_Id == groupId
                        orderby uc.Councils_Name
                        select uc).ToList();

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static List<DbUnionCouncils> GetUnionCouncilList(List<int?> listTehsilId, int? groupId)
        {
            try
            {
                DBContextHelperLinq db = new DBContextHelperLinq();

                return (from uc in db.DbUnionCouncils
                        join t in db.DbTehsils on
                        uc.Tehsil_Id equals t.Tehsil_Id
                        where listTehsilId.Contains(t.Tehsil_Id) && uc.Group_Id == groupId
                        orderby uc.Councils_Name
                        select uc).ToList();

            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public static List<DbUnionCouncils> GetUnionCouncilList(List<int> listTehsilId, int? groupId)
        {
            try
            {
                DBContextHelperLinq db = new DBContextHelperLinq();

                return (from uc in db.DbUnionCouncils
                        join t in db.DbTehsils on
                        uc.Tehsil_Id equals t.Tehsil_Id
                        where listTehsilId.Contains(t.Tehsil_Id) && uc.Group_Id == groupId
                        orderby uc.Councils_Name
                        select uc).ToList();

            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public static IEnumerable<SelectListItem> GetSelectListofUnionCouncilForCampaignAndTehsilIds(int campaignId, string tehsilId)
        {
            try
            {
                if (tehsilId != null || !string.IsNullOrEmpty(tehsilId))
                {
                    int groupId = RepoDbHierarchyCampaignGroupMapping.GetGroupIdForCampaignAndHierarchyIds(campaignId, (int)Config.Hierarchy.UnionCouncil);
                    List<DbUnionCouncils> lstUc = null;
                    string[] t = tehsilId.Split(new char[] { ',' });
                    var ids = Utility.GetIntArrayFromStringArray(t).ToList<int>();
                    if (groupId == -1)
                    {
                        lstUc = GetUnionCouncilList(ids, null);
                    }
                    else
                    {
                        lstUc = GetUnionCouncilList(ids, groupId);
                    }
                    List<SelectListItem> lstSelectList = new List<SelectListItem>();
                    //lstSelectList.Add(new SelectListItem()
                    //{
                    //    Value = "-1",
                    //    Text = "--Select--",
                    //    Selected = true
                    //});
                    for (int i = 0; i < lstUc.Count; i++)
                    {
                        lstSelectList.Add(new SelectListItem() { Text = lstUc[i].Councils_Name, Value = lstUc[i].UnionCouncil_Id.ToString() });
                    }
                    return new SelectList(lstSelectList, "Value", "Text");
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static List<DbUnionCouncils> GetUnionCouncilList(DBContextHelperLinq db, List<int?> listTehsilId, int? groupId)
        {
            try
            {
                return (from uc in db.DbUnionCouncils
                        join t in db.DbTehsils on
                        uc.Tehsil_Id equals t.Tehsil_Id
                        where listTehsilId.Contains(t.Tehsil_Id) && uc.Group_Id == groupId
                        orderby uc.Councils_Name
                        select uc).ToList();

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static List<DbUnionCouncils> GetUnionCouncilListByCouncilAndCampaign(int tehsilId, int? groupId)
        {
            try
            {
                DBContextHelperLinq db = new DBContextHelperLinq();

                return (from uc in db.DbUnionCouncils
                        join t in db.DbTehsils on
                        uc.Tehsil_Id equals t.Tehsil_Id
                        where t.Tehsil_Id == tehsilId && uc.Group_Id == groupId
                        orderby uc.Councils_Name
                        select uc).ToList();

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static DbUnionCouncils GetById(int ucId)
        {
            try
            {
                DBContextHelperLinq db = new DBContextHelperLinq();
                return db.DbUnionCouncils.Where(n => n.UnionCouncil_Id == ucId).ToList().First();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static List<DbUnionCouncils> GetByTehsilId(int tehsilId, int? groupId)
        {
            try
            {
                DBContextHelperLinq db = new DBContextHelperLinq();
                return db.DbUnionCouncils.Where(n => n.Tehsil_Id == tehsilId && n.Group_Id == groupId).ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static List<DbUnionCouncils> GetByTehsilIdsStr(string tehsilIds, int? groupId)
        {
            try
            {
                List<int> listIds = tehsilIds.Split(',').Select(int.Parse).ToList();
                //DBContextHelperLinq db = new DBContextHelperLinq();
                //return db.DbDistricts.Where(n => n.Division_Id == divisionId).ToList();

                using (var db = new DBContextHelperLinq())
                {
                    //return string.Join(",", list.Select(n => n.ToString()).ToArray());
                    return db.DbUnionCouncils.AsNoTracking().Where(m => listIds.Contains((int)m.Tehsil_Id) && m.Group_Id == groupId).ToList();
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static string GetByUcIdsStr(string idsStr)
        {
            try
            {
                List<int> listIds = idsStr.Split(',').Select(int.Parse).ToList();
                using (var db = new DBContextHelperLinq())
                {
                    //return string.Join(",", list.Select(n => n.ToString()).ToArray());
                    List<string> listNames = db.DbUnionCouncils.AsNoTracking().Where(m => listIds.Contains(m.UnionCouncil_Id)).Select(n => n.Councils_Name).ToList();
                    return string.Join(",", listNames.Select(n => n.ToString()).ToArray());
                }

            }
            catch (Exception ex)
            {
                return null;
            }
        }


        public bool SetUpdated(ApiResponseSyncSEModel.ResponseSEDataMarkaz.Markaz syncRespMarkaz, int crmTehsilId)
        {
            if (crmTehsilId != this.Tehsil_Id
                || !syncRespMarkaz.markaz_name.Equals(this.Councils_Name)
                || syncRespMarkaz.is_Active != this.Is_Active)
            {
                this.Councils_Name = syncRespMarkaz.markaz_name;
                this.Is_Active = syncRespMarkaz.is_Active;
                this.Tehsil_Id = crmTehsilId;
                return true;
            }
            else
            {
                return false;
            }
        }

        public static DbUnionCouncils GetDbMarkaz(ApiResponseSyncSEModel.ResponseSEDataMarkaz.Markaz syncRespMarkaz, int? groupId, int tehsilId)
        {
            DbUnionCouncils dbUc = new DbUnionCouncils();
            dbUc.Councils_Name = syncRespMarkaz.markaz_name;
            dbUc.Tehsil_Id = tehsilId;
            dbUc.Group_Id = groupId;
            dbUc.Is_Active = syncRespMarkaz.is_Active;
            return dbUc;
        }

        public static bool BulkMerge(List<DbUnionCouncils> listToMerge, SqlConnection con)
        {
            //SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["PITB.CMS"].ConnectionString);
            //con.Open();
            BulkOperation<DbUnionCouncils> bulkOp = new BulkOperation<DbUnionCouncils>(con);
            bulkOp.BatchSize = 1000;
            bulkOp.ColumnInputExpression = c => new
            {
                c.Councils_Name,
                c.Group_Id,
                c.Tehsil_Id,
                c.Is_Active
            };
            bulkOp.DestinationTableName = "PITB.Union_Councils";
            bulkOp.ColumnOutputExpression = c => c.Id;
            bulkOp.ColumnPrimaryKeyExpression = c => c.Id;
            bulkOp.BulkMerge(listToMerge);
            return true;
        }

        #endregion
    }
}
