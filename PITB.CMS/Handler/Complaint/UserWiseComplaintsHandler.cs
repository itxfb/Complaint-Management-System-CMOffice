using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;
using PITB.CMS.Models.DB;

namespace PITB.CMS.Handler.Complaint
{
    public class UserWiseComplaintsHandler
    {
        public static void SyncUserWiseComplaints()
        {
            
        }

        public static List<DbUserWiseComplaints> GetUserWiseComplaints(DbUsers dbUser, List<DbComplaint> listComplaints, int campaignId, Config.ComplaintType complaintType, Config.StakeholderComplaintListingType stakeholderComplaintListingType )
        {
            List<DbUserWiseComplaints> listDbUserWiseComplaints = new List<DbUserWiseComplaints>();
            DbUserWiseComplaints dbUserWiseComplaints = null;
            foreach (DbComplaint dbComplaint in listComplaints)
            {
                dbUserWiseComplaints = new DbUserWiseComplaints();
                dbUserWiseComplaints.Campaign_Id = campaignId;
                dbUserWiseComplaints.Complaint_Id = dbComplaint.Id;
                dbUserWiseComplaints.User_Id = dbUser.User_Id;
                dbUserWiseComplaints.Complaint_Type = (int)complaintType;
                dbUserWiseComplaints.Complaint_Subtype = (int) stakeholderComplaintListingType;
                listDbUserWiseComplaints.Add(dbUserWiseComplaints);
            }
            return listDbUserWiseComplaints;
        }
    }
}