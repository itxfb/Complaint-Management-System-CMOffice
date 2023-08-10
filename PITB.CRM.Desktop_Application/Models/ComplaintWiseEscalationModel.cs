using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using PITB.CMS;

namespace PITB.CRM.Desktop_Application.Models
{
    public class ComplaintWiseEscalationModel
    {
        public DbComplaint dbComplaint { get; set; }
        public Timer timerEscalation { get; set; }

        public List<EscalationFields> listEscalationFields { get; set; }

        public int currEscalationIndex { get; set; }

        public ComplaintWiseEscalationModel(DbComplaint dbComplaint)
        {
            this.dbComplaint = dbComplaint;
            PopulateEscalationFields(dbComplaint);
        }

        private void PopulateEscalationFields(DbComplaint dbComplaint)
        {
            listEscalationFields = new List<EscalationFields>();
            EscalationFields complaintWiseEscalation = null;
            int count = 0;
            while (Utility.IsPropertyAndNotNull(dbComplaint, "Dt" + (count + 1)))
            {
                complaintWiseEscalation = new EscalationFields();
                complaintWiseEscalation.dt = (DateTime) Utility.GetPropertyThroughReflection(dbComplaint, "Dt" + (count + 1));
                complaintWiseEscalation.hierarchy = (int?)Utility.GetPropertyThroughReflection(dbComplaint, "SrcId" + (count + 1));
                complaintWiseEscalation.userHierarchy = (int?)Utility.GetPropertyThroughReflection(dbComplaint, "UserSrcId" + (count + 1));

                listEscalationFields.Add(complaintWiseEscalation);
                count++;
            }
            
        }
    
    }

    public class ComplaintEscalationState
    {
        public DateTime dt { get; set; }

        public int? hierarchy { get; set; }

        public int? userHierarchy { get; set; }
    }

    public class EscalationFields
    {
        public DateTime dt { get; set; }

        public int? hierarchy { get; set; }

        public int? userHierarchy { get; set; }
    }
}
