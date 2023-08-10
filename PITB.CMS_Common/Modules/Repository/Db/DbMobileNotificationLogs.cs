using System.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace PITB.CMS_Common.Models
{
    public partial class DbMobileNotificationLogs
    {

        public static List<DbMobileNotificationLogs> Get(int userId, string tagId, string mobileDeviceId)
        {
            try
            {
                DBContextHelperLinq db = new DBContextHelperLinq();
                List<DbMobileNotificationLogs> listStatusChangeLogs =
                    db.DbMobileNotificationLogs.AsNoTracking().Where(n => n.User_Id == userId && n.Tag_Id == tagId && n.Mobile_Device_Id == mobileDeviceId).ToList();//.OrderBy(n => n.StatusChangeDateTime).ToList();
                return listStatusChangeLogs;
                //return Db.Complaints.OrderByDescending(m=>m.Created_Date).Skip(pageIndex).Take(numberOfComplaints).ToList();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public static List<int> GetNotificationsList(List<int> listNotication, int userId, string tagId, string mobileDeviceId)
        {
            try
            {
                DBContextHelperLinq db = new DBContextHelperLinq();
                List<DbMobileNotificationLogs> listStatusChangeLogs =
                    db.DbMobileNotificationLogs.AsNoTracking().Where(n => /*listNotication.Contains(n.Notification_Id) &&*/ n.User_Id == userId && n.Tag_Id == tagId && n.Mobile_Device_Id == mobileDeviceId).ToList();//.OrderBy(n => n.StatusChangeDateTime).ToList();
                List<int> logsToSubtract = listStatusChangeLogs.Select(n => n.Notification_Id).ToList();
                List<int> listNotifications = listNotication.Where(n => !logsToSubtract.Contains(n)).ToList();
                //listStatusChangeLogs.Where(n=>!listNotication.Contains(n.Notification_Id))

                return listNotifications;
                //return Db.Complaints.OrderByDescending(m=>m.Created_Date).Skip(pageIndex).Take(numberOfComplaints).ToList();
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
