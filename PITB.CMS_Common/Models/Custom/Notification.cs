using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PITB.CMS_Common.Models.Custom
{
    public class Notification
    {
        public int Count { get; set; }
        public List<DbNotificationLogs> ListNotification { get; set; }
    }
}