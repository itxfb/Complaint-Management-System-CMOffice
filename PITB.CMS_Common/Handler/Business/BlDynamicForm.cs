using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
//using BridgeDTO.Form.DynamicForm;
using PITB.CMS_Common.Models;
namespace PITB.CMS_Common.Handler.Business
{
    public class BlDynamicForm
    {
        //public static DynamicForm GetDynamicForm(string paramsStr)
        //{
        //    Dictionary<string,string> dict = Utility.ConvertCollonFormatToDict(paramsStr);
        //    List<DbFormControl> listDbForm = DbFormControl.GetBy(-1, dict["FormTag"]);
        //    DynamicForm df = CreateDynamicFormWithDbFields(listDbForm);
        //    return df;
        //    //return null;
        //}

        //private static DynamicForm CreateDynamicFormWithDbFields(List<DbFormControl> listDbForm)
        //{
        //    DynamicForm dynamicForm = new DynamicForm();
        //    FormField ff = null;
        //    DbFormControl dbFC = null;
        //    bool isFieldPresent = false;
        //    for (int i = 0; i < listDbForm.Count; i++)
        //    {
        //        dbFC = listDbForm[i];
        //        isFieldPresent = false;
        //        if (dbFC.Control_Type == (int) FormFieldType.TextField)
        //        {
        //            TextFormField textFormField = new TextFormField()
        //            {
        //                Id = "TxtFormField__"+dbFC.Id,
        //                Label = dbFC.Field_Name
        //            };
        //            ff = textFormField;
        //            ff.TagId = dbFC.Tag_Id;
        //            isFieldPresent = true;
        //        }
        //        else if (dbFC.Control_Type == (int)FormFieldType.OptionList)
        //        {
        //            OptionsFormField optionsFormField = new OptionsFormField()
        //            {
        //                Id = "OptionsFormField__"+dbFC.Id,
        //                Label = dbFC.Field_Name,
        //                Options = new List<Option>()
        //            };
        //            ff = optionsFormField;
        //            ff.TagId = dbFC.Tag_Id;
        //            isFieldPresent = true;
        //        }
        //        else if (dbFC.Control_Type == (int)FormFieldType.ButtonSubmit)
        //        {
        //            SubmitBtn submitFormField = new SubmitBtn()
        //            {
        //                Id = "SubmitButton__" + dbFC.Id,
        //                Label = dbFC.Field_Name
        //            };
        //            ff = submitFormField;
        //            ff.TagId = dbFC.Tag_Id;
        //            isFieldPresent = true;
        //        }
        //        else if (dbFC.Control_Type == (int)FormFieldType.Checkbox)
        //        {
        //            CheckboxFormField checkboxFormField = new CheckboxFormField()
        //            {
        //                Id = "CheckboxFormField__" + dbFC.Id,
        //                Label = dbFC.Field_Name
        //            };
        //            ff = checkboxFormField;
        //            ff.TagId = dbFC.Tag_Id;
        //            isFieldPresent = true;
        //        }
        //        if (isFieldPresent)
        //        {
        //            ff.ListPermissions = dbFC.ListDbFormPermission.Select(n => new FormPermission()
        //            {
        //                Key = n.Permission_Id,
        //                Value = n.Permission_Value
        //            }).ToList();
        //            dynamicForm.Fields.Add(ff);
        //        }
        //        SetInitalValues(ff);

        //    }
        //    return dynamicForm;
        //}

        //private static void SetInitalValues(FormField ff)
        //{
        //    if (ff.Kind == FormFieldType.OptionList)
        //    {
        //        string initalApiParam = ff.GetStrPermissionValue("Initialization_Api_Params");
        //        if (!string.IsNullOrEmpty(initalApiParam))
        //        {
        //            OptionsFormField opF = (OptionsFormField)ff;
        //            Dictionary<string,string> dict = Utility.ConvertCollonFormatToDict(initalApiParam);
        //            List<DbCategoryMapping> listDbCategory = DbCategoryMapping.GetBy(Convert.ToInt32(dict["Campaign_Id"]), dict["Tag_Id"],
        //                Convert.ToInt32(dict["Parent_Id"]));
        //            opF.Options = DbCategoryMapping.GetListOption(listDbCategory);
        //        }
        //    }
        //}
    }
}