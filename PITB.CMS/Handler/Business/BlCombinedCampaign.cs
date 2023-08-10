using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using AngleSharp.Extensions;
using AutoMapper.Internal;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Information;
using PITB.CMS.Handler.FileUpload;
using PITB.CMS.Helper.Database;
using PITB.CMS.Helper.Extensions;
using PITB.CMS.Models.Custom;
using PITB.CMS.Models.DB;
using PITB.CMS.Handler.Complaint.Assignment;
using pk.gov.punjab.pws.sdk;
using pk.gov.punjab.pws.sdk.Security;
using WebGrease.Css;

namespace PITB.CMS.Handler.Business
{
    public class BlCombinedCampaign
    {
        public static dynamic SyncComplainant(dynamic submitSyncComplainant, int appId, Config.Language language)
        {
            DBContextHelperLinq db = new DBContextHelperLinq();
            DbPersonInformation dbPersonalInfo = DbPersonInformation.GetByCnicAndMobileNo(submitSyncComplainant.Cnic, submitSyncComplainant.MobileNo, db);
            dynamic syncModel = null;

            //int currDbVersion = DbVersion.GetDbVersion(Config.VersionType.MobileApp, (Config.AppID)appId);

            string status, message;

            // platformId Check


            // if (dbPersonalInfo != null) // username and cnic present
            {
                // if (dbPersonalInfo.Imei_No == null || dbPersonalInfo.Imei_No == submitSyncComplainant.ImeiNo)
                {

                    status = Config.ResponseType.Success.ToString();
                    message = "Successfully Synced";
                    //if (dbVersionId < currDbVersion)
                    {
                        syncModel = GetSyncModelAgaistCnicAndAppId(submitSyncComplainant.Cnic, appId,
                            language);
                    }
                    //else
                    //{
                    //    syncModel = new ExpandoObject();
                    //}

                    //if (dbPersonalInfo != null)
                    //{
                    //    DbUsersVersionMapping.Update_AddVersion(Config.UserType.Complainant, dbPersonalInfo.Person_id, (int)platformId, appId, appVersionId);
                    //}

                    //TextMessageHandler.SendVerificationMessageToComplainant(dbPersonalInfo);
                }
                //else // wrong mobile
                //{
                //    status = Config.ResponseType.Failure.ToString();
                //    message = "Sync Unsuccessful Already Registered with another imei";
                //}
            }
            //   else // if username not present
            //   {
            //       status = Config.ResponseType.Failure.ToString();
            //       message = "Sync Unsuccessful Mobile no or cnic incorrect";
            //   }
            //if (syncModel == null)
            //{
            //    syncModel = new SyncModel();
            //}

            syncModel.Status = status;
            syncModel.Message = message;
            //syncModel.DbVersionId = currDbVersion;
            return syncModel;
        }

