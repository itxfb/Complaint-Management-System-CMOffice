using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Web.ModelBinding;
using System.Web.Services;
using ModelStateDictionary = System.Web.Mvc.ModelStateDictionary;

namespace PITB.CMS.Models.View.Wasa
{
    public class VmAddComplaintWasa 
    {
        public VmComplaintWasa ComplaintVm { get; set; }
        public VmPersonalInfoWasa PersonalInfoVm { get; set; }
        public VmSuggestionWasa SuggestionVm { get; set; }
        public VmInquiryWasa InquiryVm { get; set; }

        public string currentComplaintTypeTab { get; set; }

        public const string TabComplaint = "Complaint";
        public const string TabSuggestion = "Suggestion";
        public const string TabInquiryVm = "Inquiry";


        public static void DiscartUnnecessaryValuesFromModelDictionary(ModelStateDictionary modelStateDict, VmAddComplaintWasa vmAddComplaint, bool discartTehsilAndUc)
        {
           
            List<string> keyList = modelStateDict.Keys.ToList();

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

        public VmAddComplaintWasa()
        {
            ComplaintVm = new VmComplaintWasa();
            SuggestionVm = new VmSuggestionWasa();
            InquiryVm = new VmInquiryWasa();
            PersonalInfoVm = new VmPersonalInfoWasa();
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