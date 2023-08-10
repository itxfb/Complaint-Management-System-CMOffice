using System.Data;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using Z.BulkOperations;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using PITB.CMS_Common;

namespace PITB.CMS_DB.Models
{

    public partial class DbComplaint
    {
        #region HelperMethods

        /*public static Config.CommandStatus AddEditComlaint(VmAddComplaint vm, bool isEditing)
        {
            //Dictionary<string,object> spParam = new Dictionary(string, object)();

            //const BindingFlags flags = BindingFlags.Public | BindingFlags.Instance;
            //foreach()

            Dictionary<string, object> dictValue = vm.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public).ToDictionary(prop => prop.Name, prop => prop.GetValue(vm, null));

            DBHelper.GetDataTableByStoredProcedure("asasd", null);
            return UtilityExtensions.GetStatus("1");
        }*/

        public static dynamic GetComplaintWithAllData(List<DbComplaint> listDbComplaint)
        {
            try
            {
                DBContextHelperLinq db = new DBContextHelperLinq();
                //List<int> listStatusChangeLogIds = db.DbComplaintStatusChangeLog.AsNoTracking().Where(n => n.Complaint_Id == Complaint_Id && n.StatusId == statusId).Select(n => n.Id).ToList();

                dynamic d = (from c in listDbComplaint
                                 //where st.Complaint_Id == Complaint_Id /*&& st.StatusId == statusId*/
                             select new
                             {
                                 DbComplaints = c,
                                 DbComplaintStatusChangeLog =
                                     (from st in db.DbComplaintStatusChangeLog
                                      where c.Id == st.Complaint_Id
                                      select st),

                                 //DbComplaintStatusChangeLog2 =
                                 //(from st in db.DbComplaintStatusChangeLog
                                 // from att in st.ListDbAttachments
                                 // where c.Id == st.Complaint_Id
                                 // select st,att),

                                 //DbComplaintStatusChangeLogAttachments =
                                 //from att in db.DbAttachments
                                 //where att.ReferenceType == (int)Config.AttachmentReferenceType.ChangeStatus && att.ReferenceTypeId == c.Id
                                 //select (att),

                                 DbAttachments =
                                     from att in db.DbAttachments
                                     where
                                         att.Complaint_Id == c.Id &&
                                         att.ReferenceType == (int)Config.AttachmentReferenceType.Add
                                     //&& att.ReferenceTypeId == statusId
                                     select (att)
                             }).ToList();

                //foreach (DbComplaintStatusChangeLog dbComplaintStatusLogs in (List<DbComplaintStatusChangeLog>)d.DbComplaintStatusChangeLog)
                //{
                //    dbComplaintStatusLogs.ListDbAttachments = 
                //}

                //var v = ( from st in db.DbComplaintStatusChangeLog
                //    join att in db.DbAttachments on st.Id equals att.ReferenceTypeId
                //    where st.Complaint_Id == Complaint_Id && st.StatusId == statusId
                //           select new { DbComplaintStatusChangeLog = st, DbAttachments = att }).ToList();

                //d.Complaint_Id == Complaint_Id && d.StatusId == statusId
                return d;
                //return listStatusChangeLogIds;
                //return Db.Complaints.OrderByDescending(m=>m.Created_Date).Skip(pageIndex).Take(numberOfComplaints).ToList();
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        public static dynamic GetStatusChangeLogWithAttachment(int Complaint_Id/*, int statusId*/)
        {
            try
            {
                DBContextHelperLinq db = new DBContextHelperLinq();
                //List<int> listStatusChangeLogIds = db.DbComplaintStatusChangeLog.AsNoTracking().Where(n => n.Complaint_Id == Complaint_Id && n.StatusId == statusId).Select(n => n.Id).ToList();

                var v1 = (from st in db.DbComplaintStatusChangeLog
                          where st.Complaint_Id == Complaint_Id /*&& st.StatusId == statusId*/
                          select new
                          {
                              DbComplaintStatusChangeLog = st,
                              DbAttachments =
                              from att in db.DbAttachments
                              where att.ReferenceType == (int)Config.AttachmentReferenceType.ChangeStatus //&& att.ReferenceTypeId == statusId
                              select (att)
                          }).ToList();



                //var v = ( from st in db.DbComplaintStatusChangeLog
                //    join att in db.DbAttachments on st.Id equals att.ReferenceTypeId
                //    where st.Complaint_Id == Complaint_Id && st.StatusId == statusId
                //           select new { DbComplaintStatusChangeLog = st, DbAttachments = att }).ToList();

                //d.Complaint_Id == Complaint_Id && d.StatusId == statusId
                return v1;
                //return listStatusChangeLogIds;
                //return Db.Complaints.OrderByDescending(m=>m.Created_Date).Skip(pageIndex).Take(numberOfComplaints).ToList();
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        public static List<DbComplaint> GetListByPersonId(int personId)
        {
            try
            {
                using (DBContextHelperLinq db = new DBContextHelperLinq())
                {
                    return db.DbComplaints.AsNoTracking().Where(m => m.Person_Id == personId).ToList();
                }


            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public static List<DbComplaint> GetByDateRange(int refField7Int, DateTime fromDate, DateTime toDate)
        {
            try
            {
                using (DBContextHelperLinq db = new DBContextHelperLinq())
                {
                    return db.DbComplaints.AsNoTracking().Where(m => m.RefField7_Int == refField7Int && ((m.Created_Date >= fromDate && m.Created_Date <= toDate) || (m.StatusChangedDate_Time >= fromDate && m.StatusChangedDate_Time <= toDate))).ToList();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public static List<DbComplaint> GetListByComplaintId(int complaintId)
        {
            try
            {
                DBContextHelperLinq db = new DBContextHelperLinq();
                return db.DbComplaints.AsNoTracking().Where(m => m.Id == complaintId)
                    .Include(n => n.listCategory)
                    .Include(n => n.listSubCategory)
                    .Include(n => n.listProvince)
                    .Include(n => n.listDistrict)
                    .Include(n => n.listTehsil)
                    .Include(n => n.listUc)
                    .Include(n => n.status).ToList();



            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static List<DbComplaint> GetListByCampaignIds(string campaignIds)
        {
            try
            {
                string[] lst = campaignIds.Split(new char[] { ',' });
                int[] campaignIdslst = new int[lst.Length];
                for (int i = 0; i < lst.Length; i++)
                {
                    campaignIdslst[i] = Int32.Parse(lst[i]);
                }
                DBContextHelperLinq db = new DBContextHelperLinq();
                return db.DbComplaints.AsNoTracking().Join(campaignIdslst, n => n.Compaign_Id, x => x, (n, x) => n).ToList<DbComplaint>();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static DbComplaint GetByComplaintId(int complaintId)
        {
            try
            {
                using (DBContextHelperLinq db = new DBContextHelperLinq())
                {
                    //DBContextHelperLinq db = new DBContextHelperLinq();
                    return db.DbComplaints.Where(m => m.Id == complaintId).ToList().FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static DbComplaint GeByComplaintIdAllColumnsIncluded(int complaintId)
        {
            try
            {
                DBContextHelperLinq db = new DBContextHelperLinq();
                return db.DbComplaints.AsNoTracking().Where(m => m.Id == complaintId)
                    .Include(n => n.listCategory)
                    .Include(n => n.listSubCategory)
                    .Include(n => n.listProvince)
                    .Include(n => n.listDistrict)
                    .Include(n => n.listTehsil)
                    .Include(n => n.listUc)
                    .Include(n => n.status).FirstOrDefault();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static DbComplaint GetByComplaintId(DBContextHelperLinq db, int complaintId)
        {
            try
            {
                //using (DBContextHelperLinq db = new DBContextHelperLinq())
                {
                    //DBContextHelperLinq db = new DBContextHelperLinq();
                    return db.DbComplaints.Where(m => m.Id == complaintId).ToList().FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /*
        public static int? GetTableRefByComplaintId(int complaintId)
        {
            try
            {
                using (DBContextHelperLinq db = new DBContextHelperLinq())
                {
                    //DBContextHelperLinq db = new DBContextHelperLinq();
                    return db.DbComplaints.Where(m => m.Id == complaintId).ToList().FirstOrDefault().TableRowRefId;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        */

        public static DbComplaint GetByComplaintId(int complaintId, DBContextHelperLinq db)
        {
            try
            {
                //DBContextHelperLinq db = new DBContextHelperLinq();
                return db.DbComplaints.Where(m => m.Id == complaintId).ToList().FirstOrDefault();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static DbComplaint GetBy(int complaintId, DBContextHelperLinq db = null)
        {
            try
            {
                DBContextHelperLinq dbC = (db == null) ? new DBContextHelperLinq() : db;
                DbComplaint dbComplaint = dbC.DbComplaints.AsNoTracking().Where(m => m.Id == complaintId).ToList().FirstOrDefault();
                if (db == null) dbC.Dispose();
                return dbComplaint;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static List<DbComplaint> GetBy(int campaignId, List<int> listComputedStatusId)
        {
            try
            {
                using (DBContextHelperLinq db = new DBContextHelperLinq())
                {
                    //DBContextHelperLinq db = new DBContextHelperLinq();
                    return
                        db.DbComplaints.Where(
                            m =>
                                m.Compaign_Id == campaignId &&
                                listComputedStatusId.Contains(m.Complaint_Computed_Status_Id)).ToList();
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static List<DbComplaint> GetBy(int campaignId, Config.ComplaintType typeId, List<string> listRefField1)
        {
            try
            {
                using (DBContextHelperLinq db = new DBContextHelperLinq())
                {
                    //DBContextHelperLinq db = new DBContextHelperLinq();
                    return
                        db.DbComplaints.Where(
                            m =>
                                m.Compaign_Id == campaignId && m.Complaint_Type == typeId && listRefField1.Contains(m.RefField1)).ToList();
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static List<DbComplaint> GetByOriginAndAssigneePresence(DBContextHelperLinq db, int campaignId,
            Config.ComplaintType complaintType, List<int> listComputedStatusId, bool isAssignedToOrigin,
            bool isAssigneePresent)
        {
            try
            {
                //using (DBContextHelperLinq db = new DBContextHelperLinq())
                {
                    //DBContextHelperLinq db = new DBContextHelperLinq();
                    return
                        db.DbComplaints.Where(
                            m =>
                                (m.Compaign_Id == campaignId && m.Complaint_Type == complaintType &&
                                 listComputedStatusId.Contains(m.Complaint_Computed_Status_Id)) &&
                                (m.Is_AssignedToOrigin == isAssignedToOrigin ||
                                 m.Is_AssigneePresent == isAssigneePresent)).ToList();
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        public static DataTable GetAgentListingReport(string from, string to, string agentIds, string campaignIds)
        {
            Dictionary<string, object> paramz = new Dictionary<string, object>
            {
                {"@AgentIds", agentIds},
                {"@CampaignIds", campaignIds},
                {"@From", from},
                {"@To", to}
            };


            return DBHelper.GetDataTableByStoredProcedure("[PITB].[Report_Agent_Wise_Count]", paramz);
            //DBContextHelperLinq db = new DBContextHelperLinq();
            //var data = (
            //    from d in db.DbComplaints
            //    join u in db.DbUsers on d.User_Id equals u.User_Id
            //    join s in db.DbStatuses on d.Complaint_Status_Id equals s.Id
            //    where agentIds.Contains(d.User_Id.Value) && campaignIds.Contains(d.Compaign_Id.Value)
            //    group d by new {u.Name, s.Status}
            //    into groupdata
            //  select groupdata).ToList()
            //    .Select(n => new Tuple<string, string, int>(n.Key.Name, n.Key.Status, n.Count())).ToList();
            //        //select (new Tuple<string, string, int>(groupdata.Key.Name, groupdata.Key.Status, groupdata.Count())))
            //        //.ToList();
            //return data;
        }


        public static int GetHierarchyIdValueAgainstComplaint(DbComplaint dbComplaint,
            Config.Hierarchy? hierarchyId = null)
        {
            Config.Hierarchy hierarchyValue = (hierarchyId == null)
                ? (Config.Hierarchy)dbComplaint.Complaint_Computed_Hierarchy_Id
                : (Config.Hierarchy)hierarchyId;
            switch (hierarchyValue)
            {
                case Config.Hierarchy.Division:
                    return Convert.ToInt32(dbComplaint.Division_Id);
                    break;

                case Config.Hierarchy.District:
                    return Convert.ToInt32(dbComplaint.District_Id);
                    break;

                case Config.Hierarchy.Province:
                    return Convert.ToInt32(dbComplaint.Province_Id);
                    break;

                case Config.Hierarchy.Tehsil:
                    return Convert.ToInt32(dbComplaint.Tehsil_Id);
                    break;

                case Config.Hierarchy.UnionCouncil:
                    return Convert.ToInt32(dbComplaint.UnionCouncil_Id);
                    break;

                case Config.Hierarchy.Ward:
                    return Convert.ToInt32(dbComplaint.Ward_Id);
                    break;

            }
            return -1;
        }

        public static int GetHierarchyIdValueAgainstHierarchyId(Config.Hierarchy hierarchyValue, DbComplaint dbComplaint)
        {
            //Config.Hierarchy hierarchyValue = (Config.Hierarchy)dbComplaint.Complaint_Computed_Hierarchy_Id;
            switch (hierarchyValue)
            {
                case Config.Hierarchy.Division:
                    return Convert.ToInt32(dbComplaint.Division_Id);
                    break;

                case Config.Hierarchy.District:
                    return Convert.ToInt32(dbComplaint.District_Id);
                    break;

                case Config.Hierarchy.Province:
                    return Convert.ToInt32(dbComplaint.Province_Id);
                    break;

                case Config.Hierarchy.Tehsil:
                    return Convert.ToInt32(dbComplaint.Tehsil_Id);
                    break;

                case Config.Hierarchy.UnionCouncil:
                    return Convert.ToInt32(dbComplaint.UnionCouncil_Id);
                    break;

                case Config.Hierarchy.Ward:
                    return Convert.ToInt32(dbComplaint.Ward_Id);
                    break;

            }
            return -1;
        }


        public static List<ComplaintPartial> GetComplaintPartialListByCampaignId(List<int> listCampaign)
        {
            using (DBContextHelperLinq db = new DBContextHelperLinq())
            {
                return db.DbComplaints.Where(n => listCampaign.Contains((int)n.Compaign_Id))
                    .Select(
                        n =>
                            new ComplaintPartial
                            {
                                CampaignId = (int)n.Compaign_Id,
                                StatusId = n.Complaint_Computed_Status_Id
                            })
                    .ToList();
            }

        }

        public static dynamic GetListByPersonCnicPaging(string personCnic, int campaignId, int pageNumber, int pageSize, List<int> complaintStatusId, byte filterType, DateTime startDate, DateTime endDate)
        {
            try
            {
                dynamic d = new ExpandoObject();
                Expression<Func<DbComplaint, bool>> wherePredicate1 = c => true;
                Expression<Func<DbComplaint, bool>> wherePredicate2 = c => true;
                Expression<Func<DbComplaint, bool>> dateWhereExpression = m => m.Created_Date >= startDate && m.Created_Date <= endDate;

                Expression<Func<DbComplaint, dynamic>> orderByExpression = complaint => new { CreatedDate = (DateTime)complaint.Created_Date };//null;
                //Expression<Func<TObject, DateTime>> Expr = obj => obj.StartDate;


                if (filterType == 1)
                {
                    //recent

                    orderByExpression = complaint => new { CreatedDate = (DateTime)complaint.Created_Date };
                }
                else if (filterType == 2)
                {
                    //order by votes
                    orderByExpression = complaint => new { VoteUpCount = (int)complaint.Vote_Up_Count };


                }

                if (string.IsNullOrEmpty(personCnic))
                {
                    wherePredicate1 = p => p.Compaign_Id == campaignId;

                }
                else
                {
                    wherePredicate1 = p => p.Compaign_Id == campaignId && p.Person_Cnic == personCnic;

                }
                if (complaintStatusId.Count > 0)
                {
                    wherePredicate2 = m => complaintStatusId.Contains((int)m.Complaint_Computed_Status_Id);
                }

                //combinedWhereExpression = wherePredicate1 && wherePredicate2;

                List<DbComplaint> listDbComplaint = null;

                using (DBContextHelperLinq db = new DBContextHelperLinq())
                {
                    //return db.DbComplaints.Where(n => n.Created_Date >= startDate && n.Created_Date <= endDate).ToList();

                    d.listDbComplaint =
                        db.DbComplaints.AsNoTracking()
                        .Where(dateWhereExpression)
                        .Where(wherePredicate1)
                        .Where(wherePredicate2)
                        .OrderByDescending(orderByExpression)
                        .Skip(pageNumber)
                        .Take(pageSize - pageNumber)
                        .ToList();

                    d.TotalComplaints = db.DbComplaints.AsNoTracking()
                        .Where(dateWhereExpression)
                        .Where(wherePredicate1)
                        .Where(wherePredicate2).Count();

                }

                return d;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public static List<DbComplaint> GetByCampaignId(int campaignId)
        {
            using (DBContextHelperLinq db = new DBContextHelperLinq())
            {
                return db.DbComplaints.Where(n => n.Compaign_Id == campaignId).ToList();
            }
        }

        public static List<DbComplaint> GetByCampaignId(DBContextHelperLinq db, int campaignId,
            Config.ComplaintType complaintType)
        {
            //using (DBContextHelperLinq db = new DBContextHelperLinq())
            {
                return
                    db.DbComplaints.Where(n => n.Compaign_Id == campaignId && n.Complaint_Type == complaintType)
                        .ToList();
            }
        }

        public static List<DbComplaint> GetByCampaignId(int campaignId, Config.ComplaintType complaintType)
        {
            using (DBContextHelperLinq db = new DBContextHelperLinq())
            {
                return
                    db.DbComplaints.Where(n => n.Compaign_Id == campaignId && n.Complaint_Type == complaintType)
                        .ToList();
            }
        }

        public static List<AgingReportListingData> GetAgingReportListingData(int campaignId,
            Config.ComplaintStatus complaintStatus)
        {
            try
            {
                using (DBContextHelperLinq db = new DBContextHelperLinq())
                {
                    List<AgingReportListingData> listAgingReport = db.DbComplaints.Where(
                        n =>
                            n.Compaign_Id == campaignId &&
                            n.Complaint_Computed_Status_Id == (int)complaintStatus)
                        .Select(n => new AgingReportListingData
                        {
                            ComplaintId = n.Id,
                            CreatedDateTime = (DateTime)n.Created_Date,
                            MaxDateTime = (DateTime)n.MaxSrcIdDate,
                            StatusId = n.Complaint_Computed_Status_Id,
                            ResolvedDateTime = (DateTime)n.StatusChangedDate_Time
                        }).ToList();

                    return listAgingReport;
                }
            }
            catch (Exception ex)
            {

                throw;
            }

        }

        public List<DbUserCategory> GetUserCategoryList()
        {
            List<DbUserCategory> listDbUserCategory = new List<DbUserCategory>();
            if (this.UserCategoryId1 != null)
            {
                DbUserCategory dbUserCategory = new DbUserCategory();

                dbUserCategory.Parent_Category_Id = UserCategoryId1;
                if (this.UserCategoryId2 != null)
                {
                    dbUserCategory.Child_Category_Id = UserCategoryId2;
                }
                dbUserCategory.Category_Hierarchy = 1;
                listDbUserCategory.Add(dbUserCategory);
            }

            if (this.UserCategoryId3 != null)
            {
                DbUserCategory dbUserCategory = new DbUserCategory();

                dbUserCategory.Parent_Category_Id = UserCategoryId2;
                dbUserCategory.Child_Category_Id = UserCategoryId3;

                dbUserCategory.Category_Hierarchy = 2;
                listDbUserCategory.Add(dbUserCategory);
            }

            if (this.UserCategoryId4 != null)
            {
                DbUserCategory dbUserCategory = new DbUserCategory();

                dbUserCategory.Parent_Category_Id = UserCategoryId3;
                dbUserCategory.Child_Category_Id = UserCategoryId4;

                dbUserCategory.Category_Hierarchy = 3;
                listDbUserCategory.Add(dbUserCategory);
            }

            if (this.UserCategoryId5 != null)
            {
                DbUserCategory dbUserCategory = new DbUserCategory();

                dbUserCategory.Parent_Category_Id = UserCategoryId4;
                dbUserCategory.Child_Category_Id = UserCategoryId5;

                dbUserCategory.Category_Hierarchy = 4;
                listDbUserCategory.Add(dbUserCategory);
            }
            this.ListUserCategory = listDbUserCategory;
            return listDbUserCategory;
        }


        public bool SetUpdated(DbSchoolsMapping dbSchoolsMapping,
            List<DbCrmIdsMappingToOtherSystem> listHierarchyMapping, List<DbProvince> listProvince,
            List<DbDivision> listDivision, List<DbDistrict> listDistrict, List<DbTehsil> listTehsil,
            List<DbUnionCouncils> listUc)
        {
            //---- Assign Region ------
            if (dbSchoolsMapping.System_Markaz_Id != this.UnionCouncil_Id ||
                dbSchoolsMapping.School_Type != Config.SchoolTypeDict[this.RefField4] ||
                dbSchoolsMapping.System_School_Gender != Convert.ToBoolean(Config.SchoolGenderDict[this.RefField5]))
            // if markaz doesnt match
            {
                this.UnionCouncil_Id = dbSchoolsMapping.System_Markaz_Id;
                this.UnionCouncil_Name = listUc.Where(n => n.Id == this.UnionCouncil_Id).FirstOrDefault().Councils_Name;

                if (dbSchoolsMapping.System_Tehsil_Id != this.Tehsil_Id) // if tehsil doesnt match
                {
                    this.Tehsil_Id = dbSchoolsMapping.System_Tehsil_Id;
                    this.Tehsil_Name =
                        listTehsil.Where(n => n.Tehsil_Id == this.Tehsil_Id).FirstOrDefault().Tehsil_Name;

                    if (dbSchoolsMapping.System_District_Id != this.District_Id) // if district doesnt match
                    {
                        this.District_Id = dbSchoolsMapping.System_District_Id;

                        DbDistrict dbDistrict =
                            listDistrict.Where(n => n.District_Id == this.District_Id).FirstOrDefault();


                        this.District_Name = dbDistrict.District_Name;

                        if (dbDistrict.Division_Id != this.Division_Id) // if division doesnt match
                        {
                            this.Division_Id = dbDistrict.Division_Id;

                            DbDivision dbDivision =
                                listDivision.Where(n => n.Division_Id == this.Division_Id).FirstOrDefault();

                            this.Division_Name = dbDivision.Division_Name;

                            if (dbDivision.Province_Id != this.Province_Id) // if province doesnt match
                            {
                                this.Province_Id = dbDivision.Province_Id;

                                DbProvince dbProvince =
                                    listProvince.Where(n => n.Province_Id == this.Province_Id).FirstOrDefault();

                                this.Province_Name = dbProvince.Province_Name;
                            }
                        }
                    }

                    BlSchool.ReEvaluateEscallation(this, dbSchoolsMapping);
                }
                /*
                BlSchool.ReEvaluateEscallation(this, dbSchoolsMapping);

                if (dbSchoolsMapping.School_Type != Config.SchoolTypeDict[this.RefField4] ||
                dbSchoolsMapping.System_School_Gender != Convert.ToBoolean(Config.SchoolGenderDict[this.RefField5]))
                {
                    BlSchool.ReEvaluateEscallation(this, dbSchoolsMapping);
                }
                else
                {
                    return false;
                }*/
            }

            // If school Info Has been changed
            //if(!this.RefField1.Equals(dbSchoolsMapping.school_emis_code) 
            //    || !this.RefField2.Equals(dbSchoolsMapping.school_name)
            //    || !this.RefField3.Equals(dbSchoolsMapping.School_Type)
            //    || !this.RefField5.Equals(dbSchoolsMapping.school_gender)
            //{
            //    Convert.ToInt16(Config.SchoolTypeMapDict[dbSchoolsMapping.school_level])
            //}
            this.RefField1 = dbSchoolsMapping.school_emis_code;
            this.RefField2 = dbSchoolsMapping.school_name;
            this.RefField3 = dbSchoolsMapping.school_level;
            this.RefField4 = Config.SchoolTypeDict.FirstOrDefault(x => x.Value == dbSchoolsMapping.School_Type).Key;
            this.RefField5 =
                Config.SchoolGenderDict.FirstOrDefault(
                    x => x.Value == Convert.ToInt32(dbSchoolsMapping.System_School_Gender)).Key;
            this.RefField6 = dbSchoolsMapping.markaz_name;


            // If schoolType and gender has been changed


            return false;
        }

        public static bool BulkMerge(List<DbComplaint> listToMerge, SqlConnection con, int currentRetryCount = 0,
            int totalRetryCount = Config.FunctionRetryCount)
        {
            try
            {
                BulkOperation<DbComplaint> bulkOp = new BulkOperation<DbComplaint>(con);
                bulkOp.BatchSize = 1000;
                bulkOp.ColumnInputExpression = c => new
                {
                    c.Province_Id,
                    c.Province_Name,

                    c.Division_Id,
                    c.Division_Name,

                    c.District_Id,
                    c.District_Name,

                    c.Tehsil_Id,
                    c.Tehsil_Name,

                    c.UnionCouncil_Id,
                    c.UnionCouncil_Name,

                    c.Dt1,
                    c.SrcId1,
                    c.UserSrcId1,

                    c.Dt2,
                    c.SrcId2,
                    c.UserSrcId2,

                    c.Dt3,
                    c.SrcId3,
                    c.UserSrcId3,

                    c.Dt4,
                    c.SrcId4,
                    c.UserSrcId4,

                    c.Dt5,
                    c.SrcId5,
                    c.UserSrcId5,

                    c.Dt6,
                    c.SrcId6,
                    c.UserSrcId6,

                    c.Dt7,
                    c.SrcId7,
                    c.UserSrcId7,

                    c.Dt8,
                    c.SrcId8,
                    c.UserSrcId8,

                    c.Dt9,
                    c.SrcId9,
                    c.UserSrcId9,

                    c.Dt10,
                    c.SrcId10,
                    c.UserSrcId10,

                    c.RefField1,
                    c.RefField2,
                    c.RefField3,
                    c.RefField4,
                    c.RefField5,
                    c.RefField6,


                    c.Origin_HierarchyId,
                    c.Origin_UserHierarchyId,
                    c.Origin_UserCategoryId1,
                    c.Origin_UserCategoryId2,
                    c.Is_AssignedToOrigin,

                    c.MaxLevel,
                    c.MinSrcId,
                    c.MaxSrcId,
                    c.MinUserSrcId,
                    c.MaxUserSrcId,
                    c.MinSrcIdDate,
                    c.MaxSrcIdDate,

                    c.UserCategoryId1,
                    c.UserCategoryId2

                };
                bulkOp.DestinationTableName = "PITB.Complaints";
                bulkOp.ColumnOutputExpression = c => c.Id;
                bulkOp.ColumnPrimaryKeyExpression = c => c.Id;
                bulkOp.BulkMerge(listToMerge);
                return true;
            }
            catch (Exception ex)
            {
                if (currentRetryCount != totalRetryCount)
                {
                    BulkMerge(listToMerge, con, currentRetryCount + 1, totalRetryCount);
                }
            }
            return false;
        }

        #endregion
    }
}