        public static dynamic SubmitComplaint(dynamic submitComplaintModel)
        {

            dynamic response = new ExpandoObject();
            try
            {


                //LocationCoordinate complaintLocation = new LocationCoordinate(
                //                                        Convert.ToDouble(submitComplaintModel.lattitude),
                //                                        Convert.ToDouble(submitComplaintModel.longitude)
                //                                        );

                //int provinceId = -1, divisionId = -1, districtId = -1, tehsilId = -1, ucId = -1, wardId = -1;
                //LocationMapping mapping;
                //if (LocationHandler.IsLocationExistInPolygon(complaintLocation, Config.Hierarchy.UnionCouncil,
                //    out mapping))
                //{

                //    List<LocationMapping> allDistrictDivisionTehsilTownsList =
                //        LocationHandler.GetAllAboveHirerchyByHirerchyIdAndType(mapping);
                //    provinceId =
                //        allDistrictDivisionTehsilTownsList.First(m => m.Hierarchy == Config.Hierarchy.Province)
                //            .HirerchyTypeID;
                //    divisionId =
                //        allDistrictDivisionTehsilTownsList.First(m => m.Hierarchy == Config.Hierarchy.Division)
                //            .HirerchyTypeID;
                //    districtId =
                //        allDistrictDivisionTehsilTownsList.First(m => m.Hierarchy == Config.Hierarchy.District)
                //            .HirerchyTypeID;
                //    tehsilId =
                //        allDistrictDivisionTehsilTownsList.First(m => m.Hierarchy == Config.Hierarchy.Tehsil)
                //            .HirerchyTypeID;
                //    ucId =
                //        allDistrictDivisionTehsilTownsList.First(m => m.Hierarchy == Config.Hierarchy.UnionCouncil)
                //            .HirerchyTypeID;
                //}
                //else
                //{
                //    return new ComplaintSubmitResponse(Config.ResponseType.Failure.ToString(), "We are sorry, LWMC does not operate in this area.", "");
                //}

                //submitComplaintModel.provinceID = 1;
                //submitComplaintModel.districtID = 27;
                //submitComplaintModel.tehsilID = tehsilId;
                //submitComplaintModel.ucID = ucId;
                //submitComplaintModel.wardID = wardId;


                //Config.AppID app = (Config.AppID)appId;
                DateTime nowDate = DateTime.Now;

                Dictionary<string, object> paramDict = new Dictionary<string, object>();

                DbPersonInformation dbPersonInfo = DbPersonInformation.GetByCnicAndMobileNo
                    (
                        submitComplaintModel.personCnic,
                        submitComplaintModel.personContactNumber
                     );

                //if (string.IsNullOrEmpty(submitComplaintModel.userId) &&
                //    string.IsNullOrEmpty(submitComplaintModel.userProvider))
                //{
                //    dbPersonInfo = DbPersonInformation.GetByCnic(submitComplaintModel.cnic);

                //}
                //else
                //{
                //    int dbPersonId = DbPersonInformation.GetPersonIdByUserId(submitComplaintModel.userId,
                //        submitComplaintModel.userProvider);
                //    dbPersonInfo = DbPersonInformation.GetPersonInformationByPersonId(dbPersonId);

                //}


                int? personId = 0;
                if (dbPersonInfo != null) personId = dbPersonInfo.Person_id;



                // complaint Info
                paramDict.Add("@Id", -1);
                paramDict.Add("@Person_Id", personId.ToDbObj());



                // Complaint Info
                paramDict.Add("@DepartmentId", submitComplaintModel.departmentId);
                paramDict.Add("@Complaint_Type", Config.ComplaintType.Complaint);
                paramDict.Add("@Complaint_Category", submitComplaintModel.categoryId);
                paramDict.Add("@Complaint_SubCategory", submitComplaintModel.subCategoryId);
                paramDict.Add("@Compaign_Id", ((int)Config.Campaign.CombinedCampaign).ToDbObj());



                // if (app == Config.AppID.FixitLwmc)
                {

                    // provinceId = submitComplaintModel.provinceID;
                    //  divisionId = Convert.ToInt32(DbDistrict.GetById((int)submitComplaintModel.districtID).Division_Id);
                    //districtId = submitComplaintModel.districtID;

                    paramDict.Add("@Province_Id", 1); //submitComplaintModel.provinceID.ToDbObj());
                    paramDict.Add("@Division_Id", 4);

                    paramDict.Add("@District_Id",27); //submitComplaintModel.districtID.ToDbObj());
                }



                //tehsilId = submitComplaintModel.tehsilID;
                //ucId = submitComplaintModel.ucID;
                //wardId = submitComplaintModel.wardID;

                if (!Utility.PropertyExists(submitComplaintModel,"tehsilId"))
                {
                    submitComplaintModel.tehsilId = null;
                }
                paramDict.Add("@Tehsil_Id", ((object)submitComplaintModel.tehsilId).ToDbObj());

                if (!Utility.PropertyExists(submitComplaintModel, "ucId"))
                {
                    submitComplaintModel.ucId = null;
                }
                paramDict.Add("@UnionCouncil_Id", ((object)submitComplaintModel.ucId).ToDbObj());
                
                paramDict.Add("@Ward_Id", (null as object).ToDbObj());
                paramDict.Add("@Complaint_Remarks", submitComplaintModel.comment);
                paramDict.Add("@Agent_Comments", (null as object).ToDbObj());
                paramDict.Add("@ComplaintSrc", ((int)Config.ComplaintSource.Mobile).ToDbObj());



                paramDict.Add("@Agent_Id", (null as object).ToDbObj());
                paramDict.Add("@Complaint_Address", (null as object).ToDbObj());
                paramDict.Add("@Business_Address", (null as object).ToDbObj());

                paramDict.Add("@Complaint_Status_Id", Config.ComplaintStatus.PendingFresh);//If complaint is adding then set complaint status to 1 (Pending(Fresh)
                paramDict.Add("@Created_Date", nowDate.ToDbObj());
                paramDict.Add("@Created_By", (null as object).ToDbObj());
                paramDict.Add("@Complaint_Assigned_Date", (null as object).ToDbObj());
                paramDict.Add("@Completed_Date", (null as object).ToDbObj());
                //paramDict.Add("@Updated_Date", (null as object).ToDbObj());
                paramDict.Add("@Updated_By", (null as object).ToDbObj());
                paramDict.Add("@Is_Deleted", false);
                paramDict.Add("@Date_Deleted", (null as object).ToDbObj());
                paramDict.Add("@Deleted_By", (null as object).ToDbObj());

                paramDict.Add("@IsComplaintEditing", false);




           

                paramDict.Add("@p_Person_id", personId.ToDbObj());
                //paramDict.Add("@p_Person_External_User_Id", submitComplaintModel.userId);
                //paramDict.Add("@p_Person_External_Provider", submitComplaintModel.userProvider);
                paramDict.Add("@Person_Name", submitComplaintModel.personName);
                paramDict.Add("@Person_Father_Name", (null as object).ToDbObj());
                paramDict.Add("@Cnic_No", submitComplaintModel.personCnic);
                paramDict.Add("@Gender", (1).ToDbObj());
                paramDict.Add("@Mobile_No", submitComplaintModel.personContactNumber);
                paramDict.Add("@Secondary_Mobile_No", (null as object).ToDbObj());
                paramDict.Add("@LandLine_No", (null as object).ToDbObj());
                paramDict.Add("@Person_Address", (null as object).ToDbObj());
                paramDict.Add("@Email", (null as object).ToDbObj());
                paramDict.Add("@Nearest_Place", (null as object).ToDbObj());
                paramDict.Add("@p_Province_Id", (1).ToDbObj());
                paramDict.Add("@p_Division_Id", (4).ToDbObj());
                paramDict.Add("@p_District_Id", (27).ToDbObj());
                paramDict.Add("@p_Tehsil_Id", (null as object).ToDbObj());
                paramDict.Add("@p_Town_Id", (null as object).ToDbObj());
                paramDict.Add("@p_Uc_Id", (null as object).ToDbObj());
                paramDict.Add("@p_Created_By", (null as object).ToDbObj());
                paramDict.Add("@p_Updated_By", (null as object).ToDbObj());
                paramDict.Add("@IsProfileEditing", false);

                if (!(string.IsNullOrEmpty(submitComplaintModel.lattitude) &&
                    string.IsNullOrEmpty(submitComplaintModel.lattitude)))
                {
                    paramDict.Add("@Latitude", Convert.ToDouble(submitComplaintModel.lattitude));
                    paramDict.Add("@Longitude", Convert.ToDouble(submitComplaintModel.longitude));

                }

                float catRetainingHours = 0;
                float? subcatRetainingHours = 0;

                //AssignmentMatrix
                List<AssignmentModel> assignmentModelList = null;

                subcatRetainingHours = DbComplaintSubType.GetRetainingByComplaintSubTypeId((int)submitComplaintModel.subCategoryId);
                if (subcatRetainingHours == null) // when subcategory doesnot have retaining hours
                {
                    catRetainingHours = DbComplaintType.GetRetainingHoursByTypeId((int)submitComplaintModel.categoryId);
                    //cateogryType = Config.CategoryType.Main;
                }
                else
                {
                    catRetainingHours = (float)subcatRetainingHours;
                    //cateogryType = Config.CategoryType.Sub;
                }

                //catRetainingHours = DbComplaintType.GetRetainingHoursByTypeId((int) submitComplaintModel.categoryID);
                List<Pair<int?, int?>> listHierarchyPair = new List<Pair<int?, int?>>
				{
					new Pair<int?, int?>((int?)Config.Hierarchy.Province, (int?)1),
					new Pair<int?, int?>((int?)Config.Hierarchy.Division, (int?)4),
					new Pair<int?, int?>((int?)Config.Hierarchy.District, (int?)27),
					new Pair<int?, int?>((int?)Config.Hierarchy.Tehsil, null),
					new Pair<int?, int?>((int?)Config.Hierarchy.UnionCouncil, null),
					new Pair<int?, int?>((int?)Config.Hierarchy.Ward, null)
				};
                assignmentModelList = AssignmentHandler.GetAssignmentModel(new FuncParamsModel.Assignment(nowDate,
                    DbAssignmentMatrix.GetByCampaignIdAndCategoryId((int)Config.Campaign.CombinedCampaign,
                        (int)submitComplaintModel.categoryId, (int)submitComplaintModel.subCategoryId, null, null,
                        listHierarchyPair), catRetainingHours)); // nowDate,
                for (int i = 0; i < 10; i++)
                {
                    if (i < assignmentModelList.Count)
                    {
                        paramDict.Add("@Dt" + (i + 1), assignmentModelList[i].Dt.ToDbObj());
                        paramDict.Add("@SrcId" + (i + 1), assignmentModelList[i].SrcId.ToDbObj());
                        paramDict.Add("@UserSrcId" + (i + 1), assignmentModelList[i].UserSrcId.ToDbObj());
                    }
                    else
                    {
                        paramDict.Add("@Dt" + (i + 1), (null as object).ToDbObj());
                        paramDict.Add("@SrcId" + (i + 1), (null as object).ToDbObj());
                        paramDict.Add("@UserSrcId" + (i + 1), (null as object).ToDbObj());
                    }
                }

                // ------ adding supporting params for escalation params -----------------

                paramDict.Add("@MaxLevel", assignmentModelList.Count);

                paramDict.Add("@MinSrcId", AssignmentHandler.GetMinSrcId(assignmentModelList));
                paramDict.Add("@MaxSrcId", AssignmentHandler.GetMaxSrcId(assignmentModelList));

                paramDict.Add("@MinUserSrcId", AssignmentHandler.GetMinUserSrcId(assignmentModelList));
                paramDict.Add("@MaxUserSrcId", AssignmentHandler.GetMaxUserSrcId(assignmentModelList));


                paramDict.Add("@MinSrcIdDate", AssignmentHandler.GetMinDate(assignmentModelList));
                paramDict.Add("@MaxSrcIdDate", AssignmentHandler.GetMaxDate(assignmentModelList));

                OriginHierarchy originHierarchy = OriginHierarchy.GetOrigin(assignmentModelList);
                paramDict.Add("@Origin_HierarchyId", originHierarchy.OriginHierarchyId);
                paramDict.Add("@Origin_UserHierarchyId", originHierarchy.OriginUserHierarchyId);
                paramDict.Add("@Origin_UserCategoryId1", originHierarchy.OriginUserCategoryId1);
                paramDict.Add("@Origin_UserCategoryId2", originHierarchy.OriginUserCategoryId2);
                paramDict.Add("@Is_AssignedToOrigin", originHierarchy.IsAssignedToOrigin);

                // ----------- end adding custom params --------
                paramDict.Add("@LocationArea", (null as object).ToDbObj());


                //if (submitComplaintModel.picturesList != null)
                //{
                //    List<dynamic> listPictures = submitComplaintModel.picturesList;
                //    List<PostModel.File.Single> listPostedFiles = PostModel.File.GetListFiles(listPictures.Select(x => x.picture).OfType<string>().ToList(), "-1", ".jpg", "image/jpeg");
                //    // fileUploadModel = new FileUploadModel(p.picture, Config.AttachmentSaveType.WebServer, "Image", "image/jpeg", ".jpg", campaignId, complaintId, complaintId, Config.AttachmentReferenceType.Add, Convert.ToInt32(Config.FileType.File), apiRequestId);
                //    FileUploadHandler.UploadMultipleFiles(new PostModel.File(listPostedFiles), Config.AttachmentReferenceType.Add, -1 + "-" + -1, -1, Config.TAG_COMPLAINT_ADD);
                //    //FileUploadHandler.UploadMultipleFiles(listPostedFiles);
                //    //foreach (dynamic p in Utility.GetDynamicList(submitComplaintModel.picturesList))
                //    //{


                //    //    //BlApiHandler.StartUploadUtility(picture.picture, "Image", "image/jpeg", ".jpg", complaintIdStr, Config.FileType.File, apiRequestId);
                //    //}
                //}




                //DataTable dt = DBHelper.GetDataTableByStoredProcedure("[PITB].[Add_Complaints_Crm]", paramDict);
                DataTable dt = DBHelper.GetDataTableByStoredProcedure("[PITB].[Add_Complaints_Crm]", paramDict);
                //Config.CommandMessage cm = new Config.CommandMessage(UtilityExtensions.GetStatus(dt.Rows[0][0].ToString()), dt.Rows[0][1].ToString());
                string[] complaintIdStrArr = dt.Rows[0][1].ToString().Split('-');
                int campaignId = Convert.ToInt32(complaintIdStrArr[0]);
                int complaintId = Convert.ToInt32(complaintIdStrArr[1]);
                string complaintIdStr = dt.Rows[0][1].ToString();
                string complaintFullId = campaignId + "-" + complaintId;
                

                //BlLwmcApiHandler.SaveMobileRequest(submitComplaintModel, Convert.ToInt32(complaintIdStr.Split('-')[1]), apiRequestId);
                
                if (submitComplaintModel.picturesList != null)
                {
                    List<dynamic> listPictures = submitComplaintModel.picturesList;
                    List<PostModel.File.Single> listPostedFiles = PostModel.File.GetListFiles(listPictures.Select(x => x.picture).OfType<string>().ToList(), complaintFullId, ".jpg", "image/jpeg",Config.AttachmentType.File);
                    // fileUploadModel = new FileUploadModel(p.picture, Config.AttachmentSaveType.WebServer, "Image", "image/jpeg", ".jpg", campaignId, complaintId, complaintId, Config.AttachmentReferenceType.Add, Convert.ToInt32(Config.FileType.File), apiRequestId);
                    FileUploadHandler.UploadMultipleFiles(new PostModel.File(listPostedFiles), Config.AttachmentReferenceType.Add, complaintFullId, complaintId, Config.TAG_COMPLAINT_ADD);
                    //FileUploadHandler.UploadMultipleFiles(listPostedFiles);
                    //foreach (dynamic p in Utility.GetDynamicList(submitComplaintModel.picturesList))
                    //{
                       

                    //    //BlApiHandler.StartUploadUtility(picture.picture, "Image", "image/jpeg", ".jpg", complaintIdStr, Config.FileType.File, apiRequestId);
                    //}
                }
                //if (!string.IsNullOrEmpty(submitComplaintModel.video))
                //{
                //    FileUploadModel fileUploadModel = new FileUploadModel(submitComplaintModel.video, Config.AttachmentSaveType.WebServer, "Video", "application/octet-stream", submitComplaintModel.videoFileExtension, campaignId, complaintId, complaintId, Config.AttachmentReferenceType.Add, Convert.ToInt32(Config.FileType.Video), apiRequestId);
                //    FileUploadHandler.StartUploadUtilityPWS(fileUploadModel);
                //    //BlApiHandler.StartUploadUtility(submitComplaintModel.video, "Video", "application/octet-stream",
                //    //    submitComplaintModel.videoFileExtension, complaintIdStr, Config.FileType.Video, apiRequestId);
                //}
           

                // string encryptedComplaintNo = UtilityExtensions.Encrypt(dt.Rows[0][1].ToString().Split('-')[1]);
                new Thread(delegate()
                {
                    DbCampaign dbCampaign = DbCampaign.GetById(campaignId);
                    DbComplaintType dbComplaintType = DbComplaintType.GetById((int)submitComplaintModel.categoryId);

                    string sms = "Your feedback has been submitted in campaign (" + dbCampaign.Campaign_Name + ")\n\nCategory : " +
                       dbComplaintType.Name + "\nFeedback Id : " + complaintFullId;
                    PITB.CMS.Handler.Messages.TextMessageHandler.SendMessageToPhoneNo(
                        submitComplaintModel.personContactNumber, sms);
                    //PITB.CMS.Handler.Messages.TextMessageHandler.SendMessageOnComplaintLaunch(submitComplaintModel.personContactNumber,
                    // submitComplaintModel.campaignID, Convert.ToInt32(dt.Rows[0][1].ToString().Split('-')[1]),
                    // submitComplaintModel.categoryID);
                }).Start();

                //new Thread(delegate()
                //{
                //    PITB.CMS.Handler.Messages.TextMessageHandler.PrepareAndSendMessageToStakeholdersOnComplaintLaunch(
                //        PITB.CMS.Models.DB.DbComplaint.GetByComplaintId(complaintId));
                //}).Start();

                response = Utility.GetApiResponse(true, "Your Complaint Id = " + complaintFullId);
                response.ComplaintId = complaintFullId;
                return response;
                //return new ComplaintSubmitResponse(Config.ResponseType.Success.ToString(), "Your Complaint Id = " + dt.Rows[0][1].ToString(), dt.Rows[0][1].ToString().Split('-')[1]);
            }
            catch (Exception ex)
            {
                response = Utility.GetApiResponse(false, response.Message);
                return response;
                //return new ComplaintSubmitResponse(Config.ResponseType.Failure.ToString(), "Server Error", "");
            }
        }

