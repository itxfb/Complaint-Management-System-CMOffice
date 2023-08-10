using System;
using System.Collections.Generic;
using System.Linq;
using Bridge;
//using BridgeDTO.Form.Attribute;

namespace BridgeDTO.Form.DynamicForm
{
    [FileName("Form.js")]
    public class DynamicForm
    {
        public List<FormField> Fields { get; set; }

        public DynamicForm()
        {
            this.Fields = new List<FormField>();
        }

         public DynamicForm(object dynamicForm)
            : this()
        {
            
            dynamic obj = dynamicForm;
            
            dynamic rawFields = obj["Fields"];
            this.Fields = new List<FormField>();
            int count = 0;
            foreach (var rawField in rawFields)
            {
                
                if (rawField["Kind"] == (int)FormFieldType.RadioList)
                {
                    RadioFormField radioField = new RadioFormField()
                    {
                        Id = rawField["Id"],
                        Label = rawField["Label"],
                        TagId = rawField["TagId"]
                    };
                    List<Option> listOption = rawField["Options"];
                    
                   radioField.Options = new List<Option>();
                    foreach (var option in listOption)
                    {
                        radioField.Options.Add(new Option()
                        {
                            Value = option.Value,
                            Text = option.Text
                        });
                    }
                    this.Fields.Add(radioField);
                   
                }
                else if (rawField["Kind"] == (int)FormFieldType.OptionList)
                {
                    OptionsFormField optionField = new OptionsFormField()
                    {
                        Id = rawField["Id"],
                        Label = rawField["Label"],
                        TagId = rawField["TagId"]
                    };
                    List<Option> listOption = rawField["Options"];
                    optionField.Options = new List<Option>();

                    
                    foreach (var option in listOption)
                    {
                        optionField.Options.Add(new Option()
                        {
                            Value = option.Value,
                            Text = option.Text
                        });
                    }
                    
                    this.Fields.Add(optionField);

                }
                else if (rawField["Kind"] == (int)FormFieldType.TextField)
                {
                    this.Fields.Add(new TextFormField()
                    {
                        Id = rawField["Id"],
                        Label = rawField["Label"],
                        TagId = rawField["TagId"]
                        //Required = rawField["Required"]
                    });
                }
                else if (rawField["Kind"] == (int)FormFieldType.Checkbox)
                {
                    this.Fields.Add(new CheckboxFormField()
                    {
                        Id = rawField["Id"],
                        Label = rawField["Label"],
                        TagId = rawField["TagId"]//,
                        //Required = rawField["Required"]
                    });
                }
                else if (rawField["Kind"] == (int)FormFieldType.ButtonSubmit)
                {
                    this.Fields.Add(new SubmitBtn()
                    {
                        Id = rawField["Id"],
                        Label = rawField["Label"],
                        TagId = rawField["TagId"]//,
                        //Required = rawField["Required"]
                    });
                }
                List<FormPermission> listPermission = (rawField["ListPermissions"] == null) ? new List<FormPermission>() : rawField["ListPermissions"];
                this.Fields[count].ListPermissions = listPermission;
                count++;
            }
        }




        public DynamicForm(DynamicForm dynamicForm)
            : this()
        {
            // start
            /*
            foreach (FormField ff in dynamicForm.Fields)
            {
                if (ff.Kind == FormFieldType.RadioList)
                {
                    RadioFormField rf = (RadioFormField)ff;
                    this.Fields.Add(new RadioFormField()
                    {
                        Id = ff.Id,
                        Label = ff.Label,
                        Options = (List<Option>)rf.Options
                    });
                }
                else if (ff.Kind == FormFieldType.TextField)
                {
                    TextFormField tf = (TextFormField)ff;
                    this.Fields.Add(new TextFormField()
                    {
                        Id = tf.Id,
                        Label = tf.Label,
                        Required = tf.Required
                    });
                }
            }
            // end
             */ 
            dynamic rawFields = dynamicForm.Fields;
            this.Fields = new List<FormField>();
            int count = 0;
            foreach (var rawField in rawFields)
            {
                
                if (rawField["Kind"] == (int)FormFieldType.RadioList)
                {
                    RadioFormField radioField = new RadioFormField()
                    {
                        Id = rawField["Id"],
                        Label = rawField["Label"],
                       
                    };
                    List<Option> listOption = rawField["Options"];
                    
                   radioField.Options = new List<Option>();
                    foreach (var option in listOption)
                    {
                        radioField.Options.Add(new Option()
                        {
                            Value = option.Value,
                            Text = option.Text
                        });
                    }
                    this.Fields.Add(radioField);
                   
                }
                else if (rawField["Kind"] == (int)FormFieldType.OptionList)
                {
                    OptionsFormField optionField = new OptionsFormField()
                    {
                        Id = rawField["Id"],
                        Label = rawField["Label"],

                    };
                    List<Option> listOption = rawField["Options"];

                    optionField.Options = new List<Option>();
                    foreach (var option in listOption)
                    {
                        optionField.Options.Add(new Option()
                        {
                            Value = option.Value,
                            Text = option.Text
                        });
                    }
                    this.Fields.Add(optionField);

                }
                else if (rawField["Kind"] == (int)FormFieldType.TextField)
                {
                    this.Fields.Add(new TextFormField()
                    {
                        Id = rawField["Id"],
                        Label = rawField["Label"],
                        Required = rawField["Required"]
                    });
                }
                List<FormPermission> listPermission = (rawField["ListPermission"] == null) ? new List<FormPermission>() : rawField["ListPermission"];
                this.Fields[count].ListPermissions = listPermission;
                count++;
            }
        }
    }
    //[Module("Form")]
    [FileName("Form.js")]
    public class FormField
    {
        public FormFieldType Kind { get; set; }
        public string Id { get; set; }
        public string Label { get; set; }

