using System.Linq;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Information;
using PITB.CMS.Models.View;

namespace PITB.CMS.Models.DB
{
    using PITB.CMS.Helper.Database;
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

        public DateTime? Created_DateTime { get; set; }

        public DateTime? Message_Created_DateTime { get; set; }

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

        public static List<DbIncomingMessages> GetMessagesByPhoneNo(string phoneNo)
        {
            try
            {
                DBContextHelperLinq db = new DBContextHelperLinq();
                return db.DbIncomingMessages.Where(n => n.Caller_No == phoneNo).OrderBy(n => n.Message_Created_DateTime).ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static List<DbIncomingMessages> GetAllMessagesGroupByPhoneNoOrderByDateTime()
        {
            try
            {
                DBContextHelperLinq db = new DBContextHelperLinq();
                return db.DbIncomingMessages.OrderByDescending(n => n.Message_Created_DateTime).ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static List<DbIncomingMessages> GetByIncomingMessageIdsList(List<int> listIncomingMessageIds )
        {
            try
            {
                DBContextHelperLinq db = new DBContextHelperLinq();
                return db.DbIncomingMessages.Where(n => listIncomingMessageIds.Contains(n.Id)).OrderBy(n => n.Message_Created_DateTime).ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static List<string> GetUniquePhoneNoList()
        {
            try
            {
                DBContextHelperLinq db = new DBContextHelperLinq();
                return db.DbIncomingMessages.GroupBy(n=>n.Caller_No).Select(n=>n.Key).ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

    }
}
