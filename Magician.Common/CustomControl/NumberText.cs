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
        ErrorProvider ep;

        string oldText = String.Empty;
        string pattern = @"^\d+$";
        string tip = String.Empty;

        int decimalPlaces = 0;

        int maxValue = 100;

        int minValue = 0;

        public Decimal? Value
        {
            get
            {
                decimal val;
                if (decimal.TryParse(this.Text, out val))
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
                decimal? tmp = this.Value;
                if(tmp == null)
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
                decimal? tmp = this.Value;
                if (tmp == null)
                {
                    return null;
                }
                return Convert.ToInt32(tmp);
            }
        }

        private void combinePattern()
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

                combinePattern();
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
                combinePattern();
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
                combinePattern();
            }
        }

        public NumberText()
        {
            this.KeyPress += NumberText_KeyPress;
            //this.Validating += NumberText_Validating;
            //this.TextChanged += NumberText_TextChanged;
            this.Leave += NumberText_Leave;
            ep = new ErrorProvider();
        }

        private void NumberText_Leave(object sender, EventArgs e)
        {
            if (this.Text.Length == 0)
            {
                return;
            }
            if (this.Text[this.Text.Length - 1] == '.')
            {
                this.Text = this.Text.Substring(0, this.Text.Length - 1);
            }

            this.Text = Convert.ToDecimal(this.Text).ToString("f" + decimalPlaces);

        }

        //private void NumberText_TextChanged(object sender, EventArgs e)
        //{
        //    if (this.Text == oldText)
        //    {
        //        return;
        //    }

        //    if (this.Text.LastIndexOf('-') > 0)
        //    {
        //        int idx = this.Text.LastIndexOf('-');
        //        this.Text = this.Text.Substring(0, idx) + this.Text.Substring(idx + 1);
        //        ep.SetError(this, "负号只能出现在头部");
        //        return;
        //    }

        //    if (!(Regex.IsMatch(this.Text, pattern)))
        //    {
        //        ep.SetError(this, tip);
        //        return;
        //    }

        //    decimal val = Convert.ToDecimal(this.Text);
        //    if (val > maxValue || val < minValue)
        //    {
        //        ep.SetError(this, "数值超出范围");
        //        return;
        //    }

        //    oldText = val.ToString("f2");

        //}

        //private void NumberText_Validating(object sender, CancelEventArgs e)
        //{
        //    if (this.Text.Equals(string.Empty))
        //    {
        //        return;
        //    }
        //    if (!(Regex.IsMatch(this.Text, pattern)))
        //    {
        //        e.Cancel = true;
        //        ep.SetError(this, string.Concat("只允许输入范围[", minValue, "-", maxValue, "]的数值"));
        //        return;
        //    }

        //    if (this.Text.LastIndexOf('-') > 0)
        //    {
        //        e.Cancel = true;
        //        return;
        //    }

        //    decimal val = Convert.ToDecimal(this.Text);
        //    if (val > maxValue || val < minValue)
        //    {
        //        e.Cancel = true;
        //        return;
        //    }

        //    this.Text = val.ToString("f2");
        //}

        private void NumberText_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != '.' && e.KeyChar != '-' && e.KeyChar != '\b' && !Char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
                return;
            }

            if(e.KeyChar == '-' && minValue >= 0)
            {
                e.Handled = true;
                return;
            }

            StringBuilder newValue = new StringBuilder();
            // newValue.Append(this.Text.Substring(0, this.SelectionStart));
            // if (e.KeyChar == '\b')
            // {
            //     if (this.SelectionLength == 0 && newValue.Length > 0)
            //     {
            //         newValue.Remove(newValue.Length - 1, 1);
            //     }
            // }
            // else
            // {
            //     newValue.Append(e.KeyChar);
            // }
            // newValue.Append(this.Text.Substring(this.SelectionStart + this.SelectionLength));

            String newText = newValue.ToString();
            if (newText.Equals("-") || newText.Length == 0 || newText.IndexOf('.') == newText.Length - 1)
            {
                return;
            }

            if (!(Regex.IsMatch(newText, pattern)))
            {
                e.Handled = true;
                ep.SetError(this, tip);
                return;
            }

            decimal val = Convert.ToDecimal(newText);
            if (val > maxValue || val < minValue)
            {
                e.Handled = true;
                ep.SetError(this, string.Concat("极限范围[", minValue, ",", maxValue, "]"));
                return;
            }
            else
            {
                ep.Clear();
            }
        }
    }
}
