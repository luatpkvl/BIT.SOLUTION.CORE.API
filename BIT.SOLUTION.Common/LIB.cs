using System;

namespace BIT.SOLUTION.Common
{
    /// <summary>
    /// thuộc tính này là json
    /// </summary>
    public class ColumnJsonAttribute : Attribute { }
    /// <summary>
    /// cần gi log
    /// </summary>
    public class IgnoreLogAttribute : Attribute { }
    /// <summary>
    /// code
    /// </summary>
    public class CodeAttribute : Attribute { }
    /// <summary>
    /// thuộc tính update
    /// </summary>
    public class IgnoreUpdateAttribute : Attribute { }
    public class ColumnAttribute : Attribute { }
    /// <summary>
    /// Không được lấy vào Entity
    /// </summary>
    public class NotMappedAttribute : Attribute { }
    /// <summary>
    /// tên table
    /// </summary>
    public class TableNameAttribute : Attribute 
    {
        public string Name { set; get; }
        public TableNameAttribute()
        {
            this.Name = string.Empty;
        }

        public TableNameAttribute(string name)
        {
            this.Name = name;
        }
    }
}
