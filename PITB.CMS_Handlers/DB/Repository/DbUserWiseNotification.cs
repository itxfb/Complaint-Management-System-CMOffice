using System.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;
using PITB.CMS_Models.DB;

namespace PITB.CMS_Handlers.DB.Repository
{

    public class RepoDbUserWiseNotification
    {
        public static List<DbUserWiseNotification> GetUserWiseNotification(Int64 refId, DBContextHelperLinq dbContext)
        {
            return dbContext.DbUserWiseNotification.Where(n => n.NotificationLogId == refId).ToList();
        }
    }
}