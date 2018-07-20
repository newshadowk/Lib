using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Lib.Base
{
    public static class TypeExtensions
    {
        public static void CopyProperties<T>(this object toObj, T fromObj)
        {
            var sourcePublicProperties = toObj.GetType().GetProperties().OrderBy(o => o.Name).ToArray();
            var copyPublicProperties = typeof(T).GetProperties().OrderBy(o => o.Name).ToArray();
            if (sourcePublicProperties.Length > 0 && sourcePublicProperties.Length == copyPublicProperties.Length)
            {
                for (int i = 0; i < sourcePublicProperties.Length; i++)
                {
                    var propertyInfo = sourcePublicProperties[i];
                    var copyAttributes = propertyInfo.GetCustomAttributes(typeof(CopyPropertyValueAttribute), false);
                    if (copyAttributes.Length == 1 && ((CopyPropertyValueAttribute)copyAttributes[0]).IsCopyPropertyValue)
                        if (propertyInfo.GetSetMethod() != null)
                            sourcePublicProperties[i].SetValue(toObj, copyPublicProperties[i].GetValue(fromObj, null), null);
                }
            }
        }

        public static void CopyPropertiesByAll<T>(this T toObj, T fromObj)
        {
            var sourcePublicProperties = typeof(T).GetProperties();
            if (sourcePublicProperties.Length == 0)
                return;

            foreach (var sourcePublicProperty in sourcePublicProperties)
            {
                sourcePublicProperty.SetValue(toObj, sourcePublicProperty.GetValue(fromObj, null), null);
            }
        }

        public static T CopyAllProperties<T>(this object fromObj) where T : new()
        {
            T toObj = new T();
            CopyAllProperties(fromObj, toObj);
            return toObj;
        }

        public static void CopyAllProperties<T>(this object fromObj, T toObj)
        {
            List<PropertyInfo> toPs = typeof(T).GetProperties().OrderBy(o => o.Name).ToList();
            List<PropertyInfo> fromPs = fromObj.GetType().GetProperties().OrderBy(o => o.Name).ToList();

            foreach (PropertyInfo fromP in fromPs)
            {
                var toP = toPs.Find(i => i.Name.ToLower() == fromP.Name.ToLower());
                if (toP == null)
                    continue;
                toP.SetValue(toObj, fromP.GetValue(fromObj, null), null);
            }
        }

        public static object GetValueWithSplit<T>(this T sourceObj, string propertyPath, out Type dataType, params char[] separator)
        {
            var propertyPaths = new ArrayList(propertyPath.Split(separator));
            return sourceObj.GetChildValue(propertyPaths, out dataType);
        }

        public static TTo Convert<TTo>(this Enum obj) where TTo : struct
        {
            foreach (var item in Enum.GetNames(typeof(TTo)))
            {
                if (obj.ToString() == item)
                    return (TTo)Enum.Parse(typeof(TTo), item);
            }
            return default(TTo);
        }
        private static object GetChildValue<T>(this T sourceObj, ArrayList propertyPaths, out Type dataType)
        {
            dataType = null;
            if (propertyPaths.Count >= 1)
            {
                var propertyInfo = sourceObj.GetType().GetProperty((string)propertyPaths[0]);
                if (propertyInfo == null)
                    return null;
                var sourceObjChildValue = propertyInfo.GetValue(sourceObj, null);
                if (sourceObjChildValue == null)
                    return null;
                propertyPaths.RemoveAt(0);
                if (propertyPaths.Count == 0)
                {
                    dataType = propertyInfo.PropertyType;
                    return sourceObjChildValue;
                }
                return sourceObjChildValue.GetChildValue(propertyPaths, out dataType);
            }
            return null;
        }

    }
}
