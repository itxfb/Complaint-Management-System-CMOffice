﻿using System.Linq;
using PITB.CRM_API.Helper.Database;
namespace PITB.CRM_API.Models.DB
{
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

        public static List<DbReplyMessages> GetTopMsgs(DBContextHelperLinq db, int campaignId, int msgStatus, int recordCount)
        {
            try
            {
                //DBContextHelperLinq db = new DBContextHelperLinq();
                return db.DbReplyMessages.Where(n => n.Campaign_Id == campaignId && n.Status == msgStatus).Take(recordCount).ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static List<DbReplyMessages> GetMessageWithGroupAndCampaign(DBContextHelperLinq db, int groupId)
        {
            try
            {
                //DBContextHelperLinq db = new DBContextHelperLinq();
                return db.DbReplyMessages.Where(n => n.Group_Id == groupId).ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
