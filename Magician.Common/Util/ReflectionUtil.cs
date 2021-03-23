using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Magician.Common.Core;

namespace Magician.Common.Util
{
    public class ReflectionUtil
    {
        private const string TARGET_FILE_POSTFIX = ".plugin.dll";

        #region GetTypeFullName

        public static string GetTypeFullName(Type t)
        {
            return t.FullName + "," + t.Assembly.FullName.Split(',')[0];
        }

        #endregion

        #region LoadDerivedInstance

        /// <summary>
        ///     LoadDerivedInstance 将程序集中所有继承自TBase的类型实例化
        /// </summary>
        /// <typeparam name="TBase">基础类型（或接口类型）</typeparam>
        /// <param name="asm">目标程序集</param>
        /// <returns>TBase实例列表</returns>
        public static IList<TBase> LoadDerivedInstance<TBase>(Assembly asm)
        {
            IList<TBase> list = new List<TBase>();

            var supType = typeof(TBase);
            foreach (var t in asm.GetTypes())
                if (supType.IsAssignableFrom(t) && !t.IsAbstract && !t.IsInterface)
                {
                    var instance = (TBase) Activator.CreateInstance(t);
                    list.Add(instance);
                }

            return list;
        }

        #endregion

        #region GetProperty

        /// <summary>
        ///     GetProperty 根据指定的属性名获取目标对象该属性的值
        /// </summary>
        public static object GetProperty(object obj, string propertyName)
        {
            var t = obj.GetType();

            return t.InvokeMember(propertyName, BindingFlags.Default | BindingFlags.GetProperty, null, obj, null);
        }

        #endregion

        #region GetFieldValue

        /// <summary>
        ///     GetFieldValue 取得目标对象的指定field的值，field可以是private
        /// </summary>
        public static object GetFieldValue(object obj, string fieldName)
        {
            var t = obj.GetType();
            var field = t.GetField(fieldName,
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance);
            if (field == null)
            {
                var msg = string.Format("The field named '{0}' not found in '{1}'.", fieldName, t);
                throw new Exception(msg);
            }

            return field.GetValue(obj);
        }

        #endregion

        #region SetFieldValue

        /// <summary>
        ///     SetFieldValue 设置目标对象的指定field的值，field可以是private
        /// </summary>
        public static void SetFieldValue(object obj, string fieldName, object val)
        {
            var t = obj.GetType();
            var field = t.GetField(fieldName,
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.SetField | BindingFlags.Instance);
            if (field == null)
            {
                var msg = string.Format("The field named '{0}' not found in '{1}'.", fieldName, t);
                throw new Exception(msg);
            }

            field.SetValue(obj, val);
        }

        #endregion

        #region GetFullMethodName

        public static string GetMethodFullName(MethodInfo method)
        {
            return string.Format("{0}.{1}()", method.DeclaringType, method.Name);
        }

        #endregion

        #region GetType

        /// <summary>
        ///     GetType  通过完全限定的类型名来加载对应的类型。typeAndAssName如"ESBasic.Filters.SourceFilter,ESBasic"。
        ///     如果为系统简单类型，则可以不带程序集名称。
        /// </summary>
        public static Type GetType(string typeAndAssName)
        {
            var names = typeAndAssName.Split(',');
            if (names.Length < 2) return Type.GetType(typeAndAssName);

            return GetType(names[0].Trim(), names[1].Trim());
        }

        /// <summary>
        ///     GetType 加载assemblyName程序集中的名为typeFullName的类型。assemblyName不用带扩展名，如果目标类型在当前程序集中，assemblyName传入null
        /// </summary>
        public static Type GetType(string typeFullName, string assemblyName)
        {
            if (assemblyName == null) return Type.GetType(typeFullName);

            //搜索当前域中已加载的程序集
            var asses = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var ass in asses)
            {
                var names = ass.FullName.Split(',');
                if (names[0].Trim() == assemblyName.Trim()) return ass.GetType(typeFullName);
            }

            //加载目标程序集
            var tarAssembly = Assembly.Load(assemblyName);
            if (tarAssembly != null) return tarAssembly.GetType(typeFullName);

            return null;
        }

        #endregion

        #region LoadDerivedType

        /// <summary>
        ///     LoadDerivedType 加载directorySearched目录下所有程序集中的所有派生自baseType的类型
        /// </summary>
        /// <param name="baseType">基类（或接口）类型</param>
        /// <param name="directorySearched">搜索的目录</param>
        /// <param name="searchChildFolder">是否搜索子目录中的程序集</param>
        /// <param name="copyToMemory">是否拷贝至内存</param>
        /// <returns>所有从BaseType派生的类型列表</returns>
        public static IList<Type> LoadDerivedType(Type baseType, string directorySearched, bool searchChildFolder,
            bool copyToMemory)
        {
            IList<Type> derivedTypeList = new List<Type>();
            if (searchChildFolder)
                LoadDerivedTypeInAllFolder(baseType, derivedTypeList, directorySearched, copyToMemory);
            else
                LoadDerivedTypeInOneFolder(baseType, derivedTypeList, directorySearched, copyToMemory);

            return derivedTypeList;
        }

