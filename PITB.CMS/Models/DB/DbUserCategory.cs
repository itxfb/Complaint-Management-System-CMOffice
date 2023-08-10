using System.Data.SqlClient;
using System.Linq;
using Z.BulkOperations;

namespace PITB.CMS.Models.DB
{
    using PITB.CMS.Helper.Database;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data;
    using System.Data.Entity.Spatial;

    [Table("PITB.UserCategory")]
    public partial class DbUserCategory
    {
        public int Id { get; set; }

        //[ForeignKey("DbComplaint")]
        public int? User_Id { get; set; }

        public int? Parent_Category_Id { get; set; }

        public int? Child_Category_Id { get; set; }

        public int? Category_Hierarchy { get; set; }

        //public DbUsers DbUsers { get; set; }

        #region HelperMethods


        public static List<DbUserCategory> GetEmptyParentChildList()
        {
            List<DbUserCategory> listDbUserCategory = new List<DbUserCategory>();
            DbUserCategory dbUserCat = new DbUserCategory();
            dbUserCat.Parent_Category_Id = null;
            dbUserCat.Child_Category_Id = null;
            dbUserCat.Category_Hierarchy = 1;
            listDbUserCategory.Add(dbUserCat);
            return listDbUserCategory;
        }

        public static List<DbUserCategory> GetCategories(int userId)
        {
            try
            {
                DBContextHelperLinq db = new DBContextHelperLinq();
                //db.DbUserCategory.
                //return null;
                return db.DbUserCategory.Where(n=>n.User_Id==userId).ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static List<DbUserCategory> GetuserCategoryList(int? userHierarchyId1, int? userHierarchyId2)
        {
            List<DbUserCategory> listUserCat = new List<DbUserCategory>();
            DbUserCategory dbUserCategory = new DbUserCategory();
            dbUserCategory.Parent_Category_Id = userHierarchyId1;
            dbUserCategory.Child_Category_Id = userHierarchyId2;
            dbUserCategory.Category_Hierarchy = 1;
            listUserCat.Add(dbUserCategory);
            return listUserCat;
        }

        public static bool BulkMerge(List<DbUserCategory> listToMerge, SqlConnection con, int currentRetryCount = 0, int totalRetryCount = Config.FunctionRetryCount)
        {
            try
            {
                BulkOperation<DbUserCategory> bulkOp = new BulkOperation<DbUserCategory>(con);
                bulkOp.BatchSize = 1000;
                bulkOp.ColumnInputExpression = c => new
                {
                    c.User_Id,
                    c.Parent_Category_Id,
                    c.Child_Category_Id,
                    c.Category_Hierarchy
                };
                bulkOp.DestinationTableName = "PITB.UserCategory";
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

        #endregion
    }
}