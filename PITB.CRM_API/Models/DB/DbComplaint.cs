using System.Data;
using System.Linq;
using System.Linq.Expressions;
//using Microsoft.Ajax.Utilities;
using Microsoft.Ajax.Utilities;
using PITB.CRM_API.Helper.Database;
using PITB.CRM_API.Models.API;
using PITB.CRM_API.Models.Custom;


namespace PITB.CRM_API.Models.DB
{
    using System;
    using CMS;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity;

    [Table("PITB.Complaints")]
    
    public class DbComplaint
    {
        [Key]
        [Column(Order = 0)]
        public int Id { get; set; }

        public int? Person_Id { get; set; }

        public string Person_External_User_Id { get; set; }
        public string Person_External_Provider { get; set; }

        [StringLength(400)]
        public string Person_Name { get; set; }

        [StringLength(50)]
        public string Person_Contact { get; set; }

        [StringLength(50)]
        public string Person_Cnic { get; set; }

        public int? Person_Province_Id { get; set; }

        [StringLength(200)]
        public string Person_Province_Name { get; set; }

        public int? Person_Division_Id { get; set; }

        [StringLength(200)]
        public string Person_Division_Name { get; set; }

        public int? Person_District_Id { get; set; }

        [StringLength(200)]
        public string Person_District_Name { get; set; }

        public int? Person_Tehsil_Id { get; set; }

        [StringLength(200)]
        public string Person_Tehsil_Name { get; set; }

        public int? Person_Uc_Id { get; set; }

        [StringLength(200)]
        public string Person_Uc_Name { get; set; }

        public int? Complaint_Type { get; set; }

        public int? Department_Id { get; set; }

        [StringLength(200)]
        public string Department_Name { get; set; }

        public int? Complaint_Category { get; set; }

        [StringLength(200)]
        public string Complaint_Category_Name { get; set; }

        public int? Complaint_SubCategory { get; set; }

        [StringLength(200)]
        public string Complaint_SubCategory_Name { get; set; }

        public int? Compaign_Id { get; set; }

        [StringLength(200)]
        public string Campaign_Name { get; set; }

        public int? Province_Id { get; set; }

        [StringLength(200)]
        public string Province_Name { get; set; }

        public int? Division_Id { get; set; }

        [StringLength(200)]
        public string Division_Name { get; set; }

        public int? District_Id { get; set; }

        [StringLength(200)]
        public string District_Name { get; set; }

        public int? Tehsil_Id { get; set; }

        [StringLength(200)]
        public string Tehsil_Name { get; set; }

        public int? UnionCouncil_Id { get; set; }

        [StringLength(200)]
        public string UnionCouncil_Name { get; set; }

        public int? Ward_Id { get; set; }

        [StringLength(200)]
        public string Ward_Name { get; set; }

        public int? Agent_Id { get; set; }

        [StringLength(500)]
        public string Complaint_Address { get; set; }

        [StringLength(500)]
        public string Business_Address { get; set; }

        public string Complaint_Remarks { get; set; }

        public int? Complaint_Status_Id { get; set; }