        #region LoadDerivedTypeInAllFolder

        private static void LoadDerivedTypeInAllFolder(Type baseType, IList<Type> derivedTypeList, string folderPath,
            bool copyToMemory)
        {
            LoadDerivedTypeInOneFolder(baseType, derivedTypeList, folderPath, copyToMemory);
            var folders = Directory.GetDirectories(folderPath);
            if (folders.Length == 0) return;
            foreach (var nextFolder in folders)
                LoadDerivedTypeInAllFolder(baseType, derivedTypeList, nextFolder, copyToMemory);
        }

        #endregion

        #region LoadDerivedTypeInOneFolder

        private static void LoadDerivedTypeInOneFolder(Type baseType, ICollection<Type> derivedTypeList, string folderPath,
            bool copyToMemory)
        {
            var files = Directory.GetFiles(folderPath);
            foreach (var file in files)
            {
                if (!file.EndsWith(TARGET_FILE_POSTFIX)) continue;

                Assembly asm = null;

                #region Asm

                try
                {
                    if (copyToMemory)
                    {
                        var pluginStream = FileUtil.Read(file);
                        asm = Assembly.Load(pluginStream);
                    }
                    else
                    {
                        asm = Assembly.LoadFrom(file);
                    }
                }
                catch (Exception)
                {
                    // ignore;
                }

                if (asm == null) continue;

                #endregion

                var types = asm.GetTypes();

                foreach (var t in types)
                    if (t.IsSubclassOf(baseType) || baseType.IsAssignableFrom(t))
                        if (!t.IsAbstract)
                            derivedTypeList.Add(t);
            }
        }

        #endregion

        #endregion

        #region SetProperty

        /// <summary>
        ///     SetProperty 如果list中的object具有指定的propertyName属性，则设置该属性的值为proValue
        /// </summary>
        public static void SetProperty(IList<object> listObj, string propertyName, object proValue)
        {
            foreach (var target in listObj) SetProperty(target, propertyName, proValue);
        }

        public static void SetProperty(object obj, string propertyName, object proValue)
        {
            SetProperty(obj, propertyName, proValue, true);
        }

        /// <summary>
        ///     SetProperty 如果object具有指定的propertyName属性，则设置该属性的值为proValue
        /// </summary>
        public static void SetProperty(object obj, string propertyName, object proValue, bool ignoreError)
        {
            var t = obj.GetType();
            var pro = t.GetProperty(propertyName);
            if (pro == null || !pro.CanWrite)
            {
                if (ignoreError) return;
                var msg = string.Format("The setter of property named '{0}' not found in '{1}'.", propertyName, t);
                throw new Exception(msg);
            }

            #region 尝试转换类型

            try
            {
                proValue = TypeUtil.ChangeType(proValue, pro.PropertyType);
            }
            catch
            {
                // ignored
            }

            #endregion

            object[] args = {proValue};
            t.InvokeMember(propertyName, BindingFlags.Public | BindingFlags.IgnoreCase |
                                         BindingFlags.Instance | BindingFlags.SetProperty, null, obj, args);
        }

        public static void SetProperty(object obj, Dictionary<string, object> dicProp)
        {
            if (dicProp == null) return;

            var t = obj.GetType();
            var piArray = t.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            //遍历属性集合，修改相应属性的值
            foreach (var pi in piArray)
            {
                if (!dicProp.ContainsKey(pi.Name)) continue;

                var proValue = dicProp[pi.Name];
                try
                {
                    proValue = TypeUtil.ChangeType(proValue, pi.PropertyType);
                }
                catch
                {
                    // ignored
                }

                object[] args = { proValue };
                t.InvokeMember(pi.Name, BindingFlags.Public | BindingFlags.IgnoreCase |
                                             BindingFlags.Instance | BindingFlags.SetProperty, null, obj, args);
            }
        }

        #endregion

        #region CopyProperty

        /// <summary>
        ///     CopyProperty 将source中的属性的值赋给target上同名的属性
        ///     使用CopyProperty可以方便的实现拷贝构造函数
        /// </summary>
        public static void CopyProperty(object source, object target)
        {
            CopyProperty(source, target, null);
        }

