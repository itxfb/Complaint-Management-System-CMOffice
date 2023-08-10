using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using PITB.CMS_Common;
using PITB.CMS_Common.Models;
using PITB.CRM.Public_Web.Handler;
using PITB.CRM.Public_Web.Helpers;
using PITB.CRM.Public_Web.Helpers.Database;
using PITB.CRM.Public_Web.Helpers.Extensions;
using PITB.CRM.Public_Web.Models;
using PITB.CRM.Public_Web.Models.ViewModels;
using PITB.CRM.Public_Web.Utilities;

namespace PITB.CRM.Public_Web.BusinessLayer
{
    public class BlComplaint
    {
        public static VmNewsFeed LoadComplaintsBetween(VmFilters filter)
        {
            VmNewsFeed vm = new VmNewsFeed();
            ComplaintData cd = null;
            //   string query = CreateQuery(49, 1, 10, "Created_Date", "DESC");
            filter.Paging.OrderByField = "a.Created_Date";
            filter.Paging.OrderByDirection = "DESC";
            string query = ListingLogicPublic.GetQuery(filter);

            SqlConnection connection = DBHelper.GetConnection();
            SqlCommand cmd = new SqlCommand(query, connection);
            connection.Open();
            int i = 0;
            using (connection)
            {
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    if (dr.HasRows)
                    {
                        cd = (new ComplaintData()
                        {
                            ComplaintId = Convert.ToInt32(dr["Id"]),
                            CampaignId = Convert.ToInt32(dr["Compaign_Id"]),
                            Description = Convert.ToString(dr["Complaint_Remarks"]),
                            //Status = Convert.ToString(dr["Complaint_Computed_Status"]),
                           // Status = GetPublicComplaintStatusByDbStatusId(Convert.ToInt32(dr["Complaint_Computed_Status_Id"])).ToString().ToSentenceCase(),
                            Category = Convert.ToString(dr["Complaint_Category_Name"]),
                            District = Convert.ToString(dr["District_Name"]),
                            Town = Convert.ToString(dr["Tehsil_Name"]),
                            UC = Convert.ToString(dr["UnionCouncil_Name"]),
                            SocialSharedData = LoadSocialShareData(Convert.ToInt32(dr["Id"])),
                            ImageList = LoadAttachments(Convert.ToInt32(dr["Id"]), Config.AttachmentReferenceType.Add),
                            StatusChangeAttachments = LoadAttachments(Convert.ToInt32(dr["Id"]), Config.AttachmentReferenceType.ChangeStatus),
                            Created_DateTime = Convert.ToDateTime(dr["Created_Date"]),
                            StatusChangedText = Convert.ToString(dr["StatusChangedComments"]),
                            //  StatusId = Convert.ToInt32(dr["Complaint_Computed_Status_Id"])
                            StatusId = (int)GetPublicComplaintStatusByDbStatusId(Convert.ToInt32(dr["Complaint_Computed_Status_Id"])),
                            StatusChangedByName = Convert.ToString(dr["StatusChangedByName"]),
                            
                        }
                            );
                        try
                        {
                            if (dr["Latitude"] != DBNull.Value && dr["Longitude"] != DBNull.Value)
                            {

                                cd.Location = new Location()
                                {
                                    Latitude = Convert.ToDouble(dr["Latitude"]),
                                    Longitude = Convert.ToDouble(dr["Longitude"])
                                };
                            }
                        }
                        catch (Exception ex)
                        {
                            throw ex;

                        }

                        if (dr["StatusChangedDate_Time"] != DBNull.Value)
                        {
                            cd.StatusChangeDateTime = Convert.ToDateTime(dr["StatusChangedDate_Time"]);
                        }
                        vm.ListComplaints.Add(cd);

                    }
                    i++;
                }
            }
            return vm;
        }
        private static string GetListingQuery(VmFilters filter)
        {

            string selectQuery;
            if (filter.Filterable)
            {
                selectQuery = string.Format(@"SELECT * FROM (
                SELECT Id,Compaign_Id,Complaint_Remarks,Complaint_Computed_Status,Complaint_Computed_Status_Id,Complaint_Category_Name,
			                District_Name,Tehsil_Name,UnionCouncil_Name,Created_Date ,StatusChangedComments,StatusChangedDate_Time,
                            Latitude,Longitude,
			                ROW_NUMBER() OVER (ORDER BY {0} {1}) AS RowNum
			                FROM PITB.Complaints
                WHERE  
                            --Campaign
                            Compaign_Id={2}
                            --District
                            EXISTS(SELECT 1 FROM dbo.SplitString('{3}',',') X WHERE X.Item=District_Id) AND

                            --Tehsil OR Town
                            EXISTS(SELECT 1 FROM dbo.SplitString('{4}',',') X WHERE X.Item=Tehsil_Id) AND

                            --UC
                            EXISTS(SELECT 1 FROM dbo.SplitString('{5}',',') X WHERE X.Item=UnionCouncil_Id) AND

                            --SubCategory
                            EXISTS(SELECT 1 FROM dbo.SplitString('{6}',',') X WHERE X.Item=Complaint_SubCategory) AND

                            --Status
                            EXISTS(SELECT 1 FROM dbo.SplitString('{7}',',') X WHERE X.Item=Complaint_Computed_Status_Id) 
                ) AS tbl
                WHERE tbl.RowNum BETWEEN {8} AND {9}"
                    , filter.Paging.OrderByField                    //0
                    , filter.Paging.OrderByDirection                //1
                    , filter.CampaignId                             //2
                    , filter.DistrictIds.ToCommaSepratedString()    //3
                    , filter.TownTehsilId.ToCommaSepratedString()   //4
                    , filter.Uc.ToCommaSepratedString()             //5
                    , filter.Categories.ToCommaSepratedString()     //6
                    , filter.StatusId.ToCommaSepratedString()       //7
                    , filter.Paging.FromPage                        //8
                    , filter.Paging.ToPage);                        //9


            }

