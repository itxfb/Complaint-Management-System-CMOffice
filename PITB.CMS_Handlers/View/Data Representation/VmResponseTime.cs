using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace PITB.CMS_Handlers.View.Data_Representation
{
    public class VmResponseTime
    {
        [Required]
        [Display(Name="Hierarchy Level")]
        public string SelectedHierarchyId { get; set; }
        public IEnumerable<SelectListItem> HierarchyList { get; set; }


        [Required]
        [Display(Name = "Campaigns")]
        public string SelectedCampaignIds { get; set; }
        public IEnumerable<SelectListItem> CampaignList { get; set; }

        [Required]
        [Display(Name = "Escalation Level")]
        public string SelectedEscalationLevel { get; set; }
        public IEnumerable<SelectListItem> EscalationLevelList { get; set; }
    }
}