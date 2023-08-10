using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using PITB.CRM_API.Helper.Database;
using PITB.CRM_API.Models.Authentication;
using PITB.CRM_API.Models.Custom;
using Newtonsoft.Json;

namespace PITB.CRM_API.Handlers.Authentication
{
    public class ApiAuthenticationHandler
    {
        public static AuthenticationModel GetAuthenticationModel(string jsonStr)
        {
            try
            {
                AuthenticationModel authModel =
                        (AuthenticationModel)JsonConvert.DeserializeObject(jsonStr, typeof(AuthenticationModel));
                if (HttpContext.Current.Request.IsSecureConnection)
                {
                    authModel.HttpSchemePermission = Uri.UriSchemeHttps;
                }
                else
                {
                    authModel.HttpSchemePermission = Uri.UriSchemeHttp;
                }

                if (IsAuthenticated(authModel))
                {
                    authModel.IsAuthenticated = true;
                }
                else
                {
                    authModel.IsAuthenticated = false;
                }



                return authModel;
            }
            catch (Exception ex)
            {
                return null;
            }
            return null;
        }

        //public static AuthenticationModel GetAuthenticationModel(HttpHeaders headers)
        //{
        //    return null;
        //}

        public static AuthenticationModel GetAuthenticationModel(HttpRequestMessage request /*HttpHeaders headers*/)
        {
            try
            {
                HttpHeaders headers = request.Headers;
                 AuthenticationModel authModel = new AuthenticationModel(
                     headers.GetValues("SystemName").First(),
                     Utility.GetClientIpAddress(HttpContext.Current.Request),
                     headers.GetValues("Username").First(),
                     headers.GetValues("Password").First()
                     );
                if (headers.Contains("input_type"))
                {
                    authModel.ClientKey = "input_type="+  headers.GetValues("input_type").First();
                }

                if (headers.Contains("AppVersion"))
                {
                    authModel.AppVersion = headers.GetValues("AppVersion").First();
                }

                authModel.HttpSchemePermission = request.RequestUri.Scheme;
                

                if (IsAuthenticated(authModel))
                {
                    authModel.IsAuthenticated = true;
                }
                else
                {
                    authModel.IsAuthenticated = false;
                }



                return authModel;
            }
            catch (Exception ex)
            {
                return null;
            }
            return null;
        }


        public static bool IsAuthenticated(AuthenticationModel authenticationModel)
        {
            try
            {
                Dictionary<string, object> paramDict = new Dictionary<string, object>();
                paramDict.Add("@SystemName", authenticationModel.SystemName.ToDbObj());
                //paramDict.Add("@Ip", authenticationModel.Ip.ToDbObj());
                //paramDict.Add("@AppVersion", authenticationModel.AppVersion.ToDbObj());
                paramDict.Add("@Username", authenticationModel.Username.ToDbObj());
                paramDict.Add("@ClientKey", authenticationModel.ClientKey.ToDbObj());
                paramDict.Add("@Password", authenticationModel.Password.ToDbObj());

                List<AuthenticationModel> listAuthenticationModel = DBHelper.GetDataTableByStoredProcedure("[PITB].[Get_Client_System]", paramDict).ToList<AuthenticationModel>();

                
                
                if (listAuthenticationModel.FirstOrDefault() != null)
                {

                    // Check Scheme
                    List<string> listStr = CMS.Utility.Split(listAuthenticationModel.FirstOrDefault().HttpSchemePermission, ",").ToList();
                    if (listStr.Where(n=>n.Equals(authenticationModel.HttpSchemePermission,StringComparison.OrdinalIgnoreCase)).FirstOrDefault()==null)
                    {
                        authenticationModel.IsAuthenticated = false;
                        authenticationModel.SetAuthenticationError();
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
                    if (!string.IsNullOrEmpty(authenticationModel.AppVersion))
                    {
                        bool isAppVersionFound = false;
                        foreach (AuthenticationModel authModel in listAuthenticationModel)
                        {
                            List<int> listAppVersion = Utility.GetIntList(authModel.AppVersion);
                            if (listAppVersion.Contains(int.Parse(authenticationModel.AppVersion)))
                            {
                                isAppVersionFound = true;
                            }
                        }

                        if (!isAppVersionFound)
                        {
                            authenticationModel.IsAuthenticated = false;
                            authenticationModel.SetAppUpdateStatus(listAuthenticationModel.FirstOrDefault());
                            return authenticationModel.IsAuthenticated;
                        }
                    }

                    

                    string requestIp = Utility.GetClientIpAddress(HttpContext.Current.Request);
                    listAuthenticationModel = listAuthenticationModel.Where(n => n.IsIpAllowed).ToList(); 
                    if (listAuthenticationModel.Count > 0)// if ip is allowed
                    {
                        listAuthenticationModel = listAuthenticationModel.Where(n => n.Ip == requestIp).ToList();
                        if (listAuthenticationModel.Count > 0)
                        {
                            authenticationModel.IsAuthenticated = true;
                            authenticationModel.SetSuccess();
                        }
                        else
                        {
                            authenticationModel.IsAuthenticated = false;
                            authenticationModel.SetAuthenticationError();
                        }
                    }
                    else // if 
                    {
                        authenticationModel.IsAuthenticated = true;
                        authenticationModel.SetSuccess();
                    }

                    
                }
                else
                {
                    authenticationModel.IsAuthenticated = false;
                    authenticationModel.SetAuthenticationError();
                }

                return authenticationModel.IsAuthenticated;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

    }
}