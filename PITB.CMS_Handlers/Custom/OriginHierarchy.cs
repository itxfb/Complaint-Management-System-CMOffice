using PITB.CMS_Models.Custom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PITB.CMS_Handlers.Custom
{
    public class OriginHierarchyHandler
    {
        public void DeepCopyAssignMatrix(List<AssignmentModel> listAssignmentMatrixToCopy)
        {
            OriginHierarchy originHierarchy = new OriginHierarchy();
            originHierarchy.ListAssignmentMatrixInitial = new List<AssignmentModel>();
            AssignmentModel assignmentModel = null;

            for (int i = 0; i < listAssignmentMatrixToCopy.Count; i++)
            {
                assignmentModel = new AssignmentModel();
                assignmentModel.SrcId = listAssignmentMatrixToCopy[i].SrcId;
                assignmentModel.UserSrcId = listAssignmentMatrixToCopy[i].UserSrcId;
                assignmentModel.Dt = listAssignmentMatrixToCopy[i].Dt;

                originHierarchy.ListAssignmentMatrixInitial.Add(assignmentModel);
            }
        }

        public static OriginHierarchy GetOrigin(List<AssignmentModel> listAssingmentMatrix)
        {
            OriginHierarchy originHierarchy = new OriginHierarchy();
            AssignmentModel assignmentModel = listAssingmentMatrix.OrderBy(n => n.LevelId).FirstOrDefault();
            if (assignmentModel != null)
            {
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