
using PITB.CMS_Common;
using PITB.CMS_Models.DB;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;

namespace PITB.CMS_Models.Custom
{
    public class ConfigCatWiseDynamicForm
    {
        public Dictionary<Category, List<FormField>> DictConfig;

        //public List<DbDynamicComplaintFields> ListDynamicComplaintFields;
        public ConfigCatWiseDynamicForm()
        {
            DictConfig = new Dictionary<Category, List<FormField>>();
        }

        public class Category
        {
            public string Name { get; set; }

            public string Value { get; set; }
        }

        public class FormField
        {
            public Config.DynamicControlType Type { get; set; }
            public int ControlId { get; set; }
            public string Name { get; set; }
            public string Value { get; set; }
            public bool IsRequired { get; set; }
            

        }

        public bool IsFormValid(Dictionary<string, string> formCollectionDict)
        {
            //, Dictionary<Category, List<FormField>> dictConfig
            List<Category> listCat = new List<Category>();
            

            for (int i = 0; i < DictConfig.Count; i++)
            {
                KeyValuePair<Category, List<FormField>> keyVal = DictConfig.ElementAt(i);
                Category cat = keyVal.Key;
                string value = formCollectionDict[cat.Name];
                if (value.Equals(cat.Value)) //  if value exists check if form is valid
                {
                    List<FormField> listFormField = keyVal.Value;
                    for (int j = 0; j < listFormField.Count; j++)
                    {
                        FormField formField = listFormField[j];
                        value = formCollectionDict[formField.Name];

                        if (formField.IsRequired && string.IsNullOrEmpty(value))
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        public List<DbDynamicComplaintFields> GetDbDynamicComplaintFields(Dictionary<string, string> dictFormCollection, int complaintId)
        {
            //, Dictionary<Category, List<FormField>> dictConfig
            

            List<Category> listCat = new List<Category>();
            List<DbDynamicComplaintFields> listDynamicComplaintFields = new List<DbDynamicComplaintFields>();
            DbDynamicComplaintFields dbDf = null;

            for (int i = 0; i < DictConfig.Count; i++)
            {
                KeyValuePair<Category, List<FormField>> keyVal = DictConfig.ElementAt(i);
                Category cat = keyVal.Key;
                string value = dictFormCollection[cat.Name];
                if (value.Equals(cat.Value)) //  if value exists check if form is valid
                {
                    List<FormField> listFormField = keyVal.Value;

                    // List Form 
                    List<DbDynamicFormControls> listDynamicFormControls = DbDynamicFormControls.GetByControlIds(listFormField.Select(n=>n.ControlId).ToList());
                    // end 

                    DbDynamicFormControls dbDynamicControl = null;

                    for (int j = 0; j < listFormField.Count; j++)
                    {
                        FormField formField = listFormField[j];
                        value = dictFormCollection[formField.Name];

                        dbDynamicControl = listDynamicFormControls.Where(n => n.Id == formField.ControlId).FirstOrDefault();

                        
                        // Adding form Fields into Db Model
                        if (formField.Type == Config.DynamicControlType.TextBox)
                        {
                            dbDf = new DbDynamicComplaintFields();
                            dbDf.ComplaintId = complaintId;
                            dbDf.SaveTypeId = Convert.ToInt32(Config.DynamicFieldType.SingleText);
                            dbDf.CategoryHierarchyId = Convert.ToInt32(Config.CategoryHierarchyType.None);
                            dbDf.CategoryTypeId = -1;
                            dbDf.ControlId = formField.ControlId;
                            dbDf.FieldName = dbDynamicControl.FieldName;
                            //dbDf.FieldName = formField.Name;
                            if (!string.IsNullOrEmpty(value))
                            {
                                dbDf.FieldValue = value.Trim();
                            }
                        }
                        else if (formField.Type == Config.DynamicControlType.DropDownList)
                        {
                            dbDf = new DbDynamicComplaintFields();
                            dbDf.ComplaintId = complaintId;
                            dbDf.SaveTypeId = Convert.ToInt32(Config.DynamicFieldType.MultiSelection);
                            dbDf.CategoryHierarchyId = Convert.ToInt32(Config.CategoryHierarchyType.MainCategory);

                            dbDf.ControlId = formField.ControlId;
                            dbDf.FieldName = dbDynamicControl.FieldName;
                            //dbDf.FieldName = formField.Name;
                            if (!string.IsNullOrEmpty(value)|| !value.Equals("-1"))
                            {
                               string[] tempStrArr = value.Split(new string[] { Config.Separator },
                                    StringSplitOptions.None);
                                dbDf.CategoryTypeId = Convert.ToInt32(tempStrArr[0]);
                                dbDf.FieldValue = tempStrArr[1].ToString().Trim();
                            }
                            else
                            {
                                dbDf.CategoryTypeId = -1;
                                dbDf.FieldValue = null;
                            }
                        }
                        listDynamicComplaintFields.Add(dbDf);
                        // End Adding Form Fields into Db Model

                        //if (formField.IsRequired && string.IsNullOrEmpty(value))
                        //{
                        //    return false;
                        //}
                    }
                }
            }
            return listDynamicComplaintFields;
        }
    }
}