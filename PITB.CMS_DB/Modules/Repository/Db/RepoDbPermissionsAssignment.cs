using PITB.CMS_DB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Z.BulkOperations;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using PITB.CMS_Common;
using System.Data;

namespace PITB.CMS_DB.Modules.Repository.Db
{
    public class RepoDbPermissionsAssignment : GenericRepository<DbPermissionsAssignment>
    {
        public RepoDbPermissionsAssignment(CMSDbContext dbContext) : base(dbContext)
        {

        }

        public List<DbPermissionsAssignment> GetListOfPermissions(int type, int typeId)
        {
            return Get(where: m => m.Type == type && typeId == m.Type_Id).ToList();
        }

        public List<DbPermissionsAssignment> GetListOfPermissionsByTypeAndPermissionId(int type, int permissonId)
        {
            return Get(where: m => m.Type == type && permissonId == m.Permission_Id).ToList();
        }

        public List<DbPermissionsAssignment> GetListOfPermissionsByTypeTypeIdAndPermissionId(int type, int typeId, int permissonId, bool asNoTracking = true)
        {
            return Get(where: m => m.Type == type && m.Type_Id == typeId && permissonId == m.Permission_Id, orderBy:null, asNoTracking, null).ToList();
        }

        public List<DbPermissionsAssignment> GetListOfPermissionsByTypeTypeIdListAndPermissionId(int type, List<int> listTypeId, int permissonId)
        {
            return Get(where: m => m.Type == type && listTypeId.Any(s => s == m.Type_Id) && permissonId == m.Permission_Id).ToList();
        }

        public List<DbPermissionsAssignment> GetListOfPermissions(int type, List<int?> listTypeIds)
        {
            return Get(where: m => m.Type == type && listTypeIds.Contains(m.Type_Id)).ToList();
        }
        public bool BulkMerge(List<DbPermissionsAssignment> listToMerge, SqlConnection con, int currentRetryCount = 0, int totalRetryCount = Config.FunctionRetryCount)
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
