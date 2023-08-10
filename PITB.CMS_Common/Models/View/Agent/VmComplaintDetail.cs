using System.Collections.Generic;

using System;

namespace PITB.CMS_Common.Models.View
{
    public class VmComplaintDetail
    {
        public string ViewTag { get; set; }
        public VmPersonalInfo vmPersonlInfo {get; set;}

        public string complaintIdStr { get; set; }
        public int ComplaintId { get; set; }

        public Config.ComplaintType? Complaint_Type { get; set; }
        public int? Complaint_Category { get; set; }

        public string Person_Name { get; set; }
        public string Person_Cnic { get; set; }
        public string Person_Contact { get; set; }
        public string ComplaintCategoryName { get; set; }
        public int? Complaint_SubCategory { get; set; }
        public string Complaint_SubCategoryName { get; set; }
        public string Complaint_DepartmentName { get; set; }
        public int? Complaint_DepartmentId{ get; set; }

        public int? Compaign_Id { get; set; }
        public string Compain_Name { get; set; }

        public int? Province_Id { get; set; }
        public string Province_Name { get; set; }

        public int? Division_Id { get; set; }
        public string Division_Name { get; set; }

        public int? District_Id { get; set; }
        public string District_Name { get; set; }

        public int? Tehsil_Id { get; set; }
        public string Tehsil_Name { get; set; }

        public int? UnionCouncil_Id { get; set; }
        public string UnionCouncil_Name { get; set; }

        public bool Is_Anonymous { get; set; }

        public string Complaint_Address { get; set; }

        public string Complaint_Remarks { get; set; }

        public string Agent_Comments { get; set; }

        public string Created_By { get; set; }
        public DateTime? Created_DateTime { get; set; }

        public string currentStatusStr { get; set; }
        public string currentStatusId { get; set; }
        public string currStatusCommentsStr { get; set; }

        public int followupCount { get; set; }
        public string followupComment { get; set; }

        public List<DbDynamicComplaintFields> ListDynamicComplaintFields { get; set; }


        public VmComplaintDetail()
        {

        }

        public VmComplaintDetail(VmComplaintDetail vmComplaintDetail)
        {
            this.vmPersonlInfo = vmComplaintDetail.vmPersonlInfo;
            this.complaintIdStr = vmComplaintDetail.complaintIdStr;
            this.ComplaintId = vmComplaintDetail.ComplaintId;
            this.Person_Name = vmComplaintDetail.Person_Name;
            this.Person_Cnic = vmComplaintDetail.Person_Cnic;
            this.Person_Contact = vmComplaintDetail.Person_Contact;
            this.Complaint_Type = vmComplaintDetail.Complaint_Type;
            this.Complaint_Category = vmComplaintDetail.Complaint_Category;
            this.ComplaintCategoryName = vmComplaintDetail.ComplaintCategoryName;
            this.Complaint_SubCategory = vmComplaintDetail.Complaint_SubCategory;
            this.Complaint_SubCategoryName = vmComplaintDetail.Complaint_SubCategoryName;
            this.Complaint_DepartmentName = vmComplaintDetail.Complaint_DepartmentName;

            this.Compaign_Id = vmComplaintDetail.Compaign_Id;
            this.Compain_Name = vmComplaintDetail.Compain_Name;

            this.Province_Id = vmComplaintDetail.Province_Id;
            this.Province_Name = vmComplaintDetail.Province_Name;

            this.Division_Id = vmComplaintDetail.Division_Id;
            this.Division_Name = vmComplaintDetail.Division_Name;

            this.District_Id = vmComplaintDetail.District_Id;
            this.District_Name = vmComplaintDetail.District_Name;

            this.Tehsil_Id = vmComplaintDetail.Tehsil_Id;
            this.Tehsil_Name = vmComplaintDetail.Tehsil_Name;

            this.UnionCouncil_Id = vmComplaintDetail.UnionCouncil_Id;
            this.UnionCouncil_Name = vmComplaintDetail.UnionCouncil_Name;

            this.Complaint_Address = vmComplaintDetail.Complaint_Address;

            this.Complaint_Remarks = vmComplaintDetail.Complaint_Remarks;

            this.Agent_Comments = vmComplaintDetail.Agent_Comments;

            this.Created_By = vmComplaintDetail.Created_By;
            this.Created_DateTime = vmComplaintDetail.Created_DateTime;

            this.currentStatusStr = vmComplaintDetail.currentStatusStr;
            this.currentStatusId = vmComplaintDetail.currentStatusId;
            this.currStatusCommentsStr = vmComplaintDetail.currStatusCommentsStr;

            this.ListDynamicComplaintFields = vmComplaintDetail.ListDynamicComplaintFields;

            this.followupCount = vmComplaintDetail.followupCount;
            this.followupComment = vmComplaintDetail.followupComment;
        }

        

