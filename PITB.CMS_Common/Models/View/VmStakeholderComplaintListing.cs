using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.SqlServer.Server;
using System.ComponentModel.DataAnnotations;
using PITB.CMS_Common.Helper.Extensions;
using PITB.CMS_Common.Helper.Attributes;
using PITB.CMS_Common.Enums;

namespace PITB.CMS_Common.Models.View
{
    public class VmStakeholderComplaintListing
    {
        [ExcelReport(ColumnName = "Complaint No.", ReportName = "Report1")]
        public string Complaint_No { get; set; }
        public int ComplaintId { get; set; }
        public string Id { get; set; }

        public string Campaign_Name { get; set; }
        public int Province_Id { get; set; }
        [ExcelReport(ColumnName = "Province", ReportName = "Report1")]
        public string Province_Name { get; set; }
        public int Division_Id { get; set; }
        [ExcelReport(ColumnName = "Division", ReportName = "Report1")]
        public string Division_Name { get; set; }
        public int District_Id { get; set; }
        [ExcelReport(ColumnName = "District", ReportName="Report1")]
        public string District_Name { get; set; }
        public int Tehsil_Id { get; set; }
        [ExcelReport(ColumnName = "Tehsil", ReportName="Report1")]
        public string Tehsil_Name{ get; set; }
        [ExcelReport(ColumnName = "UC No", ReportName = "Report1")]
        public int Uc_No { get; set; }
        public int UnionCouncil_Id { get; set; }
        [ExcelReport(ColumnName = "UnionCouncil", ReportName="Report1")]
        public string UnionCouncil_Name { get; set; }
        public int Ward_Id { get; set; }
        public string Ward_Name { get; set; }

        public string LocationArea { get; set; }
        [ExcelReport(ColumnName = "Citizen Name", ReportName="Report1")]
        public string Person_Name { get; set; }
        [ExcelReport(ColumnName = "Initiated Date", ReportName = "Report1")]
        public string Created_Date { get; set; }

        [ExcelReport(ColumnName = "Closed Date", ReportName = "Report1")]
        public string Status_Changed_Date { get; set; }

        [ExcelReport(ColumnName = "Category", ReportName = "Report1")]
        public string Complaint_Category_Name { get; set; }
        [ExcelReport(ColumnName = "AM Name", ReportName = "Report1")]

        public string Complaint_SubCategory_Name { get; set; }
        public string Complaint_Stakeholder_Name { get; set; }
        public string Department_Name { get; set; }

        [ExcelReport(ColumnName = "Status", ReportName = "Report1")]
        public virtual string Complaint_Computed_Status { get; set; }

        [ExcelReport(ColumnName = "Before Picture",ReportName="Report1")]
        public System.Byte[] BeforePicture { get; set; }
        [ExcelReport(ColumnName = "After Picture", ReportName = "Report1")]
        public byte[] AfterPicture { get; set; }
        [ExcelReport(ColumnName = "Details", ReportName = "Report1")]

        public string BeforePictureSrc { get; set; }

        public string AfterPictureSrc { get; set; }
        public string Details { get; set; }
        [ExcelReport(ColumnName="Escalation",ReportName="Report1")]
        public string Escalation_Level { get; set; }
        public int Total_Rows { get; set; }
        public int? Complaint_Computed_Hierarchy_Id { get; set; }
        public string Complaint_Computed_Hierarchy { get; set; }
        public int? userCategoryId1 { get; set; }
        public int? userCategoryId2 { get; set; }

        public int? Complaint_Computed_User_Hierarchy_Id { get; set; }
        public string Computed_Remaining_Time_To_Escalate { get; set; }

        private int? _followupCount;

        private int? _overdueDays;

        private int? _transferedCount;
        
        public int Is_Action_Completed { get; set; }
        public string Ref_Complaint_Id { get; set; }
        public int Current_Action_Step { get; set; }

        public int Action_Logs_Count { get; set; }

        private string _feedbackStatus = null;
        public string CNFP_Feedback_Value
        {
            get
            {
                //System.Diagnostics.Debug.WriteLine("In get:  ComplaintId: {0}, Complaint_Computed_Status: {1}, CNFP_Feedback_Id: {2}", ComplaintId, Complaint_Computed_Status, CNFP_Feedback_Id);
                    if (CNFP_Feedback_Id == null)
                    {
                        if (Complaint_Computed_Status != null && Complaint_Computed_Status.Equals(System.Enum.GetName(typeof(Config.ComplaintStatus), Config.ComplaintStatus.Resolved)))
                        {
                            _feedbackStatus = "Pending";
                        }
                        else
                        {
                            _feedbackStatus = "Not Applicable";
                        }

                    }
                    return _feedbackStatus.FirstLetterUpperCase();
            }
            set
            {
               // System.Diagnostics.Debug.WriteLine("In set: ComplaintId: {0}, Complaint_Computed_Status: {1}, CNFP_Feedback_Id: {2}, CNFP_Feedback_Value_Original: {3}, CNFP_Feedback_Value_New: {4}", ComplaintId, Complaint_Computed_Status, CNFP_Feedback_Id, CNFP_Feedback_Value,value);
                _feedbackStatus = value;
            }
        }
        public int? CNFP_Feedback_Id { get; set; }

