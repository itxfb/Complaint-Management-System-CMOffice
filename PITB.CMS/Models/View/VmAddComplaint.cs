using System;
using System.CodeDom;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Web.ModelBinding;
using System.Web.Services;
using Amazon.EC2.Model;
using ModelStateDictionary = System.Web.Mvc.ModelStateDictionary;
using Foolproof;

namespace PITB.CMS.Models.View
{
    public class VmAddComplaint 
    {
        public VmComplaint ComplaintVm { get; set; }
        public VmPersonalInfo PersonalInfoVm { get; set; }
        public VmSuggestion SuggestionVm { get; set; }
        public VmInquiry InquiryVm { get; set; }


    

        public string currentComplaintTypeTab { get; set; }

        public const string TabComplaint = "Complaint";
        public const string TabSuggestion = "Suggestion";
        public const string TabInquiry = "Inquiry";
        public const string TabInquiryVm = "Inquiry";


        public static void DiscartUnnecessaryValuesFromModelDictionary(ModelStateDictionary modelStateDict, VmAddComplaint vmAddComplaint, bool discartTehsilAndUc, List<string> listValuesToDiscart = null)
        {
            List<string> keyList = modelStateDict.Keys.ToList();

            if (listValuesToDiscart != null)
            {
                for (int i = 0; i < keyList.Count; i++)
                {
                    for (int j = 0; j < listValuesToDiscart.Count; j++)
                    {
                        if (keyList[i].ToLower().Contains(listValuesToDiscart[j]))
                        {
                            modelStateDict.Remove(keyList[i]);
                        }
                    }
                }
            }
            switch (vmAddComplaint.currentComplaintTypeTab)
            {
                case VmAddComplaint.TabComplaint:
                    for (int i = 0; i < keyList.Count; i++)
                    {
                        if (keyList[i].Contains("SuggestionVm") || keyList[i].Contains("InquiryVm"))
                        {
                            modelStateDict.Remove(keyList[i]);
                        }
                        if (discartTehsilAndUc)
                        {
                            if (keyList[i].Contains("Tehsil") || keyList[i].Contains("Union"))
                            {
                                modelStateDict.Remove(keyList[i]);
                            }
                        }
                    }
                break;

                case VmAddComplaint.TabSuggestion:
                    for (int i = 0; i < keyList.Count; i++)
                    {
                        if (keyList[i].Contains("ComplaintVm") || keyList[i].Contains("InquiryVm"))
                        {
                            modelStateDict.Remove(keyList[i]);
                        }
                        if (discartTehsilAndUc)
                        {
                            if (keyList[i].Contains("Tehsil") || keyList[i].Contains("Union"))
                            {
                                modelStateDict.Remove(keyList[i]);
                            }
                        }
                    }
                break;

                case VmAddComplaint.TabInquiryVm:
                    for (int i = 0; i < keyList.Count; i++)
                    {
                        if (keyList[i].Contains("ComplaintVm") || keyList[i].Contains("SuggestionVm"))
                        {
                            modelStateDict.Remove(keyList[i]);
                        }
                        if (discartTehsilAndUc)
                        {
                            if (keyList[i].Contains("Tehsil") || keyList[i].Contains("Union"))
                            {
                                modelStateDict.Remove(keyList[i]);
                            }
                        }
                    }
                break;

                default:
                break;
            }
            
            //modelState.Remove("")
        }

        public VmAddComplaint()
        {
            ComplaintVm=new VmComplaint();
            SuggestionVm = new VmSuggestion();
            InquiryVm = new VmInquiry();
            PersonalInfoVm = new VmPersonalInfo();
        }

        

        public void HardCopyComplaintIntoSuggestionAndInquiry()
        {
            this.SuggestionVm.ListOfProvinces = this.ComplaintVm.ListOfProvinces;
            this.SuggestionVm.ListOfComplaintTypes = this.ComplaintVm.ListOfComplaintTypes;
            this.SuggestionVm.ListOfDepartment = this.ComplaintVm.ListOfDepartment;
            this.SuggestionVm.hasDepartment = this.ComplaintVm.hasDepartment;

            this.InquiryVm.ListOfProvinces = this.ComplaintVm.ListOfProvinces;
            this.InquiryVm.ListOfComplaintTypes = this.ComplaintVm.ListOfComplaintTypes;
            this.InquiryVm.ListOfDepartment = this.ComplaintVm.ListOfDepartment;
            this.InquiryVm.hasDepartment = this.ComplaintVm.hasDepartment;
        }

    }
}