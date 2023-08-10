using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.EnterpriseServices;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using System.Web.UI.WebControls;
using Amazon.CognitoSync.Model;
using Amazon.DeviceFarm.Model;
using Amazon.EC2;
using Amazon.IdentityManagement.Model;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using PITB.CMS.Handler.Complaint;
using PITB.CMS.Handler.DataTableJquery;
using PITB.CMS.Handler.StakeHolder;
using PITB.CMS.Helper.Database;
using PITB.CMS.Models.DB;
using PITB.CMS.Models.View;
using PITB.CMS.Models.Custom;
using PITB.CMS.Handler.Authentication;
using PITB.CMS.Handler.Complaint.Assignment;
using PITB.CMS.Helper;
using PITB.CMS.Handler.DynamicFields;
using PITB.CMS.Handler.Permission;
using PITB.CMS.Handler.Messages;
using PITB.CMS.Models.View.Dynamic;
using AutoMapper;
using PITB.CMS.Models.Custom.DataTable;
using PITB.CMS.Handler.Complaint.Status;
using PITB.CMS.Handler.Complaint.Transfer;
using System.Web.Mvc;
using System.Threading;
using System.Text;
using PITB.CMS;
using System.Xml;

namespace PITB.CMS.Handler.Business
{
    public class BlPLRA
    {
        public static List<VmStakeholderComplaintListingPLRA> GetStakeHolderServerSideListDenormalized(string to, string from, DataTableParamsModel dtModel, string filePath)
        {
            List<VmStakeholderComplaintListingPLRA> data = new List<VmStakeholderComplaintListingPLRA>();
            try
            {
                int startRow = dtModel.Start;
                int endRow = dtModel.End;
                string url = "http://cms.punjab-zameen.gov.pk/PLRA_Comp_Service.asmx/GetComplaintsDetails";
                string startDate = Utility.GetDateTimeStr(to, "yyyy-MM-dd");
                string endDate = Utility.GetDateTimeStr(from, "yyyy-MM-dd");
                Dictionary<string, object> paramDict = new Dictionary<string, object>();
                paramDict.Add("DateFrom", startDate);
                paramDict.Add("DateTo", endDate);
                paramDict.Add("StartIndex", startRow);
                paramDict.Add("EndIndex", endRow);
                StringBuilder urlParameters = new StringBuilder();
                foreach (var prop in paramDict)
                {
                    urlParameters.AppendFormat("{0}={1}&", prop.Key, prop.Value);
                }
                string urlTemps = urlParameters.ToString().TrimEnd('&');
                url = url + "?" + urlTemps;
                string xml = SOAPHelper.HttpGetProtocolConsumeWebRequest(url);
                //string xml = System.IO.File.ReadAllText(filePath);
                //var validXmlChars = xml.Where(ch => XmlConvert.IsXmlChar(ch)).ToArray();
                //string newXml = new string(xml);
                XmlDocument document = new XmlDocument();
                document.LoadXml(xml);
                XmlNodeList nodes = document.GetElementsByTagName("tblServiceTickDetails");
                if (nodes != null && nodes.Count > 0)
                {
                    for (int i = 0; i < nodes.Count; i++)
                    {
                        int nodesCount = nodes[i].ChildNodes.Count;
                        VmStakeholderComplaintListingPLRA nodeData = new VmStakeholderComplaintListingPLRA();
                        for (int j = 0; j < nodesCount; j++)
                        {
                            string name = nodes[i].ChildNodes[j].Name;
                            string value = nodes[i].ChildNodes[j].InnerText;

                            switch (name)
                            {
                                case "Ticket_Id":
                                    nodeData.ComplaintId = value;
                                    break;
                                case "Type":
                                    nodeData.Type = value;
                                    break;
                                case "Name":
                                    nodeData.Name = value;
                                    break;
                                case "Phone":
                                    nodeData.Phone = value;
                                    break;
                                case "Division":
                                    nodeData.Division = value;
                                    break;
                                case "District":
                                    nodeData.District = value;
                                    break;
                                case "Center":
                                    nodeData.Center = value;
                                    break;
                                case "Additional_Center":
                                    nodeData.Additional_Center = value;
                                    break;
                                case "detail":
                                    nodeData.Detail = value;
                                    break;
                                case "WorkCode":
                                    nodeData.WorkCode = value;
                                    break;
                                case "Created_Date":
                                    nodeData.Created_Date = value;
                                    break;
                                case "Total_Records":
                                    nodeData.Total_Rows = Int32.Parse(value);
                                    break;
                                default:
                                    break;
                            }
                            
                        }
                        data.Add(nodeData);
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return data;
        }
    }
}