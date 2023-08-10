namespace PITB.CMS.Models.DB
{
    using PITB.CMS.Helper.Database;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using System.Linq;

    [Table("PITB.Dynamic_Form_Controls")]
    public partial class DbDynamicFormControls
    {
        public int Id { get; set; }

        public int? Campaign_Id { get; set; }

        public int? Control_Type { get; set; }

        [StringLength(200)]
        public string FieldName { get; set; }

        public int? CategoryHierarchyId { get; set; }


        public int? CategoryTypeId { get; set; }


        public bool? IsRequired { get; set; }

        public int? Priority { get; set; }

        public bool? IsEditable { get; set; }

        public bool? IsAutoPopulate { get; set; }

        public bool? Is_Active { get; set; }

        public string TagId { get; set; }

        public static List<DbDynamicFormControls> GetByCampaignId(int campaignId)
        {
            try
            {
                using (DBContextHelperLinq db = new DBContextHelperLinq())
                {
                    return db.DbDynamicForm.AsNoTracking().Where(m => m.Campaign_Id == campaignId && m.Is_Active==true).OrderBy(m => m.Control_Type).ToList();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public static List<DbDynamicFormControls> GetBy(int campaignId, string tagId)
        {
            try
            {
                using (DBContextHelperLinq db = new DBContextHelperLinq())
                {
                    return db.DbDynamicForm.AsNoTracking().Where(m => m.Campaign_Id == campaignId && m.TagId==tagId && m.Is_Active == true).OrderBy(m => m.Control_Type).ToList();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public static DbDynamicFormControls GetByControlId(int controlId)
        {
            try
            {
                using (DBContextHelperLinq db = new DBContextHelperLinq())
                {
                    return db.DbDynamicForm.AsNoTracking().Where(m => m.Id ==controlId && m.Is_Active == true).OrderBy(m => m.Control_Type).FirstOrDefault();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public static List<DbDynamicFormControls> GetByControlIds(List<int> listControlIds)
        {
            try
            {
                using (DBContextHelperLinq db = new DBContextHelperLinq())
                {
                    return db.DbDynamicForm.AsNoTracking().Where(m => listControlIds.Contains(m.Id) && m.Is_Active == true).OrderBy(m => m.Control_Type).ToList();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
