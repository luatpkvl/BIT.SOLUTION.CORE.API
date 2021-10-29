using BIT.SOLUTION.Lib.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BIT.SOLUTION.Lib
{
    public class BITField:BitEntity
    {
        public virtual long? ID { set; get; }
        public virtual string FieldName { set; get; }
        public virtual bool? IsAutoCreateSequemceAfterSave { set; get; }
        public virtual string Value { set; get; }
        public virtual bool? IsUnique { set; get;}
    }
}
