using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Amazon.ECS.Model;
using PITB.CMS.Handler.Complaint.Assignment;
using PITB.CMS.Models.Custom;
using PITB.CMS.Models.DB;

namespace PITB.CMS.Handler.Complaint
{
    public class EscalationHandler
    {
        public static List<EscalationModel> GetEscalationListOfComplaint(DbComplaint dbComplaint)
        {
            int campId = Convert.ToInt32(dbComplaint.Compaign_Id);
            int catId = Convert.ToInt32(dbComplaint.Complaint_Category);
            int subCatId = Convert.ToInt32(dbComplaint.Complaint_SubCategory);
            int computedHierarchyId = (int)dbComplaint.Complaint_Computed_Hierarchy_Id;
            int hierarchyIdVal = 0;
            List<AssignmentModel> listAssignmentModel = AssignmentHandler.GetAssignmnetModelByCampaignCategorySubCategory(campId, catId, subCatId, true);
            
            List<EscalationModel> listEscalationModels = new List<EscalationModel>();
            EscalationModel escModel = null;
            int count = 1;
            foreach (AssignmentModel assignmentModel in listAssignmentModel)
            {
                escModel = new EscalationModel();
                escModel.HierarchyId = (Config.Hierarchy) assignmentModel.SrcId;
                escModel.HierarchyStr = count + ") "+((Config.Hierarchy)escModel.HierarchyId).GetDisplayName();
                if (computedHierarchyId == assignmentModel.SrcId)
                {
                    escModel.HierarchyStr = escModel.HierarchyStr + " [Current]";
                }
                hierarchyIdVal = DbComplaint.GetHierarchyIdValueAgainstHierarchyId(escModel.HierarchyId, dbComplaint);
                escModel.UserNameCommaSep = DbUsers.GetStakeholderUsernames(campId, escModel.HierarchyId, hierarchyIdVal, assignmentModel.UserSrcId, catId, dbComplaint.UserCategoryId1, dbComplaint.UserCategoryId2);
                listEscalationModels.Add(escModel);
                count++;
            }

            return listEscalationModels;
        }
    }



    public class EscalationModel
    {
        public Config.Hierarchy HierarchyId { get; set; }

        public string HierarchyStr { get; set; }

        public List<string> ListUsernames { get; set; }

        public string UserNameCommaSep { get; set; }

    }

}