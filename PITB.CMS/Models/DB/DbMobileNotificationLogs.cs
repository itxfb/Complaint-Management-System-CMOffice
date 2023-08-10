using System.Linq;

namespace PITB.CMS.Models.DB
{
    using PITB.CMS.Helper.Database;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("PITB.Mobile_Notification_Logs")]
    public partial class DbMobileNotificationLogs
    {
        public long Id { get; set; }

        public int? Campaign_Id { get; set; }

        public int? User_Id { get; set; }

        [StringLength(500)]
        public string Tag_Id { get; set; }

        [StringLength(1000)]
        public string Mobile_Device_Id { get; set; }

        public int Notification_Id { get; set; }

        public string Description { get; set; }

        public DateTime? Created_DateTime { get; set; }

        public DateTime? Updated_DateTime { get; set; }

        public int? Status { get; set; }

        public static List<DbMobileNotificationLogs> Get(int userId, string tagId, string mobileDeviceId)
        {
            try
            {
                DBContextHelperLinq db = new DBContextHelperLinq();
                List<DbMobileNotificationLogs> listStatusChangeLogs =
                    db.DbMobileNotificationLogs.AsNoTracking().Where(n => n.User_Id == userId && n.Tag_Id==tagId && n.Mobile_Device_Id==mobileDeviceId).ToList();//.OrderBy(n => n.StatusChangeDateTime).ToList();
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