        public int Computed_Overdue_Days
        {
            get
            {
                return Convert.ToInt32(_overdueDays);
            }
            set
            {
                _overdueDays = value;
            }
        }

        public int FollowupCount
        {
            get
            {
                return Convert.ToInt32(_followupCount);
            }
            set
            {
                _followupCount = value;
            }
        }

        public int TransferedCount
        {
            get
            {
                return Convert.ToInt32(_transferedCount);
            }
            set
            {
                _transferedCount = value;
            }
        }

        private int? _Callback_Count;

        public int CallbackCount
        {
            get
            {
                return Convert.ToInt32(_Callback_Count);
            }
            set
            {
                _Callback_Count = value;
            }
        }

        private int? _statusReopenedCount;
        public int StatusReopenedCount
        {
            get
            {
                return Convert.ToInt32(_statusReopenedCount);
            }
            set
            {
                _statusReopenedCount = value;
            }
        }
        public string Computed_Remaining_Time_Percentage { get; set; }

        public string Computed_Total_Time_Percentage_Since_Launch { get; set; }
    }

    public class VmStakeholderComplaintListingSchoolEducation
    {
        public int ComplaintId { get; set; }
        public string Id { get; set; }
        //public string Campaign_Name { get; set; }

        public string RefField1 { get; set; }

        public string RefField2 { get; set; }

        public string RefField3 { get; set; }

        public string RefField4 { get; set; }

        public string RefField5 { get; set; }

        public string RefField6 { get; set; }

        public string Person_Cnic { get; set; }

        public string District_Name { get; set; }

        public string Tehsil_Name { get; set; }

        //public string UnionCouncil_Name { get; set; }

        public string Person_Name { get; set; }
        public string Created_Date { get; set; }

        public string Department_Name { get; set; }

        public string Complaint_Category_Name { get; set; }

        public string Computed_Overdue_Days { get; set; }

        public string Complaint_SubCategory_Name { get; set; }

        public string Complaint_Remarks { get; set; }

        private string _complaint_Computed_Status;
        public string Complaint_Computed_Status
        {
            get
            {
                if (_complaint_Computed_Status == "Unsatisfactory Closed")
                {
                    _complaint_Computed_Status = Config.SchoolEducationUnsatisfactoryStatus;
                    
                }
                return _complaint_Computed_Status;
            }
            set
            {
                _complaint_Computed_Status = value;
            }
        }

        public int Total_Rows { get; set; }

        public string Complaint_Computed_Hierarchy { get; set; }

        public string Computed_Remaining_Time_To_Escalate { get; set; }

        public string Assigned_To_Name { get; set; }

        public string StatusChangedDate_Time { get; set; }

        public string Person_Contact { get; set; }

        private int? _followupCount;

        public int FollowupCount
        {
            get
            {
                return Convert.ToInt32(_followupCount);
            }
            set
            {
                _followupCount = value;
            }
        }

        private int? _statusReopenedCount;
        public int StatusReopenedCount
        {
            get
            {
                return Convert.ToInt32(_statusReopenedCount);
            }
            set
            {
                _statusReopenedCount = value;
            }
        }
        private int? _Callback_Count;

        public int Callback_Count
        {
            get
            {
                return Convert.ToInt32(_Callback_Count);
            }
            set
            {
                _Callback_Count = value;
            }
        }
        private string _Callback_Status;

        public string Callback_Status
        {
            get
            {
                if (_Callback_Status != null)
                {
                    var value = (CallStatuses)Convert.ToInt32(_Callback_Status);
                    var type = typeof(CallStatuses);
                    var memberInfo = type.GetMember(value.ToString());
                    var attributes = memberInfo[0].GetCustomAttributes(typeof(DisplayAttribute), false);
                    var description = ((DisplayAttribute)attributes[0]).Name;
                    return description;
                }
                return _Callback_Status;
            }
            set
            {
                _Callback_Status = value;
            }
        }
        private string _Callback_Comment;

        public string Callback_Comment
        {
            get
            {
                return _Callback_Comment;
            }
            set
            {
                _Callback_Comment = value;
            }
        }
    }
}