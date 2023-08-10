using System.Linq;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Logical;
using PITB.CMS.Helper.Database;

namespace PITB.CMS.Models.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

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

        public static DbPoliceAction GetAction(int complaintId, bool isActive)
        {
            try
            {
                using (DBContextHelperLinq db = new DBContextHelperLinq())
                {
                    return db.DbPoliceAction.Where(n => n.Complaint_Id == complaintId && n.IsActive == isActive).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static DbPoliceAction GetAction(int complaintId,bool isComplete, bool isActive, DBContextHelperLinq _db=null)
        {
            DBContextHelperLinq db = (_db == null) ? new DBContextHelperLinq() : _db;
            try
            {
                return db.DbPoliceAction.Where(n => n.Complaint_Id == complaintId && n.IsComplete == isComplete && n.IsActive == isActive).FirstOrDefault();
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                if (_db == null)
                {
                    db.Dispose();
                }
            }
        }

        public static List<DbPoliceAction> GetActions(int complaintId)
        {
            try
            {
                using (DBContextHelperLinq db = new DBContextHelperLinq())
                {
                    return db.DbPoliceAction.Where(n => n.Complaint_Id == complaintId).ToList();
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
