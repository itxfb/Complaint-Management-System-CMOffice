using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Text.RegularExpressions;
using PITB.CMS_Common.Helper.Database;
using PITB.CMS_Common.ApiModels.Custom;

namespace PITB.CMS_Common.ApiHandlers.Business.SchoolEducation
{
    public class BlPublicSchools
    {
        public static List<DistrictModel> GetDistrictList()
        {
            List<DistrictModel> list = null;
            try
            {
                string queryStr = "SELECT Districts.id,Districts.District_Name,Divisions.Division_Name,Provinces.Province_Name FROM PITB.Districts " +
                "INNER JOIN PITB.Divisions on Districts.Division_Id = Divisions.Id " +
                "INNER JOIN PITB.Provinces on Provinces.id = Divisions.Province_Id " +
                "WHERE Districts.Is_Active = 1 AND Provinces.id = 1" ;
                DataTable lObjData = DBHelper.GetDataTableByQueryString(queryStr, null);
                if (lObjData != null && lObjData.Rows.Count > 0)
                {
                    list = new List<DistrictModel>();
                    foreach (DataRow row in lObjData.Rows){
                        DistrictModel model = new DistrictModel();
                        model.id = Int32.Parse(row["id"].ToString());
                        model.District_Name =  row["District_Name"].ToString();
                        model.Division_Name =  row["Division_Name"].ToString();
                        model.Province_Name =  row["Province_Name"].ToString();
                        list.Add(model);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return list;
        }
        public static CategoryModelResponse GetCategoryData()
        {
            List<CategoryModel> list = null;
            try
            {
                string queryStr = "SELECT Department.Id As 'DepartmentId',Department.Name As 'Department',Complaints_Type.Id As 'TypeId',Complaints_Type.Name As 'Type',Complaints_SubType.Id As 'SubtypeId' , Complaints_SubType.Name As 'SubType' FROM PITB.Complaints_SubType "+
                "INNER JOIN PITB.Complaints_Type ON Complaints_Type.Id = Complaints_SubType.Complaint_Type_Id " +
                "INNER JOIN PITB.Department ON Complaints_Type.DepartmentId = Department.Id " +
                "WHERE Complaints_Type.Campaign_Id = 47  AND Department.Group_Id is NULL " +
                "GROUP BY Department.Id,Department.Name,Complaints_Type.Id,Complaints_Type.Name,Complaints_SubType.Id,Complaints_SubType.Name ";
                DataTable lObjData = DBHelper.GetDataTableByQueryString(queryStr, null);
                if (lObjData != null && lObjData.Rows.Count > 0)
                {
                    list = new List<CategoryModel>();
                    foreach (DataRow row in lObjData.Rows)
                    {
                        CategoryModel model = new CategoryModel();
                        model.Department = row["Department"].ToString().TrimEndLine();
                        model.Type = row["Type"].ToString().TrimEndLine();
                        model.SubType = row["SubType"].ToString().TrimEndLine();
                        model.DepartmentId = row["DepartmentId"].ToString().TrimEndLine();
                        model.SubtypeId = row["SubtypeId"].ToString().TrimEndLine();
                        model.TypeId = row["TypeId"].ToString().TrimEndLine();
                        list.Add(model);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            CategoryModelResponse response = new CategoryModelResponse();
            response.categoryList = list;
            return response;
        }
        public struct DistrictModel
        {
            public int id;
            public string District_Name;
            public string Division_Name;
            public string Province_Name;
        }
        public struct CategoryModel
        {
            public string Department;
            public string Type;
            public string SubType;
            public string DepartmentId;
            public string TypeId;
            public string SubtypeId;
        }
        public class CategoryModelResponse : ApiStatus
        {
            public List<CategoryModel> categoryList = new List<CategoryModel>();
        }
    }
}