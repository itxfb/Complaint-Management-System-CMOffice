using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PITB.CMS.Helper.Database;
using PITB.CMS.Models.DB;
using PITB.CMS.Models.View;
using PITB.CMS.Models.View.Select2;

namespace PITB.CMS.Handler.Business
{
    public class BlDynamicCategories
    {
        public static List<DbDynamicCategories> GetDynamicCategoriesChildrenAgainstCatId(int catId)
        {
            List<DbDynamicCategoriesMapping> listDynCatMapping =
                DbDynamicCategoriesMapping.GetDynamicCategoriesByCategoryId(catId);

            List<DbDynamicCategories> listChildrenDynCat =
                DbDynamicCategories.GetByCategoryIdList(listDynCatMapping.Select(n=>(int)n.Category_Id).ToList()).OrderBy(n=>n.CategoryTypeId).ThenBy(n=>n.Name).ToList();

            return listChildrenDynCat;
        }

        public static List<VmServerSideDropDownList> GetServerSideSearchCategories(int campaignId, int categoryTypeId, string searchStr, int from, int to)
        {
            List<VmServerSideDropDownList> listDynamic = null;
            string searchQuery = Utility.GetDBSearchQueryOnIndividualWords("Name", searchStr);
            if (searchQuery != "")
            {
                string fullQuery = @"select * FROM (
                                        select Id AS Id, Name AS Text,ROW_NUMBER() OVER (ORDER BY Name ) AS RowNum, count(*)  OVER() AS TotalRows 
                                        from PITB.Dynamic_Categories where CampaignId=" + campaignId + " and CategoryTypeId=" + categoryTypeId +" and "+ searchQuery+") as a "+
                                        "where RowNum BETWEEN "+from+" AND "+to;
                listDynamic = DBHelper.GetDataTableByQueryString(fullQuery, null).ToList<VmServerSideDropDownList>();
            }
            else
            {
                listDynamic = new List<VmServerSideDropDownList>();
            }
            return listDynamic;
        }

        

        public static VmSelect2ServerSideDropDownLIst GetDropDownSchoolListModel(int districtId, int schoolCategory, string searchStr, int page)
        {
            Tuple<int, int> rowTuple = Utility.GetRowRangeFromPageIndex(page);
            int from = rowTuple.Item1;
            int to = rowTuple.Item2;
            VmSelect2ServerSideDropDownLIst vmDDL = new VmSelect2ServerSideDropDownLIst();
            List<VmServerSideDropDownList> listServerSideCategories = BlSchool.GetServerSideSearchSchools(districtId, schoolCategory, searchStr, from, to);
            vmDDL.ListItems = listServerSideCategories.Select(n => new Select2ListItem { id = n.Id + Config.Separator + n.Text, text = n.Text }).ToList();
            vmDDL.TotalCount = (listServerSideCategories.Count > 0) ? listServerSideCategories[0].TotalRows : 0;

            return vmDDL;
        }

        public static VmSelect2ServerSideDropDownLIst GetDropDownListModel(int campaignId, int categoryId, string searchStr, int page)
        {
            Tuple<int,int> rowTuple = Utility.GetRowRangeFromPageIndex(page);
            int from = rowTuple.Item1;
            int to = rowTuple.Item2;
            VmSelect2ServerSideDropDownLIst vmDDL = new VmSelect2ServerSideDropDownLIst();
            List<VmServerSideDropDownList> listServerSideCategories = GetServerSideSearchCategories(campaignId, categoryId, searchStr, from, to);
            vmDDL.ListItems = listServerSideCategories.Select(n => new Select2ListItem { id = n.Id+Config.Separator+n.Text, text = n.Text }).ToList();
            vmDDL.TotalCount = (listServerSideCategories.Count > 0) ? listServerSideCategories[0].TotalRows : 0;

            return vmDDL;
        }
    }
}