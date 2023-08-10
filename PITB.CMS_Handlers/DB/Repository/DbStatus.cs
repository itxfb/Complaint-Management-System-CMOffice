using PITB.CMS_Common;
using PITB.CMS_Models.DB;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;


namespace PITB.CMS_Handlers.DB.Repository
{
    public class RepoDbStatus
    {
        #region Helpers

        public static List<DbStatus> GetAll()
        {
            try
            {
                using (DBContextHelperLinq db =new DBContextHelperLinq() )
                {
                    return db.DbStatuses.AsNoTracking().ToList();
                }
            }
            catch (Exception)
            {
                
                throw;
            }
        }

        public static DbStatus GetById(int statusId)
        {
            try
            {
                using (DBContextHelperLinq db = new DBContextHelperLinq())
                {
                    return db.DbStatuses.AsNoTracking().Where(n=>n.Complaint_Status_Id==statusId).FirstOrDefault();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public static List<DbStatus> GetByStatusIds(List<int> listStatusIds)
        {
            try
            {
                using (DBContextHelperLinq db = new DBContextHelperLinq())
                {
                    DbStatus listDbStatus = null;
                    return db.DbStatuses.AsNoTracking().Where(n => listStatusIds.Contains(n.Complaint_Status_Id)).ToList();
                   
                    //return db.DbStatuses.AsNoTracking().Where(m => m.Campaigns.Contains(campaignId.ToString())).ToList();
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public static List<DbStatus> GetByStatusIds(List<Config.ComplaintStatus> listStatusIds)
        {
            try
            {
                using (DBContextHelperLinq db = new DBContextHelperLinq())
                {
                    return db.DbStatuses.AsNoTracking().Where(n => listStatusIds.Contains((Config.ComplaintStatus)n.Complaint_Status_Id)).ToList().ToList();
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public static List<DbStatus> GetByStatusIds(List<int> listStatusIds, List<DbStatus> listDbStatuses )
        {
            try
            {
                //using (DBContextHelperLinq db = new DBContextHelperLinq())
                //{
                 //   DbStatus listDbStatus = null;
                    return listDbStatuses.Where(n => listStatusIds.Contains(n.Complaint_Status_Id)).ToList();

                    //return db.DbStatuses.AsNoTracking().Where(m => m.Campaigns.Contains(campaignId.ToString())).ToList();
                //}
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public static List<DbStatus> GetByCampaignIds(List<int> listCampaignIds)
        {
            try
            {
                using (DBContextHelperLinq db = new DBContextHelperLinq())
                {

                    List<DbStatus> listDbStatus = db.DbStatuses.AsNoTracking().Select(n => n).ToList();
                    listDbStatus = listDbStatus.Where(
                        n => Utility.GetIntList(n.Campaigns).ToList().Any(p => listCampaignIds.Contains(p))).ToList();
                    return listDbStatus;
                    //return db.DbStatuses.AsNoTracking().Where(m => m.Campaigns.Contains(campaignId.ToString())).ToList();
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public static List<DbStatus> GetByCampaignIds(List<int> listCampaignIds, List<DbStatus> listDbStatuses)
        {
            try
            {
                //using (DBContextHelperLinq db = new DBContextHelperLinq())
                //{

                List<DbStatus> listDbStatus = listDbStatuses.Select(n => n).ToList();
                    listDbStatus = listDbStatus.Where(
                        n => Utility.GetIntList(n.Campaigns).ToList().Any(p => listCampaignIds.Contains(p))).ToList();
                    return listDbStatus;
                    //return db.DbStatuses.AsNoTracking().Where(m => m.Campaigns.Contains(campaignId.ToString())).ToList();
                //}
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public static List<DbStatus> GetByCampaignId(int campaignId)
        {
            try
            {
                using (DBContextHelperLinq db = new DBContextHelperLinq())
                {
                    List<DbStatus> listDbStatus = db.DbStatuses.AsNoTracking().Select(n => n).ToList();
                    listDbStatus = listDbStatus.Where(
                        n => Utility.GetIntList(n.Campaigns).ToList().Any(p => p == campaignId)).ToList();
                    return listDbStatus;
                      
                    //return db.DbStatuses.AsNoTracking().Where(m => Utility.GetIntList(m.Campaigns).ToList().Any(p => p==campaignId)).ToList();
                    //return db.DbStatuses.AsNoTracking().Where(m=>m.Campaigns.Contains (campaignId.ToString())).ToList();
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        public static DbStatus GetByCampaignIdAndStatusId(string campaignId,int statusId)
        {
            try
            {
                using (DBContextHelperLinq db = new DBContextHelperLinq())
                {
                    return
                        db.DbStatuses.AsNoTracking()
                            .FirstOrDefault(m => campaignId.Contains(m.Campaigns) && m.Complaint_Status_Id == statusId);
                }
            }
            catch (Exception)
            {

                throw;
            }
        }


        public static void SetAlteredStatus(List<DbStatus> listDbStatuses, string matchedStr, string strtoReturn)
        {
            foreach (DbStatus dbStatus in listDbStatuses)
            {
                dbStatus.Status = Utility.GetAlteredStatus(dbStatus.Status, matchedStr, strtoReturn);
            }

        }
        #endregion
    }
}