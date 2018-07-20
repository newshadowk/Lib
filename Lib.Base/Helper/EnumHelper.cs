// ReSharper disable CSharpWarnings::CS1591
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace Lib.Base
{
    public static class EnumHelper
    {
        private static readonly ConcurrentDictionary<Type, EnumMap> _maps = new ConcurrentDictionary<Type, EnumMap>();

        public static string GetStringFromEnum(Enum item)
        {
            if (item == null)
                return string.Empty;

            Type enumType = item.GetType();

            EnumMap outMap;
            if (_maps.TryGetValue(enumType, out outMap))
                return outMap[item];

            var newMap = new EnumMap(enumType);
            _maps.AddOrUpdate(enumType, newMap, (type, enumMap) => newMap);

            return newMap[item];
        }

        #region Nested type: EnumMap

        private class EnumMap
        {
            private readonly Type _internalEnumType;
            private readonly Dictionary<Enum, string> _map;

            public EnumMap(Type enumType)
            {
                if (!enumType.IsSubclassOf(typeof (Enum)))
                    throw new InvalidCastException();

                _internalEnumType = enumType;
                FieldInfo[] staticFiles = enumType.GetFields(BindingFlags.Public | BindingFlags.Static);

                _map = new Dictionary<Enum, string>(staticFiles.Length);

                for (int i = 0; i < staticFiles.Length; i++)
                {
                    if (staticFiles[i].FieldType == enumType)
                    {
                        object[] attrs = staticFiles[i].GetCustomAttributes(typeof (DescriptionAttribute), true);

                        //Description，
                        string description = attrs.Length > 0
                                                 ? ((DescriptionAttribute) attrs[0]).Description
                                                 : staticFiles[i].Name;

                        _map.Add((Enum) staticFiles[i].GetValue(enumType), description);
                    }
                }
            }

            public string this[Enum item]
            {
                get
                {
                    if (item.GetType() != _internalEnumType)
                        throw new ArgumentException();
                    return _map[item];
                }
            }
        }

        #endregion
    }
}