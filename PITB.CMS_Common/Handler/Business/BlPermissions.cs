using System.Data.Entity;
using PITB.CMS_Common.Handler.Authentication;
using PITB.CMS_Common.Helper.Database;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PITB.CMS_Common.Models;

namespace PITB.CMS_Common.Handler.Business
{
    public class BlPermissions
    {
        public static void DeleteAndAddPermissionFromDb(DBContextHelperLinq db, Config.PermissionsType permissionType, Config.Permissions permissionId, int userId)
        {
            DeletePersonInformation(db, permissionType, permissionId, userId);
            /*
            List<DbPermissionsAssignment> listPermissionAssignment = DbPermissionsAssignment.GetListOfPermissions(db, (int)permissionType, userId, (int)permissionId);

            if (listPermissionAssignment.Count > 0)
            {
                foreach (DbPermissionsAssignment permissionAssignment in listPermissionAssignment)
                {
                    db.DbPermissionsAssignments.Remove(permissionAssignment);
                }

                //db.SaveChanges();
            }
            */
 

            DbPermissionsAssignment dbPermissionsAssignment = new DbPermissionsAssignment();
            dbPermissionsAssignment.Type = (int) permissionType;
            dbPermissionsAssignment.Type_Id = userId;
            dbPermissionsAssignment.Permission_Id = (int) permissionId;
            db.DbPermissionsAssignments.Add(dbPermissionsAssignment);

            //db.SaveChanges();
        }

        public static void DeletePersonInformation(DBContextHelperLinq db, Config.PermissionsType permissionType, Config.Permissions permissionId, int userId)
        {
            List<DbPermissionsAssignment> listPermissionAssignment = DbPermissionsAssignment.GetListOfPermissions(db, (int)permissionType, userId, (int)permissionId);

            if (listPermissionAssignment.Count > 0)
            {
                //db.DbPermissionsAssignments.RemoveRange(listPermissionAssignment);

               

                //   db.Entry(permissionAssignment).State = EntityState.Deleted;

                foreach (DbPermissionsAssignment permissionAssignment in listPermissionAssignment)
                {
                    db.DbPermissionsAssignments.Attach(permissionAssignment);
                    db.Entry(permissionAssignment).State = EntityState.Deleted;
                }

                //db.SaveChanges();
            }
        }
    }
}