            else
            {
                selectQuery = string.Format(@"SELECT * FROM 
			(
			SELECT Id,Compaign_Id,Complaint_Remarks,Complaint_Computed_Status,Complaint_Computed_Status_Id,Complaint_Category_Name,
			District_Name,Tehsil_Name,UnionCouncil_Name,Created_Date ,StatusChangedComments,StatusChangedDate_Time,
            Latitude,Longitude,
			ROW_NUMBER() OVER (ORDER BY {0} {1}) AS RowNum
			FROM pitb.complaints WHERE Compaign_Id={2} ) AS tbl
			WHERE tbl.RowNum BETWEEN {3} AND {4}"
              , filter.Paging.OrderByField //0
                  , filter.Paging.OrderByDirection //1
              , filter.CampaignId //2
               , filter.Paging.FromPage
               , filter.Paging.ToPage);
            }


            return selectQuery;
        }
        private static SocialShareData LoadSocialShareData(int complaintId)
        {
            string query = string.Format(@"DECLARE @UpVotes INT,@DownVotes INT
                    SET @UpVotes =(SELECT COUNT(1) FROM pitb.Complaint_Votes WHERE Is_Positive_Vote=1 AND Complaint_ID={0})
                    SET @DownVotes =(SELECT COUNT(1) FROM pitb.Complaint_Votes WHERE Is_Positive_Vote=0 AND Complaint_ID={0})
                    SELECT * FROM PITB.Complaints_Social_Sharing
			                            WHERE Complaint_Id={0}
                    SELECT @UpVotes UpVotes,@DownVotes DownVotes", complaintId);
            SocialShareData social = new SocialShareData();
            SqlConnection connection = DBHelper.GetConnection();

            SqlCommand cmd = new SqlCommand(query, connection);
            connection.Open();
            using (connection)
            {
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    if (dr.HasRows)
                    {
                        social.UserId = Convert.ToString(dr["User_Id"]);
                        social.PostId = Convert.ToString(dr["Post_Id"]);
                        social.FirstName = Convert.ToString(dr["First_Name"]);
                        social.LastName = Convert.ToString(dr["Last_Name"]);
                        social.Provider = dr["Provider"].ToString();
                        social.DateTime = Convert.ToDateTime(dr["Created_DateTime"]);

                    }



                }
                if (dr.NextResult())
                {
                    while (dr.Read())
                    {
                        social.UpVotes = Convert.ToInt32(dr["UpVotes"]);
                        social.DownVotes = Convert.ToInt32(dr["DownVotes"]);
                    }
                }
            }
            return social;
        }
        private static List<string> LoadAttachments(int complaintId, Config.AttachmentReferenceType referenceType)
        {

            string query = string.Format(@"SELECT Source_Url FROM PITB.Attachments 
			WHERE ReferenceType={0} AND Complaint_Id={1}", Convert.ToByte(referenceType), complaintId);
            List<string> listAttachments = new List<string>();
            SqlConnection connection = DBHelper.GetConnection();
            SqlCommand cmd = new SqlCommand(query, connection);
            connection.Open();
            using (connection)
            {
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    if (dr.HasRows)
                    {

                        listAttachments.Add(Convert.ToString(dr["Source_Url"]));


                    }
                }
            }
            return listAttachments;
        }
        public static VmFilters LoadFilters()
        {
            VmFilters filters = new VmFilters(Config.Campaign.FixItLwmc);
            //VmFilters filters = new VmFilters(Config.Campaign.FixIt);



            return filters;
        }
        public static SubmiteVoteResponse SubmitVoteForComplaint(VmSubmitVote submitModel, CookieUserModel user)
        {
            SubmiteVoteResponse response = new SubmiteVoteResponse() { Status = true, Message = "Thank you for your vote !" };
            using (DBContextHelperLinq db = new DBContextHelperLinq())
            {
                DbComplaintVote userVote = db.DbComplaintVotes.FirstOrDefault(m => m.Complaint_ID == submitModel.ComplaintId && m.Voted_By == user.Data.UserId);
                if (userVote == null)
                {
                    DbComplaintVote vote = new DbComplaintVote
                    {
                        Complaint_ID = submitModel.ComplaintId,
                        Is_Positive_Vote = (byte)submitModel.IsVoteUp,
                        Vote_DateTime = DateTime.Now,
                        Voted_By = user.Data.UserId,
                        Voted_By_Provider = user.Data.Provider,
                        First_Name = user.Data.FirstName,
                        Last_Name = user.Data.LastName
                    };
                    db.DbComplaintVotes.Add(vote);
                    db.SaveChanges();
                }
                else
                {
                    if (userVote.Is_Positive_Vote == (byte)submitModel.IsVoteUp)
                    {
                        userVote.Is_Positive_Vote = (byte)Config.UserVote.NoVote;
                    }
                    else
                    {
                        userVote.Is_Positive_Vote = (byte)submitModel.IsVoteUp;
                    }
                    userVote.Updated_Date_Time = DateTime.Now;
                    db.Entry(userVote).State = EntityState.Modified;
                    db.SaveChanges();
                }
                List<DbComplaintVote> votes = db.DbComplaintVotes.AsNoTracking().Where(m => m.Complaint_ID == submitModel.ComplaintId).ToList();
                if (votes != null && votes.Any())
                {
                    response.UpVoteCount = votes.Count(m => m.Is_Positive_Vote == (byte)Config.UserVote.UpVote);
                    response.DownVoteCount = votes.Count(m => m.Is_Positive_Vote == (byte)Config.UserVote.DownVote);
                    UpdateVoteCountOfComplaint(submitModel.ComplaintId, response.UpVoteCount, response.DownVoteCount);
                }
            }
            return response;
        }
        public static void UpdateVoteCountOfComplaint(long complaintId, int upVoteCount, int downVoteCount)
        {
            Dictionary<string, object> paramz = new Dictionary<string, object>()
            {
                {"@Complaint_ID",complaintId},
                {"@VoteUpCount",upVoteCount},
                {"@VoteDownCount",downVoteCount}
            };
            DBHelper.CrudOperation("[PITB].[Update_Complaint_Vote_Counts]", paramz);
        }


        public static Config.PublicComplaintStatus GetPublicComplaintStatusByDbStatusId(int dbStatusId)
        {
            int statusId;
            DbStatusDictionaryWithPublicComplaintStatuses.TryGetValue((Config.ComplaintStatus)dbStatusId, out statusId);
            return (Config.PublicComplaintStatus)statusId;
        }

        public static List<int> GetDbStatusIdsIntListByPublicComplaintStatus(int publicComplaintStatusId)
        {
            int[] statusIds;
            if (PublicComplaintStatusDictionary.TryGetValue((Config.PublicComplaintStatus)publicComplaintStatusId,
                out statusIds))
            {
                return statusIds.ToList();

            }
            return null;
        }
        public static List<int> GetDbStatusIdsIntListByPublicComplaintStatus(List<int> publicComplaintStatusId)
        {
            List<int> statusList = new List<int>();
            if (publicComplaintStatusId != null && publicComplaintStatusId.Count > 0)
            {
                foreach (int i in publicComplaintStatusId)
                {
                    if (i > 0)
                    {
                        statusList.AddRange(GetDbStatusIdsIntListByPublicComplaintStatus(i));

                    }
                }
            }

            return statusList;
        }

        public static VmNewsFeed GetDetailofComplaint(string encryptedString)
        {
            int complaintId = Convert.ToInt32(Utils.Decrypt(encryptedString));
            DbComplaint dbComplaint = DbComplaint.GetByComplaintId(complaintId);
            VmNewsFeed model=null;
            if (dbComplaint != null)
            {
                model = new VmNewsFeed();
                model.ListComplaints.Add(new ComplaintData()
                {
                    ComplaintId = dbComplaint.Id,
                    Category = dbComplaint.Complaint_Category_Name,
                    //Status = dbComplaint.Complaint_Computed_Status,
                    Description = dbComplaint.Complaint_Remarks,
                    Created_DateTime = Convert.ToDateTime(dbComplaint.Created_Date),
                    Location = new Location() { Latitude = Convert.ToDouble(dbComplaint.Latitude), Longitude = Convert.ToDouble(dbComplaint.Longitude) },
                    District = dbComplaint.District_Name,
                    UC = dbComplaint.UnionCouncil_Name,
                    Town = dbComplaint.Tehsil_Name,
                    StatusChangedText = dbComplaint.StatusChangedComments,
                    StatusChangeDateTime = Convert.ToDateTime(dbComplaint.StatusChangedDate_Time),
                    StatusId =  (int)GetPublicComplaintStatusByDbStatusId(dbComplaint.Complaint_Computed_Status_Id),

                    //NearestLocation = dbComplaint.l

                });
                List<DbAttachments> attachmentse = DbAttachments.GetByComplaintAndAttachmentRef(complaintId, (int)Config.AttachmentReferenceType.Add, complaintId);
                model.ListComplaints[0].StatusChangeAttachments = new List<string>();
                
                
                  var dbStatusChangeLog=  DbComplaintStatusChangeLog.GetStatusChangeLogsAgainstComplaintId(complaintId);
                if (dbStatusChangeLog.Any())
                {
                    var statusChangeLog=dbStatusChangeLog.FirstOrDefault(
                        m => m.StatusId == (int) Config.ComplaintStatus.ResolvedUnverifiedEscalatable);

                    List<DbAttachments> attachmentsResolved = DbAttachments.GetByComplaintAndAttachmentRef(complaintId, (int)Config.AttachmentReferenceType.ChangeStatus,statusChangeLog.Id);
                    if (attachmentsResolved.Any())
                    {
                        foreach (var att1 in attachmentsResolved)
                        {

                            model.ListComplaints[0].StatusChangeAttachments.Add(att1.Source_Url);

                        }

                    }
                }
                if (attachmentse.Any())
                {
                    foreach (var att in attachmentse)
                    {
                        model.ListComplaints[0].ImageList= new List<string>();
                        model.ListComplaints[0].ImageList.Add(att.Source_Url);

                    }

                }
               

            }



            return model;
        }

        #region Status Dictionaries

        public static Dictionary<Config.PublicComplaintStatus, int[]> PublicComplaintStatusDictionary = new Dictionary<Config.PublicComplaintStatus, int[]>()
        {
            {
                Config.PublicComplaintStatus.Pending , new int[]{(int)Config.ComplaintStatus.PendingFresh,(int)Config.ComplaintStatus.PendingReopened}
                
            },
            {
                Config.PublicComplaintStatus.Resolved , new int[]{(int)Config.ComplaintStatus.ResolvedVerified,(int)Config.ComplaintStatus.ResolvedUnverifiedEscalatable}
                
            },
             {
                Config.PublicComplaintStatus.Inapplicable , new int[]{(int)Config.ComplaintStatus.Notapplicable,(int)Config.ComplaintStatus.InApplicableEscalateable  }
                
            },
             {
                Config.PublicComplaintStatus.UnsatisfactoryClosed , new int[]{(int)Config.ComplaintStatus.UnsatisfactoryClosed}
                
            }
        };

        public static Dictionary<Config.ComplaintStatus, int> DbStatusDictionaryWithPublicComplaintStatuses = new Dictionary<Config.ComplaintStatus, int>()
        {
            {
                Config.ComplaintStatus.PendingFresh, (int)Config.PublicComplaintStatus.Pending
                
            },
            {
                Config.ComplaintStatus.PendingReopened ,(int)Config.PublicComplaintStatus.Pending
                
            },
             {
                Config.ComplaintStatus.ResolvedVerified , (int)Config.PublicComplaintStatus.Resolved
                
            },
             {
                 Config.ComplaintStatus.Notapplicable  ,(int)Config.PublicComplaintStatus.Inapplicable
                
            },
             {
                 Config.ComplaintStatus.UnsatisfactoryClosed  , (int)Config.PublicComplaintStatus.UnsatisfactoryClosed
                
            },
             {
                 Config.ComplaintStatus.ResolvedUnverifiedEscalatable  , (int)Config.PublicComplaintStatus.Resolved
                
            },
               {
                 Config.ComplaintStatus.InApplicableEscalateable  , (int)Config.PublicComplaintStatus.Inapplicable
                
            }
        };
        #endregion

    }
}