        public string TagId { get; set; }

        public string PlaceHolder { get; set; }
        public List<FormPermission> ListPermissions { get; set; }

        public bool IsPermissionPresent(string str)
        {
            return (ListPermissions.FirstOrDefault(n => n.Key.Equals(str)) != null);
        }

        public int GetPermissionValueInt(string str)
        {
            FormPermission fp = ListPermissions.FirstOrDefault(n => n.Key.Equals(str));
            if (fp != null)
            {
                return Convert.ToInt32(fp.Value);
            }
            return -1;
        }

        public string GetStrPermissionValue(string str)
        {
            FormPermission fp = ListPermissions.FirstOrDefault(n => n.Key.Equals(str));
            return (fp != null ? fp.Value : "" );
        }

        public bool GetBoolPermissionValue(string str)
        {
            FormPermission fp = ListPermissions.FirstOrDefault(n => n.Key.Equals(str));
            if (fp != null)
            {
                return Convert.ToBoolean(fp.Value);
            }
            return false;
        }

        public bool IsPermissionSubStringPresent(string str)
        {
            return (ListPermissions.FirstOrDefault(n => n.Key.Contains(str)) != null);
        }
    }
    [FileName("Form.js")]
    public class FormPermission
    {
        public string Key { get; set; }

        public string Value { get; set; }
    }
    [FileName("Form.js")]
    public class ValidateMode
    {
        public string ValidationName { get; set; }
        public string ValidationErrorMessage { get; set; }
    }
    [FileName("Form.js")]
    //[Module("Form")]
    public enum FormFieldType
    {
        TextField = 1,
        TextArea = 2,
        
        RadioList = 3,
        
        OptionList = 4,
        MultiselectOptionList = 5,
        
        Checkbox = 6,

        Calender = 7,
        ButtonSubmit = 8
    }
    //[Module("Form")]
    [FileName("Form.js")]
    public class TextFormField : FormField
    {
        public bool Required { get; set; }

         public TextFormField()
        {
            this.Kind = FormFieldType.TextField;
            this.Required = false;
        }
    }
    [FileName("Form.js")]
    public class Option
    {
        public string Value { get; set; }

        public string Text { get; set; }
    }

    [FileName("Form.js")]
    public class SubmitBtn : FormField
    {
        //public  List<string> ListTagsToSubmit { get; set; }
        public SubmitBtn()
        {
            this.Kind = FormFieldType.ButtonSubmit;
        }
    }

    [FileName("Form.js")]
    public class CheckboxFormField : FormField
    {
        public bool Required { get; set; }
        //
        public CheckboxFormField()
        {
            this.Kind = FormFieldType.Checkbox;
            this.Required = false;
        }
    }
   
    [FileName("Form.js")]
    public class OptionsFormField : FormField
    {
        public List<Option> Options { get; set; }

        public OptionsFormField()
        {
            this.Kind = FormFieldType.OptionList;
        }
    }
    [FileName("Form.js")]
    public class RadioFormField : FormField
    {
        public List<Option> Options { get; set; }

        public RadioFormField()
        {
            this.Kind = FormFieldType.RadioList;
        }
    }
}
