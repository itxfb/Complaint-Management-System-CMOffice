using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PITB.CMS_Common.Models
{
    [Table("PublicUserOTP", Schema = "PITB")]
    public partial class DbPublicUserOTP
    {
        [Key]
        public int Id { get; set; }
        public string Email { get; set; }
        public string OTP { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsVerified { get; set; }
    }
}
