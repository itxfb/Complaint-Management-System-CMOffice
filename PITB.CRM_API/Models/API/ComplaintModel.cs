using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PITB.CRM_API.Models.API
{
    public class ExtendedComplaintModel
    {
        public ComplaintModel cmModel;
        public PersonalInfoModel piModel;
        public SuggesstionModel suModel;
        public InquiryModel iqModel;

        public ExtendedComplaintModel()
        {
            cmModel = new ComplaintModel();
            piModel = new PersonalInfoModel();
            suModel = new SuggesstionModel();
            iqModel = new InquiryModel();
        }
    }
    public class ComplaintModel
    {
        public int Id { get; set; }
        public int? Person_Id { get; set; }
        public int? Complaint_Type { get; set; }
        public int? Complaint_Category { get; set; }
        public int? Complaint_SubCategory { get; set; }
        public int? Compaign_Id { get; set; }
        public string CampaignLogoUrl { get; set; }
        public int? Province_Id { get; set; }
        public int? Division_Id { get; set; }
        public int? District_Id { get; set; }
        public int? Tehsil_Id { get; set; }
        public int? UnionCouncil_Id { get; set; }
        public int? Ward_Id { get; set; }
        public int? Agent_Id { get; set; }
        public string Complaint_Address
        {
            get { return HomeShopNo + " " + StreetNameNo + " " + LocalityArea; }
        }
        public string Business_Address { get; set; }
        public string Complaint_Remarks { get; set; }
        public int? Complaint_Status_Id { get; set; }
        public string Complaint_Status { get; set; }
        public DateTime? Created_Date { get; set; }
        public int? Created_By { get; set; }
        public string Agent_Comments { get; set; }
        public string HomeShopNo { get; set; }
        public string StreetNameNo { get; set; }
        public string LocalityArea { get; set; }
        public int? departmentId { get; set; }
        public bool hasDepartment { get; set; }
            
    }
    public class PersonalInfoModel
    {
        public int? Person_id { get; set; }
        public string Person_Name { get; set; }
        public string Person_Father_Name { get; set; }
        public string Cnic_No { get; set; }
        public bool IsCnicPresent { get; set; }
        public string Gender { get; set; }
        public string Mobile_No { get; set; }     
        public string Secondary_Mobile_No { get; set; }
        public string LandLine_No { get; set; }
        public string Person_Address { get; set; }
        public string Email { get; set; }
        public string Nearest_Place { get; set; }
        public int? Province_Id { get; set; }
        public int? Division_Id { get; set; }
        public int? District_Id { get; set; }
        public string District_Name { get; set; }
        public int? Tehsil_Id { get; set; }
        public int? Town_Id { get; set; }
        public int? Uc_Id { get; set; }
    }
    public class SuggesstionModel
    {
        public int Id { get; set; }
        public int? Person_Id { get; set; }
        public int? Complaint_Type { get; set; }
        public int? Complaint_Category { get; set; }
        public int? Complaint_SubCategory { get; set; }
        public int? Compaign_Id { get; set; }
        public string CampaignLogoUrl { get; set; }
        public int? Province_Id { get; set; }
        public int? Division_Id { get; set; }
        public int? District_Id { get; set; }
        public int? Tehsil_Id { get; set; }
        public int? UnionCouncil_Id { get; set; }
        public int? Ward_Id { get; set; }
        public int? Agent_Id { get; set; }
        public string Complaint_Address
        {
            get { return HomeShopNo + " " + StreetNameNo + " " + LocalityArea; }
        }
        public string Business_Address { get; set; }
        public string Complaint_Remarks { get; set; }
        public int? Complaint_Status_Id { get; set; }
        public string Complaint_Status { get; set; }
        public DateTime? Created_Date { get; set; }
        public int? Created_By { get; set; }
        public string Agent_Comments { get; set; }
        public string HomeShopNo { get; set; }
        public string StreetNameNo { get; set; }
        public string LocalityArea { get; set; }
        public int? departmentId { get; set; }
        public bool hasDepartment { get; set; }
    }
    public class InquiryModel
    {
        public int Id { get; set; }
        public int? Person_Id { get; set; }
        public int? Complaint_Type { get; set; }
        public int? Complaint_Category { get; set; }
        public int? Complaint_SubCategory { get; set; }
        public int? Compaign_Id { get; set; }
        public string CampaignLogoUrl { get; set; }
        public int? Province_Id { get; set; }
        public int? Division_Id { get; set; }
        public int? District_Id { get; set; }
        public int? Tehsil_Id { get; set; }
        public int? UnionCouncil_Id { get; set; }
        public int? Ward_Id { get; set; }
        public int? Agent_Id { get; set; }
        public string Complaint_Address
        {
            get { return HomeShopNo + " " + StreetNameNo + " " + LocalityArea; }
        }
        public string Business_Address { get; set; }
        public string Complaint_Remarks { get; set; }
        public int? Complaint_Status_Id { get; set; }
        public string Complaint_Status { get; set; }
        public DateTime? Created_Date { get; set; }
        public int? Created_By { get; set; }
        public string Agent_Comments { get; set; }
        public string HomeShopNo { get; set; }
        public string StreetNameNo { get; set; }
        public string LocalityArea { get; set; }
        public int? departmentId { get; set; }
        public bool hasDepartment { get; set; }
    }

}