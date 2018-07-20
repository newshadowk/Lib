using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Lib.Base
{
    /// <summary>
    /// JSON
    /// </summary>
    public static class JsonExtensions
    {
        public static T ToObject<T>(this string str)
        {
            if (string.IsNullOrEmpty(str))
                return default(T);

            return JsonConvert.DeserializeObject<T>(str);
        }
            
        public static object ToObject(this string str, Type t)
        {
            if (string.IsNullOrEmpty(str))
                return default;

            return JsonConvert.DeserializeObject(str, t);
        }

        public static T ToObjectAsAnonymous<T>(this string str, T obj)
        {
            return JsonConvert.DeserializeAnonymousType(str, obj);
        }

        public static string ToJson<T>(this T obj)
        {
            if (obj == null)
                return null;
            return ToJsonPascalType(obj);

            //return JsonConvert.SerializeObject(obj, Formatting.None, new JsonSerializerSettings
            //{
            //    ContractResolver = new CamelCasePropertyNamesContractResolver()
            //});
        }

        public static string ToJsonPascalType<T>(this T obj)
        {
            return JsonConvert.SerializeObject(obj, Formatting.None);
        }

        public static T JsonClone<T>(this T obj)
        {
            return obj.ToJson().ToObject<T>();
        }
    }
}