using PITB.CMS_Common.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PITB.CMS_Handlers.Business
{
    public class BlStatus
    {
        public static List<DbStatus> GetFilteredStatusList(List<DbStatus> listDbStatus, int statusToRemove)
        {
            //SelectListItem item = null;
            //List<SelectListItem> listStatus = new List<SelectListItem>();

            listDbStatus.RemoveAll(n => n.Complaint_Status_Id == statusToRemove || (n.Complaint_Status_Id == Convert.ToInt32(Config.ComplaintStatus.PendingFresh) || n.Complaint_Status_Id == Convert.ToInt32(Config.ComplaintStatus.UnsatisfactoryClosed)));
            return listDbStatus;
            //foreach (DbStatus dbStatus in listDbStatus)
            //{
            //    if (statusToRemove != dbStatus.Complaint_Status_Id && Convert.ToInt32(Config.ComplaintStatus.PendingFresh) != dbStatus.Complaint_Status_Id && Convert.ToInt32(Config.ComplaintStatus.UnsatisfactoryClosed) != dbStatus.Complaint_Status_Id)
            //    {
            //        item = new SelectListItem();
            //        item.Value = dbStatus.Complaint_Status_Id.ToString();
            //        item.Text = dbStatus.Status;
            //        listStatus.Add(item);
            //    }
            //}
            //return listStatus;
        }
    }
}