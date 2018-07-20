using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace Lib.Base
{
    /// <summary>
    /// 
    /// </summary>
    public static class EnumExtensions
    {
        public static List<T> ToEnumList<T>()
        {
            List<T> ret = new List<T>();
            var enumType = typeof(T);
            if (!enumType.IsEnum)
                return null;
            foreach (object v in Enum.GetValues(enumType))
                ret.Add((T) v);
            return ret;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="enumType"></param>
        /// <returns></returns>
        public static string GetEnumDescription(this Enum enumType)
        {
            Type type = enumType.GetType();
            FieldInfo fd = type.GetField(enumType.ToString());
            if (fd == null) return string.Empty;
            object[] attrs = fd.GetCustomAttributes(typeof(DescriptionAttribute), false);
            string name = string.Empty;
            foreach (DescriptionAttribute attr in attrs)
            {
                name = attr.Description;
            }
            return name;
        }

        /// <summary>
        /// Convert value to enum.
        /// </summary>
        /// <typeparam name="T">Type of enum.</typeparam>
        /// <param name="value">Value of enum.</param>
        /// <exception cref="System.ArgumentException">If out of range.</exception>
        /// <returns>Return the converted enum.</returns>
        public static T ToEnumFromValue<T>(this object value)
        {
            var type = typeof(T);
            if (!type.IsEnum)
            {
                throw new ArgumentException("Type is not a Enum. Type:" + type);
            }
            if (Enum.IsDefined(type, value))
            {
                return (T)Enum.ToObject(type, value);
            }
            throw new ArgumentException("Invalid data to be converted to enum");
        }

        /// <summary>
        /// Convert value to enum.
        /// </summary>
        /// <typeparam name="T">Type of enum.</typeparam>
        /// <param name="name">Name of enum.</param>
        /// <exception cref="System.ArgumentException">If out of range.</exception>
        /// <returns>Return the converted enum.</returns>
        public static T ToEnumFromName<T>(this string name)
        {
            var type = typeof(T);
            if (!type.IsEnum)
            {
                throw new ArgumentException("Type is not a Enum. Type:" + type);
            }
            if (Enum.IsDefined(type, name))
            {
                return (T)Enum.Parse(type, name);
            }
            throw new ArgumentException("Invalid data to be converted to enum");
        }

        public static T ToEnumFromNameWithoutException<T>(this string name)
        {
            if (name == null)
            {
                return default(T);
            }
            var type = typeof(T);
            if (!type.IsEnum)
            {
                throw new ArgumentException("type is not a Enum. Type:" + type);
            }
            if (Enum.IsDefined(type, name))
            {
                return (T)Enum.Parse(type, name);
            }
            return default(T);
        }

        public static bool Validate<T>(this T e)
        {
            var type = typeof (T);
            if (!type.IsEnum)
            {
                throw new ArgumentException("type is not a Enum. Type:" + type);
            }

            foreach (T value in Enum.GetValues(type))
            {
                if (e.Equals(value))
                    return true;
            }
            return false;
        }

        public static bool Validate<T>(int value)
        {
            var type = typeof(T);
            if (!type.IsEnum)
            {
                throw new ArgumentException("type is not a Enum. Type:" + type);
            }

            foreach (T val in Enum.GetValues(type))
            {
                if (val.Equals(Enum.ToObject(type, value)))
                    return true;
            }
            return false;
        }
    }
}
