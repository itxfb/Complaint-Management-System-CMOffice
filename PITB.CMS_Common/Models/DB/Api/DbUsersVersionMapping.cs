using System.Data.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace PITB.CMS_Common.Models
{

    [Table("PITB.Users_Version_Mapping")]
    public partial class DbUsersVersionMapping
    {
        [Key]
        [Column("Id")]
        public int Id { get; set; }

        public int User_Type { get; set; }

        public int User_Id { get; set; }

        public int Platform_Id { get; set; }

        public int App_Id { get; set; }

        public int App_Version { get; set; }
    }
}