using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Newtonsoft.Json;
using PITB.CMS_Common.ApiModels.Authentication;
using PITB.CMS_Common.ApiModels.API.Datapack;
using PITB.CMS_Common.ApiHandlers.Authentication;
using PITB.CMS_Common.ApiHandlers.Business.Datapack;
using PITB.CMS_Common;

namespace PITB.CRM_API.Controllers.SchoolEducation.PMIU
{
    [RoutePrefix("api/Datapack")]
    public class PmiuDataPackController : ApiController
    {    
        [AcceptVerbs("POST", "GET")]
        [Route("GetMasterComplaintData/{appId:int?}")]
        public ModelResponse Master([FromBody]JToken jsonBody, [FromUri] int appId = 0)
        {
            AuthenticationModel authModel = ApiAuthenticationHandler.GetAuthenticationModel(jsonBody.ToString());
            if (authModel.IsAuthenticated)
            {
                DateTime StartDate;
                DateTime EndDate;
                DateTime.TryParse((string)jsonBody["startDate"], out StartDate);
                DateTime.TryParse((string)jsonBody["endDate"], out EndDate);
                if (StartDate != null && EndDate != null)
                {
                    ModelResponse response = new ModelResponse();
                    response.list = BlDatapack.GetMasterComplaintData(StartDate, EndDate);
                    if (response.list != null)
                    {
                        response.Status = Config.ResponseType.Success.ToString();
                        response.Message = "Success";
                        return response;
                    }
                    else
                    {
                        response.Status = Config.ResponseType.Failure.ToString();
                        response.Message = "Server Error";
                        return response;
                    }
                }
                else
                {
                    ModelResponse response = new ModelResponse();
                    response.Status = Config.ResponseType.ParameterError.ToString();
                    response.Message = "Server Error";
                    return response;
                }
                
            }
            else
            {
                ModelResponse response = new ModelResponse();
                response.Status = Config.ResponseType.AuthenticationFailed.ToString();
                response.Message = "Authentication Error";
                return response;
            }
        }
        [AcceptVerbs("POST", "GET")]
        [Route("GetClosureAndTimelinessData/{appId:int?}")]
        public ModelResponse ClosureAndTimelinessData([FromBody]JToken jsonBody, [FromUri] int appId = 0)
        {
            AuthenticationModel authModel = ApiAuthenticationHandler.GetAuthenticationModel(jsonBody.ToString());
            

            if (authModel.IsAuthenticated)
            {
                DateTime StartDate;
                DateTime EndDate;
                ClosureAndTimelinessParameterModel parameterModel = (ClosureAndTimelinessParameterModel)JsonConvert.DeserializeObject(jsonBody.ToString(), typeof(ClosureAndTimelinessParameterModel));
                if (parameterModel == null)
                {
                    parameterModel = new ClosureAndTimelinessParameterModel();
                }
                if (DateTime.TryParse(parameterModel.StartDate, out StartDate) && DateTime.TryParse(parameterModel.EndDate, out EndDate))
                {
                    ModelResponse response = new ModelResponse();
                    ClosureAndTimelinessModel model = BlDatapack.GetClosureAndTimelinessData(parameterModel);
                    if (model != null)
                    {
                        if (parameterModel.District.Equals("All") && parameterModel.Tehsil.Equals("All") && parameterModel.Markaz.Equals("All"))
                        {
                            response.list = model.HierarchyList.SelectMany(x => x.HierarchyList.SelectMany(z => z.HierarchyList.Select(y => new { MarkazId = z.RowId, MarkazName = z.RowLabel, TehsilId = y.RowId, TehsilName = y.RowLabel, DistrictId = x.RowId, DistrictName = x.RowLabel, Closurerate = y.ClosureRate, Timeliness = y.Timeliness })));
                        }
                        else if (parameterModel.District.Equals("All") && parameterModel.Tehsil.Equals("All"))
                        {
                            response.list = model.HierarchyList.SelectMany(x => x.HierarchyList.Select(y => new { TehsilId = y.RowId, TehsilName = y.RowLabel, DistrictId = x.RowId, DistrictName = x.RowLabel, Closurerate = y.ClosureRate, Timeliness = y.Timeliness }));
                        }
                        else if (parameterModel.District.Equals("All"))
                        {
                            response.list = model.HierarchyList.Select(y => new { DistrictId = y.RowId, DistrictName = y.RowLabel, Closurerate = y.ClosureRate, Timeliness = y.Timeliness });
                        }          
                        response.Status = Config.ResponseType.Success.ToString();
                        response.Message = "Success";
                        return response;
                    }
                    if (response.list != null)
                    {
                        response.Status = Config.ResponseType.Success.ToString();
                        response.Message = "Success";
                        return response;
                    }
                    else
                    {
                        response.Status = Config.ResponseType.Failure.ToString();
                        response.Message = "Server Error";
                        return response;
                    }
                }
                else
                {
                    ModelResponse response = new ModelResponse();
                    response.Status = Config.ResponseType.ParameterError.ToString();
                    response.Message = "Server Error";
                    return response;
                }

            }
            else
            {
                ModelResponse response = new ModelResponse();
                response.Status = Config.ResponseType.AuthenticationFailed.ToString();
                response.Message = "Authentication Error";
                return response;
            }
        }
       
