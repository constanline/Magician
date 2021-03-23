using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using Magician.Common.CustomControl;
using Magician.Common.Style;
using Magician.Common.Util;

namespace Magician.Common.Core
{
    public static class Extension
    {
        public static DialogResult MaskDialog(this Form form)
        {
            var maskForm = new MaskForm(form);
            return maskForm.ShowDialog();
        }

        /// <summary>
        ///     加载语言
        /// </summary>
        /// <param name="form">加载语言的窗口</param>
        internal static void LoadLanguage(this MagicianForm form)
        {
            form.Resources.ApplyResources(form, "$this");
            MultiLanguage.Loading(form, form.Resources);
        }

        public static void ApplyControlStyle<T>(this T model, BaseStyle.EPresetStyle presetStyle) where T : Control
        {
            model.ApplyProp(StyleManager.GetStyle(typeof(T), presetStyle));
        }

        public static void ApplyProp<T>(this T model, Dictionary<string, object> dicProp)
        {
            ReflectionUtil.SetProperty(model, dicProp);
        }

        public static bool Has<T>(this Enum type, T value)
        {
            try
            {
                return ((int) (object) type & (int) (object) value) == (int) (object) value;
            }
            catch
            {
                return false;
            }
        }

        public static bool Is<T>(this Enum type, T value)
        {
            try
            {
                return (int) (object) type == (int) (object) value;
            }
            catch
            {
                return false;
            }
        }


        public static T Add<T>(this Enum type, T value)
        {
            try
            {
                return (T) (object) ((int) (object) type | (int) (object) value);
            }
            catch (Exception ex)
            {
                throw new ArgumentException(
                    string.Format(
                        "Could not append value from enumerated type '{0}'.",
                        typeof(T).Name
                    ), ex);
            }
        }


        public static T Remove<T>(this Enum type, T value)
        {
            try
            {
                return (T) (object) ((int) (object) type & ~(int) (object) value);
            }
            catch (Exception ex)
            {
                throw new ArgumentException(
                    string.Format(
                        "Could not remove value from enumerated type '{0}'.",
                        typeof(T).Name
                    ), ex);
            }
        }
        /// <summary>  
        /// 获取枚举变量值的 Description 属性  
        /// </summary>  
        /// <param name="obj">枚举变量</param>  
        /// <returns>如果包含 Description 属性，则返回 Description 属性的值，否则返回枚举变量值的名称</returns>  
        public static string GetDescription(this Enum obj)
        {
            try
            {
                var enumType = obj.GetType();

                var fi = enumType.GetField(Enum.GetName(enumType, obj));
                var dna = (DescriptionAttribute) Attribute.GetCustomAttribute(fi, typeof(DescriptionAttribute));
                if (dna != null && string.IsNullOrEmpty(dna.Description) == false)
                    return dna.Description;
                return null;
            }
            catch
            {
                return "未设定";
            }

        }

        ///// <summary>
        ///// 获取当前语言键值对应字符串
        ///// </summary>
        ///// <param name="form">窗体</param>
        ///// <param name="key">键值</param>
        ///// <returns></returns>
        //public static string GetLanguageString(this MultiLanguageForm form, string key)
        //{
        //    return form.Resources.GetString(key);
        //}
    }
}