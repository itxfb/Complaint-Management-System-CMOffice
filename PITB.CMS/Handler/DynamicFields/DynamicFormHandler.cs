using PITB.CMS.Helper.Database;
using PITB.CMS.Models.DB;
using PITB.CMS.Models.View;
using PITB.CMS.Models.View.Dynamic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PITB.CMS.Models.View.Form;

namespace PITB.CMS.Handler.DynamicFields
{
    public class DynamicFormHandler
    {
        public static List<VmFormControl> GetDynamicFieldsAgainstCampaignId(int campaignId)
        {
            return null;
            //List<DbFormControls> listDbFormControl = DbFormControls.GetByCampaignId(campaignId); 
            ////List<DbDynamicFormControls> listDbDynamicForm = DbDynamicFormControls.GetByCampaignId(campaignId);
            ////List<DbDynamicCategories> listDbDymicCategories = DbDynamicCategories.GetByCampaignId(campaignId);
            //return PopulateDynamicControls(listDbDynamicForm, listDbDymicCategories);
        }

        private static List<VmFormControl> PopulateDynamicControls(List<DbDynamicFormControls> listDbDynamicForm, List<DbDynamicCategories> listDbDymicCategories)
        {
            return null;
            //List<VmDynamicField> listVmDynamicFields = new List<VmDynamicField>();
            //List<SelectListItem> listDynamicCategories = null;
            //VmDynamicField tempVmDynamicFields = null;

            //foreach (DbDynamicFormControls dF in listDbDynamicForm)
            //{
            //    switch ((Config.DynamicControlType)dF.Control_Type)
            //    {
            //        case Config.DynamicControlType.DropDownList:
            //            listDynamicCategories = GetSeletedItemListFromDynamicCategories((int)dF.CategoryTypeId, listDbDymicCategories);
            //            tempVmDynamicFields = new VmDynamicDropDownList(dF.Id, (int)dF.Control_Type, dF.FieldName, (bool)dF.IsRequired, (int)dF.Priority, listDynamicCategories);
            //            listVmDynamicFields.Add(tempVmDynamicFields);
            //            break;

            //        case Config.DynamicControlType.TextBox:
            //            tempVmDynamicFields = new VmDynamicTextbox(dF.Id, (int)dF.Control_Type, dF.FieldName, (bool)dF.IsRequired, (int)dF.Priority/*, ""*/);
            //            listVmDynamicFields.Add(tempVmDynamicFields);
            //            break;

            //        case Config.DynamicControlType.Label:
            //            tempVmDynamicFields = new VmDynamicLabel(dF.Id, (int)dF.Control_Type, dF.FieldName, (bool)dF.IsRequired, (int)dF.Priority, Convert.ToBoolean(dF.IsEditable), Convert.ToBoolean(dF.IsAutoPopulate));
            //            listVmDynamicFields.Add(tempVmDynamicFields);
            //            break;

            //        case Config.DynamicControlType.DropDownListServerSideSearchable:
            //            //listDynamicCategories = GetSeletedItemListFromDynamicCategories((int)dF.CategoryTypeId, listDbDymicCategories);
            //            tempVmDynamicFields = new VmDynamicDropDownListServerSide(dF.Id, (int)dF.Control_Type, (int)dF.CategoryTypeId, dF.FieldName, (bool)dF.IsRequired, (int)dF.Priority, new List<SelectListItem>());
            //            listVmDynamicFields.Add(tempVmDynamicFields);
            //            break;
            //    }
            //}
            //return listVmDynamicFields;
        }
    }
}