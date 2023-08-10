using System.Linq;
using Newtonsoft.Json;
using PITB.CRM_API.Helper.Database;

namespace PITB.CRM_API.Models.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("PITB.Statuses")]
    public partial class DbStatuses
    {
        public int Id { get; set; }

        [StringLength(50)]
        public string Status { get; set; }
        [JsonIgnore]

        [StringLength(100)]
        public string Campaigns { get; set; }
        [JsonIgnore]

        public bool? IsEscalatable { get; set; }


        public int? EscalationStatus { get; set; }

        public string EscalationAction { get; set; }

        [JsonIgnore]

        public bool? ForceEscalateOnStatusChange { get; set; }
        
        #region HelperMethods

            public static DbStatuses GetById(int statusId)
            {
                try
                {
                    using (DBContextHelperLinq db = new DBContextHelperLinq())
                    {
                        return db.DbStatuses.AsNoTracking().Where(n => n.Id == statusId).FirstOrDefault();
                    }
                }
                catch (Exception)
                {

                    throw;
                }
            }

            public static List<DbStatuses> GetByCampaignId(int campaignId)
            {
                try
                {
                    using (DBContextHelperLinq db = new DBContextHelperLinq())
                    {
                        return db.DbStatuses.AsNoTracking().ToList().Where(n => n.Campaigns.Split(',').Contains(campaignId.ToString())).ToList();
                    }
                }
                catch (Exception ex)
                {

                    throw;
                }
            }

            public static List<DbStatuses> GetByStatusIds(List<Config.ComplaintStatus> listStatusIds )
            {
                try
                {
                    using (DBContextHelperLinq db = new DBContextHelperLinq())
                    {
                        return db.DbStatuses.AsNoTracking().Where(n=>listStatusIds.Contains((Config.ComplaintStatus)n.Id)).ToList().ToList();
                    }
                }
                catch (Exception ex)
                {

                    throw;
                }
            }

            public static List<DbStatuses> GetByStatusIds(List<int> listStatusIds)
            {
                try
                {
                    using (DBContextHelperLinq db = new DBContextHelperLinq())
                    {
                        DbStatuses listDbStatus = null;
                        return db.DbStatuses.AsNoTracking().Where(n => listStatusIds.Contains(n.Id)).ToList();

                        //return db.DbStatuses.AsNoTracking().Where(m => m.Campaigns.Contains(campaignId.ToString())).ToList();
                    }
                }
                catch (Exception ex)
                {

                    throw;
                }
            }

            public static List<DbStatuses> GetByCampaignIds(List<int> listCampaignIds)
            {
                try
                {
                    using (DBContextHelperLinq db = new DBContextHelperLinq())
                    {

                        List<DbStatuses> listDbStatus = db.DbStatuses.AsNoTracking().Select(n => n).ToList();
                        listDbStatus = listDbStatus.Where(
                            n => Utility.GetIntList(n.Campaigns).ToList().Any(listCampaignIds.Contains)).ToList();
                        return listDbStatus;
                        //return db.DbStatuses.AsNoTracking().Where(m => m.Campaigns.Contains(campaignId.ToString())).ToList();
                    }
                }
                catch (Exception ex)
                {

                    throw;
                }
            }

            
        #endregion
    }
}
