using System.Linq;
using PITB.CRM_API.Helper.Database;

namespace PITB.CRM_API.Models.DB
{
    using PITB.CRM_API.Helper.Database;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("PITB.AssignmentMatrix")]
    public partial class DbAssignmentMatrix
    {
        [Column(TypeName = "numeric")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public decimal Id { get; set; }

        public int? CampaignId { get; set; }

        public int? CategoryType { get; set; }

        public int? CategoryId { get; set; }

        public int? FromSourceId { get; set; }

        public int? ToSourceId { get; set; }

        public int? ToUserSourceId { get; set; }

        public int? LevelId { get; set; }
        public double RetainingHours { get; set; }

        public bool? IsActive { get; set; }

        #region Helpers
        public static List<DbAssignmentMatrix> GetByCampaignIdAndCategoryId(int campId, int catId, int subCatId)
        {
            try
            {
                using (var db = new DBContextHelperLinq())
                {
                    List<DbAssignmentMatrix> listAssignMatrix = null;
                    // For Sub Category
                    listAssignMatrix = db.DbAssignmentMatrix.AsNoTracking().Where(n => n.CampaignId == campId && n.IsActive==true && n.CategoryId == subCatId && n.CategoryType == (int)Config.CategoryType.Sub).OrderBy(n => n.LevelId).ToList();

                    if (listAssignMatrix.Count == 0)// if sub category is not find in assignment matrix
                    {
                        listAssignMatrix = db.DbAssignmentMatrix.AsNoTracking().Where(n => n.CampaignId == campId && n.IsActive == true && n.CategoryId == catId && n.CategoryType == (int)Config.CategoryType.Main).OrderBy(n => n.LevelId).ToList();
                    }
                    if (listAssignMatrix.Count == 0)// if category is not find in assignment matrix
                    {
                        listAssignMatrix = db.DbAssignmentMatrix.AsNoTracking().Where(n => n.CampaignId == campId && n.IsActive == true && n.CategoryId == null).OrderBy(n => n.LevelId).ToList();
                    }
                    return listAssignMatrix;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion
    }
}