        public static dynamic GetComplainantComplaints(dynamic model)
        {
            
            ////if (model.ComplaintStatus > 0)
            ////{
            ////    switch (model.ComplaintStatus)
            ////    {
            ////        case (int)Config.ComplaintStatus.ResolvedUnverifiedEscalatable:
            ////            statusIds.Add((int)Config.ComplaintStatus.ResolvedUnverifiedEscalatable);
            ////            statusIds.Add((int)Config.ComplaintStatus.ResolvedVerified);
            ////            break;
            ////        case (int)Config.ComplaintStatus.PendingFresh:
            ////            statusIds.Add((int)Config.ComplaintStatus.PendingFresh);
            ////            statusIds.Add((int)Config.ComplaintStatus.PendingReopened);

            ////            break;

            ////        default:
            ////            statusIds.Add(model.ComplaintStatus);

            ////            break;
            ////    }

            ////}
            //DateTime startDate, endDate;
            //DateTime.TryParseExact(model.StartDate, "dd/MM/yyyy", null, DateTimeStyles.None, out startDate);
            //DateTime.TryParseExact(model.EndDate, "dd/MM/yyyy", null, DateTimeStyles.None, out endDate);
            //endDate = endDate.Add(new TimeSpan(23, 59, 59));

            ///* ----------------------------------
            //    Initialize GetComplainantComplaintModel
            //------------------------------------*/
            //GetComplainantComplaintModel complaintsModel = new GetComplainantComplaintModel
            //{
            //    ListComplaint = new List<ComplainantComplaintModel>(),
            //    Status = Config.ResponseType.Success.ToString(),
            //    Message = Config.ResponseType.Success.ToString()

            //};

            ///* ----------------------------------------------------------------
            // * 
            // *  Load complaints 
            // *  ----------------
            // *  If user provides its identity i.e cnic or user Id 
            // *      then load user created complaints
            // *  else load all complaints 
            // *      Calculating distance of all complaints in database
            // *      Loading nearest complaints to user
            // *      
            // * ----------------------------------------------------------------*/


            //#region Get Complaints

            //List<DbComplaint> listDbComplaints = null;
            //TempComplainatModel tempComplaintModel = null;
            //if (model.CnicOrSocialPresent == Config.CnicOrSocialPresent.None)
            //{

            //    if (model.Filter == Config.FilterTypeApi.Nearest)
            //    {
            //        Dictionary<int, LocationCoordinate> complaintDictionary =
            //        DbComplaint.GetAllComplaintsOfCampaignWithCoordinates(model.CampaignId, startDate, endDate);
            //        Dictionary<int, double> complaintIdWithDistance = new Dictionary<int, double>();

            //        foreach (KeyValuePair<int, LocationCoordinate> complaintWithlocationCoordinate in complaintDictionary)
            //        {
            //            complaintIdWithDistance.Add(
            //                complaintWithlocationCoordinate.Key,
            //                GetDistanceBetweenTwoPoints(
            //                    model.Latitude,
            //                    model.Longitude,
            //                    complaintWithlocationCoordinate.Value.Lt,
            //                    complaintWithlocationCoordinate.Value.Lg));
            //        }

            //        //List<LocationCoordinate> listOfRadiusPoints = LocationHandler.GetRadiusPoints(lat, lng, 0.5);

            //        /*
            //         * ***************************************************************
            //         *  Order the complaints ascending with shortest distance first
            //         * ***************************************************************
            //         */

            //        List<int> orderedComplaints = complaintIdWithDistance
            //            //  .Where(m => m.Value <= Config.RadiusRangeInKiloMeters)
            //            .OrderBy(m => m.Value) //ordering on calculated distance
            //            .Select(m => m.Key)
            //            .Skip(model.From)
            //            .Take(model.To)
            //            .ToList();


            //        listDbComplaints = DbComplaint.GetListByComplaintIds(orderedComplaints, statusIds,
            //            (byte)model.Filter, startDate, endDate);




            //        //IMPORTANT !!!
            //        //Sorting complaints list as ordered complaints list
            //        listDbComplaints = listDbComplaints.OrderBy(d => orderedComplaints.IndexOf(d.Id)).ToList();

            //        //Setting count
            //        complaintsModel.TotalComplaints = complaintIdWithDistance.Count;
            //    }
            //    else
            //    {
            //        tempComplaintModel = DbComplaint.GetListByCampaignIdPaging(model.CampaignId, model.From, model.To, statusIds,
            //          (byte)model.Filter, startDate, endDate);

            //        listDbComplaints = tempComplaintModel.listDbComplaints;
            //        complaintsModel.TotalComplaints = tempComplaintModel.TotalComplaints;
            //        //complaintsModel.TotalComplaints = DbComplaint.GetAllComplaintsCountInCampaign(model.CampaignId, startDate, endDate);
            //    }

            //}
            //else
            //{
            //    int personId = -1;
            //    switch (model.CnicOrSocialPresent)
            //    {
            //        case Config.CnicOrSocialPresent.OnlyCnic:
            //        case Config.CnicOrSocialPresent.CnicAndSocial:

            //            tempComplaintModel = DbComplaint.GetListByPersonCnicPaging(model.Cnic, model.CampaignId,
            //                model.From, model.To, statusIds, (byte)model.Filter, startDate, endDate);

            //            listDbComplaints = tempComplaintModel.listDbComplaints;
            //            complaintsModel.TotalComplaints = tempComplaintModel.TotalComplaints;

            //            //complaintsModel.TotalComplaints = string.IsNullOrEmpty(model.Cnic)
            //            //   ? DbComplaint.GetAllComplaintsCountInCampaign(model.CampaignId, startDate, endDate)
            //            //   : DbComplaint.GetAllComplaintsCountInCampaign(model.CampaignId, model.Cnic, model.Cell, startDate, endDate);

            //            break;
            //        case Config.CnicOrSocialPresent.OnlySocial:
            //            personId = DbPersonInformation.GetPersonIdByUserId(model.UserId, model.UserProvider);
            //            tempComplaintModel = DbComplaint.GetListByPersonUserIdAndProviderPaging(model.UserId, model.UserProvider, model.CampaignId, model.From, model.To, statusIds, (byte)model.Filter);
            //            listDbComplaints = tempComplaintModel.listDbComplaints;
            //            complaintsModel.TotalComplaints = tempComplaintModel.TotalComplaints;
            //            //complaintsModel.TotalComplaints = DbComplaint.GetCountOfComplaintsByPersonId(personId);
            //            break;

            //        //case Config.CnicOrSocialPresent.CnicAndSocial:
            //        //    personId = DbPersonInformation.GetPersonIdByUserId(model.UserId, model.UserProvider, model.Cnic);
            //        //    listDbComplaints = DbComplaint.GetListByPersonId(personId, model.CampaignId, model.From, model.To, model.ComplaintStatus, (byte)model.Filter);
            //        //    complaintsModel.TotalComplaints = DbComplaint.GetCountOfComplaintsByPersonId(personId);
            //        //    break;

            //    }
            //}
            ////if (string.IsNullOrEmpty(model.Cnic) && string.IsNullOrEmpty(model.Cell) & string.IsNullOrEmpty(model.UserId) && string.IsNullOrEmpty(model.UserProvider))
            ////{
            ////    Dictionary<int, LocationCoordinate> complaintDictionary = DbComplaint.GetAllComplaintsOfCampaignWithCoordinates(model.CampaignId);
            ////    Dictionary<int, double> complaintIdWithDistance = new Dictionary<int, double>();

            ////    foreach (KeyValuePair<int, LocationCoordinate> complaintWithlocationCoordinate in complaintDictionary)
            ////    {
            ////        complaintIdWithDistance.Add(
            ////            complaintWithlocationCoordinate.Key,
            ////            GetDistanceBetweenTwoPoints(
            ////                                        model.Latitude,
            ////                                        model.Longitude,
            ////                                        complaintWithlocationCoordinate.Value.Lt,
            ////                                        complaintWithlocationCoordinate.Value.Lg));
            ////    }

            ////    //List<LocationCoordinate> listOfRadiusPoints = LocationHandler.GetRadiusPoints(lat, lng, 0.5);

            ////    /*
            ////     * ***************************************************************
            ////     *  Order the complaints ascending with shortest distance first
            ////     * ***************************************************************
            ////     */

            ////    List<int> orderedComplaints = complaintIdWithDistance
            ////        //  .Where(m => m.Value <= Config.RadiusRangeInKiloMeters)
            ////                                .OrderBy(m => m.Value) //ordering on calculated distance
            ////                                .Select(m => m.Key)
            ////                                .Skip(model.From)
            ////                                .Take(model.To)
            ////                                .ToList();


            ////    listDbComplaints = DbComplaint.GetListByComplaintIds(orderedComplaints, model.ComplaintStatus, (byte)model.Filter);
            ////    complaintsModel.TotalComplaints = complaintIdWithDistance.Count;
            ////}
            ////else
            ////{



            ////    if (!string.IsNullOrEmpty(model.UserId) && !string.IsNullOrEmpty(model.UserProvider) &&
            ////        !string.IsNullOrEmpty(model.Cnic))
            ////    {
            ////        DbPersonInformation dbPerson = DbPersonInformation.GetByCnic(model.Cnic, model.UserId, model.UserProvider);
            ////        listDbComplaints = DbComplaint.GetListByPersonId(dbPerson.Person_id, model.CampaignId, model.From, model.To, model.ComplaintStatus, (byte)model.Filter);

            ////    }

            ////    else if (string.IsNullOrEmpty(model.UserId) && string.IsNullOrEmpty(model.UserProvider))
            ////    {
            ////        listDbComplaints = DbComplaint.GetListByPersonCnicPaging(model.Cnic, model.CampaignId, model.From, model.To, model.ComplaintStatus, (byte)model.Filter);
            ////        complaintsModel.TotalComplaints = string.IsNullOrEmpty(model.Cnic)
            ////            ? DbComplaint.GetAllComplaintsCountInCampaign(model.CampaignId)
            ////            : DbComplaint.GetAllComplaintsCountInCampaign(model.CampaignId, model.Cnic, model.Cell);
            ////    }
            ////    else
            ////    {

            ////        int personId = DbPersonInformation.GetPersonIdByUserId(model.UserId, model.UserProvider);
            ////        listDbComplaints = DbComplaint.GetListByPersonId(personId, model.CampaignId, model.From, model.To, model.ComplaintStatus, (byte)model.Filter);
            ////        complaintsModel.TotalComplaints = DbComplaint.GetCountOfComplaintsByPersonId(personId);
            ////    }
            ////}

            //#endregion
            List<int> statusIds = Utility.GetIntList((string)model.complaintStatuses);

            DateTime startDate, endDate;
            startDate = Utility.GetDateTime(model.startDate);
            endDate = Utility.GetDateTime(model.endDate);
            endDate = endDate.Add(new TimeSpan(23, 59, 59));
            dynamic tempComplaintModel = null;
            tempComplaintModel = DbComplaint.GetListByPersonCnicPaging((string)model.cnic, (int)Config.Campaign.CombinedCampaign,
                           (int)model.from, (int)model.to, statusIds, (byte)((string)model.filter).ParseEnum<Config.FilterTypeApi>(), startDate, endDate);
            List<DbComplaint> listDbComplaints = tempComplaintModel.listDbComplaint;
            int totalComplaints = tempComplaintModel.TotalComplaints;

            //List<Picture> listStatusPicturesUrl = new List<Picture>();
            //List<Picture> listComplaintPicturesUrl = new List<Picture>();
            //List<Video> listComplaintVideoUrl = new List<Video>();


            List<dynamic> listStatusPicturesUrl = null;
            List<dynamic> listComplaintPicturesUrl = null;
            List<dynamic> listComplaintVideoUrl = null;


            //List<int> listChangeStatusLogIds = null;
            List<DbAttachments> listDbAttachment = new List<DbAttachments>();

            // Get status History
            // List<DbComplaintStatusChangeLog> listStatusChangeLogs = DbComplaintStatusChangeLog.GetStatusChangeLogsAgainstComplaintIds(Utility.GetNullableIntList(((List<DbComplaint>)tempComplaintModel.listDbComplaints).Select(n => n.Id).ToList()));

            // End Get status History


            //Dictionary<string, TranslatedModel> translationDict = PITB.CRM_API.Models.DB.DbTranslationMapping.GetTranslationDictionaryFromTranslationMapping(PITB.CRM_API.Models.DB.DbTranslationMapping.GetAllTranslation());

            Dictionary<string, dynamic> translationDict = DbTranslationMapping.GetTranslationDictionaryFromTranslationMapping(DbTranslationMapping.GetAllTranslation());

            /*  Enable them when these fields also need to be translated
             * 
            syncModel.ListProvince.GetTranslatedList("Province_UrduName", translationDict, Config.Language.Urdu);
            syncModel.ListDistrict.GetTranslatedList("District_UrduName", translationDict, Config.Language.Urdu);
            syncModel.ListTehsil.GetTranslatedList("Tehsil_UrduName", translationDict, Config.Language.Urdu);
            syncModel.ListUnionCouncils.GetTranslatedList("Councils_UrduName", translationDict, Config.Language.Urdu);
            syncModel.ListWards.GetTranslatedList("Wards_UrduName", translationDict, Config.Language.Urdu);
            syncModel.ListCategory.GetTranslatedList<DbComplaintType>("Name", translationDict, language);
             *
             */

            //syncModel.ListDepartment.GetTranslatedList("Category_UrduName", "Name", translationDict, Config.Language.Urdu);
            //syncModel.ListDepartment
            //syncModel.ListDepartment = new List<ExpandoObject>();
            //foreach (DbDepartment dbDepartment in listDbDepartment)
            //{
            //    dynamic d = new ExpandoObject();
            //    d.Id = dbDepartment.Id;
            //    d.Name = dbDepartment.Name;
            //    d.NameUrdu = Utility.GetTranslatedValue(translationDict, dbDepartment.Name, Config.Language.Urdu.ToString()); // Utility.GetDictionary() translationDict[dbDepartment.Name];
            //    syncModel.ListDepartment.Add(d);
            //}

            dynamic responseModel = new ExpandoObject();
            responseModel.ListComplaint = new List<ExpandoObject>();
            responseModel.TotalComplaints = tempComplaintModel.TotalComplaints;
            foreach (DbComplaint dbComplaint in listDbComplaints)
            {
                if (dbComplaint.Id == 557023)
                {
                    
                }
                listStatusPicturesUrl = new List<dynamic>();
                listComplaintPicturesUrl = new List<dynamic>();
                listComplaintVideoUrl = new List<dynamic>();
                #region Attachements
                
                //dynamic d = DbComplaint.GetComplaintWithAllData(listDbComplaints/*, (int)Config.ComplaintStatus.Resolved*/);
                List<DbComplaintStatusChangeLog> listStatusChangeLogs = DbComplaintStatusChangeLog.StatusChangeLogsListAgainstComplaintId(dbComplaint.Id, (int)Config.ComplaintStatus.ResolvedUnverified);


                foreach (DbComplaintStatusChangeLog dbComplaintStatusChangeLog in listStatusChangeLogs) //  get all attachment against change log ids
                {
                    listDbAttachment.AddRange(DbAttachments.GetByComplaintAndAttachmentRef(dbComplaint.Id, (int)Config.AttachmentReferenceType.ChangeStatus, dbComplaintStatusChangeLog.Id));
                }

                if (listDbAttachment != null)
                {
                    foreach (DbAttachments dbAttachment in listDbAttachment)
                    {
                        dynamic d = new ExpandoObject();
                        d.picture = dbAttachment.Source_Url;
                        listStatusPicturesUrl.Add(d.picture); 
                    }
                }


                listDbAttachment = DbAttachments.GetByRefAndComplaintIdAndFileType(dbComplaint.Id, Config.AttachmentReferenceType.Add, dbComplaint.Id, Config.AttachmentType.File);

                if (listDbAttachment != null)
                {
                    foreach (DbAttachments dbAttachment in listDbAttachment)
                    {
                        dynamic d = new ExpandoObject();
                        d.picture = dbAttachment.Source_Url;
                        listComplaintPicturesUrl.Add(d.picture);
                    }
                }

                listDbAttachment = DbAttachments.GetByRefAndComplaintIdAndFileType(dbComplaint.Id, Config.AttachmentReferenceType.Add, dbComplaint.Id, Config.AttachmentType.Video);

                if (listDbAttachment != null)
                {
                    foreach (DbAttachments dbAttachment in listDbAttachment)
                    {
                        dynamic d = new ExpandoObject();
                        d.video = dbAttachment.Source_Url;
                        listComplaintVideoUrl.Add(d.picture);
                    }
                }
                #endregion


                //#region Votes
                //List<DbComplaintVote> listComplaintVotes = DbComplaintVote.GetVotesByComplaintId(Convert.ToInt32(dbComplaint.Id));
                //Config.UserVote userVote = GetUserVoteFromAllVotes(listComplaintVotes, model.Cnic, model.Cell, model.UserId, model.UserProvider);
                //#endregion



                #region Complaint Model Stuff


                //ComplainantComplaintModel complaintModel = new ComplainantComplaintModel()
                //{
                dynamic complaintModel = new ExpandoObject();
                
                
                
                
                complaintModel.LocationArea = dbComplaint.LocationArea;

                //responseModel.ExternalUserId = dbComplaint.Person_External_User_Id;
                //ExternalProvider = (string.IsNullOrEmpty(dbComplaint.Person_External_Provider))
                //    ? Config.ExternalProvider.None
                //    : (Config.ExternalProvider)
                //        Enum.Parse(typeof (Config.ExternalProvider), dbComplaint.Person_External_Provider, true);


                //complaintModel.campaignName = DbCampaign.GetById(Convert.ToInt32(dbComplaint.Compaign_Id)).Campaign_Name;
                complaintModel.complaintId = dbComplaint.Id;
                //complaintModel.campaignId = Convert.ToInt32(dbComplaint.Compaign_Id);
                

                complaintModel.cnic = dbComplaint.Person_Cnic;
                complaintModel.comment = dbComplaint.Complaint_Remarks;
                complaintModel.date = Convert.ToDateTime(dbComplaint.Created_Date).ToString("dd/MM/yyyy hh:mm tt");
                
                

                
                complaintModel.personContactNumber = dbComplaint.Person_Contact;
                complaintModel.personName = dbComplaint.Person_Name;
                
                complaintModel.departmentId = Convert.ToInt32(dbComplaint.Department_Id);
                complaintModel.departmentName = dbComplaint.Department_Name;
                complaintModel.categoryId = Convert.ToInt32(dbComplaint.Complaint_Category);
                complaintModel.categoryName = dbComplaint.Complaint_Category_Name;
                complaintModel.subCategoryId = Convert.ToInt32(dbComplaint.Complaint_SubCategory);
                complaintModel.subCategoryName = dbComplaint.Complaint_SubCategory_Name;

                complaintModel.provinceId = Convert.ToInt32(dbComplaint.Province_Id);
                complaintModel.provinceName = dbComplaint.Province_Name;

                complaintModel.divisionId = Convert.ToInt32(dbComplaint.Division_Id);
                complaintModel.divisionName = dbComplaint.Division_Name;

                complaintModel.districtId = Convert.ToInt32(dbComplaint.District_Id);
                complaintModel.districtName = dbComplaint.District_Name;

                complaintModel.tehsilId = Convert.ToInt32(dbComplaint.Tehsil_Id);
                complaintModel.tehsilName = dbComplaint.Tehsil_Name;

                complaintModel.ucId = Convert.ToInt32(dbComplaint.UnionCouncil_Id);
                complaintModel.ucName = dbComplaint.UnionCouncil_Name;

                complaintModel.wardId = Convert.ToInt32(dbComplaint.Ward_Id);
                complaintModel.wardName = dbComplaint.Ward_Name;

                complaintModel.statusId = dbComplaint.Complaint_Computed_Status_Id;
                complaintModel.statusStr = dbComplaint.Complaint_Computed_Status;
                complaintModel.Latitude = dbComplaint.Latitude;
                complaintModel.Longitude = dbComplaint.Longitude;

                //responseModel.UpVotes = Convert.ToInt32(dbComplaint.Vote_Up_Count);
                //responseModel.DownVotes = Convert.ToInt32(dbComplaint.Vote_Down_Count);
                //responseModel.UserVote = userVote;
                complaintModel.ComplainantRemarks = dbComplaint.Complainant_Remark_Str;
                complaintModel.StakeHolderRemarks = dbComplaint.StatusChangedComments;
                complaintModel.StakeHolderRemarksDate = (dbComplaint.StatusChangedDate_Time == null)
                    ? string.Empty
                    : dbComplaint.StatusChangedDate_Time.ToString();
                complaintModel.ComplainantSatisfactionStatus = (dbComplaint.Complainant_Remark_Id == null)
                    ? (int) Config.ComplainantRemarkType.None
                    : (int) ((Config.ComplainantRemarkType) Convert.ToInt32(dbComplaint.Complainant_Remark_Id));
                //responseModel.DistanceFromYourLocation = (dbComplaint.Latitude.HasValue)
                //    ? GetDistanceBetweenTwoPoints(model.Latitude, model.Longitude, dbComplaint.Latitude.Value,
                //        dbComplaint.Longitude.Value, 'K')
                //    : -1;
                complaintModel.ComputedRemainingTotalTime = dbComplaint.Computed_Remaining_Total_Time;

                complaintModel.ListPicturesComplaintsUrl = listComplaintPicturesUrl;
                complaintModel.ListPicturesStatusUrl = listStatusPicturesUrl;
                complaintModel.ListVideoComplaintsUrl = listComplaintVideoUrl;

                //};
                #endregion


                #region Status Id

                //switch (dbComplaint.Complaint_Computed_Status_Id)
                //{
                //    case (int)Config.ComplaintStatus.ResolvedUnverifiedEscalatable:
                //    case (int)Config.ComplaintStatus.ResolvedVerified:

                //        complaintModel.statusId = (int)Config.ComplaintStatus.ResolvedUnverifiedEscalatable;

                //        break;
                //    case (int)Config.ComplaintStatus.PendingFresh:
                //    case (int)Config.ComplaintStatus.PendingReopened:

                //        complaintModel.statusId = (int)Config.ComplaintStatus.PendingFresh;

                //        break;
                //    default:
                //        complaintModel.statusId = Convert.ToInt32(dbComplaint.Complaint_Computed_Status_Id);

                //        break;
                //}


                #endregion


                #region OldCommentedStuff
                //complaintModel.categoryName = DbComplaintType.GetById(complaintModel.categoryID);



                //(complaintModel.categoryID != 0) ? DbComplaintType.GetById(complaintModel.categoryID).Name : "";
                //(complaintModel.subCategoryID != 0) ? DbComplaintSubType.GetById(complaintModel.subCategoryID).Name : "";





                //(complaintModel.provinceID != 0) ? DbProvince.GetById(complaintModel.provinceID).Province_Name : "";
                //(complaintModel.districtID != 0) ? DbDistrict.GetById(complaintModel.districtID).District_Name : "";
                //(complaintModel.tehsilID != 0) ? DbTehsil.GetById(complaintModel.tehsilID).Tehsil_Name : "";
                #endregion


                #region Translations

                //complaintModel.campaignName = Utility.GetTranslatedValue(translationDict, complaintModel.campaignName, ((Config.Language) model.languageId).ToString()); // Utility.GetDictionary() translationDict[dbDepartment.Name];
                complaintModel.departmentName = Utility.GetTranslatedValue(translationDict, complaintModel.departmentName, ((Config.Language)model.languageId).ToString()); // Utility.GetDictionary() translationDict[dbDepartment.Name];
                complaintModel.categoryName = Utility.GetTranslatedValue(translationDict, complaintModel.categoryName, ((Config.Language)model.languageId).ToString()); // Utility.GetDictionary() translationDict[dbDepartment.Name];
                complaintModel.subCategoryName = Utility.GetTranslatedValue(translationDict, complaintModel.subCategoryName, ((Config.Language)model.languageId).ToString()); // Utility.GetDictionary() translationDict[dbDepartment.Name];
                complaintModel.statusStr = Utility.GetTranslatedValue(translationDict, complaintModel.statusStr, ((Config.Language)model.languageId).ToString()); // Utility.GetDictionary() translationDict[dbDepartment.Name];


                //complaintModel.GetTranslatedModel<ComplainantComplaintModel>("campaignName", translationDict, (Config.Language)model.LanguageId);

                //complaintModel.GetTranslatedModel<ComplainantComplaintModel>("categoryName", translationDict, (Config.Language)model.LanguageId);
                //complaintModel.GetTranslatedModel<ComplainantComplaintModel>("subCategoryName", translationDict, (Config.Language)model.LanguageId);

                //complaintModel.GetTranslatedModel<ComplainantComplaintModel>("statusStr", translationDict, (Config.Language)model.LanguageId);
                #endregion

                // Add Resolve History
                // Populate History
                DbComplaintStatusChangeLog dbStatusLog = listStatusChangeLogs.Where(n => n.Complaint_Id == dbComplaint.Id).OrderByDescending(n => n.StatusChangeDateTime).FirstOrDefault();
                //foreach (DbComplaintStatusChangeLog dbStatusLog in listComplaintStatusChangeLogs)
                dynamic logHistory = new ExpandoObject();
                if (dbStatusLog != null)
                {
                    //complaintModel.ListLogHistory = new List<LogHistory>();
                    
                    logHistory.ComplaintId = Convert.ToInt32(dbStatusLog.Complaint_Id);

                    logHistory.StatusChangeComments = dbStatusLog.Comments;
                    logHistory.StatusChangeDateTime = dbStatusLog.StatusChangeDateTime.ToString("dd/MM/yyyy hh:mm tt");
                    logHistory.StatusId = Convert.ToByte(dbStatusLog.StatusId);
                    logHistory.StatusChangedByUserName = dbStatusLog.ChangedBy.Name;
                    logHistory.StatusChangedByUserContact = dbStatusLog.ChangedBy.Phone;
                    logHistory.StatusChangedByUserHierarchy = dbStatusLog.ChangedBy.User_Hierarchy_Id;//((Config.LwmcResolverHirarchyId)Convert.ToByte(dbStatusLog.ChangedBy.User_Hierarchy_Id)).ToString();
                    logHistory.Status = ((Config.ComplaintStatus)responseModel.logHistory.StatusId).GetDisplayName();
                    logHistory.ListAttachments = dbStatusLog.ListDbAttachments;
                    //complaintModel.ListLogHistory.Add(logHistory);
                }
                complaintModel.LastStatus = logHistory;
                //List<DbComplaintStatusChangeLog> listComplaintStatusChangeLogs = listStatusChangeLogs.Where(n => n.Complaint_Id == dbComplaint.Id).OrderByDescending(n=>n.StatusChangeDateTime).ToList();
                //foreach (DbComplaintStatusChangeLog dbStatusLog in listComplaintStatusChangeLogs)
                //{
                //    complaintModel.ListLogHistory = new List<LogHistory>();
                //    LogHistory logHistory = new LogHistory();
                //    logHistory.ComplaintId = Convert.ToInt32(dbStatusLog.Complaint_Id);

                //    logHistory.StatusChangeComments = dbStatusLog.Comments;
                //    logHistory.StatusChangeDateTime = dbStatusLog.StatusChangeDateTime.ToString("dd/MM/yyyy hh:mm tt");
                //    logHistory.StatusId = Convert.ToByte(dbStatusLog.StatusId);
                //    logHistory.StatusChangedByUserName = dbStatusLog.ChangedBy.Name;
                //    logHistory.StatusChangedByUserContact = dbStatusLog.ChangedBy.Phone;
                //    logHistory.StatusChangedByUserHierarchy = ((Config.LwmcResolverHirarchyId)Convert.ToByte(dbStatusLog.ChangedBy.User_Hierarchy_Id)).ToString();
                //    logHistory.Status = ((Config.ComplaintStatus)logHistory.StatusId).GetDisplayName();
                //    logHistory.ListAttachments = dbStatusLog.ListDbAttachments;
                //    complaintModel.ListLogHistory.Add(logHistory);

                //}
                // End Populate History
                // End Add Resolve History
                responseModel.ListComplaint.Add(complaintModel);
            }
            Utility.GetApiResponse(true, null, null, responseModel);
            //responseModel.SetSuccess();
            return responseModel;
        }


