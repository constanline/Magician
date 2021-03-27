using System;
using System.ComponentModel;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using CCWin.SkinControl;

namespace Magician.Common.CustomControl
{
    public partial class NumberText : SkinTextBox
    {
        #region 字段
        readonly ErrorProvider ep;

        string _errorText = string.Empty;

        bool _showErrorProvider = true;

        string pattern;

        string tip = string.Empty;

        int decimalPlaces = 0;

        int maxValue = 100;

        int minValue = 0;
        #endregion

        #region 属性
        public decimal? Value
        {
            get
            {
                if (decimal.TryParse(Text, out decimal val))
                {
                    return val;
                }
                return null;
            }
        }

        public byte? ByteValue
        {
            get
            {
                decimal? tmp = Value;
                if (tmp == null)
                {
                    return null;
                }
                return Convert.ToByte(tmp);
            }
        }

        public int? IntValue
        {
            get
            {
                decimal? tmp = Value;
                if (tmp == null)
                {
                    return null;
                }
                return Convert.ToInt32(tmp);
            }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Description("文本"), Category("文本")]
        public new int MaxLength
        {
            get;
            set;
        }

        [Description("文本"), Category("文本")]
        public bool ShowErrorProvider
        {
            get
            {
                return _showErrorProvider;
            }
            set
            {
                if (!value)
                    TipError();
                else
                    TipError(_errorText);
                _showErrorProvider = value;
            }
        }

        [Description("文本"), Category("文本")]
        public override string Text
        {
            get
            {
                return SkinTxt.Text;
            }
            set
            {
                if (pattern == null)
                {
                    CombinePattern();
                }
                if (!CheckValid(value))
                {
                    TipError(tip);
                    return;
                }
                SkinTxt.Text = value;
            }
        }


        [Description("保留小数位数"), Category("小数位数")]
        public int DecimalPlaces
        {
            get
            {
                return decimalPlaces;
            }
            set
            {
                if (value <= 0)
                    decimalPlaces = 0;
                else if (value >= 4)
                    decimalPlaces = 4;
                else
                    decimalPlaces = value;

                CombinePattern();
            }
        }

        [Description("最大数值"), Category("最大值")]
        public int MaxValue
        {
            get
            {
                return maxValue;
            }

            set
            {
                if (value >= minValue)
                {
                    maxValue = value;
                }
                CombinePattern();
            }
        }

        [Description("最小数值"), Category("最小值")]
        public int MinValue
        {
            get
            {
                return minValue;
            }

            set
            {
                if (value <= maxValue)
                {
                    minValue = value;
                }
                CombinePattern();
            }
        }
        #endregion

        #region 事件
        public new EventHandler GotFocus;

        public new EventHandler TextChanged;

        public KeyPressEventHandler BeforeKeyPress;

        public KeyPressEventHandler AfterKeyPress;
        #endregion

        #region 方法
        private void CombinePattern()
        {
            StringBuilder sb = new StringBuilder("^");
            StringBuilder sbTip = new StringBuilder(string.Concat("只允许输入范围[", minValue, "~", maxValue, "]的数值"));
            if (maxValue < 0)
            {
                sb.Append("-");
            }
            else if (minValue < 0)
            {
                sb.Append("-?");
            }

            sb.Append(@"\d+");


            if (decimalPlaces > 0)
            {
                sbTip.AppendFormat("，小数位数不能超过{0}位", decimalPlaces);
                sb.AppendFormat(@"(\.\d{{1,{0}}})?", decimalPlaces);
            }
            sb.Append("$");

            pattern = sb.ToString();
            tip = sbTip.ToString();
        }

        private void TipError(string errorText = null)
        {
            _errorText = string.IsNullOrEmpty(errorText) ? string.Empty : errorText;
            if (_showErrorProvider)
            {
                ep.SetError(this, errorText);
            }
        }

        private bool CheckValid(string newText)
        {
            if (decimal.TryParse(newText, out decimal val))
            {
                return Regex.IsMatch(newText, pattern) && val >= minValue && val <= maxValue;
            }
            return false;
        }
        #endregion

        #region 构造器
        public NumberText()
        {
            SkinTxt.GotFocus += NumberText_GotFocus;
            SkinTxt.TextChanged += NumberText_TextChanged;
            SkinTxt.KeyPress += NumberText_KeyPress;
            SkinTxt.Leave += NumberText_Leave;

            SkinTxt.MaxLength = 255;
            ep = new ErrorProvider();
        }
        #endregion

        #region 事件响应
        private void NumberText_GotFocus(object sender, EventArgs e)
        {
            GotFocus?.Invoke(sender, e);
        }

        private void NumberText_TextChanged(object sender, EventArgs e)
        {
            TextChanged?.Invoke(sender, e);
        }

        private void NumberText_Leave(object sender, EventArgs e)
        {
            TipError();
        }

        private void NumberText_KeyPress(object sender, KeyPressEventArgs e)
        {
            BeforeKeyPress?.Invoke(this, e);

            if (e.KeyChar != '.' && e.KeyChar != '-' && e.KeyChar != '\b' && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
                return;
            }

            if (e.KeyChar == '-' && minValue >= 0)
            {
                e.Handled = true;
                return;
            }

            if (e.KeyChar == '.' && decimalPlaces == 0)
            {
                e.Handled = true;
                return;
            }

            var newValue = new StringBuilder();
            newValue.Append(Text.Substring(0, SkinTxt.SelectionStart));
            if (e.KeyChar == '\b')
            {
                if (SkinTxt.SelectionLength == 0 && newValue.Length > 0)
                {
                    newValue.Remove(newValue.Length - 1, 1);
                }
            }
            else
            {
                newValue.Append(e.KeyChar);
            }
            newValue.Append(Text.Substring(SkinTxt.SelectionStart + SkinTxt.SelectionLength));

            string newText = newValue.ToString();
            if (newText.Equals("-") || newText.Length == 0 || newText.IndexOf('.') == newText.Length - 1)
            {
                return;
            }

            if (!CheckValid(newText))
            {
                e.Handled = true;
                TipError(tip);
                return;
            }
            TipError();

            AfterKeyPress?.Invoke(this, e);
        }
        #endregion
    }
}
