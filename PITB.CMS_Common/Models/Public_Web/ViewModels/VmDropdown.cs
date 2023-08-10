using System;
using System.Collections.Generic;

namespace PITB.CMS_Common.Models.Public_Web.ViewModels
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