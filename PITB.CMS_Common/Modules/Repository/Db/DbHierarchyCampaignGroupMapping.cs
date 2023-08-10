﻿using PITB.CMS_Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace PITB.CMS_Common.Models
{

    public partial class DbHierarchyCampaignGroupMapping
    {
        public static int? GetModelByCampaignId(int campId, Config.Hierarchy hierarchyId)
        {
            try
            {
                using (var db = new DBContextHelperLinq())
                {
                    DbHierarchyCampaignGroupMapping dbHierarchyCampaignGroupMapping = db.DbHierarchyCampaignGroupMappings.AsNoTracking()
                        .Where(n => n.Campaign_Id == campId && n.Hierarchy_Id == (int)hierarchyId)
                        .FirstOrDefault();

                    if (dbHierarchyCampaignGroupMapping != null && dbHierarchyCampaignGroupMapping.Group_Id != null)
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
        public static int GetGroupIdForCampaignAndHierarchyIds(int campaignId, int hierarchyId)
        {
            try
            {
                using (DBContextHelperLinq db = new DBContextHelperLinq())
                {
                    var mapping = db.DbHierarchyCampaignGroupMappings.FirstOrDefault(x => x.Campaign_Id == campaignId && x.Hierarchy_Id == hierarchyId);
                    if (mapping == null)
                    {
                        return -1;
                    }
                    return mapping.Group_Id;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static List<DbHierarchyCampaignGroupMapping> GetModelByCampaignId(int campId)
        {
            try
            {
                List<DbHierarchyCampaignGroupMapping> listDbHierarchyCampaignGroupMapping = null;
                using (var db = new DBContextHelperLinq())
                {
                    listDbHierarchyCampaignGroupMapping =
                        db.DbHierarchyCampaignGroupMappings.AsNoTracking()
                            .Where(n => n.Campaign_Id == campId)
                            .ToList();
                }
                return listDbHierarchyCampaignGroupMapping;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }

    #region #region From API
    public partial class DbHierarchyCampaignGroupMapping
    {
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
                    if (dbHierarchyCampaignGroupMapping!=null /*&& dbHierarchyCampaignGroupMapping.Group_Id != null*/)
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
    #endregion
}