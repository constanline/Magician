using System.Collections.Generic;
using System.Drawing;

namespace Magician.Common.Style
{
    public class TextBoxStyle : BaseStyle
    {
        public override Dictionary<string, object> GetStyle()
        {
            var dicStyle = new Dictionary<string, object>
            {
                {"Font", new Font("微软雅黑", 10.5f)},
                {"WaterColor", Color.FromArgb(127, 127, 127)},
                {"ForeColor", SystemColors.WindowText}
            };

            return dicStyle;
        }
    }
}