using PITB.CMS_Common;
using PITB.CMS_Models.Custom;
using PITB.CMS_Models.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace PITB.CMS_Handlers.Custom
{
    public class AssignmentModelHandler
    {
        public static List<AssignmentModel> GetList(DbComplaint dbComplaint)
        {
            int count = 1;
            DateTime? dt = (DateTime?) Utility.GetPropertyThroughReflection(dbComplaint, "Dt" + count);
            int? srcId = (int?) Utility.GetPropertyThroughReflection(dbComplaint, "SrcId" + count);
            int? userSrcId = (int?) Utility.GetPropertyThroughReflection(dbComplaint, "UserSrcId" + count);
            List<AssignmentModel> listAssignmentModel = new List<AssignmentModel>();

            while((dt!=null || srcId != null || userSrcId != null) && count<=10)
            {
                listAssignmentModel.Add(new AssignmentModel() { Dt = dt, SrcId = srcId, UserSrcId = userSrcId });
                count++;
                if (count <= 10)
                {
                    dt = (DateTime?)Utility.GetPropertyThroughReflection(dbComplaint, "Dt" + count);
                    srcId = (int?)Utility.GetPropertyThroughReflection(dbComplaint, "SrcId" + count);
                    userSrcId = (int?)Utility.GetPropertyThroughReflection(dbComplaint, "UserSrcId" + count);
                }
            }
            return listAssignmentModel;
        }
        
    }
}