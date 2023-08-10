using System.Data.Entity;
using AutoMapper.Internal;
using PITB.CMS.Helper.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PITB.CMS.Models.DB;
using PITB.CMS.Models.View;

namespace PITB.CMS.Handler.Business
{
    public class BlNotification
    {
        public static List<DbNotificationLogs> GetUserWiseNotification(int campaignId, int type, List<int> listComplaintIds)
        {
            try
            {
                int userId = Utility.GetUserFromCookie().User_Id;

                DBContextHelperLinq db = new DBContextHelperLinq();

                //List<DbNotificationLogs> listDbNotification = db.DbNotificationLogs.Where(n => n.Campaign_Id == campaignId && n.Type == type).ToList();
                List<DbNotificationLogs> listDbNotification =
                    db.DbNotificationLogs.Where(
                        n => listComplaintIds.Contains((int) n.TypeId) && n.Campaign_Id == campaignId && n.Type == type)
                        .ToList();
                List<Int64> listNotificationRowIds = listDbNotification.Select(n => n.NotificationLogId).ToList();
                List<DbUserWiseNotification> listDbUserWiseNotification =
                    db.DbUserWiseNotification.Where(
                        n =>
                            listNotificationRowIds.Contains((int) n.NotificationLogId) && n.UserId == userId &&
                            n.StatusId == (int) Config.NotificationStatus.DontSend).ToList();
                listDbNotification =
                    listDbNotification.Where(
                        n => !listDbUserWiseNotification.Select(x => x.NotificationLogId).Contains(n.NotificationLogId))
                        .ToList();
                //listDbNotification.Where(n)
                //GetSubtraction(listDbUserWiseNotification, )
                //List<int> listNotificationCount = GetSubtraction(listDbNotification, listComplaintIds);
                //return listDbNotification.Select(n=>(int)n.NotificationLogId).ToList();
                return listDbNotification.ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        /*
        public static List<int> GetSubtraction(List<DbNotificationLogs> listDbNotificationLogs,
            List<int> listComplaintIds)
        {
            //List<int> listComplaintIds = listComplaints.Select(n => n.ComplaintId).ToList();
            List<Int64?> listUserWiseComplaintIds =
                listDbNotificationLogs.Select(n => n.DbUserWiseNotification.NotificationLogId).ToList();

            listComplaintIds = listComplaintIds.Where(n => !listUserWiseComplaintIds.Contains((Int64) n)).ToList();

            return listComplaintIds;
        }*/

        public static void SetUserNotifictionStatus(int campaignId, Config.NotificationType type,
            Config.NotificationSubType subType, int typeId, Config.NotificationStatus status)
        {
            try
            {
                DBContextHelperLinq db = new DBContextHelperLinq();
                DbNotificationLogs dbNotification =
                    db.DbNotificationLogs.AsNoTracking()
                        .FirstOrDefault(
                            n =>
                                n.Campaign_Id == campaignId && n.Type == (int) type && n.SubType == (int) subType &&
                                n.TypeId == typeId);

                DbUserWiseNotification dbUserWiseNotification = new DbUserWiseNotification();
                dbUserWiseNotification.NotificationLogId = Convert.ToInt64(dbNotification.NotificationLogId);
                dbUserWiseNotification.CreatedDateTime = DateTime.Now;
                dbUserWiseNotification.UserId = Utility.GetUserFromCookie().User_Id;
                dbUserWiseNotification.StatusId = (int) Config.NotificationStatus.DontSend;


                db.DbUserWiseNotification.Add(dbUserWiseNotification);
                db.SaveChanges();
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


        public static void ResetNotification(int campaignId, Config.NotificationType type,
            Config.NotificationSubType subType, int typeId, Config.NotificationStatus status)
        {
            try
            {
                DBContextHelperLinq db = new DBContextHelperLinq();
                DbNotificationLogs dbNotification =
                    db.DbNotificationLogs.AsNoTracking()
                        .FirstOrDefault(
                            n =>
                                n.Campaign_Id == campaignId && n.Type == (int)type && n.SubType == (int)subType &&
                                n.TypeId == typeId);

                List<DbUserWiseNotification> listDbUserWiseNotification = DbUserWiseNotification.GetUserWiseNotification(dbNotification.NotificationLogId, db);

                foreach (DbUserWiseNotification dbUserWiseNotification in listDbUserWiseNotification)
                {
                    db.Entry(dbUserWiseNotification).State = EntityState.Deleted;
                    
                }
                db.SaveChanges();
                //DbUserWiseNotification dbUserWiseNotification = new DbUserWiseNotification();
                //dbUserWiseNotification.NotificationLogId = Convert.ToInt64(dbNotification.NotificationLogId);
                //dbUserWiseNotification.CreatedDateTime = DateTime.Now;
                //dbUserWiseNotification.UserId = Utility.GetUserFromCookie().User_Id;
                //dbUserWiseNotification.StatusId = (int)Config.NotificationStatus.DontSend;


                //db.DbUserWiseNotification.Add(dbUserWiseNotification);
                //db.SaveChanges();
              
            }
            catch (Exception ex)
            {
                return;
            }
        }
    }

}