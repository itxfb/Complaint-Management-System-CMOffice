using PITB.CRM_API.Models.DB;
using PITB.CRM_API.Models.Custom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PITB.CRM_API.Handler.Assignment
{
    public class AssignmentHandler
    {
        public static List<AssignmentModel> GetAssignmnetModelByCampaignCategorySubCategory(int campaignId, int categoryId, int subCategoryId, bool canPopulateMatrix)
        {
            if (canPopulateMatrix)
            {
                DateTime nowDate = DateTime.Now;
                float catRetainingHours = 0;
                float? subcatRetainingHours = 0;
                List<AssignmentModel> assignmentModelList = null;

                subcatRetainingHours = DbComplaintSubType.GetRetainingByComplaintSubTypeId(subCategoryId);
                if (subcatRetainingHours == null) // when subcategory doesnot have retaining hours
                {
                    catRetainingHours = DbComplaintType.GetRetainingHoursByTypeId(categoryId);
                    //cateogryType = Config.CategoryType.Main;
                }
                else
                {
                    catRetainingHours = (float)subcatRetainingHours;
                    //cateogryType = Config.CategoryType.Sub;
                }
                assignmentModelList = AssignmentHandler.GetAssignmentModel(nowDate,
                    DbAssignmentMatrix.GetByCampaignIdAndCategoryId(campaignId, categoryId, subCategoryId),
                    catRetainingHours);

                return assignmentModelList;
            }
            else
            {
                return new List<AssignmentModel>();
            }
        }
        public static List<AssignmentModel> GetAssignmentModel(DateTime creationDate, List<DbAssignmentMatrix> listAssignmentMatrix, float categoryRetainingHours /*= 1f*/)
        {
            listAssignmentMatrix = listAssignmentMatrix.OrderBy(n => n.LevelId).ToList();
            if (IsCategoryAssignmentMatrix(listAssignmentMatrix))
            {
                categoryRetainingHours = 1f;
            }

            DateTime assignemntDateTime = creationDate;
            List<AssignmentModel> assignmentList = new List<AssignmentModel>();
            AssignmentModel assignmentModel = null;
            for (int i = 0; i < listAssignmentMatrix.Count - 1; i++)
            {
                assignmentModel = new AssignmentModel();
                assignmentModel.SrcId = listAssignmentMatrix[i].ToSourceId;
                assignmentModel.UserSrcId = listAssignmentMatrix[i].ToUserSourceId;
                assignmentModel.Dt = assignemntDateTime.AddHours(Convert.ToDouble((listAssignmentMatrix[i + 1].RetainingHours * categoryRetainingHours)));
                assignemntDateTime = (DateTime)assignmentModel.Dt;
                assignmentList.Add(assignmentModel);
            }
            return assignmentList;
        }

        private static bool IsCategoryAssignmentMatrix(List<DbAssignmentMatrix> listAssignmentMatrix)
        {
            // if category matrix present
            DbAssignmentMatrix assignmentMatrix = listAssignmentMatrix.Where(n => n.CategoryType != null && n.CategoryId != null).FirstOrDefault();
            return (assignmentMatrix != null);
        }


        //public static List<AssignmentModel> GetAssignmentModelOnStatusChange(int userHierarchyId, int userInnerHierarchyId, DbComplaint dbComplaint, int statusId, DateTime currentDate, List<DbAssignmentMatrix> listAssignmentMatrix, float categoryRetainingTime, bool canResetOnPendingReopen)
        //{
        //    DbStatuses dbStatus = DbStatuses.GetById(statusId);


        //    listAssignmentMatrix = listAssignmentMatrix.OrderBy(n => n.LevelId).ToList();
        //    bool canPopulateDates = false;
        //    DateTime assignmentDateTime = currentDate;
        //    List<AssignmentModel> assignmentList = new List<AssignmentModel>();
        //    AssignmentModel assignmentModel = null;
        //    DbAssignmentMatrix nextAssignmentMatrixVal = null;
        //    bool forceEscalateOnStatusChange = Convert.ToBoolean(dbStatus.ForceEscalateOnStatusChange);
        //    bool isDatesPopulatable = true;
        //    if (statusId == Convert.ToInt32(Config.ComplaintStatus.PendingReopened) || forceEscalateOnStatusChange) // if reopening then reassign matrix
        //    {
        //        listAssignmentMatrix = listAssignmentMatrix.OrderBy(n => n.LevelId).ToList();
        //        if (IsCategoryAssignmentMatrix(listAssignmentMatrix))
        //        {
        //            categoryRetainingTime = 1f;
        //        }

        //        for (int i = 0; i < listAssignmentMatrix.Count - 1; i++)
        //        {
        //            assignmentModel = new AssignmentModel();
        //            if ((listAssignmentMatrix[i].ToSourceId == userHierarchyId || canResetOnPendingReopen) && isDatesPopulatable)
        //            {
        //                if (!forceEscalateOnStatusChange)
        //                {
        //                    canPopulateDates = true;
        //                    assignmentDateTime = currentDate;
        //                    isDatesPopulatable = false;
        //                }

        //            }
        //            if (canPopulateDates)
        //            {
        //                assignmentModel.SrcId = listAssignmentMatrix[i].ToSourceId;
        //                assignmentModel.Dt = assignmentDateTime.AddHours(Convert.ToDouble(listAssignmentMatrix[i + 1].RetainingHours * categoryRetainingTime));
        //                assignmentModel.UserSrcId = listAssignmentMatrix[i].ToUserSourceId;
        //                assignmentDateTime = (DateTime)assignmentModel.Dt;
        //            }
        //            else
        //            {
        //                assignmentModel.SrcId = listAssignmentMatrix[i].ToSourceId;
        //                assignmentModel.Dt = assignmentDateTime.AddHours(-Convert.ToDouble(listAssignmentMatrix[i + 1].RetainingHours * categoryRetainingTime));
        //                assignmentModel.UserSrcId = listAssignmentMatrix[i].ToUserSourceId;
        //                assignmentDateTime = (DateTime)assignmentModel.Dt;
        //            }
        //            assignmentList.Add(assignmentModel);

        //            if ((listAssignmentMatrix[i].ToSourceId == userHierarchyId || canResetOnPendingReopen) &&
        //                isDatesPopulatable)
        //            {
        //                if (forceEscalateOnStatusChange &&
        //                    userInnerHierarchyId == listAssignmentMatrix[i].ToUserSourceId)
        //                {
        //                    //i++;
        //                    canPopulateDates = true;
        //                    assignmentDateTime = currentDate;
        //                    isDatesPopulatable = false;
        //                }
        //            }
        //        }
        //    }
        //    else
        //    {
        //        assignmentList.Add(new AssignmentModel(dbComplaint.Dt1, dbComplaint.SrcId1, dbComplaint.UserSrcId1));
        //        assignmentList.Add(new AssignmentModel(dbComplaint.Dt2, dbComplaint.SrcId2, dbComplaint.UserSrcId2));
        //        assignmentList.Add(new AssignmentModel(dbComplaint.Dt3, dbComplaint.SrcId3, dbComplaint.UserSrcId3));
        //        assignmentList.Add(new AssignmentModel(dbComplaint.Dt4, dbComplaint.SrcId4, dbComplaint.UserSrcId4));
        //        assignmentList.Add(new AssignmentModel(dbComplaint.Dt5, dbComplaint.SrcId5, dbComplaint.UserSrcId5));
        //        assignmentList.Add(new AssignmentModel(dbComplaint.Dt6, dbComplaint.SrcId6, dbComplaint.UserSrcId6));
        //        assignmentList.Add(new AssignmentModel(dbComplaint.Dt7, dbComplaint.SrcId7, dbComplaint.UserSrcId7));
        //        assignmentList.Add(new AssignmentModel(dbComplaint.Dt8, dbComplaint.SrcId8, dbComplaint.UserSrcId8));
        //        assignmentList.Add(new AssignmentModel(dbComplaint.Dt9, dbComplaint.SrcId9, dbComplaint.UserSrcId9));
        //        assignmentList.Add(new AssignmentModel(dbComplaint.Dt10, dbComplaint.SrcId10, dbComplaint.UserSrcId10));
        //    }
        //    return assignmentList;
        //}


        public static int GetActualComplaintStauts(DbComplaint complaint)
        {
            DateTime currentDate = DateTime.Now;
            int statusToReturn = (int)complaint.Complaint_Status_Id;

            if (statusToReturn == Convert.ToInt32(Config.ComplaintStatus.PendingFresh) || statusToReturn == Convert.ToInt32(Config.ComplaintStatus.PendingReopened))
            {
                if (complaint.Dt2 == null && complaint.Dt1 != null && complaint.Dt1 <= currentDate)
                {
                    statusToReturn = Convert.ToInt32(Config.ComplaintStatus.UnsatisfactoryClosed);
                }
                else if (complaint.Dt3 == null && complaint.Dt2 != null && complaint.Dt2 <= currentDate)
                {
                    statusToReturn = Convert.ToInt32(Config.ComplaintStatus.UnsatisfactoryClosed);
                }
                else if (complaint.Dt4 == null && complaint.Dt3 != null && complaint.Dt3 <= currentDate)
                {
                    statusToReturn = Convert.ToInt32(Config.ComplaintStatus.UnsatisfactoryClosed);
                }
                else if (complaint.Dt5 == null && complaint.Dt4 != null && complaint.Dt4 <= currentDate)
                {
                    statusToReturn = Convert.ToInt32(Config.ComplaintStatus.UnsatisfactoryClosed);
                }
                else if (complaint.Dt5 != null && complaint.Dt5 <= currentDate)
                {
                    statusToReturn = Convert.ToInt32(Config.ComplaintStatus.UnsatisfactoryClosed);
                }
                /*else if (complaint.Dt3 != null && complaint.Dt3 <= currentDate)
                {
                    statusToReturn = Convert.ToInt32(Config.ComplaintStatus.UnsatisfactoryClosed);
                }*/
            }
            return statusToReturn;
        }

        public static int? GetMaxSrcId(List<AssignmentModel> listAssignmentModel)
        {
            if (listAssignmentModel == null || listAssignmentModel.Count == 0)
            {
                return null;
            }
            return listAssignmentModel.OrderByDescending(n => n.SrcId).FirstOrDefault().SrcId;
        }

        public static int? GetMinSrcId(List<AssignmentModel> listAssignmentModel)
        {
            if (listAssignmentModel == null || listAssignmentModel.Count == 0)
            {
                return null;
            }
            return listAssignmentModel.OrderBy(n => n.SrcId).FirstOrDefault().SrcId;
        }

        public static int? GetMaxUserSrcId(List<AssignmentModel> listAssignmentModel)
        {
            if (listAssignmentModel == null || listAssignmentModel.Count == 0)
            {
                return null;
            }
            return listAssignmentModel.OrderByDescending(n => n.UserSrcId).FirstOrDefault().UserSrcId;
        }

        public static int? GetMinUserSrcId(List<AssignmentModel> listAssignmentModel)
        {
            if (listAssignmentModel == null || listAssignmentModel.Count == 0)
            {
                return null;
            }
            return listAssignmentModel.OrderBy(n => n.UserSrcId).FirstOrDefault().UserSrcId;
        }

        public static DateTime? GetMaxDate(List<AssignmentModel> listAssignmentModel)
        {
            if (listAssignmentModel == null || listAssignmentModel.Count == 0)
            {
                return null;
            }
            return listAssignmentModel.OrderByDescending(n => n.Dt).FirstOrDefault().Dt;
        }

        public static DateTime? GetMinDate(List<AssignmentModel> listAssignmentModel)
        {
            if (listAssignmentModel == null || listAssignmentModel.Count == 0)
            {
                return null;
            }
            return listAssignmentModel.OrderBy(n => n.Dt).FirstOrDefault().Dt;
        }

    }
}