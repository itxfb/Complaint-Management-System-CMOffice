using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PITB.CMS.Models.DB;

namespace PITB.CMS.Models.Custom
{
    public class AssignmentModel
    {
        public DateTime? Dt { get; set; }

        public double? RetainingTime { get; set; }

        public int? CategoryType { get; set; }

        public int? CategoryId { get; set; }

        public int? CategoryDep1 { get; set; }

        public int? CategoryDep2 { get; set; }


        public int? SrcId { get; set; }

        public int? UserSrcId { get; set; }

        public int? LevelId { get; set; }



        

        //int Level1 { get; set; }

        public AssignmentModel(DateTime? dt, int? srcId)
        {
            this.Dt = dt;
            this.SrcId = srcId;
        }

        public AssignmentModel(DateTime? dt, int? srcId, int? userSrcId)
        {
            this.Dt = dt;
            this.SrcId = srcId;
            this.UserSrcId = userSrcId;
        }

        public AssignmentModel(DateTime? dt)
        {
            this.Dt = dt;
        }

        public AssignmentModel()
        {
        }

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