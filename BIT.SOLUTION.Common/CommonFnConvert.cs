using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace BIT.SOLUTION.Common
{
    public static class CommonFnConvert
    {
        public static JsonSerializerSettings GetJsonSerializerSettings(DateTimeZoneHandling timeZoneHandling = DateTimeZoneHandling.Local)
        {
            return new JsonSerializerSettings()
            {
                DateFormatHandling = DateFormatHandling.IsoDateFormat,
                DateTimeZoneHandling = timeZoneHandling,
                DateFormatString = "yyyy-MM-ddd'T'HH:mm:ss.fffzzz",
                NullValueHandling = NullValueHandling.Ignore,
            };
        }
        /// <summary>
        /// chuyen object sang json
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string SerializableObject(object obj)
        {
            return JsonConvert.SerializeObject(obj, GetJsonSerializerSettings());
        }
        /// <summary>
        /// jon sang object
        /// </summary>
        /// <param name="json"></param>
        /// <param name="boType"></param>
        /// <returns></returns>
        public static object DeserializeObject(string json, Type boType)
        {
            if (boType == Type.GetType("System.String"))
            {
                return json;
            }
            return JsonConvert.DeserializeObject(json, boType, GetJsonSerializerSettings());
        }
        public static T DeserializeObject<T>(string jsonData)
        {
            return JsonConvert.DeserializeObject<T>(jsonData, GetJsonSerializerSettings());
        }
        /// <summary>
        /// convert dữ liệu từ object sang dic
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static Dictionary<string,object> ConvertJobjectToDictionary(object item)
        {
            return JObject.FromObject(item).ToObject<Dictionary<string, object>>();
        }
        /// <summary>
        /// onvert dữ liệu từ object sang dic
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static Dictionary<string, object> ConvertJobjectTypeToDictionary(object item)
        {
            return item.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public).ToDictionary(prop => prop.Name, prop => prop.GetValue(item,null));
        }
    }
}
