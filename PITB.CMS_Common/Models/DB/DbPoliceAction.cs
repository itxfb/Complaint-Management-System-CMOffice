using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PITB.CMS_Common.Models
{

    [Table("PITB.Police_Action")]
    public partial class DbPoliceAction
    {
        public int Id { get; set; }

        public int? Complaint_Id { get; set; }

        public int? CallWithin08Hours { get; set; }

        [StringLength(200)]
        public string CallWithin08HoursReason { get; set; }

        public int? MeetingWithin24Hours { get; set; }

        [StringLength(200)]
        public string MeetingWithin24HoursReason { get; set; }

        //public int? ProceedingReportFilter { get; set; }

        //public int? FinalReportFilter { get; set; }

        public int? DisposalCategoryId { get; set; }

        [StringLength(50)]
        public string DisposalCategoryName { get; set; }

        [StringLength(200)]
        public string DisposalCategoryWillBeUseInCaseOf { get; set; }

        //public int? DisposalCategoryDynamicFieldsFilter { get; set; }

        public int? SatisfactionOfComplainant { get; set; }

        [StringLength(200)]
        public string SatisfactionOfComplainantReason { get; set; }

        public DateTime? CreatedDateTime { get; set; }

        public DateTime? UpdatedDateTime { get; set; }

        public DateTime? CompletedDateTime { get; set; }

        //[StringLength(500)]
        public string Comments { get; set; }

        public int? Step1UpdatedBy { get; set; }
        public DateTime? Step1UpdatedDateTime { get; set; }

        public int? Step2UpdatedBy { get; set; }
        public DateTime? Step2UpdatedDateTime { get; set; }

        public int? Step3UpdatedBy { get; set; }
        public DateTime? Step3UpdatedDateTime { get; set; }

        public int? Step4UpdatedBy { get; set; }
        public DateTime? Step4UpdatedDateTime { get; set; }

        public int? Step5UpdatedBy { get; set; }
        public DateTime? Step5UpdatedDateTime { get; set; }

        public int? CurrentStep { get; set; }

        public bool? IsComplete { get; set; }

        public bool? IsActive { get; set; }
    }
}
