using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
namespace PITB.CMS_Common.Models.View.Data_Representation
{
    public class VmAreawiseCategoryModel
    {
        [Required]
        [Display(Name = "Campaigns")]
        public string SelectedCampaignIds { get; set; }
        public IEnumerable<SelectListItem> CampaignList { get; set; }

        [Required]
        [Display(Name = "Status Level")]
        public string SelectedStatusLevel { get; set; }
        public IEnumerable<SelectListItem> StatusLevelList { get; set; }
    }
}