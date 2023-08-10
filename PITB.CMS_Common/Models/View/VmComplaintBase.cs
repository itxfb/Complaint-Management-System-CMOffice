using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web.Mvc;
using Amazon.KeyManagementService;

using PITB.CMS_Common.Models.View.Dynamic;
using Foolproof;


namespace PITB.CMS_Common.Models.View
{
    public class VmComplaintBase
    {
        public int Id { get; set; }

        public int? Person_Id { get; set; }

        //[Required]
        [Display(Name = "Complaint Type")]
        public int? Complaint_Type { get; set; }
        [Required]
        [Display(Name = "Complaint category")]
        public int? Complaint_Category { get; set; }
        [Required]
        [Display(Name = "Complaint subcategory")]
        public int? Complaint_SubCategory { get; set; }
        public int? Compaign_Id { get; set; }
        public string CampaignLogoUrl { get; set; }

        public int? Province_Id { get; set; }

        public int? Division_Id { get; set; }

        public int? District_Id { get; set; }
        [Required]
        [Display(Name = "Town/Tehsil")]
        public int? Tehsil_Id { get; set; }
        //[Required]
        [Display(Name = "Union council")]
        public int? UnionCouncil_Id { get; set; }

        [Display(Name = "Wards")]
        public int? Ward_Id { get; set; }

        public int? Agent_Id { get; set; }

        [StringLength(500)]
        public string Complaint_Address
        {
            get { return HomeShopNo + " " + StreetNameNo + " " + LocalityArea; }
        }

        [StringLength(500)]
        public string Business_Address { get; set; }
        [Required(AllowEmptyStrings = false)]
        [Display(Name = "Detail of complaint")]
        public string Complaint_Remarks { get; set; }

        public int? Complaint_Status_Id { get; set; }

        [StringLength(50)]
        public string Complaint_Status { get; set; }

        public DateTime? Created_Date { get; set; }

        public int? Created_By { get; set; }
        //[Required(AllowEmptyStrings = false)]
        //[Display(Name = "Agent comments")]
        public string Agent_Comments { get; set; }

        public bool Is_Anonymous { get; set; }
        //public bool IsPrivate { get; set; }

        public int? SchoolCategory { get; set; }
        public List<SelectListItem> SchoolCategorySelectList
        {
            get
            {
                List<SelectListItem> listSchoolCategory = new List<SelectListItem>();
                listSchoolCategory.Add(new SelectListItem() { Text = "Public", Value = "1", Selected=true });
                listSchoolCategory.Add(new SelectListItem() { Text = "Private", Value = "2" });
                listSchoolCategory.Add(new SelectListItem() { Text = "Aeo Related", Value = "3" });
                return listSchoolCategory;
                //return ListOfProvinces.Select(province => new SelectListItem() { Text = province.Province_Name, Value = province.Province_Id.ToString(), Selected = province.Province_Name.Equals("Punjab", StringComparison.OrdinalIgnoreCase) }).ToList();
            }
        }
        public List<DbProvince> ListOfProvinces {
            get;    
            set; 
        }

        public List<DbDivision> ListOfDivision
        {
            get;
            set;
        }
        public List<DbTehsil> ListOfTehsils
        {
            get; 
            set;
        }
        public string HomeShopNo { get; set; }
        public string StreetNameNo { get; set; }
        public string LocalityArea { get; set; }
        public List<SelectListItem> ProvinceSelectList
        {
            get
            {
                return ListOfProvinces.Select(province => new SelectListItem() { Text = province.Province_Name, Value = province.Province_Id.ToString(), Selected = province.Province_Name.Equals("Punjab", StringComparison.OrdinalIgnoreCase) }).ToList();
            }
        }

        public List<SelectListItem> DivisionSelectList
        {
            get
            {
                if (ListOfDivision == null)
                {
                    return new List<SelectListItem>();
                }
                return ListOfDivision.Select(division => new SelectListItem() { Text = division.Division_Name, Value = division.Division_Id.ToString()/*, Selected = division.Division_Name.Equals("Punjab", StringComparison.OrdinalIgnoreCase)*/ }).ToList();
            }
        }
        

        public List<SelectListItem> TehsilSelectList
        {
            get
            {
                if (ListOfTehsils == null)
                {
                    ListOfTehsils = new List<DbTehsil>();
                }
                return ListOfTehsils.Select(tehsil => new SelectListItem() { Text = tehsil.Tehsil_Name, Value = tehsil.Tehsil_Id.ToString() }).ToList();
            }
        }

        public List<DbComplaintType> ListOfComplaintTypes { get; set; }

        public List<SelectListItem> ComplaintCategoriesSelectList
        {
            get
            {
                return ListOfComplaintTypes.Select(complaintType => new SelectListItem() { Text = complaintType.Name, Value = complaintType.Complaint_Category.ToString() }).ToList();
            }
        }

