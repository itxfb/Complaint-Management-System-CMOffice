using PITB.CMS_Common.Handler.Authentication;
using PITB.CMS_Common.Handler.Business;
using PITB.CMS_Common.Helper.Database;
using PITB.CMS_Common.Models.Custom;
using PITB.CMS_Common.Models.View;
using PITB.CMS_Common.Models.View.Table;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PITB.CMS_Common.Handler.Complaint.Assignment;
using PITB.CMS_Common.Models;

namespace PITB.CMS_Common.Handler.Complaint.Transfer
{
    public class TransferHandler
    {
        #region TransferComplaint

        public static VmTransferComplaint GetTransferVmModel(int complaintId)
        {
            VmTransferComplaint vmTransferComplaint = new VmTransferComplaint();
            vmTransferComplaint.ComplaintId = complaintId;
            //vmTransferComplaint.ListTranferTo = new List<SelectListItem>() { };
            //List<Config.Hierarchy> listHierarchy = new List<Config.Hierarchy>(){Config.Hierarchy.Province,Config.Hierarchy.Division,Config.Hierarchy.District,Config.Hierarchy.Tehsil,Config.Hierarchy.UnionCouncil};
            DbComplaint ObjComplaint = DbComplaint.GetByComplaintId(complaintId);
            int? ObjMaxSrcId = null;
            if(ObjComplaint.MaxSrcId != null)
            {
                ObjMaxSrcId = ObjComplaint.MaxSrcId;
            }
            else
            {
                ObjMaxSrcId = Config.ListHierarchy.Count;
            }
            CMSCookie cmsCookie = AuthenticationHandler.GetCookie();
            vmTransferComplaint.CampaignId = Convert.ToInt32(cmsCookie.Campaigns);
            int currHierarchyIndex = Config.ListHierarchy.IndexOf(cmsCookie.Hierarchy_Id);
            if (vmTransferComplaint.CampaignId == (int)Config.Campaign.DcoOffice)
            {
                currHierarchyIndex = Config.ListHierarchy.IndexOf(Config.Hierarchy.Division);
                //ObjMaxSrcId = Config.ListHierarchy.IndexOf(Config.Hierarchy.Tehsil) + 1;
            }
            for (int i = currHierarchyIndex; i < ObjMaxSrcId; i++)
            {
                switch (Config.ListHierarchy[i])
                {
                    case Config.Hierarchy.Province:
                        vmTransferComplaint.VmTransferProvince = new VmTransferProvince();
                        if (currHierarchyIndex <= i) // equeal to current hierarchy
                        {
                            if (currHierarchyIndex == i) // equeal to current hierarchy
                            {
                                /*vmTransferComplaint.VmTransferProvince.ListOfProvinces = new List<DbProvince>()
                                {
                                    DbProvince.GetById((int) cmsCookie.ProvinceId)
                                };*/
                                //int? groupId = DbHierarchyCampaignGroupMapping.GetModelByCampaignId(vmTransferComplaint.CampaignId, Config.Hierarchy.Province);

                                vmTransferComplaint.VmTransferProvince.ListOfProvinces = DbProvince.AllProvincesList();
                                vmTransferComplaint.VmTransferProvince.SelectedId = Utility.GetFirstFromCommaSeperatedString(cmsCookie.ProvinceId);
                            }
                            /*else if (currHierarchyIndex + 1 == i) // when hierarchy is one more than the other one
                            {
                                vmTransferComplaint.VmTransferProvince.ListOfDivisions = DbDivision.GetByProvinceId((int)cmsCookie.ProvinceId);
                            }*/
                            else // when hierarchy is lower than current hierarchy
                            {
                                vmTransferComplaint.VmTransferProvince = new VmTransferProvince();
                            }
                            //vmTransferComplaint.VmTransferProvince.IsViewable = true;
                            vmTransferComplaint.VmTransferProvince.IsRequired = true;
                        }
                        
                        break;

                    case Config.Hierarchy.Division:
                        vmTransferComplaint.VmTransferDivision = new VmTransferDivision();
                        if (currHierarchyIndex <= i) // equeal to current hierarchy
                        {
                            if (currHierarchyIndex == i) // when hierarchy is equeal to current hierarchy
                            {
                                /*vmTransferComplaint.VmTransferDivision.ListOfDivisions = new List<DbDivision>()
                                {
                                    DbDivision.GetById((int) cmsCookie.DivisionId)
                                };*/

                                int? groupId = DbHierarchyCampaignGroupMapping.GetModelByCampaignId(vmTransferComplaint.CampaignId, Config.Hierarchy.Division);

                                vmTransferComplaint.VmTransferDivision.ListOfDivisions =
                                    DbDivision.GetByProvinceIdsStrAndGroupId(cmsCookie.ProvinceId, groupId);

                                vmTransferComplaint.VmTransferDivision.SelectedId = Utility.GetFirstFromCommaSeperatedString(cmsCookie.DivisionId);
                            }
                            /*else if (currHierarchyIndex + 1 == i) // when hierarchy is one more than the other one
                            {
                                vmTransferComplaint.VmTransferDivision.ListOfDivisions = DbDivision.GetByProvinceId((int) cmsCookie.ProvinceId);
                            }*/
                            else
                            {
                                vmTransferComplaint.VmTransferDivision = new VmTransferDivision();
                            }
                            //vmTransferComplaint.VmTransferDivision.IsViewable = true;
                            vmTransferComplaint.VmTransferDivision.IsRequired = true;
                        }

                        break;

                    case Config.Hierarchy.District:
                        vmTransferComplaint.VmTransferDistrict = new VmTransferDistrict();
                        if (currHierarchyIndex <= i) // equeal to current hierarchy
                        {
                            if (currHierarchyIndex == i) // when hierarchy is equeal to current hierarchy
                            {
                                int? groupId = DbHierarchyCampaignGroupMapping.GetModelByCampaignId(vmTransferComplaint.CampaignId, Config.Hierarchy.District);
                                
                                vmTransferComplaint.VmTransferDistrict.ListOfDistrict =
                                    DbDistrict.GetByDivisionIdsStrAndGroupId(cmsCookie.DivisionId, groupId);
                                vmTransferComplaint.VmTransferDistrict.SelectedId = Utility.GetFirstFromCommaSeperatedString(cmsCookie.DistrictId);
                                /*vmTransferComplaint.VmTransferDistrict.ListOfDistrict = new List<DbDistrict>()
                                {
                                    DbDistrict.GetById((int) cmsCookie.DistrictId)
                                };*/
                            }
                            /*else if (currHierarchyIndex + 1 == i) // when hierarchy is one more than the other one
                            {
                                vmTransferComplaint.VmTransferDistrict.ListOfDistrict = DbDistrict.GetByDivisionId((int)cmsCookie.DivisionId);
                            }*/
                            else // when hierarchy is lower than current hierarchy
                            {
                                vmTransferComplaint.VmTransferDistrict = new VmTransferDistrict();
                            }
                            //vmTransferComplaint.VmTransferDistrict.IsViewable = true;
                            vmTransferComplaint.VmTransferDistrict.IsRequired = true;
                        }

                        break;

                    case Config.Hierarchy.Tehsil:
                        vmTransferComplaint.VmTransferTehsil = new VmTransferTehsil();
                        if (currHierarchyIndex <= i) // equeal to current hierarchy
                        {
                            if (currHierarchyIndex == i) // when hierarchy is equeal to current hierarchy
                            {
                                /*vmTransferComplaint.VmTransferTehsil.ListOfTehsil = new List<DbTehsil>()
                                {
                                    DbTehsil.GetById((int) cmsCookie.TehsilId)

                                };*/
                                int? groupId = DbHierarchyCampaignGroupMapping.GetModelByCampaignId(vmTransferComplaint.CampaignId, Config.Hierarchy.Tehsil);
                                vmTransferComplaint.VmTransferTehsil.ListOfTehsil =
                                    DbTehsil.GetByDistrictIdsAndGroupId(cmsCookie.DistrictId, groupId);
                                vmTransferComplaint.VmTransferTehsil.SelectedId = Utility.GetFirstFromCommaSeperatedString(cmsCookie.TehsilId);

                            }
                            /*else if (currHierarchyIndex + 1 == i) // when hierarchy is one more than the other one
                            {
                                vmTransferComplaint.VmTransferTehsil.ListOfTehsil = DbTehsil.GetTehsilList((int)cmsCookie.TehsilId);
                            }*/
                            else // when hierarchy is lower than current hierarchy
                            {
                                vmTransferComplaint.VmTransferTehsil = new VmTransferTehsil();
                            }
                            //vmTransferComplaint.VmTransferTehsil.IsViewable = true;
                            vmTransferComplaint.VmTransferTehsil.IsRequired = true;
                        }


                        break;

                    case Config.Hierarchy.UnionCouncil:
                        vmTransferComplaint.VmTransferUc = new VmTransferUc();
                        if (currHierarchyIndex <= i) // equeal to current hierarchy
                        {
                            if (currHierarchyIndex == i) // when hierarchy is equeal to current hierarchy
                            {
                                /*vmTransferComplaint.VmTransferUc.ListOfUc = new List<DbUnionCouncils>()
                                {
                                    DbUnionCouncils.GetById((int) cmsCookie.UcId)

                                };*/
                                int? groupId = DbHierarchyCampaignGroupMapping.GetModelByCampaignId(vmTransferComplaint.CampaignId, Config.Hierarchy.UnionCouncil);
                                vmTransferComplaint.VmTransferUc.ListOfUc =
                                    DbUnionCouncils.GetByTehsilIdsStr(cmsCookie.TehsilId, groupId);
                                vmTransferComplaint.VmTransferUc.SelectedId = Utility.GetFirstFromCommaSeperatedString(cmsCookie.UcId);
                            }
                            /*else if (currHierarchyIndex + 1 == i) // when hierarchy is one more than the other one
                            {
                                vmTransferComplaint.VmTransferUc.ListOfUc = DbUnionCouncils.GetUnionCouncilList((int)cmsCookie.UcId);
                            }*/
                            else // when hierarchy is lower than current hierarchy
                            {
                                vmTransferComplaint.VmTransferUc = new VmTransferUc();
                            }
                            //vmTransferComplaint.VmTransferUc.IsViewable = true;
                            vmTransferComplaint.VmTransferUc.IsRequired = true;
                        }
                        break;
                }
                if (currHierarchyIndex <= i)
                {
                    if (vmTransferComplaint.CampaignId == (int)Config.Campaign.DcoOffice)
                    {
                        int originalHierarchyIndex = Config.ListHierarchy.IndexOf(cmsCookie.Hierarchy_Id);
                        if (i >= originalHierarchyIndex)
                        {
                            vmTransferComplaint.ListTranferTo.Add(new SelectListItem() { Text = Config.ListHierarchy[i].ToString(), Value = ((int)(Config.ListHierarchy[i])).ToString() });
                        }
                    }
                    else
                    {
                        vmTransferComplaint.ListTranferTo.Add(new SelectListItem() { Text = Config.ListHierarchy[i].ToString(), Value = ((int)(Config.ListHierarchy[i])).ToString() });
                    }
                    vmTransferComplaint.ListHierarchyAdded.Add(new SelectListItem() { Text = Config.ListHierarchy[i].ToString(), Value = ((int)(Config.ListHierarchy[i])).ToString() });
                }
            }

            vmTransferComplaint.VmLastTransferredFrom = GetVMLastTransferedFrom(complaintId);

            if (vmTransferComplaint.VmLastTransferredFrom == null) vmTransferComplaint.HasTransferedHistory = false;
            else vmTransferComplaint.HasTransferedHistory = true;

            return vmTransferComplaint;
        }

