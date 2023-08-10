using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PITB.CMS_Common.Models
{

    [Table("PITB.Reply_Messages")]
    public partial class DbReplyMessages
    {
        [Key]
        public int Id { get; set; }

        public int? Campaign_Id { get; set; }

        public string Caller_No { get; set; }

        public int? Incoming_Message_Id { get; set; }

        [StringLength(1000)]
        public string Msg_Text { get; set; }

        public byte? Status { get; set; }

        public DateTime? Created_DateTime { get; set; }

        public int? Group_Id { get; set; }
    }
}
