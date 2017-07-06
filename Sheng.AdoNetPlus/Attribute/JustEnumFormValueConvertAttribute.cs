using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sheng.AdoNetPlus
{
    public class JustEnumFormValueConvertAttribute : DataHelperConvertAttribute
    {
        public Type EnumType
        {
            get;
            set;
        }

        public override object CovertTo(object value)
        {
            return value;
        }

        public override object CovertFrom(object value)
        {
            if (value == null)
                return null;

            return Enum.Parse(EnumType, value.ToString());
        }
    }
}
