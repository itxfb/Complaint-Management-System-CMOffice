using PITB.CMS_Models.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PITB.CMS_Handlers.Custom
{
    public class Notification
    {
        public int Count { get; set; }
        public List<DbNotificationLogs> ListNotification { get; set; }
    }
}