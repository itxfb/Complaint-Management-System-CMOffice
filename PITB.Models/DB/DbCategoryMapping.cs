using System.Data.Entity;
using System.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace PITB.CMS_Models.DB
{

    [Table("PITB.Category_Mapping")]
    public class DbCategoryMapping
    {
        public int Id { get; set; }

        public int? Campaign_Id { get; set; }

        [ForeignKey("ParentCategory")]
        public int? Parent_Id { get; set; }

        [ForeignKey("ChildCategory")]
        public int? Child_Id { get; set; }

        public int? Hierarchy { get; set; }

        [StringLength(200)]
        public string Tag_Id { get; set; }

        [StringLength(200)]
        public string Parent_Name { get; set; }

        [StringLength(200)]
        public string ChildName { get; set; }


        public virtual DbCategory ParentCategory { get; set; }

        public virtual DbCategory ChildCategory { get; set; }
    }
}
