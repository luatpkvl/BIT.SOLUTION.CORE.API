namespace BIT.SOLUTION.DbHepper
{
    /// <summary>
    /// Thông tin columns của table
    /// </summary>
    public class Columns
    {
        public string FieldName { set; get; }
        public string DataType { set; get; }
        public long MaxLength { set; get; }
        public int Precision { set; get; }
        public bool IsIdentity { set; get; }
        public bool IsNullable { set; get; }
        public string Extra { set; get; }
    }
}
