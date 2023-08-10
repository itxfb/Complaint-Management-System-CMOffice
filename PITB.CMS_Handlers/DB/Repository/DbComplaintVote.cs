using PITB.CMS_Models.DB;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PITB.CMS_Handlers.DB.Repository
{
    public class RepoDbComplaintVote
    {
        #region Helpers

        public static List<DbComplaintVote> GetVotesByComplaintId(int complaintId)
        {
            using (DBContextHelperLinq db = new DBContextHelperLinq())
            {
                return db.DbComplaintVotes.AsNoTracking().Where(m => m.Complaint_ID == complaintId).ToList();
            }
        }
        #endregion
    }
}