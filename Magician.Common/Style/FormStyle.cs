using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using CCWin.SkinClass;

namespace Magician.Common.Style
{
    public class FormStyle : BaseStyle
    {
        public override Dictionary<string, object> GetStyle()
        {
            var dicStyle = new Dictionary<string, object>
            {
                {"AutoScaleMode", AutoScaleMode.None},
                {"BackColor", SystemColors.Control},
                {"CaptionBackColorBottom", Color.Transparent},
                {"CaptionBackColorTop", Color.FromArgb(225, 227, 248)},
                {"Font", new Font("微软雅黑", 10.5f)},
                {"Radius", 0},
                {"RoundStyle", RoundStyle.None}
            };

            return dicStyle;
        }
    }
}
