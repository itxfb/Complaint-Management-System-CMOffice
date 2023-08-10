using System.Collections.Generic;
using System.Web.Razor.Generator;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System;
using PITB.CMS_Models.DB;

namespace PITB.CMS_Handlers.DB.Repository
{

    public class RepoDbPermissions
    {

        public static DbPermissions GetPermissionsByPermissionAndType(int permissionId, int permissionType)
        {
            try
            {
                using (DBContextHelperLinq db = new DBContextHelperLinq())
                {
                    DbPermissions dbPermission = db.DbPermissions.AsNoTracking().Where(m => m.Permission_Id == permissionId && m.Permission_Type == permissionType).ToList().FirstOrDefault();
                    return dbPermission;
                }


            }
            catch (Exception ex)
            {
                throw;
            }
        }


    }


}
