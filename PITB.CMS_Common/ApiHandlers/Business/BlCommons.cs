using PITB.CMS_Common.ApiModels.API;
using PITB.CMS_Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PITB.CMS_Common.ApiHandlers.Business
{
    public class BlCommons
    {
        public static dynamic GetPersonInfo(int personId)
        {
            DbPersonInformation dbPersonIfo = DbPersonInformation.GetPersonInformationByPersonId(personId);
            return dbPersonIfo;
        }

        

        public static StatusHistory PopulateStatusHistory(int complaintId)
        {
            List<DbComplaintStatusChangeLog> listDbStatusChangeLog= DbComplaintStatusChangeLog.GetStatusChangeLogsAgainstComplaintId(complaintId);
            StatusHistory statusHistory = new StatusHistory();
            StatusHistoryModel statusHistoryModel = null;
            int i = 0;
            foreach (DbComplaintStatusChangeLog dbStatusChangeLog in listDbStatusChangeLog)
            {
                statusHistoryModel = new StatusHistoryModel();
                statusHistoryModel.Id = dbStatusChangeLog.Id;
                statusHistoryModel.StatusChangedByPersonId = dbStatusChangeLog.ChangedBy.Id;
                statusHistoryModel.StatusChangedByPersonName = dbStatusChangeLog.ChangedBy.Name;
                statusHistoryModel.StatusChangedByPersonDesignationAbbr = dbStatusChangeLog.ChangedBy.Designation_abbr;
                statusHistoryModel.StatusChangedByPersonDesignationName = dbStatusChangeLog.ChangedBy.Designation;
                statusHistoryModel.StatusId = dbStatusChangeLog.StatusId;
                statusHistoryModel.StatusName = dbStatusChangeLog.DbStatus.Status;
                statusHistoryModel.StatusComment = dbStatusChangeLog.Comments;
                statusHistoryModel.StatusChangeDateTime = dbStatusChangeLog.StatusChangeDateTime;
                DbAttachments.GetByRefAndComplaintId(complaintId, Config.AttachmentReferenceType.ChangeStatus,
                    statusHistoryModel.Id);
                statusHistoryModel.ListAttachment = DbAttachments.GetByRefAndComplaintId(complaintId, Config.AttachmentReferenceType.ChangeStatus,
                    statusHistoryModel.Id);//dbStatusChangeLog.ListDbAttachments;
                statusHistoryModel.OrderId = i;
                statusHistory.ListStatusHistoryModel.Add(statusHistoryModel);
                i++;
            }
            return statusHistory;
        }
    }
}