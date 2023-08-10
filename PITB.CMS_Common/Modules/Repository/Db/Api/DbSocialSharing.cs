using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;


namespace PITB.CMS_Common.Models
{
    public partial class DbSocialSharing
    {
        public static DbSocialSharing GetByComplaintId(int complaintId)
        {
            using (DBContextHelperLinq db = new DBContextHelperLinq())
            {
                return db.DbSocialSharings.FirstOrDefault(m => m.Complaint_Id == complaintId);
            }
        }
    }
}