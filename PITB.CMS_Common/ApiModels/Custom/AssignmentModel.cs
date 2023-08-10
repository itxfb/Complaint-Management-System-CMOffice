using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PITB.CMS_Common.ApiModels.Custom
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
    }
}