using System;

namespace Magician.Common.Util
{
    public class TypeUtil
    {
        #region IsSimpleType

        /// <summary>
        ///     IsSimpleType 是否为简单类型：数值、字符、字符串、日期、布尔、枚举、Type
        /// </summary>
        public static bool IsSimpleType(Type t)
        {
            if (IsNumericType(t)) return true;

            if (t == typeof(char)) return true;

            if (t == typeof(string)) return true;

            if (t == typeof(bool)) return true;

            if (t == typeof(DateTime)) return true;

            if (t == typeof(Type)) return true;

            if (t.IsEnum) return true;

            return false;
        }

        #endregion

        #region ConvertToBool

        public static bool ConvertToBool(object obj)
        {
            var t = obj.GetType();
            if (t == typeof(string)) return bool.Parse(obj.ToString());

            if (IsNumericType(t)) return obj.ToString() != "0";

            return (bool) obj;
        }

        #endregion

        #region IsNumericType 是否为数值类型

        public static bool IsNumericType(Type destDataType)
        {
            if (destDataType == typeof(int) || destDataType == typeof(uint) || destDataType == typeof(double)
                || destDataType == typeof(short) || destDataType == typeof(ushort) || destDataType == typeof(decimal)
                || destDataType == typeof(long) || destDataType == typeof(ulong) || destDataType == typeof(float)
                || destDataType == typeof(byte) || destDataType == typeof(sbyte))
                return true;

            return false;
        }

        #endregion

        #region IsIntegerCompatibleType 是否为整数兼容类型

        public static bool IsIntegerCompatibleType(Type destDataType)
        {
            if (destDataType == typeof(int) || destDataType == typeof(uint) || destDataType == typeof(short) ||
                destDataType == typeof(ushort)
                || destDataType == typeof(long) || destDataType == typeof(ulong) || destDataType == typeof(byte) ||
                destDataType == typeof(sbyte))
                return true;

            return false;
        }

        #endregion

        #region GetClassSimpleName

        /// <summary>
        ///     GetClassSimpleName 获取class的声明名称，如 Person
        /// </summary>
        public static string GetClassSimpleName(Type t)
        {
            var parts = t.ToString().Split('.');
            return parts[parts.Length - 1];
        }

        #endregion

        #region IsFixLength

        public static bool IsFixLength(Type destDataType)
        {
            if (IsNumericType(destDataType)) return true;

            if (destDataType == typeof(byte[])) return true;

            if (destDataType == typeof(DateTime) || destDataType == typeof(bool)) return true;

            return false;
        }

        #endregion

        #region ChangeType

        /// <summary>
        ///     ChangeType 对System.Convert.ChangeType进行了增强，支持(0,1)到bool的转换，字符串->枚举、int->枚举、字符串->Type
        /// </summary>
        public static object ChangeType(object val, Type targetType)
        {
            #region null

            if (val == null) return null;

            #endregion

            if (targetType.IsAssignableFrom(val.GetType())) return val;

            #region Same Type

            if (targetType == val.GetType()) return val;

            #endregion

            #region bool 1,0

            if (targetType == typeof(bool))
            {
                if (val.ToString() == "0") return false;

                if (val.ToString() == "1") return true;
            }

            #endregion

            #region Enum

            if (targetType.IsEnum)
            {
                int intVal;
                var suc = int.TryParse(val.ToString(), out intVal);
                if (!suc)
                    return Enum.Parse(targetType, val.ToString());
                return val;
            }

            #endregion

            #region Type

            if (targetType == typeof(Type)) return ReflectionUtil.GetType(val.ToString());

            #endregion

            if (targetType == typeof(IComparable)) return val;

            //将double赋值给数值型的DataRow的字段是可以的，但是通过反射赋值给object的非double的其它数值类型的属性，却不行        
            return Convert.ChangeType(val, targetType);
        }

        #endregion

        #region GetDefaultValue

        public static object GetDefaultValue(Type destType)
        {
            if (IsNumericType(destType)) return 0;

            if (destType == typeof(string)) return "";

            if (destType == typeof(bool)) return false;

            if (destType == typeof(DateTime)) return DateTime.Now;

            if (destType == typeof(Guid)) return Guid.NewGuid();

            if (destType == typeof(TimeSpan)) return TimeSpan.Zero;

            return null;
        }

        #endregion

        #region GetDefaultValueString

        public static string GetDefaultValueString(Type destType)
        {
            if (IsNumericType(destType)) return "0";

            if (destType == typeof(string)) return "\"\"";

            if (destType == typeof(bool)) return "false";

            if (destType == typeof(DateTime)) return "DateTime.Now";

            if (destType == typeof(Guid)) return "System.Guid.NewGuid()";

            if (destType == typeof(TimeSpan)) return "System.TimeSpan.Zero";


            return "null";
        }

        #endregion

        #region GetTypeByRegularName

        /// <summary>
        ///     GetTypeByFullString 通过类型的完全名称获取类型，regularName如"ESBasic.Filters.SourceFilter,ESBasic"
        /// </summary>
        public static Type GetTypeByRegularName(string regularName)
        {
            return ReflectionUtil.GetType(regularName);
        }

        #endregion

        #region GetTypeRegularName

        /// <summary>
        ///     GetTypeRegularName 获取类型的完全名称，如"ESBasic.Filters.SourceFilter,ESBasic"
        /// </summary>
        public static string GetTypeRegularName(Type destType)
        {
            var assName = destType.Assembly.FullName.Split(',')[0];

            return string.Format("{0},{1}", destType, assName);
        }

        public static string GetTypeRegularNameOf(object obj)
        {
            var destType = obj.GetType();
            return GetTypeRegularName(destType);
        }

        #endregion
    }
}