        public static dynamic GetSyncModelAgaistCnicAndAppId(string cnic, int appId, Config.Language language) // SmartApp
        {
            //List<Config.AppConfig> listAppCampaignConfig = Config.CombinedCampaignConfigurations;
            //List<Config.CampaignConfig> listCampaignConfig = Utility.GetCampaignConfigList(listAppCampaignConfig, (Config.AppID)appId);

            //List<Config.Campaign> listCampaignIds = listCampaignConfig.Select(n => n.CampaignId).ToList();

            List<Config.Campaign> listCampaignIds = new List<Config.Campaign>(){ Config.Campaign.CombinedCampaign};
            dynamic syncModel = new ExpandoObject();
            //{
            //    ListCategory = new List<DbComplaintType>(),
            //    ListSubCategory = new List<DbComplaintSubType>()
            //};
            //List<DbComplaintType> listComplaintTypeTemp = null;
            //int districtId = Config.CampDistDictionary[listCampaignIds[0]];

            //syncModel.ListDepartment = new List<DbDepartment>();
            //syncModel.ListCategory = new List<DbComplaintType>();
            //syncModel.ListSubCategory = new List<DbComplaintSubType>();

            List<DbDepartment> listDbDepartment = new List<DbDepartment>();
            List<DbComplaintType> ListCategory = new List<DbComplaintType>();
            List<DbComplaintSubType> ListSubCategory = new List<DbComplaintSubType>();

            foreach (var campaign in listCampaignIds)
            {
                var campaignId = (int)campaign;
                //listComplaintTypeTemp = DbComplaintType.GetByCampaignId(campaignId);
                listDbDepartment = DbDepartment.GetByCampaignAndGroupId(campaignId, null);
                List<int> listDepId = listDbDepartment.
                    Select(n => n.Id).ToList();
                ListCategory.AddRange(DbComplaintType.GetByDepartmentIds(Utility.GetNullableIntList(listDepId)));
                ListSubCategory.AddRange(
                    DbComplaintSubType.GetByComplaintTypes(
                        ((List<DbComplaintType>)ListCategory).Select(n => n.Complaint_Category).ToList()));
            }



            //DbPersonInformation dbPersonalInfo = DbPersonInformation.GetByCnic(cnic);

            //DbProvince dbProvince = DbProvince.GetById(Convert.ToInt32(dbPersonalInfo.Province_Id));
            //syncModel.ListProvince = (dbProvince != null) ? new List<DbProvince>() { dbProvince } : new List<DbProvince>(); //DbProvince.GetAllProvincesList();
            ////syncModel.ListDistrict = DbDistrict.GetAllDistrictList();

            //DbDistrict dbDistrict = DbDistrict.GetById(Convert.ToInt32(dbPersonalInfo.District_Id));
            //syncModel.ListDistrict = (dbDistrict != null) ? new List<DbDistrict>() { dbDistrict } : new List<DbDistrict>();
            ////syncModel.ListDistrict = new List<DbDistrict>() { DbDistrict.GetById(districtId) }; // hardcoding for bahawalpur 

            //DbTehsil dbTehsil = DbTehsil.GetById(Convert.ToInt32(dbPersonalInfo.Tehsil_Id));
            //syncModel.ListTehsil = (dbTehsil != null) ? new List<DbTehsil>() { dbTehsil } : new List<DbTehsil>();
            ////syncModel.ListTehsil = DbTehsil.GetTehsilList(syncModel.ListDistrict.ToList().FirstOrDefault().District_Id);

            ////int? groupId = DbHierarchyCampaignGroupMapping.GetModelByCampaignId((int) listCampaignConfig[0].CampaignId);

            //DbUnionCouncils dbUnionCouncil = DbUnionCouncils.GetById(Convert.ToInt32(dbPersonalInfo.Uc_Id));
            //syncModel.ListUnionCouncils = (dbTehsil != null) ? new List<DbUnionCouncils>() { dbUnionCouncil } : new List<DbUnionCouncils>();

            //DbWards dbWards = DbWards.GetByWardId(Convert.ToInt32(dbPersonalInfo.Ward_Id));
            //syncModel.ListWards = (dbWards != null) ? new List<DbWards>() { dbWards } : new List<DbWards>();

            //  syncModel.ListSubCategory = DbComplaintSubType.PopulateImageInBase64FromComplaintsSubtype(syncModel.ListSubCategory);


            Dictionary<string, dynamic> translationDict = DbTranslationMapping.GetTranslationDictionaryFromTranslationMapping(DbTranslationMapping.GetAllTranslation());

            /*  Enable them when these fields also need to be translated
             * 
            syncModel.ListProvince.GetTranslatedList("Province_UrduName", translationDict, Config.Language.Urdu);
            syncModel.ListDistrict.GetTranslatedList("District_UrduName", translationDict, Config.Language.Urdu);
            syncModel.ListTehsil.GetTranslatedList("Tehsil_UrduName", translationDict, Config.Language.Urdu);
            syncModel.ListUnionCouncils.GetTranslatedList("Councils_UrduName", translationDict, Config.Language.Urdu);
            syncModel.ListWards.GetTranslatedList("Wards_UrduName", translationDict, Config.Language.Urdu);
            syncModel.ListCategory.GetTranslatedList<DbComplaintType>("Name", translationDict, language);
             *
             */

            //syncModel.ListDepartment.GetTranslatedList("Category_UrduName", "Name", translationDict, Config.Language.Urdu);
            //syncModel.ListDepartment
            syncModel.ListDepartment = new List<ExpandoObject>();
            foreach (DbDepartment dbDepartment in listDbDepartment)
            {
                dynamic d = new ExpandoObject();
                d.Id = dbDepartment.Id;
                d.Name = dbDepartment.Name;
                d.NameUrdu = Utility.GetTranslatedValue(translationDict, dbDepartment.Name, Config.Language.Urdu.ToString()); // Utility.GetDictionary() translationDict[dbDepartment.Name];
                syncModel.ListDepartment.Add(d);
            }

            syncModel.ListCategory = new List<ExpandoObject>();
            foreach (DbComplaintType dbCategory in ListCategory)
            {
                dynamic c = new ExpandoObject();
                c.Id = dbCategory.Complaint_Category;
                c.Name = dbCategory.Name;
                c.NameUrdu = Utility.GetTranslatedValue(translationDict, dbCategory.Name, Config.Language.Urdu.ToString());
                c.DepartmentId = dbCategory.DepartmentId;
                syncModel.ListCategory.Add(c);
            }

            syncModel.ListSubCategory = new List<ExpandoObject>();
            foreach (DbComplaintSubType st in ListSubCategory)
            {
                dynamic sc = new ExpandoObject();
                sc.Id = st.Complaint_SubCategory;
                sc.Name = st.Name;
                sc.NameUrdu = Utility.GetTranslatedValue(translationDict, st.Name, Config.Language.Urdu.ToString());
                sc.CategoryId = st.Complaint_Type_Id;
                syncModel.ListSubCategory.Add(sc);
            }



            //syncModel.ListCategory.GetTranslatedList("Category_UrduName", "Name", translationDict, Config.Language.Urdu);

            //          syncModel.ListSubCategory.GetTranslatedList<DbComplaintSubType>("Name", translationDict, language);
            //syncModel.ListSubCategory.GetTranslatedList("SubCategory_UrduName", "Name", translationDict, Config.Language.Urdu);


            return syncModel;
        }

