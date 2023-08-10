using System.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace PITB.CMS_Common.Models
{

    public partial class DbReplyMessages
    {
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

    #region From API
    public partial class DbReplyMessages
    {
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
    #endregion
}
