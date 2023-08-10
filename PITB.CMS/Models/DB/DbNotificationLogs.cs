using System.Linq;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Information;
using OfficeOpenXml.FormulaParsing.Excel.Functions.RefAndLookup;
using PITB.CMS.Models.View;


namespace PITB.CMS.Models.DB
{
    using PITB.CMS.Helper.Database;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("PITB.Notification_Logs")]
    public class DbNotificationLogs
    {
        [Key]
        [Column("Id")]
        public Int64 NotificationLogId { get; set; }

        public int? Campaign_Id { get; set; }

        public int? Type { get; set; }

        public int? TypeId { get; set; }

        public int? SubType { get; set; }

        public int? SubTypeId { get; set; }

        public string Description { get; set; }

        public DateTime? Created_DateTime { get; set; }

        public DateTime? Updated_DateTime { get; set; }

        public int? Status { get; set; }


        [ForeignKey("NotificationLogId")]
        public virtual DbUserWiseNotification DbUserWiseNotification { get; set; }


        public static List<DbNotificationLogs> Get(int campaignId, int type, int status)
        {
            try
            {
                DBContextHelperLinq db = new DBContextHelperLinq();
                return db.DbNotificationLogs.Where(n => n.Campaign_Id == campaignId && n.Type == type && n.Status == status).ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static List<DbNotificationLogs> Get(int campaignId, int type)
        {
            try
            {
                DBContextHelperLinq db = new DBContextHelperLinq();
                return db.DbNotificationLogs.Where(n => n.Campaign_Id == campaignId && n.Type == type).ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static void SetNotifictionStatus(int campaignId, Config.NotificationType type, Config.NotificationSubType subType, int typeId,  Config.NotificationStatus status)
        {
            try
            {
                DBContextHelperLinq db = new DBContextHelperLinq();
                DbNotificationLogs dbNotification = db.DbNotificationLogs.FirstOrDefault(n => n.Campaign_Id == campaignId && n.Type == (int)type && n.SubType==(int)subType && n.TypeId==typeId);
                
                //dbNotification
                /*if (dbNotification != null && dbNotification.Status != (int) status)
                {
                    dbNotification.Status = (int) status;
                    db.DbNotificationLogs.Attach(dbNotification);
                    db.Entry(dbNotification).Property(n => n.Status).IsModified = true;
                    db.SaveChanges();
                }*/
            }
            catch (Exception ex)
            {
                return ;
            }
        }

    }
}