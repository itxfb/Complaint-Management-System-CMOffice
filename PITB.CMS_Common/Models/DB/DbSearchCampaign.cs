using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PITB.CMS_Common.Models
{
    
    [Table("PITB.Campaign_Tags")]
    public partial class DbSearchCampaign
    {
        [Key]
        [Column("Tag_Id")]
        public int Tag_Id { get; set; }

        [StringLength(50)]
        public string Tag_Name { get; set; }


        public int Campaign_Id { get; set; }

        public DateTime Created_At { get; set; }
        public int Created_By { get; set; }
        public bool Is_Active { get; set; }
    }
}
