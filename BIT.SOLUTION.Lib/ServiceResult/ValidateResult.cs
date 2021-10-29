using BIT.SOLUTION.Lib.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BIT.SOLUTION.Lib
{
    public class ValidateResult
    {
        /// <summary>
        /// ID ban ghi loi
        /// </summary>
        public object ID { set; get; }
        public string Code { set; get; }
        public string ErrorMessage { set; get; }
        public string AddictionInfo { set; get; }
        public Enumeration.ValidateType ValidateType { set; get; }
    }
}
