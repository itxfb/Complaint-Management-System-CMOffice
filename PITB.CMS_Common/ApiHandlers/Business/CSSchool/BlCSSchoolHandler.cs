using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PITB.CMS_Common.ApiModels.API.CSSchool;


namespace PITB.CMS_Common.ApiHandlers.Business.CSSchool
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