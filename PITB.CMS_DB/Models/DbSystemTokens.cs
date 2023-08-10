using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PITB.CMS_DB.Models
{

    [Table("PITB.System_Tokens")]
    public partial class DbSystemTokens
    {
        [Column(TypeName = "numeric")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public decimal Id { get; set; }

        public int? Type { get; set; }

        public int? Client_System_Id { get; set; }

        [StringLength(1000)]
        public string Token_Value { get; set; }

        [StringLength(1000)]
        public string Token_Rev_Value { get; set; }

        public DateTime? Created_Date_Time { get; set; }

        public DateTime? Expiry_Date_Time { get; set; }

        public TimeSpan? Token_Expiry_Time { get; set; }

        public int? Token_Expiry_Exchanges { get; set; }

        public int? Token_Current_Exchange { get; set; }

        public bool? Is_Active { get; set; }
    }
}
