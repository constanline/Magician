using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;
using Magician.Common.Util;

namespace Magician.Common.Emit
{
    public class PropertyQuickerEmitter
    {
        private int _number;
        private readonly string assemblyName = "DynamicPropertyQuickerAssembly";

        private readonly IDictionary<Type, Type>
            _dicPropertyQuickerType = new Dictionary<Type, Type>(); //TEntity -- PropertyQuicker Type          

        private readonly AssemblyBuilder _dynamicAssembly;
        private readonly ModuleBuilder _moduleBuilder;
        private readonly bool _saveFile;
        private readonly string _theFileName;

        #region CreatePropertyQuickerType

        /// <summary>
        ///     CreatePropertyQuickerType 为EntityType发射一个实现了IPropertyQuicker[TEntity]接口的类型。
        /// </summary>
        public Type CreatePropertyQuickerType(Type entityType)
        {
            lock (_dicPropertyQuickerType)
            {
                if (_dicPropertyQuickerType.ContainsKey(entityType)) return _dicPropertyQuickerType[entityType];

                var orMappingType = DoCreateOrMappingType(entityType);
                _dicPropertyQuickerType.Add(entityType, orMappingType);

                return orMappingType;
            }
        }

        #endregion

        #region Save

        public void Save()
        {
            if (_saveFile) _dynamicAssembly.Save(_theFileName);
        }

        #endregion

        #region DoCreateORMappingType

        private Type DoCreateOrMappingType(Type entityType)
        {
            try
            {
                Interlocked.Increment(ref _number);
                var parentGenericType = typeof(IPropertyQuicker<>);
                var parentClosedType = parentGenericType.MakeGenericType(entityType);

                var typeBuilder = _moduleBuilder.DefineType(
                    "Magician.Common.DynaAssembly." + TypeUtil.GetClassSimpleName(entityType) + "ORMapping" + _number,
                    TypeAttributes.Public | TypeAttributes.Class);
                typeBuilder.AddInterfaceImplementation(parentClosedType);

                #region Emit Ctor

                var ctor = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, null);
                var ctorGen = ctor.GetILGenerator();
                ctorGen.Emit(OpCodes.Ret);

                #endregion

                var baseMethod1 = ReflectionUtil.SearchMethod(parentClosedType, "GetPropertyValue",
                    new[] {entityType, typeof(string)});
                EmitGetPropertyValueMethod(typeBuilder, baseMethod1, entityType);

                var baseMethod2 =
                    parentClosedType.GetMethod(
                        "SetPropertyValue"); // ReflectionUtil.SearchMethod(parentClosedType, "SetPropertyValue", new Type[] { entityType, typeof(string), typeof(object) });
                EmitSetPropertyValueMethod(typeBuilder, baseMethod2, entityType);

                var baseMethod4 = ReflectionUtil.SearchMethod(parentClosedType, "GetValue",
                    new[] {typeof(object), typeof(string)});
                EmitGetValueMethod(typeBuilder, baseMethod4, entityType, baseMethod1);

                var baseMethod5 = ReflectionUtil.SearchMethod(parentClosedType, "SetValue",
                    new[] {typeof(object), typeof(string), typeof(object)});
                EmitSetValueMethod(typeBuilder, baseMethod5, entityType, baseMethod2);


                var target = typeBuilder.CreateType();

                return target;
            }
            catch (Exception ee)
            {
                throw new Exception(string.Format("Error Emitting ORMapping for {0}", entityType), ee);
            }
        }

        #endregion

        #region Ctor

        public PropertyQuickerEmitter() : this(false)
        {
        }

