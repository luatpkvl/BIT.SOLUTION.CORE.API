using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BIT.SOLUTION.Lib
{
    public static class SecureUtil
    {
        public static string SafeSqlLiteral(string sqlInput)
        {
            return sqlInput;
        }
        public static bool DetectSqlInjection(string sqlInput)
        {
            return false;
        }
    }
}
