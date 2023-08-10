using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PITB.CMS.Models.DB;

namespace PITB.CMS.Handler.Business
{
    public class BlUserCategory
    {
        public static List<DbUserCategory> GetSchoolEducationUserCategoryList(int? userId, int? parent, int? child, int? catHierarchy)
        {
            List<DbUserCategory> listPmiuUserCat = new List<DbUserCategory>();
            listPmiuUserCat.Add(new DbUserCategory
            {
                User_Id = userId,
                Parent_Category_Id = parent,
                Child_Category_Id = child,
                Category_Hierarchy = 1
            });

            if (parent != null)
            {
                if (parent == (int) Config.SchoolType.Elementary)
                {
                    listPmiuUserCat.Add(new DbUserCategory
                    {
                        User_Id = userId,
                        Parent_Category_Id = (int)Config.SchoolType.Primary,
                        Child_Category_Id = child,
                        Category_Hierarchy = 1
                    });
                }
                else if (parent == (int) Config.SchoolType.Primary)
                {
                    listPmiuUserCat.Add(new DbUserCategory
                    {
                        User_Id = userId,
                        Parent_Category_Id = (int)Config.SchoolType.Elementary,
                        Child_Category_Id = child,
                        Category_Hierarchy = 1
                    });
                }
            }
            return listPmiuUserCat;
        }
    }
}