using System;
using System.Collections.Generic;
using Magician.Common.CustomControl;

namespace Magician.Common.Style
{
    public class StyleManager
    {
        private static Dictionary<string, StructStyle> _dicStructStyles;

        public static Dictionary<string, object> GetStyle(Type controlType,
            BaseStyle.EPresetStyle presetStyle = BaseStyle.EPresetStyle.Default)
        {
            if (_dicStructStyles == null)
            {
                _dicStructStyles = new Dictionary<string, StructStyle>();
                InitStyle();
            }

            if (presetStyle == BaseStyle.EPresetStyle.None || !_dicStructStyles.ContainsKey(presetStyle.ToString()))
                return null;

            if (typeof(MagicianForm).IsAssignableFrom(controlType))
                return _dicStructStyles[presetStyle.ToString()].FormStyle.GetStyle();

            if (typeof(MagicianTextBox).IsAssignableFrom(controlType))
                return _dicStructStyles[presetStyle.ToString()].TextBoxStyle.GetStyle();
            return null;
        }

        private static void InitStyle()
        {
            _dicStructStyles["Default"] = new StructStyle
            {
                FormStyle = new DefaultFormStyle(), TextBoxStyle = new DefaultTextBoxStyle()
            };
        }
    }

    internal struct StructStyle
    {
        public FormStyle FormStyle;
        public TextBoxStyle TextBoxStyle;
    }
}