        [RequiredIfTrue("hasDepartment", ErrorMessage = "Required")]
        public int? departmentId { get; set; }

        public bool hasDepartment { get; set; }
        public List<DbDepartment> ListOfDepartment { get; set; }
        public List<SelectListItem> DepartmentSelectList
        {
            get
            {
                return
                    ListOfDepartment.Select(
                        department =>
                            new SelectListItem() { Text = department.Name, Value = department.Id.ToString() })
                        .ToList();
            }
        }


        #region DynamicFields

        public int DynamicFieldsCount { get; set; }
        public int MinDynamicFormPriority { get; set; }

        public int MaxDynamicFormPriority { get; set; }

        //private List<VmDynamicField> _listDynamicFields { get; set; }

        public List<VmDynamicField> ListDynamicFields { get; set; }

        

        public List<VmDynamicTextbox> ListDynamicTextBox { get; set; }

        public List<VmDynamicLabel> ListDynamicLabel { get; set; }
        public List<VmDynamicDropDownList> ListDynamicDropDown { get; set; }

        public List<VmDynamicDropDownListServerSide> ListDynamicDropDownServerSide { get; set; }



        public List<VmCheckBox> listDynamicCheckBox { get; set; }
        //public List<CustomViewModel> CustomModel { get; set; }



        #endregion

        public void PopulateListDynamicFields()
        {
            ListDynamicFields = new List<VmDynamicField>();

            if (ListDynamicTextBox != null && ListDynamicTextBox.Count > 0)
            {
                ListDynamicFields.AddRange(ListDynamicTextBox);
            }
            if (ListDynamicLabel != null && ListDynamicLabel.Count > 0)
            {
                ListDynamicFields.AddRange(ListDynamicLabel);
            }
            if (ListDynamicDropDown != null && ListDynamicDropDown.Count > 0)
            {
                ListDynamicFields.AddRange(ListDynamicDropDown);
            }
            if (ListDynamicDropDownServerSide != null && ListDynamicDropDownServerSide.Count > 0)
            {
                ListDynamicFields.AddRange(ListDynamicDropDownServerSide);
            }
            ListDynamicFields.OrderBy(n => n.Priority);
        }

        public int GetHierarchyVal(Config.Hierarchy hierarchyId)
        {
            switch (hierarchyId)
            {
                case Config.Hierarchy.Province:
                    return Convert.ToInt32(this.Province_Id);
                    break;

                case Config.Hierarchy.Division:
                    return Convert.ToInt32(this.Division_Id);
                    break;

                case Config.Hierarchy.UnionCouncil:
                    return Convert.ToInt32(this.UnionCouncil_Id);
                    break;

                case Config.Hierarchy.District:
                    return Convert.ToInt32(this.District_Id);
                    break;

                case Config.Hierarchy.Tehsil:
                    return Convert.ToInt32(this.Tehsil_Id);
                    break;
                case Config.Hierarchy.Ward:
                    return Convert.ToInt32(this.Ward_Id);
                    break;
            }

            return 0;
        }


        /*public static VmComplaintBase GetDeepCopy(VmComplaintBase obj)
        {
            
        }*/


        /*
        public DateTime? Complaint_Assigned_Date { get; set; }

        public DateTime? Completed_Date { get; set; }

        public DateTime? Updated_Date { get; set; }

        public int? Updated_By { get; set; }

        public bool Is_Deleted { get; set; }

        public DateTime? Date_Deleted { get; set; }

        public int? Deleted_By { get; set; }
        */


        /*
        [Required]
        [Display(Name = "District")]
        public int? districtID { get; set; }
        public List<SelectListItem> listDistricts;

        [Required]
        [Display(Name = "Tehsil")]
        public int? tehsilID { get; set; }
        public List<SelectListItem> listTehsils;

        [Required]
        [Display(Name = "UC")]
        public int? unionCouncilID { get; set; }
        public List<SelectListItem> listUnionCouncil;

        [Required]
        [Display(Name = "Type")]
        public int? typeID { get; set; }
        public List<SelectListItem> listType;

        [Required]
        [Display(Name = "SubType")]
        public int? subtypeID { get; set; }
        public List<SelectListItem> listSubtype;

        [StringLength(250)]
        [Required(ErrorMessage = "Please provide Street No")]
        [Display(Name = "StreetNo")]
        public string streetNo { get; set; }

        [StringLength(250)]
        [Required(ErrorMessage = "Please provide Locality")]
        [Display(Name = "Locality")]
        public string locality { get; set; }

        [StringLength(1000)]
        [Required(ErrorMessage = "Please provide Address")]
        [Display(Name = "Address")]
        public string address { get; set; }

        [StringLength(2000)]
        [Required(ErrorMessage = "Please provide Comments")]
        [Display(Name = "Comments")]
        public string comments { get; set; }
        */

    }
}