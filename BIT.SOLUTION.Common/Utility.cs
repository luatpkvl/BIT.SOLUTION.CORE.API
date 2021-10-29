using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BIT.SOLUTION.Common
{
    public static class Utility
    {
        public static string Sanitizer(string  strValue)
        {
            return strValue;
        }
        public static string GetTableName<T>()
        {
            string tableName = string.Empty;
            var tableAttribute = typeof(T).GetCustomAttribute(typeof(TableNameAttribute), true) as TableNameAttribute;
            if(tableAttribute !=null)
            {
                tableName = tableAttribute.Name;
            }
            else
            {
                tableName = typeof(T).Name;
            }
            return tableName;
        }
    }
}
