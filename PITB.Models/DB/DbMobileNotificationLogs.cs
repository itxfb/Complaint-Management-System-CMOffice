using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PITB.CMS_Models.DB
{

    [Table("PITB.Mobile_Notification_Logs")]
    public class DbMobileNotificationLogs
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
    }
}
