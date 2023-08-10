using System.Linq;
using PITB.CRM_API.Helper.Database;

namespace PITB.CRM_API.Models.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("PITB.Incoming_Messages")]
    public partial class DbIncomingMessages
    {
        [Key]
        public int Id { get; set; }

        [StringLength(50)]
        public string Caller_No { get; set; }

        [StringLength(1000)]
        public string Msg_Text { get; set; }

        public int? Campaign_Id { get; set; }

        public int? Src_Id { get; set; }

        public byte? Status { get; set; }

        public DateTime? Message_Created_DateTime { get; set; }

        public DateTime? Created_DateTime { get; set; }

        public static DbIncomingMessages GetByMessageId(int id)
        {
            try
            {
                DBContextHelperLinq db = new DBContextHelperLinq();
                return db.DbIncomingMessages.Where(n => n.Id == id).FirstOrDefault();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

    }
}
