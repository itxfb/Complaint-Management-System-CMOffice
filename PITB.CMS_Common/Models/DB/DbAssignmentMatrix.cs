using System.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;
using System.Dynamic;

namespace PITB.CMS_Common.Models
{

    [Table("PITB.AssignmentMatrix")]
    public partial class DbAssignmentMatrix
    {
        [Column(TypeName = "numeric")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public decimal Id { get; set; }

        public int? CampaignId { get; set; }

        public int? CategoryType { get; set; }

        public int? CategoryId { get; set; }

        public int? CategoryDep1 { get; set; }

        public int? CategoryDep2 { get; set; }

        public int? HierarchyLevel { get; set; }

        public int? HierarchyId { get; set; }

        public int? FromSourceId { get; set; }

        public int? ToSourceId { get; set; }

        public int? ToUserSourceId { get; set; }

        public int? LevelId { get; set; }
        public double RetainingHours { get; set; }

        public string TagStr { get; set; }

        public bool? IsActive { get; set; }

        //[NotMapped]
        //public string[] ArrTag { get; set; }

        //[NotMapped]
        //public Dictionary<string, string> DictTag { get; set; }
    }
}