        public static bool GetTransferedHistoryStatus(int complaintId)
        {
            DbComplaintTransferLog dbComplaintTransferLog = DbComplaintTransferLog.GetLastTransferOfParticularComplaint(complaintId);
            if (dbComplaintTransferLog != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private static VmLastTransferedFrom GetVMLastTransferedFrom(int complaintId)
        {
            DbComplaintTransferLog dbComplaintTransferLog = DbComplaintTransferLog.GetLastTransferOfParticularComplaint(complaintId);
            if (dbComplaintTransferLog != null)
            {
                VmLastTransferedFrom vmLastTransferedFrom = new VmLastTransferedFrom();
                vmLastTransferedFrom.UserName = dbComplaintTransferLog.User.Username;

                vmLastTransferedFrom.AssignedFromMedium =
                    ((Config.Hierarchy)((int)dbComplaintTransferLog.AssignedFromMedium)).ToString();
                vmLastTransferedFrom.AssignedFromMediumValue =
                    BlHierarchy.GetRegionValueAgainstHierarchy(
                        (Config.Hierarchy)((int)dbComplaintTransferLog.AssignedFromMedium),
                        dbComplaintTransferLog.AssignedFromMediumValue.ToString());

                vmLastTransferedFrom.AssignedToMedium =
                    ((Config.Hierarchy)((int)dbComplaintTransferLog.AssignedToMedium)).ToString();
                vmLastTransferedFrom.AssignedToMediumValue =
                    BlHierarchy.GetRegionValueAgainstHierarchy(
                        (Config.Hierarchy)((int)dbComplaintTransferLog.AssignedToMedium),
                        dbComplaintTransferLog.AssignedToMediumValue.ToString());

                vmLastTransferedFrom.AssignedDate =
                    Utility.ConvertDateTo_DD_MMMM_YY_H_MM_tt(dbComplaintTransferLog.AssignmentDateTime.Value);

                vmLastTransferedFrom.Comment = dbComplaintTransferLog.Comments;
                return vmLastTransferedFrom;
            }
            else
            {
                return null;
            }
        }

        public static void OnTransferComplaint(VmTransferComplaint vmTransferComplaint)
        {
            
            DateTime dateOfTransfer = DateTime.Now;
            CMSCookie cmsCookie = new AuthenticationHandler().CmsCookie;
            DBContextHelperLinq db = new DBContextHelperLinq();
            DbComplaint dbComplaint = DbComplaint.GetBy(vmTransferComplaint.ComplaintId, db);
            //db.DbComplaints.Attach(dbComplaint);

            DbComplaintTransferLog dbTransferLog = new DbComplaintTransferLog();

            dbTransferLog.Complaint_Id = vmTransferComplaint.ComplaintId;
            dbTransferLog.User_Id = cmsCookie.UserId;

            dbTransferLog.AssignedFromMedium = (int)cmsCookie.Hierarchy_Id;
            dbTransferLog.AssignedFromMediumValue = Utility.GetHierarchyValue(cmsCookie.Hierarchy_Id,cmsCookie).First(); //GetTransferFromHierarchyTypeId(vmTransferComplaint, (int)cmsCookie.Hierarchy_Id); //cmsCookie.HierarchyTypeId;

            dbTransferLog.AssignedToMedium = (int)vmTransferComplaint.TransferToId;
            dbTransferLog.AssignedToMediumValue = vmTransferComplaint.HierarchyTypeId;

            dbTransferLog.AssignmentDateTime = dateOfTransfer;
            dbTransferLog.IsCurrentlyActive = true;
            dbTransferLog.Comments = vmTransferComplaint.AgentComments;

            db.DbComplaintTransferLog.Add(dbTransferLog);

            MakeLastTransferOfComplaintInactive(vmTransferComplaint.ComplaintId, db);
            MakeChangesOfTransferInComplaintTable(dbComplaint, vmTransferComplaint, dbTransferLog, db);

            // Update dbComplaintTransferCount
            
            dbComplaint.TransferedCount = (dbComplaint.TransferedCount) ?? 0;
            dbComplaint.TransferedCount++;


            DBContextHelperLinq.UpdateEntity(dbComplaint, db, new List<string> { "TransferedCount" });
            
            db.SaveChanges();

            
        }

        private static int GetTransferFromHierarchyTypeId(VmTransferComplaint vmTransferComplaint, int userHierarchyId)
        {
            switch (userHierarchyId)
            {
                case (int) Config.Hierarchy.Province:
                    return Convert.ToInt32(vmTransferComplaint.VmTransferProvince.SelectedId);
                break;

                case (int)Config.Hierarchy.Division:
                    return Convert.ToInt32(vmTransferComplaint.VmTransferDivision.SelectedId);
                break;

                case (int)Config.Hierarchy.District:
                    return Convert.ToInt32(vmTransferComplaint.VmTransferDistrict.SelectedId);
                break;

                case (int)Config.Hierarchy.Tehsil:
                    return Convert.ToInt32(vmTransferComplaint.VmTransferTehsil.SelectedId);
                break;

                case (int)Config.Hierarchy.UnionCouncil:
                    return Convert.ToInt32(vmTransferComplaint.VmTransferUc.SelectedId);
                break;

                case (int)Config.Hierarchy.Ward:
                    break;

            }

            return 0;
        }

        private static void MakeLastTransferOfComplaintInactive(int complaintId, DBContextHelperLinq db)
        {
            //DBContextHelperLinq db = new DBContextHelperLinq();
            DbComplaintTransferLog tranferLog = DbComplaintTransferLog.GetLastTransferOfParticularComplaint(complaintId, db);
            if (tranferLog != null)
            {
                tranferLog.IsCurrentlyActive = false;
                db.DbComplaintTransferLog.Add(tranferLog);
                db.Entry(tranferLog).State = EntityState.Modified;
                //db.SaveChanges();
            }
        }


        private static void MakeChangesOfTransferInComplaintTable(DbComplaint dbComplaint/*int complaintId*/, VmTransferComplaint vmTransferComplaint, DbComplaintTransferLog dbTransferLog, DBContextHelperLinq db)
        {
            CMSCookie cmsCookie = AuthenticationHandler.GetCookie();
            //DbComplaint dbComplaint = DbComplaint.GetByComplaintId(complaintId, db);
            db.DbComplaints.Attach(dbComplaint);
            //db.Entry(dbComplaint).State = EntityState.Modified;
            /*
            dbComplaint.User_Id = dbTransferLog.User_Id;
            db.Entry(dbComplaint).Property(n => n.User_Id).IsModified = true;
            
            dbComplaint.AssignedFromMedium = dbTransferLog.AssignedFromMedium;
            db.Entry(dbComplaint).Property(n => n.AssignedFromMedium).IsModified = true;

            dbComplaint.AssignedFromMediumValue = dbTransferLog.AssignedFromMediumValue;
            db.Entry(dbComplaint).Property(n => n.AssignedFromMediumValue).IsModified = true;
            
            dbComplaint.AssignedToMedium = dbTransferLog.AssignedToMedium;
            db.Entry(dbComplaint).Property(n => n.AssignedToMedium).IsModified = true;
            
            dbComplaint.AssignedToMediumValue = dbTransferLog.AssignedToMediumValue;
            db.Entry(dbComplaint).Property(n => n.AssignedToMediumValue).IsModified = true;
            
            dbComplaint.AssignmentDateTime = dbTransferLog.AssignmentDateTime;
            db.Entry(dbComplaint).Property(n => n.AssignmentDateTime).IsModified = true;
            */
            dbComplaint.IsTransferred = true;
            db.Entry(dbComplaint).Property(n => n.IsTransferred).IsModified = true;

            // Change complaint fields

            dbComplaint.Status_ChangedBy = null;
            db.Entry(dbComplaint).Property(n => n.Status_ChangedBy).IsModified = true;

            dbComplaint.Status_ChangedBy_Name = null;
            db.Entry(dbComplaint).Property(n => n.Status_ChangedBy_Name).IsModified = true;

            dbComplaint.StatusChangedDate_Time = null;
            db.Entry(dbComplaint).Property(n => n.StatusChangedDate_Time).IsModified = true;

            dbComplaint.StatusChangedBy_RoleId = null;
            db.Entry(dbComplaint).Property(n => n.StatusChangedBy_RoleId).IsModified = true;

            dbComplaint.StatusChangedBy_HierarchyId = null;
            db.Entry(dbComplaint).Property(n => n.StatusChangedBy_HierarchyId).IsModified = true;

            dbComplaint.StatusChangedComments = null;
            db.Entry(dbComplaint).Property(n => n.StatusChangedComments).IsModified = true;

            // End change complaint fields


            // Changing ProvinceId, DivisionId
            foreach (Config.Hierarchy hierarchy in Config.ListHierarchy)
            {
                //if ((int) hierarchy <= (int) dbTransferLog.AssignedToMedium)
                {
                    switch ((int) hierarchy)
                    {
                        case (int) Config.Hierarchy.Province:
                            if (vmTransferComplaint.VmTransferProvince != null)
                            {
                                dbComplaint.Province_Id = (int) vmTransferComplaint.VmTransferProvince.SelectedId;
                                db.Entry(dbComplaint).Property(n => n.Province_Id).IsModified = true;

                                dbComplaint.Province_Name = DbProvince.GetById((int)dbComplaint.Province_Id).Province_Name;
                                db.Entry(dbComplaint).Property(n => n.Province_Name).IsModified = true;
                            }
                            break;

                        case (int) Config.Hierarchy.Division:
                            if (vmTransferComplaint.VmTransferDivision != null)
                            {
                                dbComplaint.Division_Id = (int) vmTransferComplaint.VmTransferDivision.SelectedId;
                                db.Entry(dbComplaint).Property(n => n.Division_Id).IsModified = true;

                                dbComplaint.Division_Name = DbDivision.GetById((int)dbComplaint.Division_Id).Division_Name;
                                db.Entry(dbComplaint).Property(n => n.Division_Name).IsModified = true;
                            }
                            break;

                        case (int) Config.Hierarchy.District:
                            if (vmTransferComplaint.VmTransferDistrict != null)
                            {
                                dbComplaint.District_Id = (int) vmTransferComplaint.VmTransferDistrict.SelectedId;
                                db.Entry(dbComplaint).Property(n => n.District_Id).IsModified = true;

                                dbComplaint.District_Name = DbDistrict.GetById((int)dbComplaint.District_Id).District_Name;
                                db.Entry(dbComplaint).Property(n => n.District_Name).IsModified = true;
                            }
                            break;

                        case (int) Config.Hierarchy.Tehsil:
                            if (vmTransferComplaint.VmTransferTehsil != null)
                            {
                                dbComplaint.Tehsil_Id = (int) vmTransferComplaint.VmTransferTehsil.SelectedId;
                                db.Entry(dbComplaint).Property(n => n.Tehsil_Id).IsModified = true;

                                dbComplaint.Tehsil_Name = DbTehsil.GetById((int)dbComplaint.Tehsil_Id).Tehsil_Name;
                                db.Entry(dbComplaint).Property(n => n.Tehsil_Name).IsModified = true;
                            }
                            break;

                        case (int) Config.Hierarchy.UnionCouncil:
                            if (vmTransferComplaint.VmTransferUc != null)
                            {
                                if (vmTransferComplaint.VmTransferUc.SelectedId != null && vmTransferComplaint.VmTransferUc.SelectedId != 0)
                                {
                                    dbComplaint.UnionCouncil_Id = (int) vmTransferComplaint.VmTransferUc.SelectedId;
                                    db.Entry(dbComplaint).Property(n => n.UnionCouncil_Id).IsModified = true;

                                    dbComplaint.UnionCouncil_Name =
                                        DbUnionCouncils.GetById((int) dbComplaint.UnionCouncil_Id).Councils_Name;
                                    db.Entry(dbComplaint).Property(n => n.UnionCouncil_Name).IsModified = true;
                                }
                                else
                                {
                                    dbComplaint.UnionCouncil_Id = 0;
                                    dbComplaint.UnionCouncil_Name = null;
                                }
                            }
                            break;
                    }
                }
                //else
                //{
                //    switch ((int)hierarchy)
                //    {
                //        case (int)Config.Hierarchy.Province:
                //            dbComplaint.Province_Id = null;
                //            db.Entry(dbComplaint).Property(n => n.Province_Id).IsModified = true;
                //            break;

                //        case (int)Config.Hierarchy.Division:
                //            dbComplaint.Division_Id = null;
                //            db.Entry(dbComplaint).Property(n => n.Division_Id).IsModified = true;
                //            break;

                //        case (int)Config.Hierarchy.District:
                //            dbComplaint.District_Id = null;
                //            db.Entry(dbComplaint).Property(n => n.District_Id).IsModified = true;
                //            break;

                //        case (int)Config.Hierarchy.Tehsil:
                //            dbComplaint.Tehsil_Id = null;
                //            db.Entry(dbComplaint).Property(n => n.Tehsil_Id).IsModified = true;
                //            break;

                //        case (int)Config.Hierarchy.UnionCouncil:
                //            dbComplaint.UnionCouncil_Id = null;
                //            db.Entry(dbComplaint).Property(n => n.UnionCouncil_Id).IsModified = true;
                //            break;
                //    }
                //}
            }
            
            // Change Status Of Complaint To Pending Fresh
            dbComplaint.Complaint_Status_Id = (int) Config.ComplaintStatus.PendingFresh;
            db.Entry(dbComplaint).Property(n => n.Complaint_Status_Id).IsModified = true;
            
            DbStatus dbStatus = DbStatus.GetById((int)dbComplaint.Complaint_Status_Id);
            dbComplaint.Escalation_Status = dbStatus.EscalationStatus;
            db.Entry(dbComplaint).Property(n => n.Escalation_Status).IsModified = true;

            dbComplaint.Complaint_Status = Config.StatusDict[(int)Config.ComplaintStatus.PendingFresh];
            db.Entry(dbComplaint).Property(n => n.Complaint_Status).IsModified = true;

            // Repopulating Assignment Matrix
            DateTime nowDate = DateTime.Now;
            float catRetainingHours = 0;
            float? subcatRetainingHours = 0;
            //Config.CategoryType cateogryType = Config.CategoryType.Main;
            //float catRetainingHours = DbComplaintType.GetRetainingHoursByTypeId((int)dbComplaint.Complaint_Category);

            subcatRetainingHours = DbComplaintSubType.GetRetainingByComplaintSubTypeId((int)dbComplaint.Complaint_SubCategory);
            if (subcatRetainingHours == null) // when subcategory doesnot have retaining hours
            {
                catRetainingHours = DbComplaintType.GetRetainingHoursByTypeId((int)dbComplaint.Complaint_Category);
                //cateogryType = Config.CategoryType.Main;
            }
            else
            {
                catRetainingHours = (float)subcatRetainingHours;
                //cateogryType = Config.CategoryType.Sub;
            }

            List<Pair<int?, int?>> listHierarchyPair = new List<Pair<int?, int?>>
                {
                    new Pair<int?, int?>((int?)Config.Hierarchy.Province, (int?)dbComplaint.Province_Id),
                    new Pair<int?, int?>((int?)Config.Hierarchy.Division, (int?)dbComplaint.Division_Id),
                    new Pair<int?, int?>((int?)Config.Hierarchy.District, (int?)dbComplaint.District_Id),
                    new Pair<int?, int?>((int?)Config.Hierarchy.Tehsil, (int?)dbComplaint.Tehsil_Id),
                    new Pair<int?, int?>((int?)Config.Hierarchy.UnionCouncil, (int?)dbComplaint.UnionCouncil_Id),
                    new Pair<int?, int?>((int?)Config.Hierarchy.Ward, (int?)dbComplaint.Ward_Id)
                };

            List<AssignmentModel> assignmentModelList = AssignmentHandler.GetAssignmentModelOnStatusChange2(Convert.ToInt32(vmTransferComplaint.TransferToId), 0, dbComplaint, (int)Config.ComplaintStatus.PendingReopened, nowDate, DbAssignmentMatrix.GetByCampaignIdAndCategoryId((int)dbComplaint.Compaign_Id, (int)dbComplaint.Complaint_Category, (int)dbComplaint.Complaint_SubCategory, null, null, listHierarchyPair), catRetainingHours, false);


            
            
           // List<AssignmentModel> assignmentModelList = AssignmentHandler.GetAssignmentModel(nowDate,DbAssignmentMatrix.GetByCampaignIdAndCategoryId((int)dbComplaint.Compaign_Id), catRetainingHours);

            for (int i = 0; i < 10; i++)
            {
                if (i < assignmentModelList.Count)
                {
                    switch (i)
                    {
                        case 0:
                            dbComplaint.Dt1 = assignmentModelList[i].Dt;
                            db.Entry(dbComplaint).Property(n => n.Dt1).IsModified = true;
                            break;

                        case 1:
                            dbComplaint.Dt2 = assignmentModelList[i].Dt;
                            db.Entry(dbComplaint).Property(n => n.Dt2).IsModified = true;
                            break;

                        case 2:
                            dbComplaint.Dt3 = assignmentModelList[i].Dt;
                            db.Entry(dbComplaint).Property(n => n.Dt3).IsModified = true;
                            break;

                        case 3:
                            dbComplaint.Dt4 = assignmentModelList[i].Dt;
                            db.Entry(dbComplaint).Property(n => n.Dt4).IsModified = true;
                            break;

                        case 4:
                            dbComplaint.Dt5 = assignmentModelList[i].Dt;
                            db.Entry(dbComplaint).Property(n => n.Dt5).IsModified = true;
                            break;

                        case 5:
                            dbComplaint.Dt6 = assignmentModelList[i].Dt;
                            db.Entry(dbComplaint).Property(n => n.Dt6).IsModified = true;
                            break;

                        case 6:
                            dbComplaint.Dt7 = assignmentModelList[i].Dt;
                            db.Entry(dbComplaint).Property(n => n.Dt7).IsModified = true;
                            break;

                        case 7:
                            dbComplaint.Dt8 = assignmentModelList[i].Dt;
                            db.Entry(dbComplaint).Property(n => n.Dt8).IsModified = true;
                            break;

                        case 8:
                            dbComplaint.Dt9 = assignmentModelList[i].Dt;
                            db.Entry(dbComplaint).Property(n => n.Dt9).IsModified = true;
                            break;

                        case 9:
                            dbComplaint.Dt10 = assignmentModelList[i].Dt;
                            db.Entry(dbComplaint).Property(n => n.Dt10).IsModified = true;
                            break;
                    }
                }
            }

            //db.SaveChanges();
        }

        

        public static List<VmTableTransferHistory> GetComplaintHistoryTableList(int complaintId)
        {
            List<DbComplaintTransferLog> listComplaintsTransferLog = DbComplaintTransferLog.GetTransferLogsAgainstComplaintId(Convert.ToInt32(complaintId));

            List<VmTableTransferHistory> listTableAssignedTo = new List<VmTableTransferHistory>();
            VmTableTransferHistory tempComplaintAssignedTo = null;
            foreach (DbComplaintTransferLog dbComplaintTransferLog in listComplaintsTransferLog)
            {
                tempComplaintAssignedTo = new VmTableTransferHistory();
                tempComplaintAssignedTo.UserName = dbComplaintTransferLog.User.Name;

                tempComplaintAssignedTo.AssignedFromMedium = ((Config.Hierarchy)((int)dbComplaintTransferLog.AssignedFromMedium)).ToString();
                tempComplaintAssignedTo.AssignedFromMediumValue = BlHierarchy.GetRegionValueAgainstHierarchy((Config.Hierarchy)((int)dbComplaintTransferLog.AssignedFromMedium), dbComplaintTransferLog.AssignedFromMediumValue.ToString());

                tempComplaintAssignedTo.AssignedToMedium = ((Config.Hierarchy)((int)dbComplaintTransferLog.AssignedToMedium)).ToString();
                tempComplaintAssignedTo.AssignedToMediumValue = BlHierarchy.GetRegionValueAgainstHierarchy((Config.Hierarchy)((int)dbComplaintTransferLog.AssignedToMedium), dbComplaintTransferLog.AssignedToMediumValue.ToString());

                tempComplaintAssignedTo.AssignedDate = Utility.ConvertDateTo_DD_MMMM_YY_H_MM_tt(dbComplaintTransferLog.AssignmentDateTime.Value);
                tempComplaintAssignedTo.Comment = dbComplaintTransferLog.Comments;

                tempComplaintAssignedTo.IsValid = Utility.GetYesNoFromBool((bool)dbComplaintTransferLog.IsCurrentlyActive);
                listTableAssignedTo.Add(tempComplaintAssignedTo);
            }
            return listTableAssignedTo;
        }

        #endregion
    }
}