        [StringLength(100)]
        public string Complaint_Status { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public int? Complaint_Computed_Status_Id { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [StringLength(100)]
        public string Complaint_Computed_Status { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public int? Complaint_Computed_Hierarchy_Id { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [StringLength(20)]
        public string Complaint_Computed_Hierarchy { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public string Computed_Remaining_Total_Time { get; set; }

        public DateTime? Created_Date { get; set; }

        public int? Created_By { get; set; }

        public DateTime? Complaint_Assigned_Date { get; set; }

        public DateTime? Completed_Date { get; set; }

        public DateTime? Updated_Date { get; set; }

        public int? Updated_By { get; set; }

        [Key]
        [Column(Order = 1)]
        public bool Is_Deleted { get; set; }

        public DateTime? Date_Deleted { get; set; }

        public int? Deleted_By { get; set; }

        public string Reason_ToDelete { get; set; }

        public string Agent_Comments { get; set; }

        public int? Status_ChangedBy { get; set; }

        [StringLength(200)]
        public string Status_ChangedBy_Name { get; set; }

        public DateTime? StatusChangedDate_Time { get; set; }

        public int? StatusChangedBy_RoleId { get; set; }

        public int? StatusChangedBy_HierarchyId { get; set; }

        public int? StatusChangedBy_User_HierarchyId { get; set; }

        [StringLength(1000)]
        public string StatusChangedComments { get; set; }

        public DateTime? Dt1 { get; set; }

        public int? SrcId1 { get; set; }

        public DateTime? Dt2 { get; set; }

        public int? SrcId2 { get; set; }

        public DateTime? Dt3 { get; set; }

        public int? SrcId3 { get; set; }

        public DateTime? Dt4 { get; set; }

        public int? SrcId4 { get; set; }

        public DateTime? Dt5 { get; set; }

        public int? SrcId5 { get; set; }

        public DateTime? Dt6 { get; set; }

        public int? SrcId6 { get; set; }

        public DateTime? Dt7 { get; set; }

        public int? SrcId7 { get; set; }

        public DateTime? Dt8 { get; set; }

        public int? SrcId8 { get; set; }

        public DateTime? Dt9 { get; set; }

        public int? SrcId9 { get; set; }

        public DateTime? Dt10 { get; set; }

        public int? SrcId10 { get; set; }

        public int? MaxLevel { get; set; }

        public bool? IsTransferred { get; set; }

        public int? ComplaintSrc { get; set; }

        public int? Complainant_Remark_Id { get; set; }

        [StringLength(2000)]
        public string Complainant_Remark_Str { get; set; }

        public int? UserSrcId1 { get; set; }

        public int? UserSrcId2 { get; set; }

        public int? UserSrcId3 { get; set; }

        public int? UserSrcId4 { get; set; }

        public int? UserSrcId5 { get; set; }

        public int? UserSrcId6 { get; set; }

        public int? UserSrcId7 { get; set; }

        public int? UserSrcId8 { get; set; }

        public int? UserSrcId9 { get; set; }

        public int? UserSrcId10 { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public int? Complaint_Computed_User_Hierarchy_Id { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [StringLength(100)]
        public string Computed_Remaining_Time_To_Escalate { get; set; }

        public int? TableRefId { get; set; }

        public int? TableRowRefId { get; set; }

        public int? UserCategoryId1 { get; set; }

        public int? UserCategoryId2 { get; set; }

        [StringLength(2000)]
        public string RefField1 { get; set; }

        [StringLength(2000)]
        public string RefField2 { get; set; }

        [StringLength(2000)]
        public string RefField3 { get; set; }

        [StringLength(2000)]
        public string RefField4 { get; set; }

        [StringLength(2000)]
        public string RefField5 { get; set; }

        [StringLength(2000)]
        public string RefField6 { get; set; }

        [StringLength(2000)]
        public string RefField7 { get; set; }

        [StringLength(2000)]
        public string RefField8 { get; set; }

        [StringLength(2000)]
        public string RefField9 { get; set; }

        [StringLength(2000)]
        public string RefField10 { get; set; }

        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public int? Vote_Up_Count { get; set; }
        public int? Vote_Down_Count { get; set; }

        public string LocationArea { get; set; }



        public virtual DbComplaintType listCategory { get; set; }

        public virtual DbComplaintSubType listSubCategory { get; set; }

        public virtual DbProvince listProvince { get; set; }
        public virtual DbDistrict listDistrict { get; set; }

        public virtual DbTehsil listTehsil { get; set; }

        public virtual DbUnionCouncils listUc { get; set; }
        //public virtual DbUsers User { get; set; }

        //public virtual DbStatus status { get; set; }


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

        public static int GetAllComplaintsCountInCampaign(int campaignId, DateTime startDate, DateTime endDate)
        {
            using (DBContextHelperLinq db = new DBContextHelperLinq())
            {
                return db.DbComplaints.Count(m => m.Compaign_Id == campaignId && m.Created_Date >= startDate && m.Created_Date <= endDate);

            }
        }
        public static int GetAllComplaintsCountInCampaign(int campaignId, string cnic, string cell, DateTime startDate, DateTime endDate)
        {
            using (DBContextHelperLinq db = new DBContextHelperLinq())
            {
                return db.DbComplaints.Count(m => m.Compaign_Id == campaignId && m.Person_Cnic == cnic && m.Person_Contact == cell
                     && m.Created_Date >= startDate && m.Created_Date <= endDate);

            }
        }

        public static Dictionary<int, LocationCoordinate> GetAllComplaintsOfCampaignWithCoordinates(int campaignId, DateTime startDate, DateTime endDate)
        {
            using (DBContextHelperLinq db = new DBContextHelperLinq())
            {

                return db.DbComplaints
                    .AsNoTracking()
                    .Where(m => m.Compaign_Id == campaignId && m.Longitude.HasValue && m.Created_Date >= startDate && m.Created_Date <= endDate)
                    .Select(t => new
                     {
                         t.Id,
                         t.Latitude,
                         t.Longitude
                     })
                    .ToDictionary(
                        t => t.Id,
                        t => new LocationCoordinate(t.Latitude.IfNotNull(d => d.Value), t.Longitude.IfNotNull(d => d.Value)));


            }
        }
        public static List<DbComplaint> GetByComplaintIds(List<int> listComplaintIds)
        {
            try
            {
                //DBContextHelperLinq db = new DBContextHelperLinq();
                DBContextHelperLinq db = new DBContextHelperLinq();
                //return db.DbComplaints.Where(m => listComplaintIds.Contains(m.Id)).Select(n=>new Tuple<int,int,string>(n.Id,n.Complaint_Computed_Status_Id,n.Complaint_Computed_Status)).ToList();
                return db.DbComplaints.Where(m => listComplaintIds.Contains(m.Id)).ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static TempComplainatModel /*List<DbComplaint>*/ GetListByCampaignIdPaging(int campaignId, int from, int to,
            List<int> complaintStatusId, byte filterType, DateTime startDate, DateTime endDate)
        {
            TempComplainatModel tempModel = new TempComplainatModel();
            Expression<Func<DbComplaint, bool>> whereExpression = null;
            Expression<Func<DbComplaint, bool>> dateWhereExpression = m => m.Created_Date >= startDate && m.Created_Date <= endDate;


            Expression<Func<DbComplaint, dynamic>> orderByExpression = null;
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

            if (complaintStatusId.Count > 0)
            {
                whereExpression = m =>complaintStatusId.Contains((int) m.Complaint_Computed_Status_Id) && m.Compaign_Id == campaignId;
            }
            else
            {
                whereExpression = m => m.Compaign_Id == campaignId;

            }

            using (DBContextHelperLinq db = new DBContextHelperLinq())
            {
                tempModel.listDbComplaints = db.DbComplaints
                            .AsNoTracking()
                            .Where(dateWhereExpression)
                            .Where(whereExpression)
                            .OrderByDescending(orderByExpression)
                            .Skip(from)
                            .Take(to-from)
                            .ToList();

                tempModel.TotalComplaints = db.DbComplaints
                            .AsNoTracking()
                            .Where(dateWhereExpression)
                            .Where(whereExpression)
                            .Count();
                //return db.DbComplaints
                //            .AsNoTracking()
                //            .Where(dateWhereExpression)
                //            .Where(whereExpression)
                //            .OrderByDescending(orderByExpression)
                //            .Skip(from)
                //            .Take(to)
                //            .ToList();
            }
            return tempModel;
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
        public static List<DbComplaint> GetListByPersonId(int personId, int campaignId, int from, int to, int complaintStatusId, byte filterType)
        {
            try
            {
                Expression<Func<DbComplaint, bool>> whereExpression = null;

                Expression<Func<DbComplaint, dynamic>> orderByExpression = null;
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
                if (complaintStatusId > 0)
                {
                    whereExpression = m => m.Person_Id == personId && m.Complaint_Computed_Status_Id == complaintStatusId;
                }
                else
                {
                    whereExpression = m => m.Person_Id == personId;

                }
                using (DBContextHelperLinq db = new DBContextHelperLinq())
                {
                    return db.DbComplaints
                                .AsNoTracking()
                                .Where(whereExpression)
                                .OrderByDescending(orderByExpression)
                                .Skip(from)
                                .Take(to)
                                .ToList();
                }


            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public static int GetCountOfComplaintsByPersonId(int personId)
        {
            using (DBContextHelperLinq db = new DBContextHelperLinq())
            {
                return db.DbComplaints.Count(m => m.Person_Id == personId);
            }
        }
        public static int GetCountOfComplaintsByExternalUserId(string userId, string provider)
        {
            using (DBContextHelperLinq db = new DBContextHelperLinq())
            {
                return db.DbComplaints.Count(m => m.Person_External_User_Id == userId && m.Person_External_Provider.Equals(provider, StringComparison.OrdinalIgnoreCase));
            }
        }
        public static List<DbComplaint> GetListByPersonCnic(string personCnic, int campaignId)
        {
            try
            {
                using (DBContextHelperLinq db = new DBContextHelperLinq())
                {
                    if (string.IsNullOrEmpty(personCnic))
                    {
                        return
                           db.DbComplaints.AsNoTracking()
                               .Where(m => m.Compaign_Id == campaignId)
                               .ToList();
                    }

                    if (campaignId == 0) // where campaignId is not included
                    {
                        return
                            db.DbComplaints.AsNoTracking()
                                .Where(m => m.Person_Cnic == personCnic)
                                .ToList();
                    }
                    else
                    {
                        return
                            db.DbComplaints.AsNoTracking()
                                .Where(m => m.Person_Cnic == personCnic && m.Compaign_Id == campaignId)
                                .ToList();
                    }
                }


            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public static TempComplainatModel GetListByPersonCnicPaging(string personCnic, int campaignId, int pageNumber, int pageSize, List<int> complaintStatusId, byte filterType, DateTime startDate, DateTime endDate)
        {
            try
            {
                TempComplainatModel tempModel = new TempComplainatModel();
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
                    wherePredicate2 = m =>complaintStatusId.Contains((int) m.Complaint_Computed_Status_Id );
                }

                //combinedWhereExpression = wherePredicate1 && wherePredicate2;



                using (DBContextHelperLinq db = new DBContextHelperLinq())
                {
                    //return db.DbComplaints.Where(n => n.Created_Date >= startDate && n.Created_Date <= endDate).ToList();

                    tempModel.listDbComplaints =
                        db.DbComplaints.AsNoTracking()
                        .Where(dateWhereExpression)
                        .Where(wherePredicate1)
                        .Where(wherePredicate2)
                        .OrderByDescending(orderByExpression)
                        .Skip(pageNumber)
                        .Take(pageSize-pageNumber)
                        .ToList();

                    tempModel.TotalComplaints = db.DbComplaints.AsNoTracking()
                        .Where(dateWhereExpression)
                        .Where(wherePredicate1)
                        .Where(wherePredicate2).Count();

                }

                return tempModel;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public static TempComplainatModel GetListByPersonUserIdAndProviderPaging(string userId, string provider, int campaignId, int pageNumber, int pageSize, List<int> complaintStatusId, byte filterType)
        {
            try
            {
                TempComplainatModel tempModel = new TempComplainatModel();
                Expression<Func<DbComplaint, bool>> wherePredicate1 = c => true;
                Expression<Func<DbComplaint, bool>> wherePredicate2 = c => true;

                Expression<Func<DbComplaint, dynamic>> orderByExpression = null;
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

                wherePredicate1 = p => p.Compaign_Id == campaignId &&
                    p.Person_External_Provider.Equals(provider, StringComparison.OrdinalIgnoreCase) &&
                    p.Person_External_User_Id.Equals(userId, StringComparison.OrdinalIgnoreCase);


                if (complaintStatusId.Count > 0)
                {
                    wherePredicate2 = m => complaintStatusId.Contains((int) m.Complaint_Computed_Status_Id);
                }


                using (DBContextHelperLinq db = new DBContextHelperLinq())
                {

                    var query =
                        db.DbComplaints.AsNoTracking()
                            .Where(wherePredicate1)
                            .OrderByDescending(orderByExpression)

                        .Skip(pageNumber)
                        .Take(pageSize - pageNumber)
                        .AsQueryable();
                    tempModel.listDbComplaints = query.Where(wherePredicate1).Where(wherePredicate2).ToList();
                    tempModel.TotalComplaints = db.DbComplaints.AsNoTracking().Where(wherePredicate1).Where(wherePredicate1).Where(wherePredicate2).Count();

                }
                return tempModel;

            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public static List<DbComplaint> GetListByComplaintIds(List<int> complaintIds, List<int> complaintStatusId, byte filterType, DateTime startDate, DateTime endDate)
        {
            Expression<Func<DbComplaint, bool>> whereExpression = null;
            Expression<Func<DbComplaint, bool>> dateWhereExpression = complaint => complaint.Created_Date >= startDate && complaint.Created_Date <= endDate;

            Expression<Func<DbComplaint, dynamic>> orderByExpression = null;
            if (filterType == 1)
            {
                //recent

                orderByExpression = complaint => new { CreatedDate = (DateTime)complaint.Created_Date };
            }
            else
            {
                //order by votes
                orderByExpression = complaint => new { VoteUpCount = (int)complaint.Vote_Up_Count };


            }
            if (complaintStatusId.Count > 0)
            {
                whereExpression = m => complaintStatusId.Contains((int)m.Complaint_Computed_Status_Id) && complaintIds.Contains(m.Id);
            }
            else
            {
                whereExpression = m => complaintIds.Contains(m.Id);

            }


            using (DBContextHelperLinq db = new DBContextHelperLinq())
            {
                return db.DbComplaints
                            .AsNoTracking()
                            .Where(dateWhereExpression)
                            .Where(whereExpression)
                            .OrderByDescending(orderByExpression)
                            .ToList();
            }
        }
        public static List<DbComplaint> GetListByComplaintIds(List<int> complaintIds, int from, int to, byte filterType)
        {
            Expression<Func<DbComplaint, bool>> whereExpression = null;
            Expression<Func<DbComplaint, dynamic>> orderByExpression = null;
            if (filterType == 1)
            {
                //recent

                orderByExpression = complaint => new { CreatedDate = (DateTime)complaint.Created_Date };
            }
            else
            {
                //order by votes
                orderByExpression = complaint => new { VoteUpCount = (int)complaint.Vote_Up_Count };


            }

            using (DBContextHelperLinq db = new DBContextHelperLinq())
            {
                return db.DbComplaints
                            .AsNoTracking()
                            .Where(m => complaintIds.Contains(m.Id))
                            .OrderByDescending(orderByExpression)
                            .Skip(from)
                            .Take(to)
                            .ToList();
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
                    .Include(n => n.listUc).ToList();
                //.Include(n=>n.status).ToList();



            }
            catch (Exception ex)
            {
                return null;
            }
        }

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

        public static DbComplaint GetByComplaintId(int complaintId)
        {
            try
            {
                DBContextHelperLinq db = new DBContextHelperLinq();
                return db.DbComplaints.AsNoTracking().Where(m => m.Id == complaintId).FirstOrDefault();
            }
            catch (Exception ex)
            {
                return null;
            }

        }
        public static List<DbComplaint> GetComplaintsData(int campaignId, string startDate, string endDate)
        {
            try
            {
                DBContextHelperLinq db = new DBContextHelperLinq();
                return db.DbComplaints.AsNoTracking().Where(m => m.Compaign_Id == campaignId && m.Created_Date >= Convert.ToDateTime(startDate) && m.Created_Date <= Convert.ToDateTime(endDate)).ToList();
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

        public static int GetHierarchyIdValueAgainstHierarchyId(DbComplaint dbComplaint)
        {
            Config.Hierarchy hierarchyValue = (Config.Hierarchy)dbComplaint.Complaint_Computed_Hierarchy_Id;
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

        public static List<DbComplaint> GetByTableRef(Config.TableRef tableRef, List<int?> listTableRowId)
        {
            try
            {
                //DBContextHelperLinq db = new DBContextHelperLinq();
                DBContextHelperLinq db = new DBContextHelperLinq();
                //return db.DbComplaints.Where(m => listComplaintIds.Contains(m.Id)).Select(n=>new Tuple<int,int,string>(n.Id,n.Complaint_Computed_Status_Id,n.Complaint_Computed_Status)).ToList();
                return db.DbComplaints.Where(m => m.TableRefId == (int)tableRef && listTableRowId.Contains(m.TableRowRefId)).ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        #endregion

    }
}
