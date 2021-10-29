using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BIT.SOLUTION.Common
{
    public static class ExtentionMethod
    {
        /// <summary>
        ///  set vaule  cho object entiti
        /// </summary>
        /// <param name="objEntity"></param>
        /// <param name="propertyName"></param>
        /// <param name="value"></param>
        public static void SetValue(this object objEntity, string propertyName, object value)
        {
            try
            {
                PropertyInfo propertyInfo = GetPropertyInfo(objEntity, propertyName);
                if(propertyInfo != null)
                {
                    Type type = propertyInfo.PropertyType;
                    if(!object.Equals(value,DBNull.Value) && propertyInfo.CanWrite)
                    {
                        if(propertyInfo.GetCustomAttribute<ColumnJsonAttribute>() != null)
                        {
                            propertyInfo.SetValue(objEntity, CommonFnConvert.DeserializeObject(value.ToString(), type), null);
                        }
                        else
                        {
                            object finalValue = value.ChangeType(type);
                            propertyInfo.SetValue(objEntity, finalValue, null);
                        }
                    }
                }


            }
            catch(Exception e)
            {
                 // ghi log
            }
        }
        /// <summary>
        /// Them gia tri neu ton tai gia tri thi update
        /// </summary>
        /// <param name="values"></param>
        /// <param name="key"></param>
        /// <param name="values"></param>
        public static void AddOrUpdateDictionary(this Dictionary<string,object> values, string key, object value)
        {
            if(!values.ContainsKey(key))
            {
                values.Add(key, value);
            }
            else
            {
                values[key] = value;
            }
        }
        /// <summary>
        /// Chuyển từ data render sang Dic
        /// </summary>
        /// <param name="dataRender"></param>
        /// <returns></returns>
        public static List<Dictionary<string,object>> ToListDictionary(this IDataReader dataRender)
        {
            if (dataRender == null) return null;
            List<Dictionary<string, object>> lstResult = new List<Dictionary<string, object>>();
            try
            {
                while(dataRender.Read())
                {
                    Dictionary<string, object> dicData = MappingDataRenderIntoDictionary(dataRender);
                    lstResult.Add(dicData);
                }
            }
            catch(Exception e)
            {
                // ghi logger
            }
            return lstResult;
        }
        /// <summary>
        /// chuyển data render sang Ilist
        /// </summary>
        /// <param name="dataRender"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static IList ToListObject(this IDataReader dataRender, Type type)
        {
            if (dataRender == null) return null;
            var listType = typeof(List<>);
            var gennericListType = listType.MakeGenericType(type);
            var listResult = (IList)Activator.CreateInstance(gennericListType);
            string propertyName = string.Empty;

            Dictionary<string, PropertyInfo> columnMappings = null;
            while (dataRender.Read())
            {
                if(columnMappings == null)
                {
                    columnMappings = MappingColumnObjectAndReader(type, dataRender, "*");
                }
                object objectItem = Activator.CreateInstance(type);

                MappingDataRenderValuesToObject(objectItem, dataRender, columnMappings);

                listResult.Add(objectItem);
            }
            return listResult;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="objectItem"></param>
        /// <param name="dataReader"></param>
        /// <param name="columnMappings"></param>
        public static void MappingDataRenderValuesToObject(object objectItem,IDataReader dataReader, Dictionary<string, PropertyInfo> columnMappings)
        {
            foreach (KeyValuePair<string,PropertyInfo> columnMapping in columnMappings)
            {
                var vVal = dataReader[columnMapping.Key];
                PropertyInfo propertyInfo = columnMapping.Value;
                if(!object.Equals(vVal,DBNull.Value))
                {
                    if(propertyInfo.GetCustomAttribute<ColumnJsonAttribute>() != null)
                    {
                        propertyInfo.SetValue(objectItem, CommonFnConvert.DeserializeObject(vVal.ToString(), propertyInfo.PropertyType), null);
                    }
                    else
                    {
                        object finalValue = vVal.ChangeType(propertyInfo.PropertyType);
                        if(finalValue != null && propertyInfo.PropertyType == typeof(string))
                        {
                            propertyInfo.SetValue(objectItem, Utility.Sanitizer(finalValue.ToString()), null);
                        }
                        else
                        {
                            propertyInfo.SetValue(objectItem, finalValue, null);
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="dataReader"></param>
        /// <param name="selectColumns"></param>
        /// <returns></returns>

        public static Dictionary<string, PropertyInfo> MappingColumnObjectAndReader(Type type, IDataReader dataReader, string selectColumns)
        {
            var columnMappings = new Dictionary<string, PropertyInfo>();
            string[] arrColums = null;
            bool isDefault = string.IsNullOrWhiteSpace(selectColumns) || selectColumns.Equals("*");
            if(!isDefault)
            {
                arrColums = selectColumns.Split(new string[1] { "*" }, StringSplitOptions.RemoveEmptyEntries);
            }
            int fieldCount = dataReader.FieldCount;
            for (int i = 0; i < fieldCount; i++)
            {
                string name = dataReader.GetName(i);
                PropertyInfo propertyInfo = type.GetProperty(name, BindingFlags.SetProperty | BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                if(propertyInfo != null && propertyInfo.CanWrite && !columnMappings.ContainsKey(name) &&(isDefault ||
                    (arrColums != null && arrColums.Any(c=>c.Trim().Equals(name,StringComparison.OrdinalIgnoreCase)))))
                {
                    columnMappings.Add(name, propertyInfo);
                }
            }
            return columnMappings;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataRender"></param>
        /// <returns></returns>
        public static Dictionary<string, object>  MappingDataRenderIntoDictionary(this IDataReader dataRender)
        {
            Dictionary<string, object> dicData = new Dictionary<string, object>();
            for (int i = 0; i < dataRender.FieldCount; i++)
            {
                string name = dataRender.GetName(i);
                if(!dicData.ContainsKey(name))
                {
                    if( !dataRender.IsDBNull(i))
                    {
                        var sqlType = dataRender.GetDataTypeName(i);
                        switch(sqlType.ToLower())
                        {
                            case "BIT":
                                dicData.Add(name, dataRender.GetBoolean(i));
                                break;
                            case "NVARCHAR":
                            case "NVAR":
                            case "TEXT":
                                var strValue = dataRender.GetString(i);
                                if(!string.IsNullOrWhiteSpace(strValue))
                                {
                                    dicData.Add(name, Utility.Sanitizer(strValue.ToString()));
                                }
                                else
                                {
                                    dicData.Add(name, strValue);
                                }
                                break;
                            default:
                                dicData.Add(name, dataRender.GetValue(i));
                                break;
                        }
                    }
                    else
                    {
                        dicData.Add(name, null);
                    }
                }
            }
            return dicData;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objEntity"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static T GetValue<T>(this object objEntity, string propertyName)
        {
            T value = default(T);
            try
            {
                if(objEntity != null && !string.IsNullOrEmpty(propertyName))
                {
                    PropertyInfo info = GetPropertyInfo(objEntity, propertyName);
                    if(info != null)
                    {
                        object objValue = info.GetValue(objEntity);
                        if(objValue != null)
                        {
                            value = (T)objValue.ChangeType(typeof(T));
                        }

                    }
                }
            }
            catch
            {
                /// log
            }
            return value;
        }
        /// <summary>
        /// Chuyển đổi liểu gia trị
        /// </summary>
        /// <param name="value"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object ChangeType(this object value, Type type)
        {
            try
            {
                if (value == null && type.IsGenericType) return Activator.CreateInstance(type);
                if (value == null) return null;
                if(type.IsEnum)
                {
                    if(value is string)
                    {
                        return Enum.Parse(type, value as string);
                    }
                    else
                    {
                        return Enum.ToObject(type, value);
                    }
                }
                if(!type.IsInterface && type.IsGenericType)
                {
                    Type innerType = type.GetGenericArguments()[0];
                    object innerValue = ChangeType(value, innerType);
                    return Activator.CreateInstance(type, new object[] { innerValue });
                }
                if (value is string && type == typeof(Guid)) return new Guid(value as string);
                if (value is Guid && type == typeof(string)) return value.ToString();
                if (!(value is IConvertible)) return value;
                return Convert.ChangeType(value, type);

            }
            catch(FormatException)
            {
                return type.GetDefaultValue();
            }
        }
        /// <summary>
        /// lấy giá trị mặc định theo kiểu giá trị
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object GetDefaultValue(this Type type)
        {
            Expression<Func<object>> e = Expression.Lambda<Func<object>>(Expression.Convert(Expression.Default(type), typeof(object)));
            return e.Compile().DynamicInvoke();
        }
        /// <summary>
        /// Chuy
        /// </summary>
        /// <param name="objEntity"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static PropertyInfo GetPropertyInfo(this object objEntity, string propertyName)
        {
            PropertyInfo propertyInfo = objEntity.GetType().GetProperty(propertyName, BindingFlags.SetProperty | BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            return propertyInfo;
        }
        public static string MD5Hash(this string input)
        {
            StringBuilder hash = new StringBuilder();
            MD5CryptoServiceProvider mD5CryptoService = new MD5CryptoServiceProvider();
            byte[] bytes = mD5CryptoService.ComputeHash(new UTF8Encoding().GetBytes(input));
            for (int i = 0; i < bytes.Length; i++)
            {
                hash.Append(bytes[i].ToString("x2"));
            }
            return hash.ToString();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataReader"></param>
        /// <returns></returns>
        public static List<T> ToListObject<T>(this IDataReader dataReader)
        {
            if (dataReader == null) return null;
            List<T> lstResult = new List<T>();
            Type rs = typeof(T);
            if (!rs.IsValueType && !rs.Name.Equals("String"))
            {
                T objectItem = default(T);
                Dictionary<string, PropertyInfo> columnMappings = null;
                while (dataReader.Read())
                {
                    if (columnMappings == null)
                    {
                        columnMappings = MappingColumnObjectAndReader(rs, dataReader, "*");
                    }
                    objectItem = Activator.CreateInstance<T>();
                    MappingDataRenderValuesToObject(objectItem, dataReader, columnMappings);
                    lstResult.Add(objectItem);
                }
            }
            else
            {
                while (dataReader.Read())
                {
                   if(!dataReader.IsDBNull(0))
                    {
                        object vVal = dataReader.GetValue(0);
                        lstResult.Add((T)vVal.ChangeType(rs));
                    }
                }
            }
            return lstResult;
        }
    }
}
