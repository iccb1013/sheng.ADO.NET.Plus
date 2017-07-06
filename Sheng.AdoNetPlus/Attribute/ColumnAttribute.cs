using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sheng.AdoNetPlus
{
    public class ColumnAttribute : RelationalMappingAttribute
    {
        public string Name
        {
            get;
            private set;
        }

        public ColumnAttribute(string name)
        {
            Name = name;
        }
    }
}
