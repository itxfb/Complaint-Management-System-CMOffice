using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PITB.CRM_API.Models.Custom
{
    public class OriginHierarchy
    {
        public int? OriginHierarchyId { get; set; }
        public int? OriginUserHierarchyId { get; set; }
        public int? OriginUserCategoryId1 { get; set; }
        public int? OriginUserCategoryId2 { get; set; }
        public bool? IsAssignedToOrigin { get; set; }

        public List<AssignmentModel> ListAssignmentMatrixInitial { get; set; }

        public void DeepCopyAssignMatrix(List<AssignmentModel> listAssignmentMatrixToCopy)
        {
            ListAssignmentMatrixInitial = new List<AssignmentModel>();
            AssignmentModel assignmentModel = null;

            for (int i = 0; i < listAssignmentMatrixToCopy.Count; i++)
            {
                assignmentModel = new AssignmentModel();
                assignmentModel.SrcId = listAssignmentMatrixToCopy[i].SrcId;
                assignmentModel.UserSrcId = listAssignmentMatrixToCopy[i].UserSrcId;
                assignmentModel.Dt = listAssignmentMatrixToCopy[i].Dt;

                ListAssignmentMatrixInitial.Add(assignmentModel);
            }
        }

        public static OriginHierarchy GetOrigin(List<AssignmentModel> listAssingmentMatrix)
        {
            OriginHierarchy originHierarchy = null;
            AssignmentModel assignmentModel = listAssingmentMatrix.OrderBy(n => n.LevelId).FirstOrDefault();
            if (assignmentModel != null)
            {
                originHierarchy = new OriginHierarchy();
                originHierarchy.OriginHierarchyId = assignmentModel.SrcId;
                originHierarchy.OriginUserHierarchyId = assignmentModel.UserSrcId;
                originHierarchy.OriginUserCategoryId1 = assignmentModel.CategoryDep1;
                originHierarchy.OriginUserCategoryId2 = assignmentModel.CategoryDep2;
                originHierarchy.IsAssignedToOrigin = true;
            }
            return originHierarchy;
        }
    }
}