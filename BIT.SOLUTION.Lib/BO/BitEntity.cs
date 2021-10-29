using BIT.SOLUTION.Common;
using BIT.SOLUTION.Lib.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BIT.SOLUTION.Lib
{
    public class BitEntity : ICloneable
    {
        [JsonIgnore]
        public string TableSchema { set; get; } = "";
        /// <summary>
        /// khong su dung thu tuc cho thao tac insert, update, delete
        /// </summary>
        [JsonIgnore]
        public bool UserProc { set; get; } = false;
        public Enumeration.BITEntityState BITEntityState { set; get; } = Enumeration.BITEntityState.None;
        [Column]
        public DateTime? CreateDate { set; get; }
        [Column]
        public string CreateBy { set; get; }
        [Column]
        public string ModifiedDate { set; get; }
        [Column]
        public string ModifiedBy { set; get; }
        [IgnoreLog]
        public virtual int? ModuleType { set; get; }
        [IgnoreLog]
        public virtual bool? IsDeletePermanently { set; get; } = false;
        [IgnoreLog]
        public List<BITField> Fields { set; get; }
        [IgnoreLog]
        public List<BITField> FieldsCustom { set; get; }
        /// <summary>
        /// nhat ky  truy cap
        /// </summary>
        [JsonIgnore]
        public virtual bool IsAuditLog { set; get; } = true;
        [JsonIgnore]
        public virtual string[] DetailFoeldConfig { set; get; }
        [JsonIgnore]
        public virtual string[] DeleteTableRelation { set; get; }
        /// <summary>
        /// duw liu truoc khi ghi auditlog
        /// </summary>
        [JsonIgnore]
        public object OldData { set; get; }
        [JsonIgnore]
        public Dictionary<string,object> OldDatas { set; get; }
        [JsonIgnore]
        public string AuditLogInfo { set; get; }
        /// <summary>
        ///  du lieu map cua 2 phan he
        /// </summary>
        [IgnoreLog]
        public List<MappingSourcodeData> MappingDatas { set; get; }
        [IgnoreLog]
        public int? MappingID { set; get; }
        public bool HasIdentity()
        {
            return !KeyIsGuid();
        }
        public  bool KeyIsGuid()
        {
            var prop = this.GetProperties().FirstOrDefault(p=>p.GetCustomAttribute<KeyAttribute>(true) != null);
            if(prop == null)
            {
                /// ghi logger
                return false;
            }
            else
            {
                return prop.PropertyType == typeof(Guid);
            }
        }
        /// <summary>
        /// kiem tra thuoc tinh co ton tai
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public bool ExitstProperty(string propertyName)
        {
            if(this.GetEntityType().GetProperty(propertyName)  == null)
            {
                return false;
            }
            return true;
        }
        /// <summary>
        /// lấy toàn bộ code column
        /// </summary>
        /// <returns></returns>
        public virtual  string GetCodeField()
        {
            string codeColumn = string.Empty;
            PropertyInfo[] props = this.GetProperties();
            PropertyInfo oProperty = null;
            if(props != null)
            {
                oProperty = props.SingleOrDefault(p=>p.GetCustomAttribute<CodeAttribute>(true) != null);
                if(oProperty !=null)
                {
                    codeColumn = oProperty.Name;
                }
            }
            return codeColumn;
        }
        private PropertyInfo[] _properties;
        /// <summary>
        /// Lấy ra Property của entity
        /// </summary>
        /// <returns></returns>
        public PropertyInfo[] GetProperties()
        {
            if(this._properties == null)
            {
                this._properties = this.GetEntityType().GetProperties();
            }
            return this._properties;
        }
        /// <summary>
        /// lấy giá trị khóa chính
        /// </summary>
        /// <returns></returns>
        public object GetPrimaryKeyValue()
        {
            string primaryKeyName = this.GetPrimaryKeyFieldName();
            return this[primaryKeyName];
        }
        /// <summary>
        /// kiểm tra có tồn tại thuộc tính hay không
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public bool ExitProperty(string propertyName)
        {
           if(this.GetEntityType().GetProperty(propertyName) == null)
            {
                return false;
            }
            return true;
        }
        /// <summary>
        /// Lấy Diction của Key
        /// </summary>
        /// <returns></returns>
        public Dictionary<string,object> GetObjectPrimaryKeyValue()
        {
            Dictionary<string, object> dicData = new Dictionary<string, object>();
            string primariKeyName = this.GetPrimaryKeyFieldName();
            dicData.Add(primariKeyName, this[primariKeyName]);
            return dicData;
        }

        private string _primaryKeyFeildName;
        /// <summary>
        /// lay khoa chi
        /// </summary>
        /// <returns></returns>
        public string GetPrimaryKeyFieldName()
        {
            if(string.IsNullOrEmpty(_primaryKeyFeildName))
            {
                PropertyInfo[] props = this.GetProperties();
                if(props != null)
                {
                    var propertyInfoKey = props.SingleOrDefault(p => p.GetCustomAttribute<KeyAttribute>(true) != null);
                    if(propertyInfoKey != null)
                    {
                        this._primaryKeyFeildName = propertyInfoKey.Name;
                    }
                }
            }
            return this._primaryKeyFeildName;
        }
        /// <summary>
        /// gan gia tri khoa chinh
        /// </summary>
        /// <param name="value"></param>
        public void SetPrimaryKeyFieldName(string value)
        {
            this._primaryKeyFeildName = value;
        }
        private string _tableName;
        /// <summary>
        /// lay ten bang trong db
        /// </summary>
        /// <returns></returns>
        public virtual string GetTableName()
        {
            if(string.IsNullOrEmpty(_tableName))
            {
                var tableNameAttribute = this.GetType().GetCustomAttribute(typeof(TableNameAttribute), true) as TableNameAttribute;
                if(tableNameAttribute != null)
                {
                    this._tableName = tableNameAttribute.Name;
                }
                else
                {
                    this._tableName = this.GetEntityType().Name;
                }
            }
            return this._tableName;
        }
        /// <summary>
        /// lấy ds của table
        /// </summary>
        private Tuple<List<string>, List<string>, List<string>> _tupleColumns = null;
        /// <summary>
        /// lay danh sach columme cua table
        /// </summary>
        /// <returns></returns>
        public Tuple<List<string>, List<string>, List<string>> GetColumns()
        {
            if(_tupleColumns == null)
            {
                List<string> columnsNormal = new List<string>();
                List<string> columnsJson = new List<string>();
                List<string> ignoreUpdateColums = new List<string>();
                foreach (PropertyInfo p in this.GetProperties())
                {
                    if(p.GetCustomAttribute<NotMappedAttribute>(true) == null)
                    {
                        if (p.GetCustomAttribute<IgnoreUpdateAttribute>(true) != null)
                        {
                            string name = p.Name;
                            ignoreUpdateColums.Add(name);
                        }
                        if (p.GetCustomAttribute<ColumnAttribute>(true) != null)
                        {
                            string name = p.Name;
                            if (p.GetCustomAttribute<ColumnJsonAttribute>(true) != null)
                            {
                                columnsJson.Add(name);
                            }
                            else
                            {
                                columnsNormal.Add(name);
                            }
                        }
                    }    
                    
                }
                _tupleColumns = new Tuple<List<string>, List<string>, List<string>>(columnsNormal, columnsJson, ignoreUpdateColums);
            }
            return _tupleColumns;
        }

        /// <summary>
        /// set khoa chinh
        /// </summary>
        /// <param name="value"></param>
        public void SetPrimaryKey(object value)
        {
            string primaryKeyName = this.GetPrimaryKeyFieldName();
            this[primaryKeyName] = value;
        }
        private Type _type;
        public Type  GetEntityType()
        {
            if (this._type == null)
            {
                this._type = this.GetType();
            }
            return _type;
        }
        /// <summary>
        /// clone to object
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            return MemberwiseClone();
        }
        /// <summary>
        /// Get or set value
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public object this[string propertyName]
        {
            get
            {
                PropertyInfo pi = this.GetEntityType().GetProperty(propertyName);
                if (pi != null)
                {
                    return pi.GetValue(this, null);
                }
                else
                {
                    throw new Exception(string.Format("<{0}> does not exitst in object <{1}>!", propertyName, this.GetEntityType().ToString()));
                }
            }
            set
            {
                PropertyInfo pi = this.GetEntityType().GetProperty(propertyName);
                if (pi != null)
                {
                     pi.SetValue(this, value, null);
                }
                else
                {
                    throw new Exception(string.Format("<{0}> does not exitst in object <{1}>!", propertyName, this.GetEntityType().ToString()));
                }
            }
        }
        private string _queryInsert;
        private string _queryUpdate;
        private string _queryInsertUpdate;
        /// <summary>
        /// lay cau lenh insert
        /// </summary>
        /// <returns></returns>
        public virtual string QueryInsert()
        {
            if(string.IsNullOrEmpty(_queryInsert))
            {
                BuildQuery();
            }
            return _queryInsert;
        }
        public virtual string QueryUpdate()
        {
            if (string.IsNullOrEmpty(_queryUpdate))
            {
                BuildQuery();
            }
            return _queryUpdate;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual string QueryDelete()
        {
            this._primaryKeyFeildName = this.GetPrimaryKeyFieldName();
            return $" DELETE FROM `{this.GetTableName()}`  WHERE {this._primaryKeyFeildName}=@{this._primaryKeyFeildName}";
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual string QueryUpdateDelete()
        {
            this._primaryKeyFeildName = this.GetPrimaryKeyFieldName();
            return $"UPDATE `{this.GetTableName()}` SET IsDeleted =TRUE  WHERE {this._primaryKeyFeildName}=@{this._primaryKeyFeildName}";
        }
        public virtual string QueryInsertUpdate()
        {
            if (string.IsNullOrEmpty(_queryInsertUpdate))
            {
                BuildQuery();
            }
            return _queryInsertUpdate;
        }
        /// <summary>
        ///  thuc hien chuoi query inser, update delete
        /// </summary>
        public virtual void BuildQuery()
        {
            string query = string.Empty;
            StringBuilder builderValue = new StringBuilder();
            StringBuilder builderParamName = new StringBuilder();
            string primaryKeyName = this.GetPrimaryKeyFieldName();
            string tableName = $"`{this.GetTableName()}`";

            bool isIdentity = this.HasIdentity();
            var tupleColumns = this.GetColumns();

            List<string> ignoreUpdateColumns = tupleColumns.Item3;

            bool isAdd = this.BITEntityState == Enumeration.BITEntityState.Add;
            void AddValueToStringBuilder(string col)
            {
                if(isAdd)
                {
                    builderValue.AppendFormat("@{0}", col);
                    builderParamName.AppendFormat("`{0}`", col);
                }
                else
                {
                    var ignoreUpdateColumn = ignoreUpdateColumns.Find(ig => ig.Equals(col, StringComparison.OrdinalIgnoreCase));
                    if (col.Equals(primaryKeyName, StringComparison.OrdinalIgnoreCase) 
                        || col.Equals(CommonKey.btsCreateDate,StringComparison.OrdinalIgnoreCase) 
                        || col.Equals(CommonKey.btsCreateBy, StringComparison.OrdinalIgnoreCase))
                    {
                        return;
                    }
                    builderValue.AppendFormat("`{0}`=@{0}", col);
                }
            }
            void AddParams(List<string> columns)
            {
                foreach (var c in columns)
                {
                    if(isIdentity && this.BITEntityState == Enumeration.BITEntityState.Add &&
                        c.Equals(primaryKeyName,StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }
                    AddValueToStringBuilder(c);
                }
            }
            AddParams(tupleColumns.Item1);
            AddParams(tupleColumns.Item2);
            if(isAdd && this.ExitstProperty(CommonKey.bitAsyncID))
            {
                if(!tupleColumns.Item1.Any(m=>m.Equals(CommonKey.bitAsyncID, StringComparison.OrdinalIgnoreCase)))
                {
                    AddValueToStringBuilder(CommonKey.bitAsyncID);
                }
            }
            switch(this.BITEntityState)
            {
                case Enumeration.BITEntityState.Add:
                    _queryInsert = $" INSERT INTO {tableName}({builderParamName.ToString().Substring(0, builderParamName.Length)}) values({builderValue.ToString().Substring(0, builderValue.Length)});";
                    if(isIdentity)
                    {
                        _queryInsert += "SELECT LAST_INSERT_ID();";
                    }
                    break;
                case Enumeration.BITEntityState.UpdateInsert:
                    _queryInsertUpdate = _queryInsert = $" UPDATE  {tableName} SET IsDelete = False, {builderValue.ToString().Substring(0, builderValue.Length)} WHERE {primaryKeyName} = @{primaryKeyName};";
                    break;
                default:
                    _queryUpdate = $" UPDATE  {tableName} SET {builderValue.ToString().Substring(0, builderValue.Length)} WHERE {primaryKeyName} = @{primaryKeyName};";
                    break;
            }
        }
        /// <summary>
        ///  lấy ra giá trị của theo columme 
        /// </summary>
        /// <returns></returns>
        public virtual Dictionary<string,object> GetValues()
        {
            Dictionary<string, object> values = new Dictionary<string, object>();
            string primaryKeyName = this.GetPrimaryKeyFieldName();
            bool isIdentity = this.HasIdentity();
            var tupleColumns = this.GetColumns();
            bool isAdd = this.BITEntityState == Enumeration.BITEntityState.Add;
            void AddValueToStringBuilder(string col)
            {
                if (isAdd)
                {
                    values.Add(col, this[col]);
                }
                else
                {
                    if (col.Equals(CommonKey.btsCreateDate, StringComparison.OrdinalIgnoreCase)
                       || col.Equals(CommonKey.btsCreateBy, StringComparison.OrdinalIgnoreCase)) return;
                    values.Add(col, this[col]);
                }
            }
            void AddParams(List<string> columns)
            {
                foreach (var c in columns)
                {
                    if (isIdentity && this.BITEntityState == Enumeration.BITEntityState.Add &&
                        c.Equals(primaryKeyName, StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }
                    AddValueToStringBuilder(c);
                }
            }
            AddParams(tupleColumns.Item1);
            AddParams(tupleColumns.Item2);
            if (isAdd && this.ExitstProperty(CommonKey.bitAsyncID))
            {
                if (!tupleColumns.Item1.Any(m => m.Equals(CommonKey.bitAsyncID, StringComparison.OrdinalIgnoreCase)))
                {
                    AddValueToStringBuilder(CommonKey.bitAsyncID);
                }
            }
            return values;
        }
    }
}
