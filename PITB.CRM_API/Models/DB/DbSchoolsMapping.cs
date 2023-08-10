using System.Linq;

namespace PITB.CRM_API.Models.DB
{
    using PITB.CRM_API.Helper.Database;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    //[Table("dbo.Schools_Mapping")]
    /*
    public partial class DbSchoolsMapping
    {
        [StringLength(100)]
        public string school_emis_code { get; set; }

        [StringLength(255)]
        public string school_name { get; set; }

        [StringLength(255)]
        public string school_level { get; set; }

        [StringLength(255)]
        public string school_gender { get; set; }

        public int? district_id { get; set; }

        [StringLength(255)]
        public string district_name { get; set; }

        public int? tehsil_id { get; set; }

        [StringLength(255)]
        public string tehsil_name { get; set; }

        public int? markaz_id { get; set; }

        [StringLength(255)]
        public string markaz_name { get; set; }

        public int? System_District_Id { get; set; }

        public int? System_Tehsil_Id { get; set; }

        public int? System_Markaz_Id { get; set; }

        public bool? System_School_Gender { get; set; }

        public short? School_Type { get; set; }

        // public virtual string School_Type_Str { get; set; }

        public int Id { get; set; }




        #region HelperMethods
        public static DbSchoolsMapping GetById(int id)
        {
            try
            {
                using (var db = new DBContextHelperLinq())
                {
                    return db.DbSchoolsMapping.AsNoTracking().Where(m => m.Id == id).FirstOrDefault();
                }

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static List<DbSchoolsMapping> GetByEmisCode(List<string> listEmisCode)
        {
            try
            {
                using (var db = new DBContextHelperLinq())
                {
                    return db.DbSchoolsMapping.AsNoTracking().Where(m => listEmisCode.Contains(m.school_emis_code)).ToList();
                }

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        #endregion
    }
    */
}