        public PropertyQuickerEmitter(bool save)
        {
            _saveFile = save;
            _theFileName = assemblyName + ".dll";

            var assemblyBuilderAccess = _saveFile ? AssemblyBuilderAccess.RunAndSave : AssemblyBuilderAccess.Run;
            _dynamicAssembly =
                AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName(assemblyName), assemblyBuilderAccess);
            if (_saveFile)
                _moduleBuilder = _dynamicAssembly.DefineDynamicModule("MainModule", _theFileName);
            else
                _moduleBuilder = _dynamicAssembly.DefineDynamicModule("MainModule");
        }

        #endregion

        #region Emit

        #region EmitGetPropertyValueMethod

        private void EmitGetPropertyValueMethod(TypeBuilder typeBuilder, MethodInfo baseMethod, Type entityType)
        {
            var methodBuilder = typeBuilder.DefineMethod("GetPropertyValue",
                baseMethod.Attributes & ~MethodAttributes.Abstract, baseMethod.CallingConvention, baseMethod.ReturnType,
                EmitHelper.GetParametersType(baseMethod));
            var compareStringMethod = typeof(string).GetMethod("op_Equality", new[] {typeof(string), typeof(string)});
            var ilGenerator = methodBuilder.GetILGenerator();
            ilGenerator.DeclareLocal(typeof(object));
            ilGenerator.Emit(OpCodes.Nop);

            var tempPros =
                entityType.GetProperties(BindingFlags.Public | BindingFlags.Instance |
                                         BindingFlags
                                             .GetProperty); // ESBasic.Helpers.TypeUtil.ConvertListToArray<PropertyInfo>(columnList);
            IList<PropertyInfo> proList = new List<PropertyInfo>();
            foreach (var propertyInfo in tempPros)
                if (propertyInfo.CanWrite && propertyInfo.CanRead)
                    proList.Add(propertyInfo);

            var pros = proList.ToArray();

            var loadNullLabel = ilGenerator.DefineLabel();
            var retLabel = ilGenerator.DefineLabel();
            var labels = new Label[pros.Length + 1];
            for (var i = 0; i < pros.Length; i++) labels[i] = ilGenerator.DefineLabel();
            labels[pros.Length] = loadNullLabel;

            for (var i = 0; i < pros.Length; i++)
            {
                var property = pros[i];
                ilGenerator.MarkLabel(labels[i]);
                ilGenerator.Emit(OpCodes.Ldarg_2);
                var proName = property.Name;
                ilGenerator.Emit(OpCodes.Ldstr, proName);
                ilGenerator.EmitCall(OpCodes.Call, compareStringMethod, new[] {typeof(string), typeof(string)});
                ilGenerator.Emit(OpCodes.Brfalse, labels[i + 1]);

                ilGenerator.Emit(OpCodes.Nop);
                if (entityType.IsValueType)
                    ilGenerator.Emit(OpCodes.Ldarga, 1);
                else
                    ilGenerator.Emit(OpCodes.Ldarg_1);
                var getPropertyMethod = entityType.GetMethod("get_" + proName, new Type[] { });
                if (entityType.IsValueType)
                    ilGenerator.EmitCall(OpCodes.Call, getPropertyMethod, new Type[] { });
                else
                    ilGenerator.EmitCall(OpCodes.Callvirt, getPropertyMethod, new Type[] { });
                if (property.PropertyType.IsValueType) ilGenerator.Emit(OpCodes.Box, property.PropertyType);

                ilGenerator.Emit(OpCodes.Stloc_0);
                ilGenerator.Emit(OpCodes.Br, retLabel);
            }

            ilGenerator.MarkLabel(loadNullLabel);
            ilGenerator.Emit(OpCodes.Ldnull);
            ilGenerator.Emit(OpCodes.Stloc_0);
            ilGenerator.Emit(OpCodes.Br, retLabel);

            ilGenerator.MarkLabel(retLabel);
            ilGenerator.Emit(OpCodes.Ldloc_0);
            ilGenerator.Emit(OpCodes.Ret);
            typeBuilder.DefineMethodOverride(methodBuilder, baseMethod);
        }

        #endregion

        #region EmitSetPropertyValueMethod

        private void EmitSetPropertyValueMethod(TypeBuilder typeBuilder, MethodInfo baseMethod, Type entityType)
        {
            var methodBuilder = typeBuilder.DefineMethod("SetPropertyValue",
                baseMethod.Attributes & ~MethodAttributes.Abstract, baseMethod.CallingConvention, baseMethod.ReturnType,
                EmitHelper.GetParametersType(baseMethod));
            var compareStringMethod = typeof(string).GetMethod("op_Equality", new[] {typeof(string), typeof(string)});
            var changeTypeMethod = typeof(TypeUtil).GetMethod("ChangeType", new[] {typeof(Type), typeof(object)});
            var ilGenerator = methodBuilder.GetILGenerator();

            ilGenerator.Emit(OpCodes.Nop);

            var tempPros =
                entityType.GetProperties(BindingFlags.Public | BindingFlags.Instance |
                                         BindingFlags
                                             .GetProperty); // ESBasic.Helpers.TypeUtil.ConvertListToArray<PropertyInfo>(columnList);
            IList<PropertyInfo> proList = new List<PropertyInfo>();
            foreach (var propertyInfo in tempPros)
                if (propertyInfo.CanWrite && propertyInfo.CanRead)
                    proList.Add(propertyInfo);
            var pros = proList.ToArray();

            var retLabel = ilGenerator.DefineLabel();
            var labels = new Label[pros.Length + 1];
            for (var i = 0; i < pros.Length; i++) labels[i] = ilGenerator.DefineLabel();
            labels[pros.Length] = retLabel;

            for (var i = 0; i < pros.Length; i++)
            {
                var property = pros[i];
                ilGenerator.MarkLabel(labels[i]);
                ilGenerator.Emit(OpCodes.Ldarg_2);
                var proName = property.Name;
                ilGenerator.Emit(OpCodes.Ldstr, proName);
                ilGenerator.EmitCall(OpCodes.Call, compareStringMethod, new[] {typeof(string), typeof(string)});
                ilGenerator.Emit(OpCodes.Brfalse, labels[i + 1]);

                ilGenerator.Emit(OpCodes.Nop);

                ilGenerator.Emit(OpCodes.Ldarg_1);
                EmitHelper.LoadType(ilGenerator, property.PropertyType);
                ilGenerator.Emit(OpCodes.Ldarg_3);
                ilGenerator.EmitCall(OpCodes.Call, changeTypeMethod,
                    new[] {typeof(Type), typeof(object)}); //先将object转换到正确的类型，即使还是一个object

                #region 类型转换 这一段是必须的，否则会导致内存状态损坏

                //注意：TypeUtil.ChangeType返回的是object。
                if (property.PropertyType.IsValueType) //值类型，则拆箱
                {
                    ilGenerator.Emit(OpCodes.Unbox_Any, property.PropertyType);
                }
                else if (property.PropertyType == typeof(byte[]) || property.PropertyType == typeof(string))
                {
                    ilGenerator.Emit(OpCodes.Castclass, property.PropertyType);
                }
                else if (property.PropertyType == typeof(object) || property.PropertyType.IsClass ||
                         property.PropertyType.IsGenericType)
                {
                    //do nothing .对应sql_variant
                }
                else
                {
                    var toStringMethod = typeof(object).GetMethod("ToString");
                    ilGenerator.EmitCall(OpCodes.Callvirt, toStringMethod, null);

                    //类型转换
                    if (property.PropertyType != typeof(string))
                    {
                        var parseMethod = property.PropertyType.GetMethod("Parse", new[] {typeof(string)});
                        ilGenerator.EmitCall(OpCodes.Callvirt, parseMethod, new[] {typeof(string)});
                    }
                }

                #endregion

                var setPropertyMethod = entityType.GetMethod("set_" + proName, new[] {property.PropertyType});
                if (entityType.IsValueType)
                    ilGenerator.EmitCall(OpCodes.Call, setPropertyMethod, new[] {property.PropertyType});
                else
                    ilGenerator.EmitCall(OpCodes.Callvirt, setPropertyMethod, new[] {property.PropertyType});

                ilGenerator.Emit(OpCodes.Br, retLabel);
            }

            ilGenerator.MarkLabel(retLabel);
            ilGenerator.Emit(OpCodes.Ret);
            typeBuilder.DefineMethodOverride(methodBuilder, baseMethod);
        }

        #endregion

        #region EmitSetValueMethod

        private void EmitSetValueMethod(TypeBuilder typeBuilder, MethodInfo baseMethod, Type entityType,
            MethodInfo setPropertyValueMethod)
        {
            var methodBuilder = typeBuilder.DefineMethod("SetValue", baseMethod.Attributes & ~MethodAttributes.Abstract,
                baseMethod.CallingConvention, baseMethod.ReturnType, EmitHelper.GetParametersType(baseMethod));
            var ilGenerator = methodBuilder.GetILGenerator();
            ilGenerator.Emit(OpCodes.Ldarg_0);
            ilGenerator.Emit(OpCodes.Ldarg_1);
            ilGenerator.Emit(OpCodes.Castclass, entityType);
            ilGenerator.Emit(OpCodes.Ldarg_2);
            ilGenerator.Emit(OpCodes.Ldarg_3);
            ilGenerator.Emit(OpCodes.Callvirt, setPropertyValueMethod);

            ilGenerator.Emit(OpCodes.Nop);
            ilGenerator.Emit(OpCodes.Ret);
            typeBuilder.DefineMethodOverride(methodBuilder, baseMethod);
        }

        #endregion

        #region EmitGetValueMethod

        private void EmitGetValueMethod(TypeBuilder typeBuilder, MethodInfo baseMethod, Type entityType,
            MethodInfo getPropertyValueMethod)
        {
            var methodBuilder = typeBuilder.DefineMethod("GetValue", baseMethod.Attributes & ~MethodAttributes.Abstract,
                baseMethod.CallingConvention, baseMethod.ReturnType, EmitHelper.GetParametersType(baseMethod));
            var ilGenerator = methodBuilder.GetILGenerator();
            ilGenerator.Emit(OpCodes.Ldarg_0);
            ilGenerator.Emit(OpCodes.Ldarg_1);

            if (entityType.IsValueType)
                ilGenerator.Emit(OpCodes.Unbox_Any, entityType);
            else
                ilGenerator.Emit(OpCodes.Castclass, entityType);


            ilGenerator.Emit(OpCodes.Ldarg_2);
            ilGenerator.Emit(OpCodes.Callvirt, getPropertyValueMethod);

            ilGenerator.Emit(OpCodes.Ret);
            typeBuilder.DefineMethodOverride(methodBuilder, baseMethod);
        }

        #endregion

        #endregion
    }
}