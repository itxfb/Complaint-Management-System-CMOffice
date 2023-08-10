using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace PITB.CMS_Models.DB
{

    [Table("PITB.Category_Group_Mapping")]
    public class DbCategoryGroupMapping
    {
        [Key]
        [Column("Id")]
        public int Id { get; set; }

        public int Type_Id { get; set; }

        public int Campaign_Id { get; set; }

        public int Group_Id { get; set; }
    }
}