namespace PITB.CRM_API.Models.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using PITB.CRM_API.Helper.Database;
    using System.Linq;

    [Table("PITB.Hierarchy_Campaign_Group_Mapping")]
    public class DbHierarchyCampaignGroupMapping
    {
        [Key]
        [Column("Id")]
        public int Id { get; set; }

        public int Hierarchy_Id { get; set; }

        public int Campaign_Id { get; set; }

        public int Group_Id { get; set; }

        /*
        public static int? GetModelByCampaignId(int campId)
        {
            try
            {
                using (var db = new DBContextHelperLinq())
                {
                    DbHierarchyCampaignGroupMapping dbHierarchyCampaignGroupMapping =  db.DbHierarchyCampaignGroupMappings.AsNoTracking()
                        .Where(n => n.Campaign_Id == campId)
                        .FirstOrDefault();
                    if (dbHierarchyCampaignGroupMapping.Group_Id != null)
                    {
                        return dbHierarchyCampaignGroupMapping.Group_Id;
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
        */

        public static int? GetModelByCampaignIdAndHierarchyId(int campId, Config.Hierarchy hierarchyId)
        {
            try
            {
                using (var db = new DBContextHelperLinq())
                {
                    DbHierarchyCampaignGroupMapping dbHierarchyCampaignGroupMapping = db.DbHierarchyCampaignGroupMappings.AsNoTracking()
                        .Where(n => n.Campaign_Id == campId && n.Hierarchy_Id == (int)hierarchyId)
                        .FirstOrDefault();
                    if (dbHierarchyCampaignGroupMapping.Group_Id != null)
                    {
                        return dbHierarchyCampaignGroupMapping.Group_Id;
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