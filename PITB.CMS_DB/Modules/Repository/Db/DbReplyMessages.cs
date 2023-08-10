using System.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace PITB.CMS_DB.Models
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
}
