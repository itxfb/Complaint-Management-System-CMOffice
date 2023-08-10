using PITB.CMS_Common;
using PITB.CMS_Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;


namespace PITB.CMS_WindowService
{
    public class HelperFunctions
    {
        public static List<int> GetUniqueCampaignIds(List<DbPermissionsAssignment> listPermission)
        {
            List<int> listUniqueCampIds = new List<int>();
            foreach (DbPermissionsAssignment dbPermissionsAssignment in listPermission)
            {
                listUniqueCampIds = listUniqueCampIds.Union(Utility.GetIntList(dbPermissionsAssignment.Permission_Value)).ToList();
            }
            return listUniqueCampIds;
        }

        public static List<StatusWiseComplaintCount> GetStatusWiseComplaintCount(List<ComplaintPartial> listComplaintPartial, List<int> listStatuses)
        {
            List<StatusWiseComplaintCount> listStatusWiseComplaint = new List<StatusWiseComplaintCount>();
            StatusWiseComplaintCount statusWiseComplaintCount = new StatusWiseComplaintCount();

            foreach (int statusId in listStatuses)
            {
                statusWiseComplaintCount = new StatusWiseComplaintCount
                {
                    StatusId = statusId,
                    StatusName = Config.StatusDict[statusId],
                    ComplaintCount = 0
                };
                
                statusWiseComplaintCount.ComplaintCount = listComplaintPartial.Where(n => n.StatusId == statusId).Count();
                
                listStatusWiseComplaint.Add(statusWiseComplaintCount);
            }

            return listStatusWiseComplaint;
        }
    }
}
