using System;
using System.ComponentModel.DataAnnotations;
using PITB.CMS.Models.View;
namespace PITB.CMS.Helper.Validation
{
    public class CustomStringLengthAttribute: ValidationAttribute
    {
        public int Length { get; private set; } 
        public CustomStringLengthAttribute(int length)
        {
            Length = length;
        }
        public CustomStringLengthAttribute()
        {
            Length = 0;
        }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            
            string comments = value.ToString();
            var vmObject = validationContext.ObjectInstance;
            Type vmType = validationContext.ObjectType;
            if (vmType.Equals(typeof(VmStatusChange)))
            {
                VmStatusChange obj = (VmStatusChange)vmObject;
                if(obj.Compaign_Id == (int)Config.Campaign.DcoOffice)
                {
                    if(comments.Length < Length)
                    {
                        return new ValidationResult(string.Format("Comments must be at least {0} characters.",Length));
                    }
                    else
                    {
                        return ValidationResult.Success;
                    }
                }
                else
                {
                    return ValidationResult.Success;
                }
            }
            else
            {
                return new ValidationResult("Type error.");
            }            
        }
    }
}