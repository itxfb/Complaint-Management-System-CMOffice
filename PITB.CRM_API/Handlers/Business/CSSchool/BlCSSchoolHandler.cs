using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PITB.CRM_API.Models.API.CSSchool;

namespace PITB.CRM_API.Handlers.Business.CSSchool
{
    public class BlCSSchoolHandler
    {
        public static CSSchoolApiModel.Response.AddComplaint AddComplaint(CSSchoolApiModel.Request.AddComplaint addComplaint)
        {
            CSSchoolApiModel.Response.AddComplaint response = new CSSchoolApiModel.Response.AddComplaint(123,"47-123");
            return response;

        }
    }
}