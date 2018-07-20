using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading;

namespace Lib.Base
{
    public class ClassHelper
    {
        public static object CreateInstance(Type t)
        {
            return Activator.CreateInstance(t);
        }

        public static object CreateInstance(string className, List<CustPropertyInfo> lcpi)
        {
            Type t = BuildType(className);
            t = AddProperty(t, lcpi);
            return Activator.CreateInstance(t);
        }

        public static object CreateInstance(List<CustPropertyInfo> lcpi)
        {
            return CreateInstance("DefaultClass", lcpi);
        }

        public static T InvokeMethod<T>(object classInstance, string methodName, params object[] args)
        {
            return (T)classInstance.GetType().GetMethod(methodName).Invoke(classInstance, args);
        }

        public static object InvokeMethod(object classInstance, string methodName, params object[] args)
        {
            return classInstance.GetType().GetMethod(methodName).Invoke(classInstance, args);
        }

        public static void SetPropertyValue(object classInstance, PropertyInfo tartgetProperty, object propertyValue)
        {
            var type = classInstance.GetType();

            if (tartgetProperty.PropertyType.IsEnum)
                propertyValue = Enum.ToObject(tartgetProperty.PropertyType, propertyValue);

            if (propertyValue == DBNull.Value || propertyValue == null)
            {
                type.InvokeMember(tartgetProperty.Name, BindingFlags.SetProperty, Type.DefaultBinder, classInstance, new object[] { null });
            }
            else
            {
                type.InvokeMember(tartgetProperty.Name, BindingFlags.SetProperty, Type.DefaultBinder, classInstance, new[] { Convert.ChangeType(propertyValue, tartgetProperty.PropertyType) });
            }
        }

        public static object GetPropertyValue(object classInstance, string propertyName)
        {
            return classInstance.GetType().InvokeMember(propertyName, BindingFlags.GetProperty,
                                                          null, classInstance, new object[] { });
        }

        public static Type BuildType()
        {
            return BuildType("DefaultClass");
        }

        public static Type BuildType(string className)
        {
            AssemblyName myAsmName = new AssemblyName();
            myAsmName.Name = "MyDynamicAssembly";

             var myAsmBuilder = AssemblyBuilder.DefineDynamicAssembly(myAsmName,
                AssemblyBuilderAccess.RunAndCollect);


            //AssemblyBuilder myAsmBuilder = myDomain.DefineDynamicAssembly(myAsmName,
            //                                                AssemblyBuilderAccess.RunAndSave);

            //。
            //ModuleBuilder myModBuilder =
            //    myAsmBuilder.DefineDynamicModule(myAsmName.Name, myAsmName.Name + ".dll");

            ModuleBuilder myModBuilder =  myAsmBuilder.DefineDynamicModule(myAsmName.Name);
            //TypeBuilder。
            TypeBuilder myTypeBuilder = myModBuilder.DefineType(className,
                                                            TypeAttributes.Public);

            //。
            Type retval = myTypeBuilder.CreateTypeInfo();

            //，Ildasm.exe，。
            //myAsmBuilder.Save(myAsmName.Name + ".dll");
            return retval;
        }

        public static Type AddProperty(Type classType, List<CustPropertyInfo> lcpi)
        {
            //，。
            MergeProperty(classType, lcpi);
            //Type。
            return AddPropertyToType(classType, lcpi);
        }

        public static Type AddProperty(Type classType, CustPropertyInfo cpi)
        {
            List<CustPropertyInfo> lcpi = new List<CustPropertyInfo>();
            lcpi.Add(cpi);
            //，。
            MergeProperty(classType, lcpi);
            //Type。
            return AddPropertyToType(classType, lcpi);
        }

        public static Type DeleteProperty(Type classType, string propertyName)
        {
            List<string> ls = new List<string>();
            ls.Add(propertyName);

            //，。
            List<CustPropertyInfo> lcpi = SeparateProperty(classType, ls);
            //Type。
            return AddPropertyToType(classType, lcpi);
        }

        public static Type DeleteProperty(Type classType, List<string> propertyNames)
        {
            //，。
            List<CustPropertyInfo> lcpi = SeparateProperty(classType, propertyNames);
            //Type。
            return AddPropertyToType(classType, lcpi);
        }

        private static void MergeProperty(Type t, List<CustPropertyInfo> lcpi)
        {
            foreach (PropertyInfo pi in t.GetProperties())
            {
                CustPropertyInfo cpi = new CustPropertyInfo(pi.PropertyType, pi.Name);
                lcpi.Add(cpi);
            }
        }

