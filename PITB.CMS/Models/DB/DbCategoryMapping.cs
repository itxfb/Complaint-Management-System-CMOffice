using System.Data.Entity;
using System.Linq;
using BridgeDTO.Form.DynamicForm;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Information;

namespace PITB.CMS.Models.DB
{
    using PITB.CMS.Helper.Database;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("PITB.Category_Mapping")]
    public partial class DbCategoryMapping
    {
        public int Id { get; set; }

        public int? Campaign_Id { get; set; }

        [ForeignKey("ParentCategory")]
        public int? Parent_Id { get; set; }

        [ForeignKey("ChildCategory")]
        public int? Child_Id { get; set; }

        public int? Hierarchy { get; set; }

        [StringLength(200)]
        public string Tag_Id { get; set; }

        [StringLength(200)]
        public string Parent_Name { get; set; }

        [StringLength(200)]
        public string ChildName { get; set; }


        public virtual DbCategory ParentCategory { get; set; }

        public virtual DbCategory ChildCategory { get; set; }

        public static List<DbCategoryMapping> GetBy(int campaignId, string TagId, int parentId)
        {
            try
            {
                using (DBContextHelperLinq db = new DBContextHelperLinq())
                {

                    //IQueryable<DbCategoryMapping> query =
                    //    db.DbCategoryMapping.AsNoTracking().Where(m => m.Campaign_Id == campaignId);

                    //if (string.IsNullOrEmpty(TagId))
                    //{
                    //    query.Where(n => n.Tag_Id == TagId);
                    //}
                    //if (parentId!=-1)
                    //{
                    //    query.Where(n=>n.Parent_Id==parentId);
                    //}
                    //return query.OrderBy(m => m.Id).Include(n => n.ParentCategory).ToList();

                    List<DbCategoryMapping> listDBCatMap = db.DbCategoryMapping.AsNoTracking().Where(m => m.Campaign_Id == campaignId
                        && m.Tag_Id == TagId
                        && m.Parent_Id == parentId).OrderBy(m => m.Id).Include(n => n.ChildCategory).Include(n => n.ParentCategory).ToList();
                    return listDBCatMap;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public static List<Option> GetListOption(List<DbCategoryMapping> listCategoryMapping)
        {
            List<Option> listOption = new List<Option>();
            foreach (DbCategoryMapping dbCategoryMapping in listCategoryMapping)
            {
                Option option = new Option();
                option.Value = dbCategoryMapping.Child_Id.ToString();
                option.Text = dbCategoryMapping.ChildCategory.Name;
                listOption.Add(option);
            }
            return listOption;
        } 
    }
}
