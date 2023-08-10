using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bridge;

namespace BridgeClassLib.Form
{
    [FileName("FormBuilder.js")]
    public class FormConfig
    {
        public const string Att_Prefix = "data-xeeshi-";
        public const string Seperator = "__";

        public const string Element_Div = "div";
        public const string Element_Input = "input";
        public const string Element_Select = "select";
        public const string Element_Span = "span";


        // Span Container
        public const string Jq_Data_HtmlSpanError = Att_Prefix + "jq-data-html-span-error";
        public const string Jq_Data_HtmlSpanErrorContainer = Att_Prefix + "jq-data-html-span-error-container";
        public const string Jq_Data_IsValid = Att_Prefix + "jq-data-IsValid";
        public const string Jq_Data_Attributes = Att_Prefix + "jq-data-Attributes";

        // Values
        public const string Val_True = "True";
        public const string Val_False = "False";
        public const string Val_Validation_Input_Case_Upper = "Upper";
        public const string Val_Validation_Input_Case_Lower = "Lower";
        public const string Val_Validation_Input_Numeric = "Numeric";
        

        //Attribute
        public const string Att_FormTag = Att_Prefix + "form-tag";
        
        public const string Att_Validation_IsRequired = Att_Prefix + "validation-is-required";
        public const string Att_Validation_IsRequired_Message = Att_Prefix + "validation-is-required_message";
        public const string Att_Validation_Span_Class_Error = Att_Prefix + "validation-span-class-error";
        public const string Att_Validation_Span_Class_Valid = Att_Prefix + "validation-Span-Class-Valid";

        // Characters validation
        public const string Att_Validation_Input_Max_Char = Att_Prefix + "validation-input-max-char";
        public const string Att_Validation_Input_Min_Char = Att_Prefix + "validation-input-min-char";
        public const string Att_Validation_Input_Min_Char_message = Att_Prefix + "validation-input-min-char-message";

        public const string Att_Input_Case = Att_Prefix + "input-case";
        public const string Att_Input_Format = Att_Prefix + "input-format";

        //Attribute for Span
        public const string Att_Validation_For_Id = Att_Prefix + "validation-for-id";

    }
}

