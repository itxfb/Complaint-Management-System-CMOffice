using System.Data.SqlClient;
using System.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;
using Z.BulkOperations;
using PITB.CMS_Common;

namespace PITB.CMS_Common.Models
{

    public partial class DbCrmIdsMappingToOtherSystem
    {

        public static List<DbCrmIdsMappingToOtherSystem> GetAll()
        {
            try
            {
                using (var db = new DBContextHelperLinq())
                {
                    return db.DbCrmIdsMappingToOtherSystem.AsNoTracking().Select(n => n).ToList();
                }

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static List<DbCrmIdsMappingToOtherSystem> Get(int? crm_Module_Id, int? crm_Module_Cat1, int? ots_System_Id, int? ots_Module_Id)
        {
            try
            {
                using (var db = new DBContextHelperLinq())
                {
                    return db.DbCrmIdsMappingToOtherSystem.AsNoTracking().Where(n => n.Crm_Module_Id == crm_Module_Id
                        && n.Crm_Module_Cat1 == crm_Module_Cat1
                        && n.OTS_System_Id == ots_System_Id
                        && n.OTS_Module_Id == ots_Module_Id).Select(n => n).ToList();
                }

            }
            catch (Exception ex)
            {
                return null;
            }
        }


        public static List<DbCrmIdsMappingToOtherSystem> Get(int? crm_Module_Id, int? crm_Module_Cat1, int? ots_System_Id, int? ots_Module_Id, List<int?> listCrmId)
        {
            try
            {
                using (var db = new DBContextHelperLinq())
                {
                    return db.DbCrmIdsMappingToOtherSystem.AsNoTracking().Where(n => n.Crm_Module_Id == crm_Module_Id
                        && n.Crm_Module_Cat1 == crm_Module_Cat1
                        && n.OTS_System_Id == ots_System_Id
                        && n.OTS_Module_Id == ots_Module_Id
                        && listCrmId.Contains(n.Crm_Id)).Select(n => n).ToList();
                }

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static List<DbCrmIdsMappingToOtherSystem> Get(List<int?> listCrmId)
        {
            try
            {
                using (var db = new DBContextHelperLinq())
                {
                    return db.DbCrmIdsMappingToOtherSystem.AsNoTracking().Where(n => listCrmId.Contains(n.Crm_Id)).ToList();
                }

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static List<DbCrmIdsMappingToOtherSystem> Get(List<int?> listCrmModuleId, int? otherSystemId, string tagId)
        {
            try
            {
                using (var db = new DBContextHelperLinq())
                {
                    return db.DbCrmIdsMappingToOtherSystem.AsNoTracking().Where(n => listCrmModuleId.Contains(n.Crm_Module_Id)
                        && n.OTS_System_Id == otherSystemId
                        && n.Crm_Module_Tag == tagId).Select(n => n).ToList();
                }

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static List<DbCrmIdsMappingToOtherSystem> Get(List<int?> listCrmModuleId, int? otherSystemId)
        {
            try
            {
                using (var db = new DBContextHelperLinq())
                {
                    return db.DbCrmIdsMappingToOtherSystem.AsNoTracking().Where(n => listCrmModuleId.Contains(n.Crm_Module_Id)
                        && n.OTS_System_Id == otherSystemId).Select(n => n).ToList();
                }

            }
            catch (Exception ex)
            {
                return null;
            }
        }


        public static Dictionary<int, int> GetDictionaryCrmToOts(List<DbCrmIdsMappingToOtherSystem> listCrmIdsMappingToOtherSystems)
        {
            try
            {
                return listCrmIdsMappingToOtherSystems.ToDictionary(x => Convert.ToInt32(x.Crm_Id),
                    x => Convert.ToInt32(x.OTS_Id));
            }
            catch (Exception ex)
            {
                return new Dictionary<int, int>();
            }
        }

        public static Dictionary<int, int> GetDictionaryOtsToCrm(List<DbCrmIdsMappingToOtherSystem> listCrmIdsMappingToOtherSystems)
        {
            try
            {
                return listCrmIdsMappingToOtherSystems.ToDictionary(x => Convert.ToInt32(x.OTS_Id), x => Convert.ToInt32(x.Crm_Id));
            }
            catch (Exception ex)
            {
                return new Dictionary<int, int>();
            }
        }

        public static List<DbCrmIdsMappingToOtherSystem> Get(int? crm_Module_Id, int? ots_System_Id, int? ots_Module_Id)
        {
            try
            {
                using (var db = new DBContextHelperLinq())
                {
                    return db.DbCrmIdsMappingToOtherSystem.AsNoTracking().Where(n => n.Crm_Module_Id == crm_Module_Id
                        && n.OTS_System_Id == ots_System_Id
                        && n.OTS_Module_Id == ots_Module_Id).Select(n => n).ToList();
                }

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static List<DbCrmIdsMappingToOtherSystem> Get(int? crm_Module_Id, int? crm_Module_Cat1, int? crm_Module_Cat2, List<int?> listCrm_Module_Cat3, int? ots_System_Id, int? ots_Module_Id)
        {
            try
            {
                using (var db = new DBContextHelperLinq())
                {
                    return db.DbCrmIdsMappingToOtherSystem.AsNoTracking().Where(n => n.Crm_Module_Id == crm_Module_Id
                        && n.Crm_Module_Cat1 == crm_Module_Cat1
                        && n.Crm_Module_Cat2 == crm_Module_Cat2
                        && listCrm_Module_Cat3.Contains(n.Crm_Module_Cat3)
                        && n.OTS_System_Id == ots_System_Id
                        && n.OTS_Module_Id == ots_Module_Id).Select(n => n).ToList();
                }

            }
            catch (Exception ex)
            {
                return null;
            }
        }


        public static List<DbCrmIdsMappingToOtherSystem> GetAll(DBContextHelperLinq db)
        {
            try
            {
                //using (var db = new DBContextHelperLinq())
                {
                    return db.DbCrmIdsMappingToOtherSystem.AsNoTracking().Select(n => n).ToList();
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static DbCrmIdsMappingToOtherSystem GetModel(DbCrmIdsMappingToOtherSystem dbTempCrmIdsMapping, int crmId, int otsId, bool isActive, DateTime createdDateTime)
        {
            //List<DbCrmIdsMappingToOtherSystem> listCrmIdsMappingToOtherSystems = new ListStack<DbCrmIdsMappingToOtherSystem>();
            DbCrmIdsMappingToOtherSystem dbCrmIdsMappingToOtherSystem = new DbCrmIdsMappingToOtherSystem();
            dbCrmIdsMappingToOtherSystem.Crm_Module_Id = dbTempCrmIdsMapping.Crm_Module_Id;
            dbCrmIdsMappingToOtherSystem.Crm_Module_Cat1 = dbTempCrmIdsMapping.Crm_Module_Cat1;
            dbCrmIdsMappingToOtherSystem.Crm_Module_Cat2 = dbTempCrmIdsMapping.Crm_Module_Cat2;
            dbCrmIdsMappingToOtherSystem.Crm_Module_Cat3 = dbTempCrmIdsMapping.Crm_Module_Cat3;
            dbCrmIdsMappingToOtherSystem.Crm_Id = crmId;
            dbCrmIdsMappingToOtherSystem.OTS_System_Id = dbTempCrmIdsMapping.OTS_System_Id;
            dbCrmIdsMappingToOtherSystem.OTS_Module_Id = dbTempCrmIdsMapping.OTS_Module_Id;
            dbCrmIdsMappingToOtherSystem.OTS_Id = otsId;
            dbCrmIdsMappingToOtherSystem.Is_Active = isActive;
            dbCrmIdsMappingToOtherSystem.Created_Date = createdDateTime;
            //listCrmIdsMappingToOtherSystems.Add(dbCrmIdsMappingToOtherSystem);
            return dbCrmIdsMappingToOtherSystem;
        }

        public static bool BulkMerge(List<DbCrmIdsMappingToOtherSystem> listCrmIdsMappingToOtherSystems, SqlConnection con, int currentRetryCount = 0, int totalRetryCount = Config.FunctionRetryCount)
        {
            try
            {
                BulkOperation<DbCrmIdsMappingToOtherSystem> bulkOp = new BulkOperation<DbCrmIdsMappingToOtherSystem>(con);
                bulkOp.BatchSize = 1000;
                bulkOp.DestinationTableName = "PITB.CrmIdsMappingToOtherSystems";
                bulkOp.ColumnInputExpression = c => new
                {
                    c.Crm_Module_Id,
                    c.Crm_Module_Cat1,
                    c.Crm_Module_Cat2,
                    c.Crm_Module_Cat3,
                    c.Crm_Id,
                    c.OTS_System_Id,
                    c.OTS_Module_Id,
                    c.OTS_Id,
                    c.Is_Active,
                    c.Created_Date
                };
                bulkOp.ColumnOutputExpression = c => c.Id;
                bulkOp.ColumnPrimaryKeyExpression = c => c.Id;
                bulkOp.BulkInsert(listCrmIdsMappingToOtherSystems);
                return true;
            }
            catch (Exception ex)
            {
                if (currentRetryCount != totalRetryCount)
                {
                    BulkMerge(listCrmIdsMappingToOtherSystems, con, currentRetryCount + 1, totalRetryCount);
                }
            }
            return false;
        }

        public static int GetOtsId(DbCrmIdsMappingToOtherSystem dbCrmIdsMapToOts)
        {
            if (dbCrmIdsMapToOts == null)
            {
                return 0;
            }
            else
            {
                return Convert.ToInt32(dbCrmIdsMapToOts.OTS_Id);
            }
        }
    }
}
