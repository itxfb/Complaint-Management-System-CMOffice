using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PITB.CMS_Common.Models
{
    [Table("PITB.Campiagn_Departments")]
    public partial class DbCampaignDepartment
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Logo { get; set; }
        public string IsActive { get; set; }
    }
}
