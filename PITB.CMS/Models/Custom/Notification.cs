using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PITB.CMS.Models.DB;

namespace PITB.CMS.Models.Custom
{
    public class Notification
    {
        public int Count { get; set; }
        public List<DbNotificationLogs> ListNotification { get; set; }
    }
}