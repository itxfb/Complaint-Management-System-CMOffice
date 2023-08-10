using System.Collections.Generic;
using System.Data.SqlClient;
using System.Web.Razor.Generator;
using System.Linq;
using Z.BulkOperations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System;
using System.Data;
using PITB.CMS_Common;
using PITB.CMS_Models.DB;

namespace PITB.CMS_Handlers.DB.Repository
{

    public class RepoDbPermissionsAssignment
    {

        public static List<DbPermissionsAssignment> GetListOfPermissions(int type, int typeId)
        {
            try
            {
                using (DBContextHelperLinq db = new DBContextHelperLinq())
                {
                    List<DbPermissionsAssignment> listDbPermission = db.DbPermissionsAssignments.AsNoTracking().Where(m => m.Type == type && typeId == m.Type_Id).ToList();
                    //List<DbPermissionsAssignment> listDbPermission = db.DbPermissionsAssignments.Select(n => n).ToList();
                    return listDbPermission;
                }


            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public static List<DbPermissionsAssignment> GetListOfPermissionsByTypeAndPermissionId(int type, int permissonId)
        {
            try
            {
                using (DBContextHelperLinq db = new DBContextHelperLinq())
                {
                    List<DbPermissionsAssignment> listDbPermission = db.DbPermissionsAssignments.AsNoTracking().Where(m => m.Type == type && permissonId == m.Permission_Id).ToList();
                    //List<DbPermissionsAssignment> listDbPermission = db.DbPermissionsAssignments.Select(n => n).ToList();
                    return listDbPermission;
                }


            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public static List<DbPermissionsAssignment> GetListOfPermissionsByTypeTypeIdAndPermissionId(int type, int typeId, int permissonId)
        {
            try
            {
                using (DBContextHelperLinq db = new DBContextHelperLinq())
                {
                    List<DbPermissionsAssignment> listDbPermission = db.DbPermissionsAssignments.AsNoTracking().Where(m => m.Type == type && m.Type_Id == typeId && permissonId == m.Permission_Id).ToList();
                    //List<DbPermissionsAssignment> listDbPermission = db.DbPermissionsAssignments.Select(n => n).ToList();
                    return listDbPermission;
                }


            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public static List<DbPermissionsAssignment> GetListOfPermissionsByTypeTypeIdListAndPermissionId(int type, List<int> listTypeId, int permissonId)
        {
            try
            {
                using (DBContextHelperLinq db = new DBContextHelperLinq())
                {
                    List<DbPermissionsAssignment> listDbPermission = db.DbPermissionsAssignments.AsNoTracking().Where(m => m.Type == type && listTypeId.Any(s => s == m.Type_Id) && permissonId == m.Permission_Id).ToList();
                    //List<DbPermissionsAssignment> listDbPermission = db.DbPermissionsAssignments.Select(n => n).ToList();
                    return listDbPermission;
                }


            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public static List<DbPermissionsAssignment> GetListOfPermissions(int type, List<int?> listTypeIds)
        {
            try
            {
                using (DBContextHelperLinq db = new DBContextHelperLinq())
                {
                    List<DbPermissionsAssignment> listDbPermission = db.DbPermissionsAssignments.AsNoTracking().Where(m => m.Type == type && listTypeIds.Contains(m.Type_Id)).ToList();
                    //List<DbPermissionsAssignment> listDbPermission = db.DbPermissionsAssignments.Select(n => n).ToList();
                    return listDbPermission;
                }


            }
            catch (Exception ex)
            {
                throw;
            }
        }


        public static List<DbPermissionsAssignment> GetListOfPermissions(DBContextHelperLinq db, int type, int typeId, int permissionId)
        {
            try
            {

                List<DbPermissionsAssignment> listDbPermission = db.DbPermissionsAssignments.AsNoTracking().Where(m => m.Type == type && m.Type_Id == typeId && m.Permission_Id == permissionId).ToList();
                //List<DbPermissionsAssignment> listDbPermission = db.DbPermissionsAssignments.Select(n => n).ToList();
                return listDbPermission;

            }
            catch (Exception ex)
            {
                throw;
            }
        }


        public static List<DbPermissionsAssignment> GetListOfPermissions(int type, int typeId, int permissionId)
        {
            try
            {
                using (DBContextHelperLinq db = new DBContextHelperLinq())
                {

                    List<DbPermissionsAssignment> listDbPermission =
                        db.DbPermissionsAssignments.AsNoTracking()
                            .Where(m => m.Type == type && m.Type_Id == typeId && m.Permission_Id == permissionId)
                            .ToList();
                    //List<DbPermissionsAssignment> listDbPermission = db.DbPermissionsAssignments.Select(n => n).ToList();
                    return listDbPermission;
                }

            }
            catch (Exception ex)
            {
                throw;
            }
        }



        public static List<DbPermissionsAssignment> GetListOfPermissions(int type, List<int?> listTypeId, int permissionId)
        {
            try
            {
                using (DBContextHelperLinq db = new DBContextHelperLinq())
                {

                    List<DbPermissionsAssignment> listDbPermission =
                        db.DbPermissionsAssignments.AsNoTracking()
                            .Where(m => m.Type == type && listTypeId.Contains(m.Type_Id) && m.Permission_Id == permissionId)
                            .ToList();
                    //List<DbPermissionsAssignment> listDbPermission = db.DbPermissionsAssignments.Select(n => n).ToList();
                    return listDbPermission;
                }

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public static bool BulkMerge(List<DbPermissionsAssignment> listToMerge, SqlConnection con, int currentRetryCount = 0, int totalRetryCount = Config.FunctionRetryCount)
        {
            try
            {
                //SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["PITB.CMS"].ConnectionString);
                //con.Open();
                BulkOperation<DbPermissionsAssignment> bulkOp = new BulkOperation<DbPermissionsAssignment>(con);
                bulkOp.BatchSize = 1000;
                bulkOp.ColumnInputExpression = c => new
                {
                    c.Type,
                    c.Type_Id,
                    c.Permission_Id,
                    c.Permission_Value
                };
                bulkOp.DestinationTableName = "PITB.Permissions_Assignment";
                bulkOp.ColumnOutputExpression = c => c.Id;
                bulkOp.ColumnPrimaryKeyExpression = c => c.Id;
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                bulkOp.BulkMerge(listToMerge);
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                if (currentRetryCount != totalRetryCount)
                {
                    BulkMerge(listToMerge, con, currentRetryCount + 1, totalRetryCount);
                }
            }
            return false;
        }


    }
}
