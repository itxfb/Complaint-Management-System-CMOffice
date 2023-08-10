using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PITB.CMS_Common.Models
{
    public partial class UserCategory
    {
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
