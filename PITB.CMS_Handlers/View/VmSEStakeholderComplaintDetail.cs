using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PITB.CMS_Handlers.View
{
    public class VmSEStakeholderComplaintDetail : VmStakeholderComplaintDetail
    {
        public string SchoolHeadName { get; set; }

        public string SchoolHeadPhoneNo { get; set; }

        public VmCallSubmit VmCallSubmit { get; set; }


        public VmComplaintResponsiblePersonInformation VmComplaintResponsiblePerson { get; set; }
        public VmSEStakeholderComplaintDetail(VmStakeholderComplaintDetail other)
            : base(other)
        {
            VmCallSubmit = new VmCallSubmit();
            VmCallSubmit.Campaign_Id = other.Compaign_Id;
            VmCallSubmit.Complaint_Id = other.ComplaintId;
            VmCallSubmit.CallComments = other.Callback_Comment;
            VmCallSubmit.callStatusId = other.Callback_Status;
            //this.Distance=other.Distance;
            VmComplaintResponsiblePerson = new VmComplaintResponsiblePersonInformation();
            
        }
    }
    public class VmFeedbackStatus
    {
        public int? feedbackStatusID { get; set; }

        public List<SelectListItem> ListFeedbackStatus;

        [MaxLength(500, ErrorMessage = "Comment should be maximum 100 characters in length")]
        public string feedbackStatusChangeComments { get; set; }

        public VmFeedbackStatus()
        {
            ListFeedbackStatus = new List<SelectListItem>();
            ListFeedbackStatus.Add(new SelectListItem() { Text = "Satisfied", Value = "1" });
            ListFeedbackStatus.Add(new SelectListItem() { Text = "Unsatisfied", Value = "2" });
            ListFeedbackStatus.Add(new SelectListItem() { Text = "Feedback Not Recieved", Value = "3" });
        }
    }
    public class VmCallSubmit
    {
        public int? Campaign_Id { get; set; }

        public int Complaint_Id { get; set; }

        [Display(Name = "Call Status")]
        public int? callStatusId { get; set; }

        public List<SelectListItem> ListCallStatus;

        public string CallComments { get; set; }

        public DateTime CallDateTime { get; set; }

        public VmCallSubmit()
        {
            callStatusId = null;
            ListCallStatus = new List<SelectListItem>();
            ListCallStatus.Add(new SelectListItem() { Text = "Complete",Value= "1" });
            ListCallStatus.Add(new SelectListItem() { Text = "User Busy", Value = "2" });
            ListCallStatus.Add(new SelectListItem() { Text = "Wrong Number/Person", Value = "3" });
            ListCallStatus.Add(new SelectListItem() { Text = "No response", Value = "4" });
        }
    }
    public class VmComplaintResponsiblePersonInformation
    {        
        [Required(AllowEmptyStrings=false,ErrorMessage="Id cannot be null")]
        public int? UserId { get; set; }

        [Display(Name = "Name")]
        [RegularExpression("^[a-zA-Z ]*$", ErrorMessage = "Only text allowed in {0} field")]
        public string Name { get; set; }

        [Display(Name = "Designation")]
        [RegularExpression("^[a-zA-Z ]*$", ErrorMessage = "Only text allowed in {0} field")]
        public string Designation { get; set; }

        [StringLength(11, MinimumLength = 11, ErrorMessage = "Provide complete {0}")]
        [Required(AllowEmptyStrings = false)]
        [Display(Name = "Contact No")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Only digits allowed in in {0}")]
        public string MobileNo{get;set;}

        public VmComplaintResponsiblePersonInformation()
        {
            Name = "None";
            Designation = "None";
            MobileNo = "None";
        }
    }
}