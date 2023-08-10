using System.Data.Entity;
using System.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace PITB.CMS_Models.DB
{

    [Table("PITB.Complainant_Feedback_Log")]
    public class DbComplainantFeedbackLog
    {
        public int Id { get; set; }

        public int? SubmittedByUserId { get; set; }

        public int? ComplainantId { get; set; }

        public int? ComplaintId { get; set; }

        public int? CampaignId { get; set; }

        public int? ConferenceCallArrangedId { get; set; }

        public string ConferenceCallArrangedIdStr { get; set; }

        public int? CallWithin08HoursStatusId { get; set; }

        [StringLength(50)]
        public string CallWithin08HoursStatusIdStr { get; set; }

        [StringLength(500)]
        public string CallWithin08HoursStatusComments { get; set; }

        public int? MeetingWithin24HoursStatusId { get; set; }

        [StringLength(50)]
        public string MeetingWithin24HoursStatusIdStr { get; set; }

        [StringLength(500)]
        public string MeetingWithin24HoursStatusComments { get; set; }

        public int? ProceedingWithin48HoursStatusId { get; set; }

        [StringLength(50)]
        public string ProceedingWithin48HoursStatusIdStr { get; set; }

        [StringLength(500)]
        public string ProceedingWithin48HoursStatusComments { get; set; }

        public int? FeedbackStatusId { get; set; }

        [StringLength(50)]
        public string FeedbackStatusIdStr { get; set; }

        [StringLength(500)]
        public string FeedbackStatusComments { get; set; }

        public DateTime? CreatedDateTime { get; set; }

        public bool? IsCurrentlyActive { get; set; }
    }
}
