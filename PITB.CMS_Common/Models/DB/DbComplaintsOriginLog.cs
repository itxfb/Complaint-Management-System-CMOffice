using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;

namespace PITB.CMS_Common.Models
{

    [Table("PITB.Complaints_Origin_Log")]
    public partial class DbComplaintsOriginLog
    {
        public int Id { get; set; }

        public int? Complaint_Id { get; set; }

        public int? Origin_HierarchyId { get; set; }

        public int? Origin_UserHierarchyId { get; set; }

        public int? Origin_UserCategoryId1 { get; set; }

        public int? Origin_UserCategoryId2 { get; set; }

        public int? Source_Id { get; set; }

        public int? Event_Id { get; set; }

        public DateTime? Created_DateTime { get; set; }

        public bool? Is_AssignedToOrigin { get; set; }

        public bool? IsCurrentlyActive { get; set; }
    }
}