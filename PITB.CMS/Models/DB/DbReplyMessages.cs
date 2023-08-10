using System.Linq;

namespace PITB.CMS.Models.DB
{
    using PITB.CMS.Helper.Database;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

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

        public static List<DbReplyMessages> GetMessagesByPhoneNo(string phoneNo)
        {
            try
            {
                DBContextHelperLinq db = new DBContextHelperLinq();
                return db.DbReplyMessages.Where(n => n.Caller_No == phoneNo).OrderBy(n => n.Created_DateTime).ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
