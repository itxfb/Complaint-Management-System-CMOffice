using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PITB.CMS_Common.Models.Custom
{
    public class Prop<T> where T : class
    {
        private T value;

        public T Value
        {
            get
            {
                return this.value;
            }
            set
            {
                this.value = value;
            }
        }

        public static implicit operator T(Prop<T> value)
        {
            return value.Value;
        }

        public static implicit operator Prop<T>(T value)
        {
            return new Prop<T> { Value = value };
        }

    }
}
