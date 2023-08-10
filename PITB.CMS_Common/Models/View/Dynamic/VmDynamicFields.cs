using Foolproof;
using PITB.CMS_Common.Models.Custom;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PITB.CMS_Common.Models.View.Dynamic
{
    public class VmDynamicField
    {
       

        public int ControlId { get; set; }
        public Config.DynamicControlType ControlType {get; set;}

        public string FieldName { get; set; }

        public bool IsRequired { get; set; }

        public int Priority { get; set; }

        public bool IsEditable { get; set; }

        public bool IsAutoPopulate { get; set; }


        public string Setting { get; set; }

        public int CategoryHierarchyId { get; set; }


        //public int? SelectedItemId { get; set; }

        public string SelectedItemString { get; set; }

        public VmDynamicField(int controlId, int controleType, string fieldName, bool isRequired, int priority)
        {
            this.ControlId = controlId;
            this.ControlType = (Config.DynamicControlType)controleType;
            this.FieldName = fieldName;
            this.IsRequired = isRequired;
            this.Priority = priority;
            //this.SelectedItemId = 0;
            this.SelectedItemString = "";

        }

        public VmDynamicField(int controlId, int controleType, string fieldName, bool isRequired, int priority, bool isEditable, bool isAutoPopulate)
        {
            this.ControlId = controlId;
            this.ControlType = (Config.DynamicControlType)controleType;
            this.FieldName = fieldName;
            this.IsRequired = isRequired;
            this.Priority = priority;
            this.IsEditable = isEditable;
            this.IsAutoPopulate = isAutoPopulate;
            
            //this.SelectedItemId = 0;
            this.SelectedItemString = "";
         
        }
        public VmDynamicField() 
        {
            //this.SelectedItemId = 0;
            this.SelectedItemString = "";
        }
    }


    public class VmDynamicDropDownList : VmDynamicField
    {
        [RequiredIfTrue("IsRequired", ErrorMessage = "Required")]
        public string SelectedItemId { get; set; }

        public List<SelectListItem> FieldValue { get; set; }

        public VmDynamicDropDownList(int controlId, int controleType, string fieldName, bool isRequired, int priority, List<SelectListItem> fieldValue, int categoryHierarchyId=0,string setting="")
            : base(controlId, controleType, fieldName, isRequired, priority)
        {
            this.FieldValue = fieldValue;
            this.CategoryHierarchyId = categoryHierarchyId;
            this.Setting = setting;
        }

        public VmDynamicDropDownList()
            : base()
        {

        }
    }

    public class VmDynamicDropDownListServerSide : VmDynamicField
    {
        [RequiredIfTrue("IsRequired", ErrorMessage = "Required")]
        public string SelectedItemId { get; set; }

        public List<SelectListItem> FieldValue { get; set; }

        public int CategoryTypeId { get; set; }

        public VmDynamicDropDownListServerSide(int controlId, int controleType, int categoryTypeId, string fieldName, bool isRequired, int priority, List<SelectListItem> fieldValue)
            : base(controlId, controleType, fieldName, isRequired, priority)
        {
            this.FieldValue = fieldValue;
            this.CategoryTypeId = categoryTypeId;
        }

        public VmDynamicDropDownListServerSide()
            : base()
        {

        }
    }

    public class VmDynamicTextbox : VmDynamicField
    {

        [RequiredIfTrue("IsRequired", ErrorMessage = "Required")]
        public string FieldValue { get; set; }

        public VmDynamicTextbox(int controlId, int controleType, string fieldName, bool isRequired, int priority/*, string fieldValue*/)
            : base(controlId, controleType, fieldName, isRequired, priority)
        {
            this.FieldValue = null;
            //IsRequired2 = true;
        }

        public VmDynamicTextbox()
            : base()
        {

        }
    }

    public class VmDynamicLabel : VmDynamicField
    {

        [RequiredIfTrue("IsRequired", ErrorMessage = "Required")]
        public string FieldValue { get; set; }

        public VmDynamicLabel(int controlId, int controleType, string fieldName, bool isRequired, int priority, bool isEditable, bool isAutoPopulate/*, string fieldValue*/)
            : base(controlId, controleType, fieldName, isRequired, priority, isEditable, isAutoPopulate)
        {
            this.FieldValue = null;
            //IsRequired2 = true;
        }

        public VmDynamicLabel()
            : base()
        {

        }
    }

    public class VmCheckBox : VmDynamicField
    {
        public int CategoryHierarchyId { get; set; }

       

        public List<VmDynamicCheckBox> FieldValue { get; set; }

        public VmCheckBox(int controlId, int controleType, string fieldName, bool isRequired, int priority, List<VmDynamicCheckBox> fieldValue)
            : base(controlId, controleType, fieldName, isRequired, priority)
        {
            this.FieldValue = fieldValue;
        }
        public VmCheckBox()
            
        {
           
        }
    }

}