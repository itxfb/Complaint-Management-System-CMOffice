using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using System.Web.WebPages.Html;
using System.Web.Mvc;

namespace PITB.CMS_Common
{
    public class UiUtility
    {
        public static List<SelectListItem> GetSelectList(IEnumerable<object> list, string keyName, string valueName, string firstValue = null, object selectedValue = null)
        {
            List<SelectListItem> listSelect = new List<SelectListItem>();
            SelectListItem slItem = null;
            if (list != null && list.Count() > 0)
            {
                foreach (object o in list)
                {
                    slItem = new SelectListItem();
                    slItem.Value = Utility.GetPropertyThroughReflection(o, keyName).ToString();
                    slItem.Text = Utility.GetPropertyThroughReflection(o, valueName).ToString();
                    slItem.Selected = selectedValue != null ? Utility.GetPropertyThroughReflection(o, keyName).ToString() == selectedValue.ToString() : false;
                    listSelect.Add(slItem);
                }
            }

            //if (list != null && list.Count()>0)
            //{
            //    listSelect = list.Select(n => new SelectListItem()
            //    {
            //        Selected = selectedValue != null ? Utility.GetPropertyThroughReflection(n, keyName).ToString() == selectedValue.ToString() : false,
            //        Value = Utility.GetPropertyThroughReflection(n, keyName).ToString(),
            //        Text = Utility.GetPropertyThroughReflection(n, valueName).ToString()
            //    }).ToList();
            //}
            //else
            //{
            //    listSelect = new List<SelectListItem>();
            //}
            if (firstValue != null)
            {
                SelectListItem selectListItem = new SelectListItem();
                selectListItem.Value = "-1";
                selectListItem.Text = firstValue;
                listSelect.Insert(0, selectListItem);
            }
            return listSelect;
            //return new SelectList(pList, "DistrictId", "DistrictName");
        }
    }
}
