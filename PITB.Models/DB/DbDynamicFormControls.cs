using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PITB.CMS_Models.DB
{

    [Table("PITB.Dynamic_Form_Controls")]
    public class DbDynamicFormControls
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
    }
}
