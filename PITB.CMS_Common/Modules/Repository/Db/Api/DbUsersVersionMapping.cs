using System.Data.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace PITB.CMS_Common.Models
{
    public partial class DbUsersVersionMapping
    {

        #region HelperMethods
        public static int Update_AddVersion(Config.UserType userType, int userId, int platformId, int appId, int appVersion)
        {
            try
            {
                DBContextHelperLinq db = new DBContextHelperLinq();
                List<DbUsersVersionMapping> listVersionMapping = db.DbUsersVersionMapping.Where(n => n.User_Type == (int)userType && n.User_Id == userId && n.Platform_Id == platformId && n.App_Id == appId).ToList();
                if (listVersionMapping.Count > 0) // if present then update
                {
                    DbUsersVersionMapping dbUserVersionMapping = listVersionMapping.First();
                    if (dbUserVersionMapping.App_Version != appVersion)
                    {
                        dbUserVersionMapping.App_Version = appVersion;
                        db.Entry(dbUserVersionMapping).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
                else
                {
                    //DbUsersVersionMapping dbUserVersionMapping = listVersionMapping.First();
                    DbUsersVersionMapping dbUserVersionMapping = new DbUsersVersionMapping();
                    dbUserVersionMapping.User_Type = (int)userType;
                    dbUserVersionMapping.User_Id = userId;
                    dbUserVersionMapping.Platform_Id = platformId;
                    dbUserVersionMapping.App_Id = appId;
                    dbUserVersionMapping.App_Version = appVersion;

                    db.DbUsersVersionMapping.Add(dbUserVersionMapping);
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                return -1;
            }
            return 1;
        }

        #endregion
    }
}