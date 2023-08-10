using System.Linq;
using PITB.CRM_API.Helper.Database;


namespace PITB.CRM_API.Models.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("PITB.School_Category_User_Mapping")]
    public class DbSchoolCategoryUserMapping
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

        public static List<DbSchoolCategoryUserMapping> Get(Config.Campaign campaignId, Config.Categories categoryType, List<int?> listCategoryId, Config.SchoolEducationAssignedTo schoolEducationAssignedTo)
        {
            try
            {
                using (DBContextHelperLinq db = new DBContextHelperLinq())
                {
                    return db.DbSchoolCategoryUserMapping.Where(n => n.Campaign_Id == (int)campaignId && n.Category_Type == (int)categoryType
                        &&  listCategoryId.Contains(n.Category_Id) && n.Assigned_To == (int)schoolEducationAssignedTo).ToList();
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }
    }
}