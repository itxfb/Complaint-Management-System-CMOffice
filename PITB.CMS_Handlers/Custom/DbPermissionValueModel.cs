using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PITB.CMS_Handlers.Custom
{
    public class DbPermissionValueModel
    {
        // for perssion value CampaignAssignmentMatrix = 6
        public class CampaignAssignmentMatrix
        {

            public List<KeyVal> ListKeyVal { get; set; }
            public List<AssignmentMatrix> ListAssignmentMatrix { get; set; }


            public class KeyVal
            {
                public string Key { get; set; }
                public string Value { get; set; }
            }

            public class AssignmentMatrix
            {
                public int CampaignId { get; set; }
                public int FromSourceId { get; set; }
                public int ToSourceId { get; set; }
                public int LevelId { get; set; }

                public float RetainingHours { get; set; }

                public int ToUserSourceId { get; set; }
            }
        }

        

    }
}