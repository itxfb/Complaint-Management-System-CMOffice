using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PITB.CMS_DB.Models
{
    [Table("PITB.User_Wise_Notification")]
    public partial class DbUserWiseNotification
    {
        [Key]
        public Int64 Id { get; set; }

        [Column("RefId")]
        public Int64? NotificationLogId { get; set; }

        public DateTime CreatedDateTime { get; set; }

        public int? UserId { get; set; }

        public int? StatusId { get; set; }
    }
}