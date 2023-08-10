using System.Data.Entity;
using System.Linq;

namespace PITB.CMS.Models.DB
{
    using PITB.CMS.Helper.Database;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("PITB.Form_Controls")]
    public partial class DbFormControl
    {
        public int Id { get; set; }

        public int? Campaign_Id { get; set; }

        public int? Control_Type { get; set; }

        public int? Priority { get; set; }

        [StringLength(200)]
        public string Field_Name { get; set; }

        [StringLength(200)]
        public string Tag_Id { get; set; }

        [ForeignKey("Control_Id")]
        public virtual List<DbFormPermissionsAssignment> ListDbFormPermission { get; set; }
        public static List<DbFormControl> GetBy(int campaignId, string TagId)
        {
            try
            {
                using (DBContextHelperLinq db = new DBContextHelperLinq())
                {
                   
                    List<DbFormControl> listDbFC = db.DbFormControl.AsNoTracking().Where(m => m.Campaign_Id == campaignId && m.Tag_Id == TagId).OrderBy(m => m.Priority).Include(n => n.ListDbFormPermission).ToList();//db.Form_Controls.Select(n => n).ToList();
                    return listDbFC;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
