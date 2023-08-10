using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PITB.CMS_Common.Models.View
{
    public class VmTagEdit
    {

        public int RecordId { get; set; }

        [Required]
        [RegularExpression(@"^([a-zA-Z0-9._-]+)$", ErrorMessage = "Username cannot contain special character")]
        public string CallerName { get; set; }


        [Required]
        public int? DepartmentId { get; set; }

        public List<SelectListItem> DepartmentList { get; set; }

        [Required]
        public int? PpmrpServiceId { get; set; }

        public List<SelectListItem> PpmrpServiceList { get; set; }

        public bool HasAlreadyAdded { get; set; }
    }

}
