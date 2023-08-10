using System.Xml.Serialization;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Logical;
using PITB.CMS_Common.Models.Custom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PITB.CMS_Common.Models;

namespace PITB.CMS_Handlers.Complaint.Assignment
{
    public class AssignmentHandler
    {
        public static List<AssignmentModel> GetAssignmnetModelByCampaignCategorySubCategory(int campaignId, int categoryId, int subCategoryId, bool canPopulateMatrix, int? categoryDep1 = null, int? categoryDep2 = null, string assignmentModelTag = null)
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
                    catRetainingHours = (float) subcatRetainingHours;
                    //cateogryType = Config.CategoryType.Sub;
                }
                assignmentModelList = AssignmentHandler.GetAssignmentModel(new FuncParamsModel.Assignment(nowDate,
                    DbAssignmentMatrix.GetByCampaignIdAndCategoryId(campaignId, categoryId, subCategoryId, categoryDep1, categoryDep2, null, assignmentModelTag),
                    catRetainingHours)/*nowDate,
                    DbAssignmentMatrix.GetByCampaignIdAndCategoryId(campaignId, categoryId, subCategoryId,categoryDep1, categoryDep2),
                    catRetainingHours*/);

                return assignmentModelList;
            }
            else
            {
                return new List<AssignmentModel> ();
            }
        }
        public static List<AssignmentModel> GetAssignmentModel(FuncParamsModel.Assignment funcParam  /*DateTime creationDate, List<DbAssignmentMatrix> listAssignmentMatrix, float categoryRetainingHours*/ )
        {
            List<DbAssignmentMatrix> listAssignmentMatrix = funcParam.ListAssignmentMatrix;
            DateTime creationDate = funcParam.CreationDate;
            float categoryRetainingHours = funcParam.CategoryRetainingHours;
            float overrideRetainingHour = funcParam.OverRideRetainingHours;
            


            listAssignmentMatrix = listAssignmentMatrix.OrderBy(n => n.LevelId).ToList();
            if (overrideRetainingHour != -1f)
            {
                categoryRetainingHours = overrideRetainingHour;
            }
            else if (IsCategoryAssignmentMatrix(listAssignmentMatrix))
            {
                categoryRetainingHours = 1f;
            }
            
            DateTime assignemntDateTime = creationDate;
            List<AssignmentModel> assignmentList = new List<AssignmentModel>();
            AssignmentModel assignmentModel = null;
            for (int i = 0; i < listAssignmentMatrix.Count-1; i++)
            {
                assignmentModel = new AssignmentModel();
                assignmentModel.SrcId = listAssignmentMatrix[i].ToSourceId;
                assignmentModel.UserSrcId = listAssignmentMatrix[i].ToUserSourceId;

                assignmentModel.CategoryType = listAssignmentMatrix[i].CategoryType;
                assignmentModel.CategoryId = listAssignmentMatrix[i].CategoryId;
                assignmentModel.CategoryDep1 = listAssignmentMatrix[i].CategoryDep1;
                assignmentModel.CategoryDep2 = listAssignmentMatrix[i].CategoryDep2;
                assignmentModel.LevelId = listAssignmentMatrix[i].LevelId;


                assignmentModel.Dt = assignemntDateTime.AddHours(Convert.ToDouble((listAssignmentMatrix[i + 1].RetainingHours * categoryRetainingHours)));
                assignmentModel.RetainingTime = listAssignmentMatrix[i+1].RetainingHours;
                assignemntDateTime = (DateTime) assignmentModel.Dt;
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

        private static bool IsCategoryAssignmentMatrix(List<AssignmentModel> listAssignmentModel)
        {
            // if category matrix present
            AssignmentModel assignmentModel = listAssignmentModel.Where(n => n.CategoryType != null && n.CategoryId != null).FirstOrDefault();
            return (assignmentModel != null);
        }
        
        //public static List<AssignmentModel> GetAssignmentModelOnStatusChange(int userHierarchyId, int userInnerHierarchyId, DbComplaint dbComplaint, int statusId, DateTime currentDate, List<DbAssignmentMatrix> listAssignmentMatrix, float categoryRetainingTime, bool canResetOnPendingReopen)
        //{
        //    DbStatus dbStatus = DbStatus.GetById(statusId);
            

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
        //        assignmentList.Add(new AssignmentModel(dbComplaint.Dt1,dbComplaint.SrcId1,dbComplaint.UserSrcId1));
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


        public static List<AssignmentModel> GetAssignmentModelOnStatusChange2(int userHierarchyId, int userInnerHierarchyId, DbComplaint dbComplaint, int statusId, DateTime currentDate, List<DbAssignmentMatrix> listAssignmentMatrix, float categoryRetainingTime, bool canResetOnPendingReopen)
        {
            DbStatus dbStatus = DbStatus.GetById(statusId);
            int campaignId = (int) dbComplaint.Compaign_Id;

            listAssignmentMatrix = listAssignmentMatrix.OrderBy(n => n.LevelId).ToList();
            string escalationAction = dbStatus.EscalationAction;
            Dictionary<string, string> dictEscAction = Utility.ConvertCollonFormatToDict(escalationAction);

            int currentAction = -1;//(int) Config.StatusEscalationAction.DontChangeDatesAtAll;

            int stericPermission = -1;
            foreach (KeyValuePair<string,string> escAction in dictEscAction)
            {
                if (escAction.Value == "*")
                {
                    stericPermission = Convert.ToInt32(escAction.Key);
                }
                else if (Utility.GetIntList(escAction.Value).Contains(campaignId))
                {
                    currentAction = Convert.ToInt32(escAction.Key);
                }
            }

            if (currentAction == -1) // if no permission has found
            {
                if (stericPermission == -1) // if there is no steric permission
                {
                    currentAction = (int) Config.StatusEscalationAction.DontChangeDatesAtAll;
                }
                else
                {
                    currentAction = stericPermission; 
                }
                 
            }

            
            if (IsCategoryAssignmentMatrix(listAssignmentMatrix))
            {
                categoryRetainingTime = 1f;
            }
            Dictionary<string, object> dictIteratorVal = GetAssignmentMatrixIterator(userHierarchyId, userInnerHierarchyId, currentDate, listAssignmentMatrix, currentAction, categoryRetainingTime);
            int iteratorStartingIndex = (int) dictIteratorVal["listDbAssignmentMatrixIndex"];
            DateTime dateToAssign = (DateTime) dictIteratorVal["dateToAssign"];

            List<AssignmentModel> listAssignmentModel = AssignmentModel.GetList(dbComplaint);
            //AssignmentModel assignmentModel = null;
            //listAssignmentModel.Add(new AssignmentModel(dbComplaint.Dt1, dbComplaint.SrcId1, dbComplaint.UserSrcId1));
            //listAssignmentModel.Add(new AssignmentModel(dbComplaint.Dt2, dbComplaint.SrcId2, dbComplaint.UserSrcId2));
            //listAssignmentModel.Add(new AssignmentModel(dbComplaint.Dt3, dbComplaint.SrcId3, dbComplaint.UserSrcId3));
            //listAssignmentModel.Add(new AssignmentModel(dbComplaint.Dt4, dbComplaint.SrcId4, dbComplaint.UserSrcId4));
            //listAssignmentModel.Add(new AssignmentModel(dbComplaint.Dt5, dbComplaint.SrcId5, dbComplaint.UserSrcId5));
            //listAssignmentModel.Add(new AssignmentModel(dbComplaint.Dt6, dbComplaint.SrcId6, dbComplaint.UserSrcId6));
            //listAssignmentModel.Add(new AssignmentModel(dbComplaint.Dt7, dbComplaint.SrcId7, dbComplaint.UserSrcId7));
            //listAssignmentModel.Add(new AssignmentModel(dbComplaint.Dt8, dbComplaint.SrcId8, dbComplaint.UserSrcId8));
            //listAssignmentModel.Add(new AssignmentModel(dbComplaint.Dt9, dbComplaint.SrcId9, dbComplaint.UserSrcId9));
            //listAssignmentModel.Add(new AssignmentModel(dbComplaint.Dt10, dbComplaint.SrcId10, dbComplaint.UserSrcId10));


            DateTime tempDateTime = dateToAssign;

            if (iteratorStartingIndex < listAssignmentMatrix.Count - 1)
            {
                listAssignmentModel[iteratorStartingIndex].Dt = dateToAssign;
                for (int i = iteratorStartingIndex + 1; i < listAssignmentMatrix.Count - 1 /*10*/; i++)
                {
                    tempDateTime = tempDateTime.AddHours(
                            Convert.ToDouble(listAssignmentMatrix[i + 1].RetainingHours * categoryRetainingTime));
                    listAssignmentModel[i].Dt = tempDateTime;
                }

                tempDateTime = dateToAssign;
                for (int i = iteratorStartingIndex - 1; i >= 0 /*10*/; i--)
                {
                    //listAssignmentModel[i].Dt =
                    //    tempDateTime.AddHours(
                    //        -Convert.ToDouble(listAssignmentMatrix[i + 1].RetainingHours*categoryRetainingTime));

                    tempDateTime = tempDateTime.AddHours(
                            -Convert.ToDouble(listAssignmentMatrix[i + 1].RetainingHours * categoryRetainingTime));
                    listAssignmentModel[i].Dt = tempDateTime;
                }
            }



            //bool canPopulateDates = false;
            //DateTime assignmentDateTime = currentDate;
            //List<AssignmentModel> assignmentList = new List<AssignmentModel>();
            //AssignmentModel assignmentModel = null;
            //DbAssignmentMatrix nextAssignmentMatrixVal = null;
            //bool forceEscalateOnStatusChange = Convert.ToBoolean(dbStatus.ForceEscalateOnStatusChange);
            //bool isDatesPopulatable = true;
            //if (statusId == Convert.ToInt32(Config.ComplaintStatus.PendingReopened) || forceEscalateOnStatusChange) // if reopening then reassign matrix
            //{
            //    listAssignmentMatrix = listAssignmentMatrix.OrderBy(n => n.LevelId).ToList();
            //    if (IsCategoryAssignmentMatrix(listAssignmentMatrix))
            //    {
            //        categoryRetainingTime = 1f;
            //    }

            //    for (int i = 0; i < listAssignmentMatrix.Count - 1; i++)
            //    {
            //        assignmentModel = new AssignmentModel();
            //        if ((listAssignmentMatrix[i].ToSourceId == userHierarchyId || canResetOnPendingReopen) && isDatesPopulatable)
            //        {
            //            if (!forceEscalateOnStatusChange)
            //            {
            //                canPopulateDates = true;
            //                assignmentDateTime = currentDate;
            //                isDatesPopulatable = false;
            //            }

            //        }
            //        if (canPopulateDates)
            //        {
            //            assignmentModel.SrcId = listAssignmentMatrix[i].ToSourceId;
            //            assignmentModel.Dt = assignmentDateTime.AddHours(Convert.ToDouble(listAssignmentMatrix[i + 1].RetainingHours * categoryRetainingTime));
            //            assignmentModel.UserSrcId = listAssignmentMatrix[i].ToUserSourceId;
            //            assignmentDateTime = (DateTime)assignmentModel.Dt;
            //        }
            //        else
            //        {
            //            assignmentModel.SrcId = listAssignmentMatrix[i].ToSourceId;
            //            assignmentModel.Dt = assignmentDateTime.AddHours(-Convert.ToDouble(listAssignmentMatrix[i + 1].RetainingHours * categoryRetainingTime));
            //            assignmentModel.UserSrcId = listAssignmentMatrix[i].ToUserSourceId;
            //            assignmentDateTime = (DateTime)assignmentModel.Dt;
            //        }
            //        assignmentList.Add(assignmentModel);

            //        if ((listAssignmentMatrix[i].ToSourceId == userHierarchyId || canResetOnPendingReopen) &&
            //            isDatesPopulatable)
            //        {
            //            if (forceEscalateOnStatusChange &&
            //                userInnerHierarchyId == listAssignmentMatrix[i].ToUserSourceId)
            //            {
            //                //i++;
            //                canPopulateDates = true;
            //                assignmentDateTime = currentDate;
            //                isDatesPopulatable = false;
            //            }
            //        }
            //    }
            //}
            //else
            //{
            //    assignmentList.Add(new AssignmentModel(dbComplaint.Dt1, dbComplaint.SrcId1, dbComplaint.UserSrcId1));
            //    assignmentList.Add(new AssignmentModel(dbComplaint.Dt2, dbComplaint.SrcId2, dbComplaint.UserSrcId2));
            //    assignmentList.Add(new AssignmentModel(dbComplaint.Dt3, dbComplaint.SrcId3, dbComplaint.UserSrcId3));
            //    assignmentList.Add(new AssignmentModel(dbComplaint.Dt4, dbComplaint.SrcId4, dbComplaint.UserSrcId4));
            //    assignmentList.Add(new AssignmentModel(dbComplaint.Dt5, dbComplaint.SrcId5, dbComplaint.UserSrcId5));
            //    assignmentList.Add(new AssignmentModel(dbComplaint.Dt6, dbComplaint.SrcId6, dbComplaint.UserSrcId6));
            //    assignmentList.Add(new AssignmentModel(dbComplaint.Dt7, dbComplaint.SrcId7, dbComplaint.UserSrcId7));
            //    assignmentList.Add(new AssignmentModel(dbComplaint.Dt8, dbComplaint.SrcId8, dbComplaint.UserSrcId8));
            //    assignmentList.Add(new AssignmentModel(dbComplaint.Dt9, dbComplaint.SrcId9, dbComplaint.UserSrcId9));
            //    assignmentList.Add(new AssignmentModel(dbComplaint.Dt10, dbComplaint.SrcId10, dbComplaint.UserSrcId10));
            //}
            return listAssignmentModel;
        }

        public static Dictionary<string, object> GetAssignmentMatrixIterator(int userHierarchyId, int userInnerHierarchyId, DateTime currDateTime,List<DbAssignmentMatrix> listDbAssignmentMatrix, int escalationAction, float categoryRetainingTime)
        {
            Dictionary<string,object> dictIteratorVal= new Dictionary<string, object>();
            dictIteratorVal.Add("listDbAssignmentMatrixIndex",0);
            //dictIteratorVal.Add("DateTime", currDateTime);
            DateTime dateToAssign = currDateTime;
            //dictIteratorVal.Add("timeConst", -1);
            if (escalationAction == (int) Config.StatusEscalationAction.ResetEscalationFromFirstUser)
            {
                dictIteratorVal["listDbAssignmentMatrixIndex"] = 0;
                dateToAssign = dateToAssign.AddHours(Convert.ToDouble(listDbAssignmentMatrix[1].RetainingHours * categoryRetainingTime));
            }
            else if (escalationAction == (int)Config.StatusEscalationAction.ResetEscalationFromUserWhoChangedStatus ||
                //escalationAction == (int)Config.StatusEscalationAction.ResetEscalationFromUserWhoChangedStatusAndGiveExtraTime ||
                 escalationAction == (int)Config.StatusEscalationAction.ImmediatelyMoveComplaintToNextUserAfterStatusChange)
            {
                //int i = 0;
                //foreach (DbAssignmentMatrix dbAssignmentMatrix in listDbAssignmentMatrix)
                DbAssignmentMatrix dbAssignmentMatrix = null;
                for(int i=0; i<listDbAssignmentMatrix.Count-1;i++)
                {
                    dbAssignmentMatrix = listDbAssignmentMatrix[i];
                    if (dbAssignmentMatrix.ToSourceId == userHierarchyId &&
                        Convert.ToInt32(dbAssignmentMatrix.ToUserSourceId) == userInnerHierarchyId)
                    {
                        dictIteratorVal["listDbAssignmentMatrixIndex"] = i;

                        if (escalationAction ==
                            (int)Config.StatusEscalationAction.ResetEscalationFromUserWhoChangedStatus)
                        {
                            dateToAssign = dateToAssign.AddHours(Convert.ToDouble(listDbAssignmentMatrix[i+1].RetainingHours * categoryRetainingTime));
                        }
                        /*else
                        {
                            dateToAssign = dateToAssign.AddHours(Convert.ToDouble(dbAssignmentMatrix.RetainingHours * categoryRetainingTime));
                        }*/
                    }
                }
            }
            else if (escalationAction == (int)Config.StatusEscalationAction.DontChangeDatesAtAll)
            {
                dictIteratorVal["listDbAssignmentMatrixIndex"] = listDbAssignmentMatrix.Count-1;
            }
            dictIteratorVal.Add("dateToAssign", dateToAssign);
            return dictIteratorVal;
        }



        //public static List<AssignmentModel> GetAssignmentModelOnStatusChange(int userHierarchyId, int userInnerHierarchyId, DbComplaint dbComplaint, int statusId, DateTime currentDate, List<AssignmentModel> listAssignmentMatrix, float categoryRetainingTime, bool canResetOnPendingReopen)
        //{
        //    DbStatus dbStatus = DbStatus.GetById(statusId);


        //    listAssignmentMatrix = listAssignmentMatrix.OrderBy(n => n.LevelId).ToList();
        //    bool canPopulateDates = false;
        //    DateTime assignmentDateTime = currentDate;
        //    //List<AssignmentModel> assignmentList = new List<AssignmentModel>();
        //    //AssignmentModel assignmentModel = null;
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

        //        for (int i = 0; i < listAssignmentMatrix.Count; i++)
        //        {
        //            //assignmentModel = new AssignmentModel();
        //            if ((listAssignmentMatrix[i].SrcId == userHierarchyId || canResetOnPendingReopen) && isDatesPopulatable)
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
        //                //assignmentModel.SrcId = listAssignmentMatrix[i].SrcId;
        //                listAssignmentMatrix[i].Dt = assignmentDateTime.AddHours(Convert.ToDouble(listAssignmentMatrix[i].RetainingTime * categoryRetainingTime));
        //                assignmentDateTime = (DateTime)listAssignmentMatrix[i].Dt;
        //            }
        //            else
        //            {
        //                listAssignmentMatrix[i].SrcId = listAssignmentMatrix[i].SrcId;
        //                listAssignmentMatrix[i].Dt = assignmentDateTime.AddHours(-Convert.ToDouble(listAssignmentMatrix[i].RetainingTime * categoryRetainingTime));
        //                assignmentDateTime = (DateTime)listAssignmentMatrix[i].Dt;
        //            }
        //            //assignmentList.Add(assignmentModel);

        //            if ((listAssignmentMatrix[i].SrcId == userHierarchyId || canResetOnPendingReopen) &&
        //                isDatesPopulatable)
        //            {
        //                if (forceEscalateOnStatusChange &&
        //                    userInnerHierarchyId == listAssignmentMatrix[i].SrcId)
        //                {
        //                    //i++;
        //                    canPopulateDates = true;
        //                    assignmentDateTime = currentDate;
        //                    isDatesPopulatable = false;
        //                }
        //            }
        //        }
        //    }
        //    /*else
        //    {
        //        assignmentList.Add(new AssignmentModel(dbComplaint.Dt1));
        //        assignmentList.Add(new AssignmentModel(dbComplaint.Dt2));
        //        assignmentList.Add(new AssignmentModel(dbComplaint.Dt3));
        //        assignmentList.Add(new AssignmentModel(dbComplaint.Dt4));
        //        assignmentList.Add(new AssignmentModel(dbComplaint.Dt5));
        //        assignmentList.Add(new AssignmentModel(dbComplaint.Dt6));
        //        assignmentList.Add(new AssignmentModel(dbComplaint.Dt7));
        //        assignmentList.Add(new AssignmentModel(dbComplaint.Dt8));
        //        assignmentList.Add(new AssignmentModel(dbComplaint.Dt9));
        //        assignmentList.Add(new AssignmentModel(dbComplaint.Dt10));
        //    }*/
        //    return listAssignmentMatrix;
        //}
        
        /*
        public static List<DbAssignmentMatrix> GetDbAssignmentMatrixList(List<AssignmentModel> listAssignmentModel)
        {
            List<DbAssignmentMatrix> listAssignmentMatrix = new List<DbAssignmentMatrix>();
            DbAssignmentMatrix dbAssignmentMatrix = null;

            int count = 0;
            int? prevSrcId = 0;
            foreach (AssignmentModel assignmentModel in listAssignmentModel)
            {
                dbAssignmentMatrix = new DbAssignmentMatrix();
                if (count == 0)
                {
                    dbAssignmentMatrix.FromSourceId = 0;
                }
                else
                {
                    dbAssignmentMatrix.FromSourceId = prevSrcId;
                }
                dbAssignmentMatrix.ToSourceId = assignmentModel.SrcId;
                dbAssignmentMatrix.ToUserSourceId = assignmentModel.UserSrcId;
                dbAssignmentMatrix.CategoryType = assignmentModel.CategoryType;
                dbAssignmentMatrix.CategoryId = assignmentModel.CategoryId;
                dbAssignmentMatrix.CategoryDep1 = assignmentModel.CategoryDep1;
                dbAssignmentMatrix.CategoryDep2 = assignmentModel.CategoryDep2;
                dbAssignmentMatrix.LevelId = count + 1;
                //dbAssignmentMatrix.CampaignId = assignmentModel.
                prevSrcId = assignmentModel.SrcId;
                count++;
            }

            if (count > 1)
            {
                dbAssignmentMatrix.
            }
        }
        */

        public static int GetActualComplaintStauts(DbComplaint complaint)
        {
            DateTime currentDate = DateTime.Now;
            int statusToReturn = (int) complaint.Complaint_Status_Id;

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
                else if (complaint.Dt5 != null &&  complaint.Dt5 <= currentDate)
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