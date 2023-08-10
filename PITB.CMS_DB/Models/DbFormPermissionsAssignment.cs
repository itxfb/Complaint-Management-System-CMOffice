using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace PITB.CMS_DB.Models
{

    [Table("PITB.Form_Permissions_Assignment")]
    public partial class DbFormPermissionsAssignment
    {
        public int Id { get; set; }

        public int? Control_Id { get; set; }

        [StringLength(100)]
        public string Permission_Id { get; set; }

        [StringLength(500)]
        public string Permission_Value { get; set; }
    }
}
