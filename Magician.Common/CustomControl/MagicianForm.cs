using System;
using System.ComponentModel;
using System.Windows.Forms;
using CCWin;
using Magician.Common.Core;
using Magician.Common.Style;

namespace Magician.Common.CustomControl
{
    public partial class MagicianForm : CCSkinMain, IMagicianControl
    {
        public ComponentResourceManager Resources;

        [Description("样式")]
        [Category("可以使用设定好的样式")]
        public BaseStyle.EPresetStyle PresetStyle { get; set; }

        public MagicianForm()
        {
            InitializeComponent();

            this.ApplyControlStyle(PresetStyle);
            Load += MagicianForm_Load;
            FormClosed += MagicianForm_FormClosed;
            Resources = new ComponentResourceManager(GetType());
        }

        /// <summary>
        ///     获取当前语言键值对应字符串
        /// </summary>
        /// <param name="key">键值</param>
        /// <returns></returns>
        protected string GetLanguageString(string key)
        {
            return Resources.GetString(key);
        }

        private void MagicianForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            MultiLanguage.LanguageChange -= form_LanguageChange;
        }

        private void MagicianForm_Load(object sender, EventArgs e)
        {
            form_LanguageChange();
            MultiLanguage.LanguageChange += form_LanguageChange;
        }

        private void form_LanguageChange()
        {
            this.LoadLanguage();
        }
    }
}