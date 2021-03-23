using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Magician.Common.Style
{
    public class DefaultFormStyle : FormStyle
    {
        public override Dictionary<string, object> GetStyle()
        {
            var dicStyle = base.GetStyle();
            dicStyle["BackColor"] = Color.White;

            return dicStyle;
        }

    }
}