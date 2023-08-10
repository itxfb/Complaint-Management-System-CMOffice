using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace PITB.CMS_Common.Helper
{
    public static class ValidatorChecker<TModel>
    {
        public static bool IsModelStateValid(ModelStateDictionary modelState)
        {
            if (modelState.IsValid)
            {
                return true;
            }

            int totalErrors = 0, requiredAttributeErrors = 0;
            Type modelType = typeof(TModel);
            foreach (var key in modelState.Keys)
            {
                if (modelState[key].Errors.Count > 0)
                {
                    totalErrors += modelState[key].Errors.Count;

                    Type currentType = modelType;
                    string[] typeParts = key.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
                    int typeIndex = 0;

                    if (typeParts.Length == 0)
                    {
                        continue;
                    }
                    else if (typeParts.Length > 1)
                    {
                        for (typeIndex = 0; typeIndex < typeParts.Length - 1; typeIndex++)
                        {
                            currentType = currentType.GetProperty(typeParts[typeIndex]).PropertyType;
                        }
                    }

                    PropertyInfo validatedProperty = currentType.GetProperty(typeParts[typeIndex]);

                    var requiredValidators = validatedProperty.GetCustomAttributes(typeof(RequiredAttribute), true);
                    requiredAttributeErrors += requiredValidators.Length;
                }
            }

            return requiredAttributeErrors == totalErrors;
        }
    }
}
