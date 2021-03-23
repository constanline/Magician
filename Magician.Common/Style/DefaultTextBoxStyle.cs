using System.Collections.Generic;
using System.Drawing;

namespace Magician.Common.Style
{
    internal class DefaultTextBoxStyle : TextBoxStyle
    {
        public override Dictionary<string, object> GetStyle()
        {
            var dicStyle = new Dictionary<string, object>
            {
                {"BackColor", Color.Transparent}
            };

            return dicStyle;
        }
    }
}