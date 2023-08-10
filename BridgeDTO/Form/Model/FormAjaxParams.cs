using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bridge;
//using BridgeDTO.Form.Attribute;

namespace BridgeDTO.Form.DynamicForm
{
    [FileName("Form.js")]
    public class FormAjaxParams
    {
        public string Url { get; set; }

        public string FormId { get; set; }

        public string UrlAfterPost { get; set; }

        public string FormTag { get; set; }
    }
}