        public static VmComplaintDetail GetComplaintDetail(DbComplaint dbComplaint, List<DbDynamicComplaintFields> ListDynamicComplaintFields)
        {
            VmComplaintDetail vm = new VmComplaintDetail();
            vm.ComplaintId = dbComplaint.Id;
            vm.Compaign_Id = dbComplaint.Compaign_Id;
            vm.complaintIdStr = vm.Compaign_Id + "-" + vm.ComplaintId;
            vm.Complaint_Type = dbComplaint.Complaint_Type;
            vm.Person_Name = dbComplaint.Person_Name;
            vm.ComplaintCategoryName = dbComplaint.listCategory.Name;
            vm.Complaint_SubCategoryName = dbComplaint.Complaint_SubCategory_Name != null ? dbComplaint.Complaint_SubCategory_Name : "";
            vm.Complaint_DepartmentName = dbComplaint.Department_Name;
            vm.Complaint_DepartmentId= dbComplaint.Department_Id;
            //vm.ComplaintCategoryName = dbComplaint.listCategory.FirstOrDefault().Descr;
            vm.Province_Name = dbComplaint.listProvince!=null?dbComplaint.listProvince.Province_Name: "";
            vm.District_Name = dbComplaint.listDistrict != null ? dbComplaint.listDistrict.District_Name : "";
            vm.Tehsil_Name = (dbComplaint.listTehsil!=null) ? dbComplaint.listTehsil.Tehsil_Name : "None";
            vm.Is_Anonymous = dbComplaint.Is_Anonymous?? false;
            
            if (dbComplaint.User == null)
            {
                vm.Created_By = "Mobile Request";
            }
            else
            {
                vm.Created_By = dbComplaint.User.Name;
            }
            vm.Created_DateTime = dbComplaint.Created_Date;
            if (dbComplaint.listUc != null)
            {
                vm.UnionCouncil_Name = dbComplaint.listUc.Councils_Name;
                
            }

            vm.Complaint_Address = dbComplaint.Complaint_Address;
            vm.Complaint_Remarks = dbComplaint.Complaint_Remarks;
            vm.Agent_Comments = dbComplaint.Agent_Comments;
            vm.currentStatusStr = dbComplaint.Complaint_Computed_Status;
            vm.currentStatusId = dbComplaint.Complaint_Computed_Status_Id.ToString();
            vm.currStatusCommentsStr = dbComplaint.StatusChangedComments;

            vm.ListDynamicComplaintFields = ListDynamicComplaintFields;

            vm.followupCount = Convert.ToInt32(dbComplaint.FollowupCount);
            if (!string.IsNullOrEmpty(dbComplaint.FollowupComment))
            {
                vm.followupComment = dbComplaint.FollowupComment.Trim();
            }
            vm.followupComment = (string.IsNullOrEmpty(dbComplaint.FollowupComment)) ? "None" : dbComplaint.FollowupComment;

            
            return vm;
        }


    }
}