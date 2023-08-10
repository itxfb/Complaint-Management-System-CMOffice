using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using PITB.CMS_Common.Helper;
using Foolproof;

namespace PITB.CMS_Models.View
{
    public class VmEditStakeholderUser
    {
        public string TitleHeading { get; set; }
        public string SubmitBtnTxt { get; set; }
        public int? UserId { get; set; }

        public bool IsEdit { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [MinLength(4, ErrorMessage = "Password should be atleast 4 characters")]
        public string Password { get; set; }

        [Display(Name = "Enter Mobile No.")]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^03\d{9}(\,03\d{9})*$", ErrorMessage = "Correct number format is 03123456789,0333...")]
        public string Phone { get; set; }

        [Display(Name="Enter Email")]
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@([a-zA-Z0-9_\-\.]+)\.([a-zA-Z]{2,5})$",ErrorMessage ="Please enter valid email address")]
        public string Email { get; set; }
        [Display(Name = "Enter Imei No.")]
        [RegularExpression(@"^([0-9]{15}|[0-9]{17})$", ErrorMessage = "Valid Imei No is 15 or 17 digits")]
        public string ImeiNo { get; set; }

        [Display(Name="Enter Address")]
        [DataType(DataType.Text)]
        public string Address { get; set; }

        [Display(Name="Enter CNIC")]
        [RegularExpression(@"^([0-9]{13})$",ErrorMessage ="Valid Cnic is 13 digits")]
        public string CNIC { get; set; }

        [Display(Name="Enter Designation")]
        [DataType(DataType.Text)]
        public string Designation { get; set; }

        [Display(Name="Enter Designation abbv")]
        [DataType(DataType.Text)]
        public string DesignationAbbrevation { get; set; }

        [Display(Name="Enter user identification Id")]
        public int? UserHierarchyId { get; set; }

        //[Required]
        public int? ProvinceId { get; set; }

        //[Required]
        public int? DivisionId { get; set; }

        //[Required]
        public int? DistrictId { get; set; }

        public int? WardId { get; set; }

        [Required]
        public int ActiveState { get; set; }
        public List<SelectListItem> ActiveStateList
        {
            get
            {

                List<SelectListItem> listItems = new List<SelectListItem>();

                listItems.Add(new SelectListItem() { Text = "Yes", Value = "1" });
                listItems.Add(new SelectListItem() { Text = "No", Value = "0" });

                return listItems;
            }
        }
        [Required]
        public int TransferState { get; set; }
        public List<SelectListItem> TransferStateList
        {
            get
            {

                List<SelectListItem> listItems = new List<SelectListItem>();

                listItems.Add(new SelectListItem() { Text = "Yes", Value = "1" });
                listItems.Add(new SelectListItem() { Text = "No", Value = "0" });

                return listItems;
            }
        }

        [Required]
        public string Campaign { get; set; }

        [Required]
        public int? Hierarchy { get; set; }

        [Required]
        public List<int> Categories { get; set; }

        public List<DbProvince> ListOfProvinces { get; set; }

        public List<SelectListItem> ProvinceSelectList
        {
            get
            {
                if (ListOfProvinces == null)
                {
                    return new List<SelectListItem>();
                }
                return ListOfProvinces.Select(province => new SelectListItem() { Text = province.Province_Name, Value = province.Province_Id.ToString(), Selected = province.Province_Name.Equals("Punjab", StringComparison.OrdinalIgnoreCase) }).ToList();
            }
        }

        public List<DbCampaign> ListOfCampaign { get; set; }

        public List<SelectListItem> CampaignSelectList
        {
            get
            {
                if (ListOfCampaign == null)
                {
                    return new List<SelectListItem>();
                }
                return ListOfCampaign.Select(campaign => new SelectListItem() { Text = campaign.Campaign_Name, Value = campaign.Id.ToString() }).ToList();
            }
        }

        public List<DbComplaintType> ListOfCategory { get; set; }

        public MultiSelectList CategoryList
        {
            get
            {
                if (ListOfCategory == null)
                {
                    return new MultiSelectList(ListOfCategory, "Complaint_Category", "Name");
                }
                return new MultiSelectList(ListOfCategory, "Complaint_Category","Name",Categories);
            }
        }

        public List<DbHierarchy> ListOfHierachy { get; set; }

        public List<SelectListItem> HierachyList
        {
            get
            {
                if (ListOfHierachy == null)
                {
                    return new List<SelectListItem>();
                }
                return ListOfHierachy.Select(Hierachy => new SelectListItem() { Text = Hierachy.HierarchyName, Value = Hierachy.Id.ToString() }).ToList();
            }
        }

        public List<DbDivision> ListOfDivision { get; set; }

        public List<SelectListItem> DivisionList
        {
            get
            {
                if (ListOfDivision == null)
                {
                    return new List<SelectListItem>();
                }
                return ListOfDivision.Select(Division => new SelectListItem() { Text = Division.Division_Name, Value = Division.Division_Id.ToString() }).ToList();
            }
        }

        public List<DbDistrict> ListOfDistrict { get; set; }

        public List<SelectListItem> DistrictList
        {
            get
            {
                if (ListOfDistrict == null)
                {
                    return new List<SelectListItem>();
                }
                return ListOfDistrict.Select(District => new SelectListItem() { Text = District.District_Name, Value = District.District_Id.ToString() }).ToList();
            }
        }
        public List<int> TehsilId { get; set; }

        public bool IsTehsilRequired { get; set; }
        public List<DbTehsil> ListOfTehsil { get; set; }

        public MultiSelectList TehsilList
        {
            get
            {
                if (ListOfTehsil == null)
                {
                    return new MultiSelectList(new List<DbTehsil>(), "Tehsil_Id", "Tehsil_Name");
                }
                return new MultiSelectList(ListOfTehsil, "Tehsil_Id", "Tehsil_Name", TehsilId);
            }
        }
        public List<int> UnionCounilId { get; set; }

        public List<DbUnionCouncils> ListOfUnionCouncils { get; set; }

        public MultiSelectList UnionCouncilsList
        {
            get
            {
                if (ListOfUnionCouncils == null)
                {
                    return new MultiSelectList(new List<DbUnionCouncils>(), "UnionCouncil_Id", "Councils_Name");
                }
                return new MultiSelectList(ListOfUnionCouncils, "UnionCouncil_Id", "Councils_Name", UnionCounilId);
            }
        }


       
        public List<DbWards> ListOfWards { get; set; }
        public List<SelectListItem> WardsList
        {
            get
            {
                if (ListOfWards == null)
                {
                    return new List<SelectListItem>();
                }
                return ListOfWards.Select(Wards => new SelectListItem() { Text = Wards.Wards_Name, Value = Wards.Ward_Id.ToString() }).ToList();
            }
        }
    }
}