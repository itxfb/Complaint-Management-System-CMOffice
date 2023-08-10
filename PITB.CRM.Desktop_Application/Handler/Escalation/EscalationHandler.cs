using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using PITB.CMS.Handler.Complaint;
using PITB.CMS.Models.DB;
using PITB.CRM.Desktop_Application.Models;
using PITB.CMS.Helper.Database;

namespace PITB.CRM.Desktop_Application.Handler.Escalation
{
    public class EscalationHandler
    {
        public List<ComplaintWiseEscalationModel> listComplaintEscalationModel;
        public int handlerStatus = 0;

        public EscalationHandler()
        {
            listComplaintEscalationModel = new List<ComplaintWiseEscalationModel>();
            handlerStatus = 1;
        }
        
        public void StartEscalation()
        {
            PopulateInitialEscalationList();
            handlerStatus = 2;
        }

        public void StopEscalation()
        {
            handlerStatus = 3;
        }

        public void PopulateInitialEscalationList()
        {
            //List<DbComplaint> listDbComplaint = null;
            //using (DBContextHelperLinq db = new DBContextHelperLinq())
            //{
            //     listDbComplaint =  db.DbComplaints.Select(n=>n).ToList();
            //}
            List<DbComplaint> listDbComplaint = DbComplaint.GetByCampaignId(1).ToList();
            //List<ComplaintWiseEscalationModel> listComplaintWiseEscalationModel = new List<ComplaintWiseEscalationModel>();

            listComplaintEscalationModel.Clear();
            int i = 0;
            ComplaintWiseEscalationModel complaintEscalModel = null;
            foreach (DbComplaint dbComplaint in listDbComplaint)
            {
                complaintEscalModel = new ComplaintWiseEscalationModel(dbComplaint);
                SetSchedularConfiguration(complaintEscalModel, dbComplaint);
                listComplaintEscalationModel.Add(complaintEscalModel);
                i++;
            }
        }

        private void SetSchedularConfiguration(ComplaintWiseEscalationModel complaintEscalModel, DbComplaint dbComplaint)
        {
            //complaintWiseEscalationModel.dbComplaint = dbComplaint;
            if (complaintEscalModel.listEscalationFields.Count > 0)
            {
                TimeSpan ts = (TimeSpan) (((DateTime)dbComplaint.Dt1) - ((DateTime)dbComplaint.Created_Date));
                //TimeSpan ts = (TimeSpan)((((DateTime)dbComplaint.Created_Date).AddMinutes(2)) - dbComplaint.Created_Date);
                if (ts.TotalMilliseconds < 4294967296)
                {
                    complaintEscalModel.timerEscalation = new Timer(new TimerCallback(OnComplaintEscalationEvent),
                        complaintEscalModel, Timeout.Infinite, Timeout.Infinite);
                    complaintEscalModel.timerEscalation.Change(20000000, Timeout.Infinite);
                    //complaintEscalModel.timerEscalation.Change((long)ts.TotalMilliseconds, Timeout.Infinite);
                
                    //complaintEscalModel.timerEscalation.Change((long) ts.TotalMilliseconds, Timeout.Infinite);
                }
            }
        }

        private void OnComplaintEscalationEvent(object e)
        {
            ComplaintWiseEscalationModel complaintEscalModel = (ComplaintWiseEscalationModel)e;
            if (complaintEscalModel.listEscalationFields.Count > 0)
            {
                // If next escalation exist in list
                if ((complaintEscalModel.currEscalationIndex + 1) < complaintEscalModel.listEscalationFields.Count)
                {
                    EscalationFields escalationFields =
                        complaintEscalModel.listEscalationFields[complaintEscalModel.currEscalationIndex + 1];
                    TimeSpan ts = (TimeSpan) (escalationFields.dt - complaintEscalModel.dbComplaint.Created_Date);
                    if (ts.TotalMilliseconds < 4294967296)
                    {
                        complaintEscalModel.timerEscalation.Change((long) ts.TotalMilliseconds, Timeout.Infinite);
                        //complaintEscalModel.timerEscalation.Change(10000, Timeout.Infinite);
                        complaintEscalModel.currEscalationIndex++;
                    }

                }
                else
                {
                    complaintEscalModel.timerEscalation.Dispose();
                    complaintEscalModel.timerEscalation = null;
                    listComplaintEscalationModel.Remove(complaintEscalModel);
                }
            }
        }

        public string GetMessage()
        {
            return "main hn don";
        }

        private void SetEscalationComplaintFields()
        {
            
        }
    }
}
