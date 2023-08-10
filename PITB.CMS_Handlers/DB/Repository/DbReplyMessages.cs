using System.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;
using PITB.CMS_Models.DB;

namespace PITB.CMS_Handlers.DB.Repository
{

    public class RepoDbReplyMessages
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