        [AcceptVerbs("POST", "GET")]
        [Route("GetClosureAndTimelinessAggregateData/{appId:int?}")]
        public ModelResponse ClosureAndTimelinessAggregateData([FromBody]JToken jsonBody, [FromUri] int appId = 0)
        {
            AuthenticationModel authModel = ApiAuthenticationHandler.GetAuthenticationModel(jsonBody.ToString());
            if (authModel.IsAuthenticated)
            {
                DateTime StartDate;
                DateTime EndDate;
                ClosureAndTimelinessParameterModel parameterModel = (ClosureAndTimelinessParameterModel)JsonConvert.DeserializeObject(jsonBody.ToString(), typeof(ClosureAndTimelinessParameterModel));
                if (parameterModel == null)
                {
                    parameterModel = new ClosureAndTimelinessParameterModel();
                }
                if (DateTime.TryParse(parameterModel.StartDate, out StartDate) && DateTime.TryParse(parameterModel.EndDate, out EndDate))
                {
                    ModelResponse response = new ModelResponse();
                    response.list = BlDatapack.GetClosureAndTimelinessAggregateData(parameterModel);
                    if (response.list != null)
                    {
                        response.Status = Config.ResponseType.Success.ToString();
                        response.Message = "Success";
                        return response;
                    }
                    else
                    {
                        response.Status = Config.ResponseType.Failure.ToString();
                        response.Message = "Server Error";
                        return response;
                    }
                }
                else
                {
                    ModelResponse response = new ModelResponse();
                    response.Status = Config.ResponseType.ParameterError.ToString();
                    response.Message = "Server Error";
                    return response;
                }

            }
            else
            {
                ModelResponse response = new ModelResponse();
                response.Status = Config.ResponseType.AuthenticationFailed.ToString();
                response.Message = "Authentication Error";
                return response;
            }
        }
        [AcceptVerbs("POST", "GET")]
        [Route("GetSchoolsWithRecurringComplaintsData/{appId:int?}")]
        public ModelResponse SchoolsWithRecurringComplaintsData([FromBody]JToken jsonBody, [FromUri] int appId = 0)
        {
            AuthenticationModel authModel = ApiAuthenticationHandler.GetAuthenticationModel(jsonBody.ToString());
            if (authModel.IsAuthenticated)
            {
                DateTime StartDate;
                DateTime EndDate;
                DateTime.TryParse((string)jsonBody["startDate"], out StartDate);
                DateTime.TryParse((string)jsonBody["endDate"], out EndDate);
                if (StartDate != null && EndDate != null)
                {
                    ModelResponse response = new ModelResponse();
                    List<SchoolsWithRecurringComplaintsModel> model = BlDatapack.GetSchoolsWithRecurringComplaintsData(StartDate, EndDate);
                    //List<object> lsq = new List<object>();
                    int count = model.GroupBy(a => a.DistrictId).Count();
                    SchoolsRecurringResponse[] lsq = new SchoolsRecurringResponse[count];
                    int i = 0;
                    if (model != null)
                    {
                        foreach (var item in model)
                        {
                            lsq[i] = new SchoolsRecurringResponse();
                            lsq[i].districtId = item.DistrictId;
                            lsq[i].districtName = item.DistrictName;
                            lsq[i].data = new List<object>();
                            //var a1 = item.OrderBy(s => s.SchoolEmisCodeNameList.OrderBy(q => q.MarkazNameList.OrderBy(w => w.SchoolNameList.OrderBy(e => e.TypeNameList.OrderBy(r => r.CountOfSubTypeName))))).Select(f=> f.SchoolEmisCodeNameList.Take(5)).Select(x => x.SelectMany(y => y.MarkazNameList.SelectMany(a => a.SchoolNameList.SelectMany(b => b.TypeNameList.SelectMany(c => c.SubTypeNameList.Select(z => new { SchoolEmisCode = y.SchoolEmisCodeName, MarkazName = a.MarkazName, SchoolName = b.SchoolName, TypeName = c.TypeName, SubTypeName = z.SubTypeName, NoOfComplaints = b.CountOfTypeName }))))));
                            var a2 = item.SchoolEmisCodeNameList.Select(x => new { markazlist = x.MarkazNameList.Select(q => new { schoolnamelist = q.SchoolNameList.Select(e => new { typelist = e.TypeNameList.OrderByDescending(w => w.CountOfSubTypeName) }) }) }).Take(5); 
                            //lsq[i].data.Add(item.Take(5).SelectMany(x => x.SchoolEmisCodeNameList.SelectMany(y => y.MarkazNameList.SelectMany(a => a.SchoolNameList.SelectMany(b => b.TypeNameList.OrderByDescending(e=> e.CountOfSubTypeName).SelectMany(c => c.SubTypeNameList.Select(z => new { DistrictId = x.DistrictId, DistrictName = x.DistrictName, SchoolEmisCode = y.SchoolEmisCodeName, MarkazName = a.MarkazName, SchoolName = b.SchoolName, TypeName = c.TypeName, SubTypeName = z.SubTypeName, NoOfComplaints = b.CountOfTypeName })))))));
                            lsq[i].data.Add(a2);                        
                        }
                        //var lst = model.GroupBy(a=>a.DistrictId).SelectMany(x => x.SchoolEmisCodeNameList.SelectMany(y => y.MarkazNameList.SelectMany(a => a.SchoolNameList.SelectMany(b => b.TypeNameList.SelectMany(c => c.SubTypeNameList.Select(z => new { DistrictId = x.DistrictId, DistrictName = x.DistrictName, SchoolEmisCode = y.SchoolEmisCodeName, MarkazName = a.MarkazName, SchoolName = b.SchoolName, TypeName = c.TypeName, SubTypeName = z.SubTypeName, NoOfComplaints = b.CountOfTypeName }))))));
                        response.list = lsq;
                        response.Status = Config.ResponseType.Success.ToString();
                        response.Message = "Success";
                        return response;
                    }
                    else
                    {
                        response.Status = Config.ResponseType.Failure.ToString();
                        response.Message = "Server Error";
                        return response;
                    }
                }
                else
                {
                    ModelResponse response = new ModelResponse();
                    response.Status = Config.ResponseType.ParameterError.ToString();
                    response.Message = "Server Error";
                    return response;
                }

            }
            else
            {
                ModelResponse response = new ModelResponse();
                response.Status = Config.ResponseType.AuthenticationFailed.ToString();
                response.Message = "Authentication Error";
                return response;
            }
        }

