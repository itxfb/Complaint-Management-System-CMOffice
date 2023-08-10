using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using PITB.CMS_Common.Models;
using PITB.CMS_Common.Models.Custom.Reports;

namespace PITB.CMS_Common.Handler.Business
{
    public class BlPMIUReports
    {
        public static List<PMIUSummaryReport.OverDueComplaint> GetOverDueComplaintSummary(string startDate, string endDate, string campId, int hierarchyId,
          int userHierarchyId, string commaSepValPMIUIds, string statusIds,  int reportType)
        {
            List<DbCrmIdsMappingToOtherSystem> listCrmDistrictIdsMapping =
                DbCrmIdsMappingToOtherSystem.Get((int)Config.CrmModule.Hierarchy, hierarchyId,
                     (int)Config.OtherSystemId.SchoolEducation, (int)Config.CrmModule.Hierarchy);
            if(!string.IsNullOrEmpty(commaSepValPMIUIds))
            {
                string[] strArr = commaSepValPMIUIds.Split(',');
                List<int> listPMIUIds = strArr.ToList().Select(n=>Convert.ToInt32(n)).ToList();
                string commaSepVal = Utility.GetCommaSepStrFromList(listCrmDistrictIdsMapping.Where(n => listPMIUIds.Contains((int)n.OTS_Id)).Select(n => n.Crm_Id).ToList());
                List<PMIUSummaryReport.OverDueComplaint> listPMIUOverdueComplaintsToReturn = new List<PMIUSummaryReport.OverDueComplaint>();
                //for (int i = 0; i < strArr.Length; i++)
            
                List<MainSummaryReport.OverDueComplaint> listOverDueComplaints = BlSchoolReports.GetOverdueComplaintsReport(startDate, endDate,
                    campId, hierarchyId, userHierarchyId, commaSepVal, statusIds, reportType);
                Mapper.CreateMap<MainSummaryReport.OverDueComplaint, PMIUSummaryReport.OverDueComplaint>();
                List<PMIUSummaryReport.OverDueComplaint> listPMIUOverdueComplaints = Mapper.Map<List<MainSummaryReport.OverDueComplaint>, List<PMIUSummaryReport.OverDueComplaint>>(listOverDueComplaints);
                //listPMIUOverdueComplaintsToReturn.AddRange(listPMIUOverdueComplaints);
                for (int j = 0; j < listPMIUOverdueComplaints.Count; j++)
                {
                    listPMIUOverdueComplaints[j].PMIUId =
                        Convert.ToInt32(listCrmDistrictIdsMapping.Where(n => n.Crm_Id == listPMIUOverdueComplaints[j].CrmId)
                            .FirstOrDefault()
                            .OTS_Id);
                }
                return listPMIUOverdueComplaints;
            }
            return null;
        }

        public static List<PMIUSummaryReport.TopOverDueComplaintsByOfficer> GetTopOverdueComplaintsByOfficerReport(string startDate, string endDate, string campId, int hierarchyId,
          int userHierarchyId, string commaSepValPMIUIds, string statusIds, int reportType)
        {
            List<DbCrmIdsMappingToOtherSystem> listCrmDistrictIdsMapping =
                DbCrmIdsMappingToOtherSystem.Get((int)Config.CrmModule.Hierarchy, hierarchyId,
                     (int)Config.OtherSystemId.SchoolEducation, (int)Config.CrmModule.Hierarchy);
            if (!string.IsNullOrEmpty(commaSepValPMIUIds))
            {
                string[] strArr = commaSepValPMIUIds.Split(',');
                List<int> listPMIUIds = strArr.ToList().Select(n => Convert.ToInt32(n)).ToList();
                string commaSepVal = Utility.GetCommaSepStrFromList(listCrmDistrictIdsMapping.Where(n => listPMIUIds.Contains((int)n.OTS_Id)).Select(n => n.Crm_Id).ToList());
                List<PMIUSummaryReport.TopOverDueComplaintsByOfficer> listPMIUOverdueComplaintsToReturn = new List<PMIUSummaryReport.TopOverDueComplaintsByOfficer>();
                //for (int i = 0; i < strArr.Length; i++)

                List<MainSummaryReport.TopOverDueComplaintsByOfficer> listOverDueComplaintsByOfficer = BlSchoolReports.GetTopOverdueComplaintsByOfficerReport(startDate, endDate,
                    campId, hierarchyId, userHierarchyId, commaSepVal, statusIds, reportType);
                Mapper.CreateMap<MainSummaryReport.OverDueComplaint, PMIUSummaryReport.OverDueComplaint>();
                List<PMIUSummaryReport.TopOverDueComplaintsByOfficer> listPMIUOverdueComplaints = Mapper.Map<List<MainSummaryReport.TopOverDueComplaintsByOfficer>, List<PMIUSummaryReport.TopOverDueComplaintsByOfficer>>(listOverDueComplaintsByOfficer);
                //listPMIUOverdueComplaintsToReturn.AddRange(listPMIUOverdueComplaints);
                for (int j = 0; j < listPMIUOverdueComplaints.Count; j++)
                {
                    listPMIUOverdueComplaints[j].PMIUId =
                        Convert.ToInt32(listCrmDistrictIdsMapping.Where(n => n.Crm_Id == listPMIUOverdueComplaints[j].CrmId)
                            .FirstOrDefault()
                            .OTS_Id);
                }
                return listPMIUOverdueComplaints;
            }
            return null;
        }
    }
}