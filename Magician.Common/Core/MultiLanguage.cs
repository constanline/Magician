using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace Magician.Common.Core
{
    public class MultiLanguage
    {
        internal static Action LanguageChange;

        //当前默认语言
        public static string CurrentLanguage = "";

        /// <summary>
        /// 修改默认语言
        /// </summary>
        /// <param name="lang">待设置默认语言</param>
        public static void SetCurrentLanguage(string lang)
        {
            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(lang);
            CurrentLanguage = lang;
            LanguageChange?.Invoke();
        }

        /// <summary>
        /// 加载语言
        /// </summary>
        /// <param name="control">控件</param>
        /// <param name="resources">语言资源</param>
        internal static void Loading(Control control, ComponentResourceManager resources)
        {
            var ms = control as MenuStrip;
            if (ms != null)
            {
                //将资源与控件对应
                resources.ApplyResources(control, ms.Name);
                if (ms.Items.Count > 0)
                    foreach (ToolStripMenuItem c in ms.Items)
                        //遍历菜单
                        Loading(c, resources);
            }

            foreach (Control c in control.Controls)
            {
                resources.ApplyResources(c, c.Name);
                Loading(c, resources);
            }
        }

        /// <summary>
        ///     遍历菜单
        /// </summary>
        /// <param name="toolStripDropDownItem">菜单项</param>
        /// <param name="resources">语言资源</param>
        private static void Loading(ToolStripDropDownItem toolStripDropDownItem, ComponentResourceManager resources)
        {
            if (toolStripDropDownItem == null) return;
            resources.ApplyResources(toolStripDropDownItem, toolStripDropDownItem.Name);
            if (toolStripDropDownItem.DropDownItems.Count <= 0) return;
            foreach (ToolStripMenuItem c in toolStripDropDownItem.DropDownItems) Loading(c, resources);
        }
    }
}