        public static dynamic SubmitCategories(dynamic model)// SmartApp
        {
            try
            {


                Dictionary<string, dynamic> translationDict =
                    DbTranslationMapping.GetTranslationDictionaryFromTranslationMapping(
                        DbTranslationMapping.GetAllTranslation());
                List<dynamic> listCategories = model.listCategories;
                int? catId = null;
                string catName = null;
                string catUrduName = null;

                DBContextHelperLinq db = null;
                List<dynamic> listCategoriesTemp = null;
                dynamic c = null;
                for (int i=0; i< listCategories.Count;i++)
                {

                    // Add Department
                    c = listCategories[i];
                    catId = (int?) c.cat1Id;
                    catName = c.cat1Name;
                    catUrduName = c.cat1UrduName;
                    db = new DBContextHelperLinq();
                    DbDepartment dbDepartment = (catId == null)
                        ? new DbDepartment()
                        : DbDepartment.GetByDepartmentId((int) catId, db);
                    if (dbDepartment.Id == 0 && catName!=null) // insert entry if not present
                    {
                        dbDepartment.Campaign_Id = (int) Config.Campaign.CombinedCampaign;
                        dbDepartment.Name = catName;
                        dbDepartment.Is_Active = true;
                        db.DbDepartment.Add(dbDepartment);
                    }

                    if (catName != null && !translationDict.ContainsKey(catName))
                    {
                        db.DbTranslationMapping.Add(new DbTranslationMapping()
                        {
                            Parent_Type_Id = (int) Config.Campaign.CombinedCampaign,
                            OrignalString = catName,
                            UrduMappedString = catUrduName,
                            Is_Active = true
                        });
                        translationDict.Add(catName, new ExpandoObject());
                    }
                    db.SaveChanges();
                    if (dbDepartment.Id == 0)
                    {
                        c.Cat1IdMine = null;
                    }
                    else
                    {
                        c.Cat1IdMine = dbDepartment.Id; 
                    }
                    
                    listCategoriesTemp = listCategories.Where(n => n.cat1Name == c.cat1Name).ToList();
                    listCategoriesTemp.All(v =>
                    {
                        v.cat1Id = c.Cat1IdMine;
                        return true;
                    });



                    // Add DbComplaintType
                    catId = (int?)c.cat2Id;
                    catName = c.cat2Name;
                    catUrduName = c.cat2UrduName;
                    db = new DBContextHelperLinq();
                    DbComplaintType dbComplaintType = (catId == null)
                        ? new DbComplaintType()
                        : DbComplaintType.GetById((int) catId, db);
                    if (dbComplaintType.Complaint_Category == 0 && catName != null) // insert entry
                    {
                        dbComplaintType.Campaign_Id = (int) Config.Campaign.CombinedCampaign;
                        dbComplaintType.Name = catName;
                        dbComplaintType.DepartmentId = dbDepartment.Id;
                        dbComplaintType.Is_Active = true;
                        dbComplaintType.RetainingHours = 1;
                        db.DbComplaints_Type.Add(dbComplaintType);
                    }
                    
                    if (catName!=null && !translationDict.ContainsKey(catName))
                    {
                        db.DbTranslationMapping.Add(new DbTranslationMapping()
                        {
                            Parent_Type_Id = (int) Config.Campaign.CombinedCampaign,
                            OrignalString = catName,
                            UrduMappedString = catUrduName,
                            Is_Active = true
                        });
                        translationDict.Add(catName, new ExpandoObject());
                    }
                    db.SaveChanges();

                    if (dbComplaintType.Complaint_Category == 0)
                    {
                        c.Cat2IdMine = null;
                    }
                    else
                    {
                        c.Cat2IdMine = dbComplaintType.Complaint_Category;
                    }


                    listCategoriesTemp = listCategories.Where(n => n.cat1Name == c.cat1Name && n.cat2Name == c.cat2Name).ToList();
                    listCategoriesTemp.All(v =>
                    {
                        v.cat2Id = c.Cat2IdMine;
                        return true;
                    });


                    // Add DbComplaintSubType
                    catId = (int?)c.cat3Id;
                    catName = c.cat3Name;
                    catUrduName = c.cat3UrduName;

                    DbComplaintSubType dbComplaintSubType = (catId == null)
                        ? new DbComplaintSubType()
                        : DbComplaintSubType.GetById((int) catId, db);
                    if (dbComplaintSubType.Complaint_SubCategory == 0 && catName != null) // insert entry
                    {
                        dbComplaintSubType.Name = catName;
                        dbComplaintSubType.Complaint_Type_Id = dbComplaintType.Complaint_Category;
                        dbComplaintSubType.Is_Active = true;
                        db.DbComplaints_SubType.Add(dbComplaintSubType);
                    }

                    if (catName != null && !translationDict.ContainsKey(catName))
                    {
                        db.DbTranslationMapping.Add(new DbTranslationMapping()
                        {
                            Parent_Type_Id = (int) Config.Campaign.CombinedCampaign,
                            OrignalString = catName,
                            UrduMappedString = catUrduName,
                            Is_Active = true
                        });
                        translationDict.Add(catName, new ExpandoObject());
                    }
                    db.SaveChanges();
                    //c.Cat3IdMine = dbComplaintSubType.Complaint_SubCategory;
                    if (dbComplaintSubType.Complaint_SubCategory == 0)
                    {
                        c.Cat3IdMine = null;
                    }
                    else
                    {
                        c.Cat3IdMine = dbComplaintSubType.Complaint_SubCategory;
                    }
                    listCategoriesTemp = listCategories.Where(n => n.cat1Name == c.cat1Name && n.cat2Name == c.cat2Name && n.cat3Name == c.cat3Name).ToList();
                    listCategoriesTemp.All(v =>
                    {
                        v.cat3Id = c.Cat3IdMine;
                        return true;
                    });
                }
                model = Utility.GetApiResponse(true, null, null, model);

            }
            catch (Exception ex)
            {
                model = Utility.GetApiResponse(false, null, null, model);
            }
            return model;
        }
    }
}