using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using PITB.CMS_Common;
using PITB.CMS_Common.Helper;
using PITB.CMS_Common.Models;
using PITB.CRM.Public_Web.BusinessLayer;

namespace PITB.CRM.Public_Web.Models.ViewModels
{
    public class VmFilters
    {
        private int campaignId;

        public int CampaignId
        {
            get { return campaignId; }
            set { campaignId = value; }
        }

        public bool Filterable { get; set; }

        public VmFilters()
        {
            this.Paging = new Paging();
            this.CampaignId = (int)Config.Campaign.FixItLwmc;


        }
        public VmFilters(Config.Campaign campaignId)
        {
            this.CampaignId = (int)campaignId;
            this.Paging = new Paging();
        }
        public List<SelectListItem> DistrictsList
        {
            get
            {
                using (DBContextHelperLinq db = new DBContextHelperLinq())
                {
                    List<DbDistrict> listDbItems = db.DbDistricts.AsNoTracking().OrderBy(m => m.District_Name).ToList();
                    List<SelectListItem> items = listDbItems.Select(dbItem => new SelectListItem()
                    {
                        Text = dbItem.District_Name,
                        Value = dbItem.District_Id.ToString()
                    }).ToList();
                    return items;
                }
            }
        }
        public List<SelectListItem> TownTehsilsList { get; set; }
        public List<SelectList> UcsList { get; set; }
        public List<SelectList> WardsList { get; set; }
        public List<SelectListItem> CategoriesSelectListItems
        {
            get
            {

                var subTypesList = from sb in DbComplaintSubType.AllSubTypesByCampaignId(CampaignId)
                                   select new { Value = sb.Complaint_SubCategory, Text = sb.Name };
                List<SelectListItem> items = subTypesList.Select(dbItem => new SelectListItem()
                {
                    Text = dbItem.Text,
                    Value = dbItem.Value.ToString()
                }).ToList();
                return items;

            }
        }
        public List<SelectListItem> StatusSelectListItems
        {
            get
            {
                //List<DbStatus> dbStatusList = DbStatus.GetByCampaignId(CampaignId);
                
                //List<SelectListItem> items = dbStatusList.Select(dbItem => new SelectListItem()
                //{
                //    Text = dbItem.Status,
                //    Value = dbItem.Complaint_Status_Id.ToString()
                //}).ToList();

                return Enum.GetValues(typeof(Config.PublicComplaintStatus)).Cast<Config.PublicComplaintStatus>().Select(v => new SelectListItem
                {
                    Text = v.ToString().ToSentenceCase(),
                    Value = ((byte)v).ToString()
                }).ToList();
                
                
            }
        }

        private List<int> statusId;

        public List<int> StatusId
        {
            get
            {
                return (statusId != null && statusId.Count > 0)
                    ? BlComplaint.GetDbStatusIdsIntListByPublicComplaintStatus(statusId)
                    : statusId;
            }
            set { statusId = value; }
        }
        
        
        public string SearchTerm { get; set; }
        public string FromDate { get; set; }
        public int[] DistrictIds { get; set; }
        public int[] TownTehsilId { get; set; }
        public int[] Uc { get; set; }
        public int[] Categories { get; set; }

        public Paging Paging { get; set; }
    }

    public class Paging
    {
        public Paging()
        {

        }
        public int PageSize { get { return Config.PageDraw; } }
        public int CurrentPage { get; set; }
        public int FromPage { get { return (CurrentPage * PageSize + 1); } }
        public int ToPage { get { return (CurrentPage == 0) ? PageSize : (FromPage + PageSize -1); } }
        public string OrderByField { get; set; }
        public string OrderByDirection { get; set; }

    }

}