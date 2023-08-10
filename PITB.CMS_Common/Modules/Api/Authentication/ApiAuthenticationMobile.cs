using PITB.CMS_Common.ApiModels.Authentication;
using PITB.CMS_Common.Helper.Database;
using PITB.CMS_Common.Modules.Api.Response;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace PITB.CMS_Common.Modules.Api.Authentication
{
    public class ApiAuthenticationMobile

    {
        public static dynamic GetAuthentication(dynamic dParam)
        {
            try
            {
                HttpRequestMessage request = dParam;
                HttpHeaders headers = request.Headers;


                dynamic d = new ExpandoObject(); //new Dictionary<string, object>();
                d.authStatus = 1;
                d.authModel = new ExpandoObject();
                d.authResponse = null;

                d.authModel.systemName = headers.GetValues("SystemName").First();
                d.authModel.ip = Utility.GetClientIpAddress(HttpContext.Current.Request);
                d.authModel.userName = headers.GetValues("Username").First();
                d.authModel.password = headers.GetValues("Password").First();

                //AuthenticationModel authModel = new AuthenticationModel(
                //    headers.GetValues("SystemName").First(),
                //    Utility.GetClientIpAddress(HttpContext.Current.Request),
                //    headers.GetValues("Username").First(),
                //    headers.GetValues("Password").First()
                //    );
                if (headers.Contains("input_type"))
                {
                    d.authModel.clientKey = "input_type=" + headers.GetValues("input_type").First();
                }

                if (headers.Contains("AppVersion"))
                {
                    d.authModel.appVersion = headers.GetValues("AppVersion").First();
                }

                d.authModel.httpSchemePermission = request.RequestUri.Scheme;
                return PopulateAuth(d);

            }
            catch (Exception ex)
            {
                return null;
            }
            return null;
        }

        public static dynamic PopulateAuth(dynamic dParam)
        {
            try
            {
                dParam.authResponse = new ExpandoObject();
                dynamic authenticationModel = dParam.authModel;
                
                Dictionary<string, object> paramDict = new Dictionary<string, object>();
                paramDict.Add("@SystemName", ((object)Utility.GetPropertyValueFromDynamic(authenticationModel,"systemName")).ToDbObj());
                //paramDict.Add("@Ip", authenticationModel.Ip.ToDbObj());
                //paramDict.Add("@AppVersion", authenticationModel.AppVersion.ToDbObj());
                paramDict.Add("@Username", ((object)Utility.GetPropertyValueFromDynamic(authenticationModel, "userName")).ToDbObj());
                paramDict.Add("@ClientKey", ((object)Utility.GetPropertyValueFromDynamic(authenticationModel, "clientKey")).ToDbObj());
                paramDict.Add("@Password", ((object)Utility.GetPropertyValueFromDynamic(authenticationModel, "password")).ToDbObj());

                //List<AuthenticationModel> listAuthenticationModel = DBHelper.GetDataTableByStoredProcedure("[PITB].[Get_Client_System]", paramDict).ToList<AuthenticationModel>();

                List<dynamic> listAuthenticationModel = DBHelper.GetDataTableByStoredProcedure("[PITB].[Get_Client_System]", paramDict).ToDynamicList();


                if (listAuthenticationModel.FirstOrDefault() != null)
                {

                    // Check Scheme
                    List<string> listStr = Utility.Split((string)listAuthenticationModel.FirstOrDefault().HttpSchemePermission, ",").ToList();
                    if (listStr.Where(n => n.Equals(authenticationModel.httpSchemePermission, StringComparison.OrdinalIgnoreCase)).FirstOrDefault() == null)
                    {
                        //authenticationModel.IsAuthenticated = false;
                        dParam.authResponse = ApiResponseHandlerMobile.SetAuthenticationError(null);
                        dParam.authStatus = -1;
                        return authenticationModel.IsAuthenticated;
                    }

                    // Check App version if not present 
                    //if (listAuthenticationModel.Where(n => n.AppVersion == authenticationModel.AppVersion).FirstOrDefault() == null)
                    //{
                    //    authenticationModel.IsAuthenticated = false;
                    //    authenticationModel.SetAppUpdateStatus(listAuthenticationModel.FirstOrDefault());
                    //    return authenticationModel.IsAuthenticated;
                    //}

                    // Check comma seperated App version if not present 
                    if (!string.IsNullOrEmpty(authenticationModel.appVersion))
                    {
                        bool isAppVersionFound = false;
                        foreach (dynamic authModel in listAuthenticationModel)
                        {
                            List<int> listAppVersion = Utility.GetIntList(authModel.AppVersion);
                            if (listAppVersion.Contains(int.Parse(authenticationModel.appVersion)))
                            {
                                isAppVersionFound = true;
                            }
                        }

                        if (!isAppVersionFound)
                        {
                            dParam.authResponse = ApiResponseHandlerMobile.SetAppUpdateUrl(null, listAuthenticationModel.FirstOrDefault().AppUpdateUrl);
                            dParam.authStatus = -1;
                            return dParam;

                            //authenticationModel.IsAuthenticated = false;
                            //authenticationModel.SetAppUpdateStatus(listAuthenticationModel.FirstOrDefault());
                            //return authenticationModel.IsAuthenticated;
                        }
                    }



                    string requestIp = Utility.GetClientIpAddress(HttpContext.Current.Request);
                    listAuthenticationModel = listAuthenticationModel.Where(n => n.IsIpAllowed).ToList();
                    if (listAuthenticationModel.Count > 0)// if ip is allowed
                    {
                        listAuthenticationModel = listAuthenticationModel.Where(n => n.Ip == requestIp).ToList();
                        if (listAuthenticationModel.Count > 0)
                        {
                            dParam.authResponse = ApiResponseHandlerMobile.SetSuccess(null);
                            dParam.authStatus = 1;
                        }
                        else
                        {
                            dParam.authResponse = ApiResponseHandlerMobile.SetAuthenticationError(null);
                            dParam.authStatus = -1;
                        }
                    }
                    else // if 
                    {
                        dParam.authResponse = ApiResponseHandlerMobile.SetSuccess(null);
                        dParam.authStatus = 1;
                    }


                }
                else
                {
                    dParam.authResponse = ApiResponseHandlerMobile.SetAuthenticationError(null);
                    dParam.authStatus = -1;
                }

                return dParam;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
