using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PITB.CMS_DB.Models
{

    [Table("PITB.Notification_Logs")]
    public partial class DbNotificationLogs
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
    }
}