        [AcceptVerbs("POST", "GET")]
        [Route("GetSchoolsWithRecurringComplaintsNewFormData/{appId:int?}")]
        public ModelResponse GetSchoolsWithRecurringComplaintsNewFormData([FromBody]JToken jsonBody, [FromUri] int appId = 0)
        {
            AuthenticationModel authModel = ApiAuthenticationHandler.GetAuthenticationModel(jsonBody.ToString());
            if (authModel.IsAuthenticated)
            {
                DateTime StartDate;
                DateTime EndDate;
                DateTime.TryParse((string)jsonBody["startDate"], out StartDate);
                DateTime.TryParse((string)jsonBody["endDate"], out EndDate);
                if (StartDate != null && EndDate != null)
                {
                    ModelResponse response = new ModelResponse();
                    List<SchoolsWithRecurringComplaintsNewFormModel> model = BlDatapack.GetSchoolsWithRecurringComplaintsNewFormData(StartDate, EndDate);                   
                    if (model != null)
                    {                        
                        response.list = model;
                        response.Status = Config.ResponseType.Success.ToString();
                        response.Message = "Success";
                        return response;
                    }
                    else
                    {
                        response.Status = Config.ResponseType.Failure.ToString();
                        response.Message = "Server Error";
                        return response;
                    }
                }
                else
                {
                    ModelResponse response = new ModelResponse();
                    response.Status = Config.ResponseType.ParameterError.ToString();
                    response.Message = "Server Error";
                    return response;
                }

            }
            else
            {
                ModelResponse response = new ModelResponse();
                response.Status = Config.ResponseType.AuthenticationFailed.ToString();
                response.Message = "Authentication Error";
                return response;
            }
        }
        [AcceptVerbs("POST", "GET")]
        [Route("GetCategoryDistributionData/{appId:int?}")]
        public ModelResponse CategoryDistributionData([FromBody]JToken jsonBody, [FromUri] int appId = 0)
        {
            AuthenticationModel authModel = ApiAuthenticationHandler.GetAuthenticationModel(jsonBody.ToString());
            if (authModel.IsAuthenticated)
            {
                DateTime StartDate;
                DateTime EndDate;
                bool start1 = DateTime.TryParse((string)jsonBody["startDate"], out StartDate);
                bool end1 = DateTime.TryParse((string)jsonBody["endDate"], out EndDate);
                if (start1 != false && end1 != false)
                {
                    ModelResponse response = new ModelResponse();
                    response.list = BlDatapack.GetCategoryDistributionData(StartDate, EndDate);
                    if (response.list != null)
                    {
                        response.Status = Config.ResponseType.Success.ToString();
                        response.Message = "Success";
                        return response;
                    }
                    else
                    {
                        response.Status = Config.ResponseType.Failure.ToString();
                        response.Message = "Server Error";
                        return response;
                    }
                }
                else
                {
                    ModelResponse response = new ModelResponse();
                    response.Status = Config.ResponseType.ParameterError.ToString();
                    response.Message = "Invalid Date format or range.";
                    return response;
                }
            }
            else
            {
                ModelResponse response = new ModelResponse();
                response.Status = Config.ResponseType.AuthenticationFailed.ToString();
                response.Message = "Authentication Error";
                return response;
            }
        }

