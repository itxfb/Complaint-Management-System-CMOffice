﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PITB.CMS.Models.DB;

namespace PITB.CMS.Models.Custom
{
    public class ReEvaluationAssignmentModel
    {
        public DbComplaint PrevDbComplaint { get; set; }

        public DbComplaint CurrDbComplaint { get; set; }

        public bool HasAssignmentChanged
        {
            get
            {
                if (PrevDbComplaint.UserCategoryId1 != CurrDbComplaint.UserCategoryId1
                    || PrevDbComplaint.UserCategoryId2 != CurrDbComplaint.UserCategoryId2

                    || PrevDbComplaint.SrcId1 != CurrDbComplaint.SrcId1
                   // || PrevDbComplaint.Dt1 != CurrDbComplaint.Dt1
                    || PrevDbComplaint.UserSrcId1 != CurrDbComplaint.UserSrcId1
                    )
                {
                    return true;    
                }
                return false;

            }
        }
    }
}