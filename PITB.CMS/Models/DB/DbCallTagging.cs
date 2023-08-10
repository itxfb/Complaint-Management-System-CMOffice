using System.Linq;
using PITB.CMS.Helper.Database;

namespace PITB.CMS.Models.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("PITB.Call_Tagging")]
    public partial class DbCallTagging
    {
        [Key]
        [Column("Id")]
        public int Id { get; set; }

        public int? Record_ID { get; set; }

        [StringLength(100)]
        public string Campaign_ID { get; set; }

        public DateTime? Start_Time { get; set; }

        public DateTime? End_Time { get; set; }

        public string Duration { get; set; }

        [StringLength(100)]
        public string Caller_Id { get; set; }

        [StringLength(100)]
        public string Caller_Name { get; set; }

        //[StringLength(100)]
        public int? Department_ID { get; set; }

        //[StringLength(100)]
        public int? PPMRP_Service_ID { get; set; }

        [StringLength(200)]
        public string Recording_ID { get; set; }

        public int? Agent_ID { get; set; }

        public string Agent_Name { get; set; }


        #region Helpers

        public static DbCallTagging GetByRecordId(int recordId)
        {
            try
            {
                using (var db = new DBContextHelperLinq())
                {
                    return db.DbCallTagging.AsNoTracking().Where(n => n.Id == recordId).First();
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public static DbCallTagging GetByRecordId(int recordId, DBContextHelperLinq db)
        {
            try
            {
                return db.DbCallTagging.AsNoTracking().Where(n => n.Id == recordId).First();
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        #endregion
    }
}
