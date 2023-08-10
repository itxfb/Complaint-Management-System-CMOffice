using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;



namespace PITB.CMS_Common.Models
{

    [Table("PITB.School_Category_User_Mapping")]
    public partial class DbSchoolCategoryUserMapping
    {
        [Key]
        [Column("Id")]
        public int Complaint_Category { get; set; }

        public int? Campaign_Id { get; set; }


        public int? Category_Type { get; set; }

        public int? Category_Id { get; set; }

        public int? Assigned_To { get; set; }

        //[NotMapped]
        //public string Category_UrduName { get; set; }
    }
}