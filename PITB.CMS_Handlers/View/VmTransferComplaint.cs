using Foolproof;
using Microsoft.SqlServer.Server;
using PITB.CMS_Common.Handler.Authentication;
using PITB.CMS_Models.Custom;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PITB.CMS_Handlers.View
{
    public class VmTransferComplaint
    {
        public int ComplaintId { get; set; }

        public int CampaignId { get; set; }

        [Required(ErrorMessage = "Transfer To is required")]
        public int? TransferToId { get; set; }
        public List<SelectListItem> ListTranferTo { get; set; }

        public int? HierarchyAddedId { get; set; }
        public List<SelectListItem> ListHierarchyAdded { get; set; }

        [Required(ErrorMessage = "Transfer comments are required")]
        public string AgentComments { get; set; }

        
        public VmTransferProvince VmTransferProvince { get; set; }

        public VmTransferDivision VmTransferDivision { get; set; }

        public VmTransferDistrict VmTransferDistrict { get; set; }

        public VmTransferTehsil VmTransferTehsil { get; set; }

        public VmTransferUc VmTransferUc { get; set; }

        public bool HasTransferedHistory { get; set; }
        public VmLastTransferedFrom VmLastTransferredFrom { get; set; }

        public VmTransferComplaint()
        {
            this.VmLastTransferredFrom =  new VmLastTransferedFrom();
            this.ListTranferTo = new List<SelectListItem>();
            this.ListHierarchyAdded = new List<SelectListItem>();
        }

        public int HierarchyTypeId
        {
            get
            {
                switch (TransferToId)
                {
                    case (int) Config.Hierarchy.Province:
                        return (int) this.VmTransferProvince.SelectedId;
                        break;

                    case (int)Config.Hierarchy.Division:
                        return (int)this.VmTransferDivision.SelectedId;
                        break;

                    case (int)Config.Hierarchy.District:
                        return (int) this.VmTransferDistrict.SelectedId;
                        break;

                    case (int)Config.Hierarchy.Tehsil:
                        return (int)this.VmTransferTehsil.SelectedId;
                        break;

                    case (int)Config.Hierarchy.UnionCouncil:
                        return (int)this.VmTransferUc.SelectedId;
                        break;
                }

                return (int)this.VmTransferProvince.SelectedId;
            }
        }
    }

    public class VmTransferParent
    {
        //public bool IsViewable { get; set; }

        [RequiredIfTrue("IsRequired", ErrorMessage = "Required")]
        public int? SelectedId { get; set; }

        public bool IsRequired { get; set; }
    }

    public class VmTransferProvince : VmTransferParent
    {
        // Province
        //public bool isViewable { get; set; };
        //public int? Province_Id { get; set; }
        public List<DbProvince> ListOfProvinces { get; set; }
        public List<SelectListItem> ProvinceSelectList
        {
            get
            {
                if (ListOfProvinces == null || ListOfProvinces.Count == 0)
                {
                    return new List<SelectListItem>();
                }
                else
                {
                    CMSCookie cookie = AuthenticationHandler.GetCookie();
                    List<SelectListItem> listProvince =
                        ListOfProvinces.Select(
                            n =>
                                new SelectListItem()
                                {
                                    Text = (cookie.listProvinceIds.Contains(n.Province_Id)) ? n.Province_Name + " [Me]" : n.Province_Name,
                                    Value =  n.Province_Id.ToString()/*,
                                    Selected = n.Province_Id == (int) cookie.ProvinceId*/
                                })
                            .ToList();
                    listProvince.Insert(0, new SelectListItem() {Text = "--Select--", Value = "", Selected = true});
                    return listProvince;
                }
            }
        }
    }

    public class VmTransferDivision : VmTransferParent
    {
        // Division
        // public int? Division_Id { get; set; }
        public List<DbDivision> ListOfDivisions { get; set; }
        public List<SelectListItem> DivisionSelectList
        {
            get
            {
                if (ListOfDivisions == null || ListOfDivisions.Count == 0) return new List<SelectListItem>();
                else
                {
                    CMSCookie cookie = AuthenticationHandler.GetCookie();
                    List<SelectListItem> listDivision =
                        ListOfDivisions.Select(
                            n =>
                                new SelectListItem()
                                {
                                    Text = (cookie.listDivisionIds.Contains(n.Division_Id)) ? n.Division_Name + " [Me]" : n.Division_Name,
                                    Value = n.Division_Id.ToString()/*,
                                    Selected = n.Province_Id == (int) cookie.ProvinceId*/
                                })
                            .ToList();
                    listDivision.Insert(0, new SelectListItem() { Text = "--Select--", Value = "", Selected = true });
                    return listDivision;
                }
            }
        }
    }

    public class VmTransferDistrict : VmTransferParent
    {
        // District
        //public int? District_Id { get; set; }

        public List<DbDistrict> ListOfDistrict { get; set; }
        public List<SelectListItem> DistrictSelectList
        {
            get
            {
                if (ListOfDistrict == null || ListOfDistrict.Count == 0) return new List<SelectListItem>();
                else
                {
                    CMSCookie cookie = AuthenticationHandler.GetCookie();
                    List<SelectListItem> listDistricts =
                        ListOfDistrict.Select(
                            n =>
                                new SelectListItem()
                                {
                                    Text = (cookie.listDistrictIds.Contains(n.District_Id)) ? n.District_Name + " [Me]" : n.District_Name,
                                    Value = n.District_Id.ToString()/*,
                                    Selected = n.Province_Id == (int) cookie.ProvinceId*/
                                })
                            .ToList();
                    listDistricts.Insert(0, new SelectListItem() { Text = "--Select--", Value = "", Selected = true });
                    return listDistricts;
                }
                /*List<SelectListItem> listDistricts = ListOfDistrict.Select(n => new SelectListItem() { Text = n.District_Name, Value = n.District_Id.ToString() }).ToList();
                listDistricts.Insert(0,new SelectListItem(){Text = "--Select--",Value = ""});
                return listDistricts;*/
            }
        }

    }

    public class VmTransferTehsil : VmTransferParent
    {
        // Tehsil
        //public int? Tehsil_Id { get; set; }

        public List<DbTehsil> ListOfTehsil { get; set; }
        public List<SelectListItem> TehsilSelectList
        {
            get
            {
                if (ListOfTehsil == null || ListOfTehsil.Count == 0) return new List<SelectListItem>();
                else
                {
                    CMSCookie cookie = AuthenticationHandler.GetCookie();
                    List<SelectListItem> listTehsils =
                        ListOfTehsil.Select(
                            n =>
                                new SelectListItem()
                                {
                                    Text = (cookie.listTehsilIds.Contains(n.Tehsil_Id)) ? n.Tehsil_Name + " [Me]" : n.Tehsil_Name,
                                    Value = n.Tehsil_Id.ToString()/*,
                                    Selected = n.Province_Id == (int) cookie.ProvinceId*/
                                })
                            .ToList();
                    listTehsils.Insert(0, new SelectListItem() { Text = "--Select--", Value = "", Selected = true });
                    return listTehsils;
                }
                //return ListOfTehsil.Select(n => new SelectListItem() { Text = n.Tehsil_Name, Value = n.Tehsil_Id.ToString() }).ToList();
            }
        }
    }

    public class VmTransferUc : VmTransferParent
    {
        // Union Council
        //public int? Uc_Id { get; set; }

        public List<DbUnionCouncils> ListOfUc { get; set; }
        public List<SelectListItem> UcSelectList
        {
            get
            {
                if (ListOfUc == null || ListOfUc.Count == 0) return new List<SelectListItem>();
                else
                {
                    CMSCookie cookie = AuthenticationHandler.GetCookie();
                    List<SelectListItem> listTehsils =
                        ListOfUc.Select(
                            n =>
                                new SelectListItem()
                                {
                                    Text = (cookie.listUcIds.Contains(n.UnionCouncil_Id)) ? n.Councils_Name + " [Me]" : n.Councils_Name,
                                    Value = n.UnionCouncil_Id.ToString()/*,
                                    Selected = n.Province_Id == (int) cookie.ProvinceId*/
                                })
                            .ToList();
                    listTehsils.Insert(0, new SelectListItem() { Text = "--Select--", Value = "", Selected = true });
                    return listTehsils;
                }
                //return ListOfUc.Select(n => new SelectListItem() { Text = n.Councils_Name, Value = n.UnionCouncil_Id.ToString() }).ToList();
            }
        }
    }

    public class VmLastTransferedFrom
    {
        public string UserName { get; set; }

        public string AssignedDate { get; set; }

        public string AssignedFromMedium { get; set; }

        public string AssignedFromMediumValue { get; set; }

        public string AssignedToMedium { get; set; }

        public string AssignedToMediumValue { get; set; }

        public string Comment { get; set; }
    }

}