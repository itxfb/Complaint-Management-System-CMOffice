using System.Linq;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Information;
using PITB.CMS.Models.View;


namespace PITB.CMS.Models.DB
{
    using PITB.CMS.Helper.Database;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("PITB.User_Wise_Notification")]
    public class DbUserWiseNotification
    {
        [Key]
        public Int64 Id { get; set; }
        
        [Column("RefId")]
        public Int64? NotificationLogId { get; set; }

        public DateTime CreatedDateTime { get; set; }

        public int? UserId { get; set; }

        public int? StatusId { get; set; }

        public static List<DbUserWiseNotification> GetUserWiseNotification(Int64 refId, DBContextHelperLinq dbContext)
        {
            return dbContext.DbUserWiseNotification.Where(n => n.NotificationLogId == refId).ToList();
        }
    }
}