using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PITB.CRM.Public_Web.Models.ViewModels
{
    public class VmDropdown
    {

        public VmDropdown()
        {
            DropdownValuesList= new List<DropdownValues>();
        }
        private string RandomClientId =  Guid.NewGuid().ToString("N");
        public string ClientSideId { get { return RandomClientId; } }
        public int SelectedValue { get; set; }
        public List<DropdownValues>  DropdownValuesList { get; set; }
        
      


        public class DropdownValues
        {
            public string Text { get; set; }
            public string Value { get; set; }
        }
    }
}