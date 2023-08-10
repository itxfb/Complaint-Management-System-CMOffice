
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PITB.CMS_Common.Models.Custom
{
    public class FuncParamsModel
    {
        public class Assignment
        {
            public DateTime CreationDate { get; set; }

            public List<DbAssignmentMatrix> ListAssignmentMatrix { get; set; }

            public float CategoryRetainingHours { get; set; }

            public float OverRideRetainingHours { get; set; }


            public Assignment()
            {
                
            }

            public Assignment(DateTime creationDate, List<DbAssignmentMatrix> listAssignmentMatrix, float categoryRetainingHours, float overRideRetainingHours=-1)
            {
                CreationDate = creationDate;
                ListAssignmentMatrix = listAssignmentMatrix;
                CategoryRetainingHours = categoryRetainingHours;
                OverRideRetainingHours = overRideRetainingHours;
            }
        }
    }
}