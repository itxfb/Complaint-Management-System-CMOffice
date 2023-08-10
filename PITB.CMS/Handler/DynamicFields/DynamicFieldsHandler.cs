using PITB.CMS.Helper;
using PITB.CMS.Helper.Database;
using PITB.CMS.Models.Custom.CustomForm;
using PITB.CMS.Models.DB;
using PITB.CMS.Models.View;
using PITB.CMS.Models.View.Dynamic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PITB.CMS.Handler.DynamicFields
{
    public class DynamicFieldsHandler
    {
        public static List<VmDynamicField> GetDynamicFieldsAgainstCampaignId(int campaignId)
        {
            List<DbDynamicFormControls> listDbDynamicForm = DbDynamicFormControls.GetBy(campaignId,null);
            List<DbDynamicCategories> listDbDymicCategories = DbDynamicCategories.GetBy(campaignId,null);
            return PopulateDynamicControls(listDbDynamicForm, listDbDymicCategories);
        }

        private static List<VmDynamicField> PopulateDynamicControls(List<DbDynamicFormControls> listDbDynamicForm, List<DbDynamicCategories> listDbDymicCategories)
        {
            List<VmDynamicField> listVmDynamicFields = new List<VmDynamicField>();
            List<SelectListItem> listDynamicCategories = null;
            VmDynamicField tempVmDynamicFields = null;

            foreach(DbDynamicFormControls dF in listDbDynamicForm)
            {
                switch ((Config.DynamicControlType)dF.Control_Type)
                {
                    case Config.DynamicControlType.DropDownList:
                        listDynamicCategories = GetSeletedItemListFromDynamicCategories((int)dF.CategoryTypeId, listDbDymicCategories);
                        tempVmDynamicFields = new VmDynamicDropDownList(dF.Id, (int)dF.Control_Type, dF.FieldName, (bool)dF.IsRequired, (int)dF.Priority, listDynamicCategories);
                        listVmDynamicFields.Add(tempVmDynamicFields);
                    break;
                    
                    case Config.DynamicControlType.TextBox:
                        tempVmDynamicFields = new VmDynamicTextbox(dF.Id, (int)dF.Control_Type, dF.FieldName, (bool)dF.IsRequired, (int)dF.Priority/*, ""*/);
                        listVmDynamicFields.Add(tempVmDynamicFields);
                    break;

                    case Config.DynamicControlType.Label:
                        tempVmDynamicFields = new VmDynamicLabel(dF.Id, (int)dF.Control_Type, dF.FieldName, (bool)dF.IsRequired, (int)dF.Priority, Convert.ToBoolean(dF.IsEditable), Convert.ToBoolean(dF.IsAutoPopulate));
                        listVmDynamicFields.Add(tempVmDynamicFields);
                    break;

                    case Config.DynamicControlType.DropDownListServerSideSearchable:
                        //listDynamicCategories = GetSeletedItemListFromDynamicCategories((int)dF.CategoryTypeId, listDbDymicCategories);
                        tempVmDynamicFields = new VmDynamicDropDownListServerSide(dF.Id, (int)dF.Control_Type, (int)dF.CategoryTypeId, dF.FieldName, (bool)dF.IsRequired, (int)dF.Priority, new List<SelectListItem>());
                        listVmDynamicFields.Add(tempVmDynamicFields);
                    break;
                }
            }
            return listVmDynamicFields;
        }

        private static List<SelectListItem> GetSeletedItemListFromDynamicCategories( int categoryTypeId, List<DbDynamicCategories> listDynamicCategories)
        {
            return listDynamicCategories.Where(n => n.CategoryTypeId == categoryTypeId).OrderBy(n=>n.Name)
                .Select(n => new SelectListItem() { Value = n.Id.ToString() + Config.Separator + n.Name, Text = n.Name }).ToList();
        }

        public static void SaveDyamicFieldsInDb(CustomForm.Post form, string currTab, int complaintId)
        {
            DBContextHelperLinq db = new DBContextHelperLinq();
            DbDynamicComplaintFields dbDf = null;
            Dictionary<string,string> dictConfig = null;
            //List<CustomForm.ElementData> listDynamicField = form.ListElementData.Where(n => n.DictConfiguration.ContainsKey("isDynamic") == "true").ToList();
            string[] tempStrArr = null;
            foreach (CustomForm.ElementData dynamicField in form.ListElementData)
            {
                dictConfig = dynamicField.DictConfiguration;
                if (dictConfig.ContainsKey("isDynamic") && dictConfig["isDynamic"] == "true")
                {
                    if (dictConfig["controlType"] == "text")
                    {
                        dbDf = new DbDynamicComplaintFields();
                        dbDf.ComplaintId = complaintId;
                        dbDf.SaveTypeId = Convert.ToInt32(Config.DynamicFieldType.SingleText);
                        dbDf.CategoryHierarchyId = Convert.ToInt32(Config.CategoryHierarchyType.None);
                        dbDf.CategoryTypeId = -1;
                        dbDf.ControlId = dictConfig["controlId"].CastObj<int?>();
                        dbDf.FieldName = DbDynamicFormControls.GetByControlId((int) dbDf.ControlId).FieldName;
                        if (dynamicField.Value != null)
                        {
                            dbDf.FieldValue = dynamicField.Value.Trim();
                        }
                        db.DbDynamicComplaintFields.Add(dbDf);
                    }
                    else if (dictConfig["controlType"] == "dropDown" || dictConfig["controlType"] == "radio")
                    {
                        dbDf = new DbDynamicComplaintFields();
                        dbDf.ComplaintId = complaintId;
                        dbDf.SaveTypeId = Convert.ToInt32(Config.DynamicFieldType.MultiSelection);
                        dbDf.CategoryHierarchyId = Convert.ToInt32(Config.CategoryHierarchyType.MainCategory);

                        dbDf.ControlId = dictConfig["controlId"].CastObj<int?>();
                        dbDf.FieldName = DbDynamicFormControls.GetByControlId((int) dbDf.ControlId).FieldName;
                        if (dynamicField.Value != null)
                        {
                            tempStrArr = dynamicField.Value.Split(new string[] {Config.Separator},
                                StringSplitOptions.None);
                            dbDf.CategoryTypeId = Convert.ToInt32(tempStrArr[0]);
                            dbDf.FieldValue = tempStrArr[1].ToString().Trim();
                        }
                        else
                        {
                            dbDf.CategoryTypeId = -1;
                            dbDf.FieldValue = null;
                        }
                        db.DbDynamicComplaintFields.Add(dbDf);
                    }
                }
            }
            db.SaveChanges();
        }

        public static void SaveDyamicFieldsInDb(VmComplaintBase vmComplaint, int complaintId)
        {
            DBContextHelperLinq db = new DBContextHelperLinq();
            DbDynamicComplaintFields dbDf = null;

            //for TextBox
            if (vmComplaint.ListDynamicTextBox != null && vmComplaint.ListDynamicTextBox.Count > 0)
            {
                foreach (VmDynamicTextbox vmTxtBox in vmComplaint.ListDynamicTextBox)
                {
                    dbDf = new DbDynamicComplaintFields();
                    dbDf.ComplaintId = complaintId;
                    dbDf.SaveTypeId = Convert.ToInt32(Config.DynamicFieldType.SingleText);
                    dbDf.CategoryHierarchyId = Convert.ToInt32(Config.CategoryHierarchyType.None);
                    dbDf.CategoryTypeId = -1;
                    dbDf.ControlId = vmTxtBox.ControlId;
                    dbDf.FieldName = vmTxtBox.FieldName;
                    if (vmTxtBox.FieldValue != null)
                    {
                        dbDf.FieldValue = vmTxtBox.FieldValue.Trim();
                    }
                    db.DbDynamicComplaintFields.Add(dbDf);
                }
            }

            //for TextBox
            if (vmComplaint.ListDynamicLabel != null && vmComplaint.ListDynamicLabel.Count > 0)
            {
                foreach (VmDynamicLabel vmLabel in vmComplaint.ListDynamicLabel)
                {
                    dbDf = new DbDynamicComplaintFields();
                    dbDf.ComplaintId = complaintId;
                    dbDf.SaveTypeId = Convert.ToInt32(Config.DynamicFieldType.SingleText);
                    dbDf.CategoryHierarchyId = Convert.ToInt32(Config.CategoryHierarchyType.None);
                    dbDf.CategoryTypeId = -1;
                    dbDf.ControlId = vmLabel.ControlId;
                    dbDf.FieldName = vmLabel.FieldName;
                    if (vmLabel.FieldValue != null)
                    {
                        dbDf.FieldValue = vmLabel.FieldValue.Trim();
                    }
                    db.DbDynamicComplaintFields.Add(dbDf);
                }
            }

            //for Dropdown List
            if (vmComplaint.ListDynamicDropDown != null && vmComplaint.ListDynamicDropDown.Count > 0)
            {
                int selectedId = 0;
                string selectedValue = null;
                string[] tempStrArr = null;

                foreach (VmDynamicDropDownList vmDropDownList in vmComplaint.ListDynamicDropDown)
                {
                    dbDf = new DbDynamicComplaintFields();
                    dbDf.ComplaintId = complaintId;
                    dbDf.SaveTypeId = Convert.ToInt32(Config.DynamicFieldType.MultiSelection);
                    dbDf.CategoryHierarchyId = Convert.ToInt32(Config.CategoryHierarchyType.MainCategory);
                    
                    dbDf.ControlId = vmDropDownList.ControlId;
                    dbDf.FieldName = vmDropDownList.FieldName;
                    if (vmDropDownList.SelectedItemId != null)
                    {
                        tempStrArr = vmDropDownList.SelectedItemId.Split(new string[] {Config.Separator},
                            StringSplitOptions.None);
                        dbDf.CategoryTypeId = Convert.ToInt32(tempStrArr[0]);
                        dbDf.FieldValue = tempStrArr[1].ToString().Trim();
                    }
                    else
                    {
                        dbDf.CategoryTypeId = -1;
                        dbDf.FieldValue = null;
                    }
                    db.DbDynamicComplaintFields.Add(dbDf);
                }
            }


            //for Dropdown ServerSide List
            if (vmComplaint.ListDynamicDropDownServerSide != null && vmComplaint.ListDynamicDropDownServerSide.Count > 0)
            {
                int selectedId = 0;
                string selectedValue = null;
                string[] tempStrArr = null;

                foreach (VmDynamicDropDownListServerSide vmDropDownListServerSide in vmComplaint.ListDynamicDropDownServerSide)
                {
                    dbDf = new DbDynamicComplaintFields();
                    dbDf.ComplaintId = complaintId;
                    dbDf.SaveTypeId = Convert.ToInt32(Config.DynamicFieldType.MultiSelection);
                    dbDf.CategoryHierarchyId = Convert.ToInt32(Config.CategoryHierarchyType.MainCategory);

                    dbDf.ControlId = vmDropDownListServerSide.ControlId;
                    dbDf.FieldName = vmDropDownListServerSide.FieldName;
                    if (vmDropDownListServerSide.SelectedItemId != null)
                    {
                        tempStrArr = vmDropDownListServerSide.SelectedItemId.Split(new string[] { Config.Separator },
                            StringSplitOptions.None);
                        dbDf.CategoryTypeId = Convert.ToInt32(tempStrArr[0]);
                        dbDf.FieldValue = tempStrArr[1].ToString().Trim();
                    }
                    else
                    {
                        dbDf.CategoryTypeId = -1;
                        dbDf.FieldValue = null;
                    }
                    db.DbDynamicComplaintFields.Add(dbDf);
                }
            }


            db.SaveChanges();
        }

    }
}