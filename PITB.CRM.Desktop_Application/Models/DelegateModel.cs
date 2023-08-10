using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PITB.CRM.Desktop_Application.Models
{
    public class DelegateModel
    {
        public delegate void CallbackPopup(string popupName, string message);

        public CallbackPopup callbackPopup { get; set; }

    }
}
