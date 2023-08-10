using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using PITB.CMS_Common.Helper;

namespace PITB.CMS_Models.View
{
    public class VmAddStakeholderUser
    {
        public string TitleHeading { get; set; }

        public string SubmitBtnTxt { get; set; }

        public int? UserId { get; set; }

        public bool IsEdit { get; set; }

        [Required]
        public string Name { get; set; }

        [Required(ErrorMessage="The Username field is required.")]
        [RegularExpression(@"^([a-zA-Z0-9._-]+)$", ErrorMessage = "Username cannot contain special character")]
        [Remote("CheckIfUsernameExists", "Account",HttpMethod ="POST",ErrorMessage ="Username already exists. Please enter a different username.")]
        public string UserName { get; set; }

        [Required]
        [MinLength(4,ErrorMessage ="Password should be atleast 4 characters.")]
        public string Password { get; set; }

        [Display(Name = "Enter Mobile No.")]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^03\d{9}(\,03\d{9})*$", ErrorMessage = "Correct number format is 03123456789,0333...")]
        public string Phone { get; set; }
        [Display(Name="Enter CNIC")]
        [RegularExpression(@"^([0-9]{13})$", ErrorMessage = "Correct number format is 3210277793346")]
        public string CNIC { get; set; }
        public int CountVerificationCodeSent { get; set; }
        public int CountPasswordChangedUsers { get; set; }
        //[Required]
        //[Required]        
        public int? ProvinceId { get; set; }

        public bool IsProvinceRequired { get; set; }

        [Display(Name="Enter Imei No.")]
        [RegularExpression(@"^([0-9]{15}|[0-9]{17})$",ErrorMessage ="Valid Imei No is 15 or 17 digit.")]
        public string ImeiNo { get; set; }

        //[Required]
       // [Required]

        public int? DivisionId { get; set; }

        public bool IsDivisionRequired { get; set; }




        //[Required]
        //[Required]
        
        public int? District { get; set; }

        public bool IsDistrictRequired { get; set; }

        
        
        
        //[Required]
        
      

        

        //[Required]


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
        [RegularExpression(@"(^[0-9]+$|^$)", ErrorMessage = "Only input numbers")]
        public int? UserHierarchy { get; set; }


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
                return ListOfCampaign.Select(campaign => new SelectListItem() { Text = campaign.Campaign_Name, Value = campaign.Id.ToString()}).ToList();
            }
        }
        [Required]
        public List<int> Categories { get; set; }

        public List<DbComplaintType> ListOfCategory { get; set; }

        public List<SelectListItem> CategoryList
        {
            get
            {
                if (ListOfCategory == null)
                {
                    return new List<SelectListItem>();
                }
                return ListOfCategory.Select(campaignType => new SelectListItem() { Text = campaignType.Name, Value = campaignType.Complaint_Category.ToString(), Selected=true }).ToList();
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
                return ListOfDivision.Select(Division => new SelectListItem() { Text = Division.Division_Name, Value = Division.Division_Id.ToString()}).ToList();
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

        public List<SelectListItem> TehsilList
        {
            get
            {
                if (ListOfTehsil == null)
                {
                    return new List<SelectListItem>();
                }
                return ListOfTehsil.Select(Tehsils => new SelectListItem() { Text = Tehsils.Tehsil_Name, Value = Tehsils.Tehsil_Id.ToString() }).ToList();
            }
        }
        public List<int> UnionCounilId { get; set; }
        
        public List<DbUnionCouncils> ListOfUnionCouncils { get; set; }

        public List<SelectListItem> UnionCouncilsList
        {
            get
            {
                if (ListOfUnionCouncils == null)
                {
                    return new List<SelectListItem>();
                }
                return ListOfUnionCouncils.Select(UnionCouncils => new SelectListItem() { Text = UnionCouncils.Councils_Name, Value = UnionCouncils.UnionCouncil_Id.ToString() }).ToList();
            }
        }
        public List<DbWards> ListOfWards { get; set; }
        public List<SelectListItem> WardsList
        {
            get
            {
                if(ListOfWards == null)
                {
                    return new List<SelectListItem>();
                }
                return ListOfWards.Select(Wards => new SelectListItem() { Text = Wards.Wards_Name,Value = Wards.Ward_Id.ToString()}).ToList();
            }
        }
    }
}