using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PITB.CMS_Models.View.Wasa.Stakeholder
{
    public class VmWasaStakeholderComplaintListing : VmStakeholderComplaintListing
    {

        public string Complaint_SubCategory_Name { get; set; }

        public string Person_Contact { get; set; }

        public string Person_Address { get; set; }

        private string _complaint_Computed_Status;

        public string Computed_Time_Passed_Since_Complaint_Launch { get; set; }

        public override string Complaint_Computed_Status
        {
            get
            {
                if (_complaint_Computed_Status == Config.PendingFreshStatus)
                {
                    _complaint_Computed_Status = Config.WasaPendingFreshStatus;

                }
                return _complaint_Computed_Status;
            }
            set
            {
                _complaint_Computed_Status = value;
            }
        }


    }
}