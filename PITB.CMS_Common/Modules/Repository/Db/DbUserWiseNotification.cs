using System.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace PITB.CMS_Common.Models
{

    public partial class DbUserWiseNotification
    {
        public static List<DbUserWiseNotification> GetUserWiseNotification(Int64 refId, DBContextHelperLinq dbContext)
        {
            return dbContext.DbUserWiseNotification.Where(n => n.NotificationLogId == refId).ToList();
        }
    }
}