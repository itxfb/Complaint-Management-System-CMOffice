using PITB.CMS_Models.Custom;
using PITB.CMS_Models.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PITB.CMS_Handlers.Custom
{
    public class UserCategoryModelHandler
    {
        public static List<UserCategoryModel> GetListUserCategoryModel(List<DbUserCategory> listDbUserCategory)
        {
            List<UserCategoryModel> listUserCategoryModel = new List<UserCategoryModel>();
            UserCategoryModel userCategoryModel = null;

            foreach (DbUserCategory dbUserCategory in listDbUserCategory)
            {
                userCategoryModel = new UserCategoryModel();
                userCategoryModel.Id = dbUserCategory.Id;
                userCategoryModel.User_Id = dbUserCategory.User_Id;
                userCategoryModel.Parent_Category_Id = dbUserCategory.Parent_Category_Id;
                userCategoryModel.Child_Category_Id = dbUserCategory.Child_Category_Id;
                userCategoryModel.Category_Hierarchy = dbUserCategory.Category_Hierarchy;
                listUserCategoryModel.Add(userCategoryModel);
            }

            return listUserCategoryModel;
        }

        public static bool AreAllCategoriesNull(List<UserCategoryModel> listUserCategory)
        {
            bool areNull = true;
            if (listUserCategory != null)
            {
                if (listUserCategory.Count == 0)
                {
                    return true;
                }
                else
                {
                    foreach (UserCategoryModel userCategory in listUserCategory)
                    {
                        if (userCategory.Parent_Category_Id == null && userCategory.Child_Category_Id == null)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
    }
}