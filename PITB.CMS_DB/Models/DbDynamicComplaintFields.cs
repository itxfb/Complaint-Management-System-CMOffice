using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PITB.CMS_DB.Models
{

    [Table("PITB.Dynamic_ComplaintFields")]
    public partial class DbDynamicComplaintFields
    {
        public int Id { get; set; }

        public int? ComplaintId { get; set; }

        public int? SaveTypeId { get; set; }

        public int? CategoryHierarchyId { get; set; }

        public int? CategoryTypeId { get; set; }

        public int? ControlId { get; set; }

        [StringLength(2000)]
        public string FieldName { get; set; }

        [StringLength(2000)]
        public string FieldValue { get; set; }

        public string Filter1Name { get; set; }

        public int? Filter1 { get; set; }

        public string Filter2Name { get; set; }

        public int? Filter2 { get; set; }

        public string Filter3Name { get; set; }

        public int? Filter3 { get; set; }

        [StringLength(200)]
        public string TagId { get; set; }
    }
}
