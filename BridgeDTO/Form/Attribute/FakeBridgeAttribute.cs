using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bridge
{
    class FakeBridgeAttribute
    {
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Enum, AllowMultiple = true)]
    public class FileName : System.Attribute
    {
        public string StrParam { get; set; }

        public FileName(string str)
        {
            
        }
    }
}