        /// <summary>
        ///     CopyProperty 将source中的属性的值赋给target上想匹配的属性，匹配关系通过propertyMapItemList确定
        /// </summary>
        public static void CopyProperty(object source, object target, IList<KeyValuePair> propertyMapItemList)
        {
            var sourceType = source.GetType();
            var sourcePros = sourceType.GetProperties();

            if (propertyMapItemList != null)
                foreach (var item in propertyMapItemList)
                {
                    var val = GetProperty(source, item.Key);
                    SetProperty(target, item.Value.ToString(), val);
                }
            else
                foreach (var sourceProperty in sourcePros)
                    if (sourceProperty.CanRead)
                    {
                        var val = GetProperty(source, sourceProperty.Name);
                        SetProperty(target, sourceProperty.Name, val);
                    }
        }

        #endregion

        #region GetAllMethods、SearchMethod

        /// <summary>
        ///     GetAllMethods 获取接口的所有方法信息，包括继承的
        /// </summary>
        public static IList<MethodInfo> GetAllMethods(params Type[] interfaceTypes)
        {
            foreach (var interfaceType in interfaceTypes)
                if (!interfaceType.IsInterface)
                    throw new Exception("Target Type must be interface!");

            IList<MethodInfo> list = new List<MethodInfo>();
            foreach (var interfaceType in interfaceTypes) DistillMethods(interfaceType, ref list);

            return list;
        }

        private static void DistillMethods(Type interfaceType, ref IList<MethodInfo> methodList)
        {
            foreach (var meth in interfaceType.GetMethods())
            {
                var isExist = false;
                foreach (var temp in methodList)
                    if (temp.Name == meth.Name && temp.ReturnType == meth.ReturnType)
                    {
                        var para1 = temp.GetParameters();
                        var para2 = meth.GetParameters();
                        if (para1.Length == para2.Length)
                        {
                            var same = true;
                            for (var i = 0; i < para1.Length; i++)
                                if (para1[i].ParameterType != para2[i].ParameterType)
                                    same = false;

                            if (same)
                            {
                                isExist = true;
                                break;
                            }
                        }
                    }

                if (!isExist) methodList.Add(meth);
            }

            foreach (var superInterfaceType in interfaceType.GetInterfaces())
                DistillMethods(superInterfaceType, ref methodList);
        }


        /// <summary>
        ///     SearchGenericMethodInType 搜索指定类型定义的泛型方法，不包括继承的。
        /// </summary>
        public static MethodInfo SearchGenericMethodInType(Type originType, string methodName, Type[] argTypes)
        {
            foreach (var method in originType.GetMethods())
                if (method.ContainsGenericParameters && method.Name == methodName)
                {
                    var succeed = true;
                    var paras = method.GetParameters();
                    if (paras.Length == argTypes.Length)
                    {
                        for (var i = 0; i < paras.Length; i++)
                            if (!paras[i].ParameterType.IsGenericParameter) //跳过泛型参数
                            {
                                if (paras[i].ParameterType.IsGenericType) //如果参数本身就是泛型类型，如IList<T>
                                {
                                    if (paras[i].ParameterType.GetGenericTypeDefinition() !=
                                        argTypes[i].GetGenericTypeDefinition())
                                    {
                                        succeed = false;
                                        break;
                                    }
                                }
                                else //普通类型的参数
                                {
                                    if (paras[i].ParameterType != argTypes[i])
                                    {
                                        succeed = false;
                                        break;
                                    }
                                }
                            }

                        if (succeed) return method;
                    }
                }

            return null;
        }

        /// <summary>
        ///     SearchMethod 包括被继承的所有方法，也包括泛型方法。
        /// </summary>
        public static MethodInfo SearchMethod(Type originType, string methodName, Type[] argTypes)
        {
            var meth = originType.GetMethod(methodName, argTypes);
            if (meth != null) return meth;

            meth = SearchGenericMethodInType(originType, methodName, argTypes);
            if (meth != null) return meth;

            //搜索基类 
            var baseType = originType.BaseType;
            while (baseType != typeof(object) && baseType != null)
            {
                var target = baseType.GetMethod(methodName, argTypes);
                if (target != null) return target;

                target = SearchGenericMethodInType(baseType, methodName, argTypes);
                if (target != null) return target;

                baseType = baseType.BaseType;
            }

            //搜索基接口
            if (originType.GetInterfaces().Length > 0)
            {
                var list = GetAllMethods(originType.GetInterfaces());
                foreach (var theMethod in list)
                {
                    if (theMethod.Name != methodName) continue;
                    var args = theMethod.GetParameters();
                    if (args.Length != argTypes.Length) continue;

                    var correctArgType = true;
                    for (var i = 0; i < args.Length; i++)
                        if (args[i].ParameterType != argTypes[i])
                        {
                            correctArgType = false;
                            break;
                        }

                    if (correctArgType) return theMethod;
                }
            }

            return null;
        }

        #endregion
    }
}