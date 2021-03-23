using System.ComponentModel;
using CCWin.SkinControl;
using Magician.Common.Core;
using Magician.Common.Style;

namespace Magician.Common.CustomControl
{
    public class MagicianTextBox : SkinTextBox
    {
        [Description("样式"), Category("可以使用设定好的样式")]
        public BaseStyle.EPresetStyle PresetStyle { get; set; }

        public MagicianTextBox()
        {
            this.ApplyControlStyle(PresetStyle);
        }
    }
}
