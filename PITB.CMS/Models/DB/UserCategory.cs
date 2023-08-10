using System.Linq;
using PITB.CMS.Helper.Database;

namespace PITB.CMS.Models.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("PITB.UserCategory2")]
    public partial class UserCategory
    {
        public int Id { get; set; }

        public int? User_Id { get; set; }

        public int? Parent_Category_Id { get; set; }

        public int? Child_Category_Id { get; set; }

        public int? Category_Hierarchy { get; set; }

        public static List<UserCategory> GetCategories(int userId)
        {
            try
            {
                DBContextHelperLinq db = new DBContextHelperLinq();
                //db.DbUserCategory.
                //return null;
                return db.UserCategory.Where(n => n.User_Id == userId).ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
