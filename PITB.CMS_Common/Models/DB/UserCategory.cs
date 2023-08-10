using System.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace PITB.CMS_Common.Models
{

    [Table("PITB.UserCategory2")]
    public partial class UserCategory
    {
        public int Id { get; set; }

        public int? User_Id { get; set; }

        public int? Parent_Category_Id { get; set; }

        public int? Child_Category_Id { get; set; }

        public int? Category_Hierarchy { get; set; }
    }
}