        private static List<CustPropertyInfo> SeparateProperty(Type t, List<string> propertyNames)
        {
            List<CustPropertyInfo> ret = new List<CustPropertyInfo>();
            foreach (PropertyInfo pi in t.GetProperties())
            {
                foreach (string s in propertyNames)
                {
                    if (pi.Name != s)
                    {
                        CustPropertyInfo cpi = new CustPropertyInfo(pi.PropertyType, pi.Name);
                        ret.Add(cpi);
                    }
                }
            }

            return ret;
        }

        private static void AddPropertyToTypeBuilder(TypeBuilder myTypeBuilder, List<CustPropertyInfo> lcpi)
        {
            PropertyBuilder custNamePropBldr;
            MethodBuilder custNameGetPropMthdBldr;
            MethodBuilder custNameSetPropMthdBldr;
            MethodAttributes getSetAttr;
            ILGenerator custNameGetIL;
            ILGenerator custNameSetIL;

            // SetGet。Public。
            getSetAttr =
                MethodAttributes.Public | MethodAttributes.SpecialName |
                    MethodAttributes.HideBySig;

            // myTypeBuilder。
            foreach (CustPropertyInfo cpi in lcpi)
            {
                //。
                FieldBuilder customerNameBldr = myTypeBuilder.DefineField(cpi.FieldName,
                                                                          cpi.Type,
                                                                          FieldAttributes.Private);

                //。
                //null，。
                custNamePropBldr = myTypeBuilder.DefineProperty(cpi.PropertyName,
                                                                 PropertyAttributes.HasDefault,
                                                                 cpi.Type,
                                                                 null);


                //Get。
                custNameGetPropMthdBldr =
                    myTypeBuilder.DefineMethod(cpi.GetPropertyMethodName,
                                               getSetAttr,
                                               cpi.Type,
                                               Type.EmptyTypes);

                custNameGetIL = custNameGetPropMthdBldr.GetILGenerator();

                custNameGetIL.Emit(OpCodes.Ldarg_0);
                custNameGetIL.Emit(OpCodes.Ldfld, customerNameBldr);
                custNameGetIL.Emit(OpCodes.Ret);

                //Set。
                custNameSetPropMthdBldr =
                    myTypeBuilder.DefineMethod(cpi.SetPropertyMethodName,
                                               getSetAttr,
                                               null,
                                               new[] { cpi.Type });

                custNameSetIL = custNameSetPropMthdBldr.GetILGenerator();

                custNameSetIL.Emit(OpCodes.Ldarg_0);
                custNameSetIL.Emit(OpCodes.Ldarg_1);
                custNameSetIL.Emit(OpCodes.Stfld, customerNameBldr);
                custNameSetIL.Emit(OpCodes.Ret);

                //(Get,Set)PropertyBuilder。
                custNamePropBldr.SetGetMethod(custNameGetPropMthdBldr);
                custNamePropBldr.SetSetMethod(custNameSetPropMthdBldr);
            }
        }

        public static Type AddPropertyToType(Type classType, List<CustPropertyInfo> lcpi)
        {
            AppDomain myDomain = Thread.GetDomain();
            AssemblyName myAsmName = new AssemblyName();
            myAsmName.Name = "MyDynamicAssembly";

            //，AssemblyBuilderAccess.RunAndSave。
            AssemblyBuilder myAsmBuilder = AssemblyBuilder.DefineDynamicAssembly(myAsmName,
                AssemblyBuilderAccess.RunAndCollect);
            //AssemblyBuilder myAsmBuilder = myDomain.DefineDynamicAssembly(myAsmName,
            //                                                AssemblyBuilderAccess.RunAndSave);

            //。
            ModuleBuilder myModBuilder =
                myAsmBuilder.DefineDynamicModule(myAsmName.Name);

            //TypeBuilder。
            TypeBuilder myTypeBuilder = myModBuilder.DefineType(classType.FullName,
                                                            TypeAttributes.Public);

            //lcpiTypeBuilder。。，。
            AddPropertyToTypeBuilder(myTypeBuilder, lcpi);

            //。
            Type retval = myTypeBuilder.CreateTypeInfo();

            //，Ildasm.exe，。
            //myAsmBuilder.Save(myAsmName.Name + ".dll");
            return retval;
        }

        public class CustPropertyInfo
        {
            private string propertyName;
            private Type type;

            public CustPropertyInfo() { }

            public CustPropertyInfo(Type type, string propertyName)
            {
                this.type = type;
                this.propertyName = propertyName;
            }

            public Type Type
            {
                get { return type; }
                set { type = value; }
            }

            public string PropertyName
            {
                get { return propertyName; }
                set { propertyName = value; }
            }

            public string FieldName
            {
                get { return propertyName.Substring(0, 1).ToLower() + propertyName.Substring(1); }
            }

            public string SetPropertyMethodName
            {
                get { return "set_" + PropertyName; }
            }

            public string GetPropertyMethodName
            {
                get { return "get_" + PropertyName; }
            }
        }
    }
}
