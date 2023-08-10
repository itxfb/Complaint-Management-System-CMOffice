using PITB.CMS.Helper.Database;

namespace PITB.CMS.Models.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    [Table("PITB.Category_Group_Mapping")]
    public class DbCategoryGroupMapping
    {
        [Key]
        [Column("Id")]
        public int Id { get; set; }

        public int Type_Id { get; set; }

        public int Campaign_Id { get; set; }

        public int Group_Id { get; set; }

        public static int? GetModelByCampaignIdAndTypeId(int campId, Config.ComplaintType typeId)
        {
            try
            {
                using (var db = new DBContextHelperLinq())
                {
                    DbCategoryGroupMapping dbCategoryCampaignGroupMapping = db.DbCategoryGroupMappings.AsNoTracking()
                        .Where(n => n.Campaign_Id == campId && n.Type_Id == (int)typeId)
                        .FirstOrDefault();
                    if (dbCategoryCampaignGroupMapping!=null && dbCategoryCampaignGroupMapping.Group_Id != null)
                    {
                        return dbCategoryCampaignGroupMapping.Group_Id;
                    }
                    else
                    {
                        return null;
                    }
                    //return db.DbHierarchyCampaignGroupMappings.AsNoTracking().Where(m => m == id).FirstOrDefault();
                }

            }
            catch (Exception ex)
            {
                return null;
            }
        }

    }
}