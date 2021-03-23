using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using CCWin.SkinClass;

namespace Magician.Common.Style
{
    public class BaseStyle
    {
        public enum EPresetStyle
        {
            [Description("默认")] Default = 0,
            [Description("不使用")] None = 9
        }

        public virtual Dictionary<string, object> GetStyle()
        {
            return new Dictionary<string, object>();
        }
    }
}