        [AcceptVerbs("POST", "GET")]
        [Route("TopAndWorstPerformingAEO/{appId:int?}")]
        public ModelResponse TopAndWorstPerformingAEO([FromBody]JToken jsonBody, [FromUri] int appId = 0)
        {
            AuthenticationModel authModel = ApiAuthenticationHandler.GetAuthenticationModel(jsonBody.ToString());
            if (authModel.IsAuthenticated)
            {
                DateTime StartDate;
                DateTime EndDate;
                ClosureAndTimelinessParameterModel parameterModel = (ClosureAndTimelinessParameterModel)JsonConvert.DeserializeObject(jsonBody.ToString(), typeof(ClosureAndTimelinessParameterModel));
                if (parameterModel == null)
                {
                    parameterModel = new ClosureAndTimelinessParameterModel();
                }
                if (DateTime.TryParse(parameterModel.StartDate, out StartDate) && DateTime.TryParse(parameterModel.EndDate, out EndDate))
                {
                    ModelResponse response = new ModelResponse();
                    ClosureAndTimelinessModel model = BlDatapack.GetClosureAndTimelinessData(parameterModel);
                    if (model != null)
                    {
                        if (parameterModel.District.Equals("All") && parameterModel.Markaz.Equals("All"))
                        {
                            TopAndWorstPerformingAEOList[] lst = new TopAndWorstPerformingAEOList[model.HierarchyList.Count];                           
                            for(int i=0;i<model.HierarchyList.Count;i++){
                                lst[i] = new TopAndWorstPerformingAEOList();
                                lst[i].DistrictId = model.HierarchyList[i].RowId;
                                lst[i].DistrictName = model.HierarchyList[i].RowLabel;
                                lst[i].TopClosure = new List<TopAndWorstPerformingAEO>();
                                lst[i].TopTimeliness = new List<TopAndWorstPerformingAEO>();
                                lst[i].WorstClosure = new List<TopAndWorstPerformingAEO>();
                                lst[i].WorstTimeliness = new List<TopAndWorstPerformingAEO>();

                                for(int j=0;j<model.HierarchyList[i].TopClosureRateHierarchyList.Count;j++){
                                    TopAndWorstPerformingAEO closure = new TopAndWorstPerformingAEO();
                                    closure.DistrictId = lst[i].DistrictId;
                                    closure.DistrictName = lst[i].DistrictName;
                                    closure.MarkazId = model.HierarchyList[i].TopClosureRateHierarchyList[j].RowId;
                                    closure.MarkazName = model.HierarchyList[i].TopClosureRateHierarchyList[j].RowLabel;
                                    closure.ClosureRate = model.HierarchyList[i].TopClosureRateHierarchyList[j].ClosureRate;
                                    closure.Timeliness = model.HierarchyList[i].TopClosureRateHierarchyList[j].Timeliness;
                                    lst[i].TopClosure.Add(closure);
                                }
                                for (int j = 0; j < model.HierarchyList[i].TopTimelinessHierarchyList.Count; j++)
                                {
                                    TopAndWorstPerformingAEO timeliness = new TopAndWorstPerformingAEO();
                                    timeliness.DistrictId = lst[i].DistrictId;
                                    timeliness.DistrictName = lst[i].DistrictName;
                                    timeliness.MarkazId = model.HierarchyList[i].TopTimelinessHierarchyList[j].RowId;
                                    timeliness.MarkazName = model.HierarchyList[i].TopTimelinessHierarchyList[j].RowLabel;
                                    timeliness.ClosureRate = model.HierarchyList[i].TopTimelinessHierarchyList[j].ClosureRate;
                                    timeliness.Timeliness = model.HierarchyList[i].TopTimelinessHierarchyList[j].Timeliness;
                                    lst[i].TopTimeliness.Add(timeliness);
                                }
                                for (int j = 0; j < model.HierarchyList[i].WorstClosureRateHierarchyList.Count; j++)
                                {
                                    TopAndWorstPerformingAEO closure = new TopAndWorstPerformingAEO();
                                    closure.DistrictId = lst[i].DistrictId;
                                    closure.DistrictName = lst[i].DistrictName;
                                    closure.MarkazId = model.HierarchyList[i].WorstClosureRateHierarchyList[j].RowId;
                                    closure.MarkazName = model.HierarchyList[i].WorstClosureRateHierarchyList[j].RowLabel;
                                    closure.ClosureRate = model.HierarchyList[i].WorstClosureRateHierarchyList[j].ClosureRate;
                                    closure.Timeliness = model.HierarchyList[i].WorstClosureRateHierarchyList[j].Timeliness;
                                    lst[i].WorstClosure.Add(closure);
                                }
                                for (int j = 0; j < model.HierarchyList[i].WorstTimelinessHierarchyList.Count; j++)
                                {
                                    TopAndWorstPerformingAEO timeliness = new TopAndWorstPerformingAEO();
                                    timeliness.DistrictId = lst[i].DistrictId;
                                    timeliness.DistrictName = lst[i].DistrictName;
                                    timeliness.MarkazId = model.HierarchyList[i].WorstTimelinessHierarchyList[j].RowId;
                                    timeliness.MarkazName = model.HierarchyList[i].WorstTimelinessHierarchyList[j].RowLabel;
                                    timeliness.ClosureRate = model.HierarchyList[i].WorstTimelinessHierarchyList[j].ClosureRate;
                                    timeliness.Timeliness = model.HierarchyList[i].WorstTimelinessHierarchyList[j].Timeliness;
                                    lst[i].WorstTimeliness.Add(timeliness);
                                }
                            }
                            
                            response.list = lst;
                            response.Status = Config.ResponseType.Success.ToString();
                            response.Message = "Success";
                            return response;
                        }
                    }

                  
                    if (response.list != null)
                    {
                        response.Status = Config.ResponseType.Success.ToString();
                        response.Message = "Success";
                        return response;
                    }
                    else
                    {
                        response.Status = Config.ResponseType.Failure.ToString();
                        response.Message = "Server Error";
                        return response;
                    }
                }
                else
                {
                    ModelResponse response = new ModelResponse();
                    response.Status = Config.ResponseType.ParameterError.ToString();
                    response.Message = "Server Error";
                    return response;
                }

            }
            else
            {
                ModelResponse response = new ModelResponse();
                response.Status = Config.ResponseType.AuthenticationFailed.ToString();
                response.Message = "Authentication Error";
                return response;
            }
        }

       
    }
}