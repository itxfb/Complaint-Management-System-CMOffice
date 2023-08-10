using PITB.CMS_Common;
using PITB.CMS_Common.Models;
using PITB.CMS_Common.Models.Custom.WindowService;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;


namespace PITB.CMS_Common
{
    public class HelperFunctions
    {


        //public static bool CustomValidator<T>(T obj)
        //{
        //    string text = string.Empty;
        //    var context = new ValidationContext(obj, serviceProvider: null, items: null);
        //    var results = new List<ValidationResult>();

        //    var isValid = Validator.TryValidateObject(obj, context, results);

        //    if (!isValid)
        //    {
        //        foreach (var validationResult in results)
        //        {
        //            Console.WriteLine(validationResult.ErrorMessage);
        //        }
        //    }

        //    return isValid;
        //}

        //public bool TryValidate(object @object, out ICollection<ValidationResult> results)
        //{
        //    var context = new ValidationContext(@object, serviceProvider: null, items: null);
        //    results = new List<ValidationResult>();
        //    return Validator.TryValidateObject(
        //        @object, context, results,
        //        validateAllProperties: true
        //    );
        //}


        public static bool Validator<T>(T obj)
        {
            bool isValid = true;
            var propertyInfos = obj.GetType().GetProperties(BindingFlags.FlattenHierarchy | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
            foreach (var item in propertyInfos)
            {
                object[] attributes = item.GetCustomAttributes(true);
                TypeConverter typeConverter = TypeDescriptor.GetConverter(typeof(ValidationAttribute));
                foreach (object attr in attributes)
                {
                    ValidationAttribute attrib = attr as ValidationAttribute;

                    if (attrib != null)
                    {
                        try
                        {
                            isValid = attrib.IsValid(item.GetValue(obj));
                            attrib.Validate(item.GetValue(obj), item.Name);
                            if (!isValid)
                                return isValid;
                        }
                        catch (Exception ex)
                        {
                            return isValid = false;
                        }
                    }
                }
            }
            return isValid;
        }
        public static List<int> GetUniqueCampaignIds(List<DbPermissionsAssignment> listPermission)
        {
            List<int> listUniqueCampIds = new List<int>();
            foreach (DbPermissionsAssignment dbPermissionsAssignment in listPermission)
            {
                listUniqueCampIds = listUniqueCampIds.Union(Utility.GetIntList(dbPermissionsAssignment.Permission_Value)).ToList();
            }
            return listUniqueCampIds;
        }

        public static List<StatusWiseComplaintCount> GetStatusWiseComplaintCount(List<ComplaintPartial> listComplaintPartial, List<int> listStatuses)
        {
            List<StatusWiseComplaintCount> listStatusWiseComplaint = new List<StatusWiseComplaintCount>();
            StatusWiseComplaintCount statusWiseComplaintCount = new StatusWiseComplaintCount();

            foreach (int statusId in listStatuses)
            {
                statusWiseComplaintCount = new StatusWiseComplaintCount
                {
                    StatusId = statusId,
                    StatusName = Config.StatusDict[statusId],
                    ComplaintCount = 0
                };

                statusWiseComplaintCount.ComplaintCount = listComplaintPartial.Where(n => n.StatusId == statusId).Count();

                listStatusWiseComplaint.Add(statusWiseComplaintCount);
            }

            return listStatusWiseComplaint;
        }
    }
}
