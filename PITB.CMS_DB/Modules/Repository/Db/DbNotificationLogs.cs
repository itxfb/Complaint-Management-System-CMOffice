using System.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;
using PITB.CMS_Common;

namespace PITB.CMS_DB.Models
{

    public partial class DbNotificationLogs
    {
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

        public static void SetNotifictionStatus(int campaignId, Config.NotificationType type, Config.NotificationSubType subType, int typeId, Config.NotificationStatus status)
        {
            try
            {
                DBContextHelperLinq db = new DBContextHelperLinq();
                DbNotificationLogs dbNotification = db.DbNotificationLogs.FirstOrDefault(n => n.Campaign_Id == campaignId && n.Type == (int)type && n.SubType == (int)subType && n.TypeId == typeId);

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
                return;
            }
        }

    }
}