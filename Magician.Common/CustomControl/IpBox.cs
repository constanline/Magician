using System;
using System.Windows.Forms;
using Magician.Common.Util;

namespace Magician.Common.CustomControl
{
    public partial class IpBox : UserControl
    {
        private string _ip;
        private readonly ErrorProvider _ep = new ErrorProvider();
        private NumberText[] _txtIpParts;

        public IpBox() : this("0.0.0.0")
        {
        }

        private void SetIp(string newIp)
        {
            _ip = newIp;
            if (NetworkUtil.ValidateIpAddress(newIp))
                _ep.Clear();
            else
                _ep.SetError(this, "格式错误");
        }

        public IpBox(string ip)
        {
            InitializeComponent();

            SetIp(ip);
            InitForm();
        }

        private void InitForm()
        {
            _txtIpParts = new[] {txtIpPart1, txtIpPart2, txtIpPart3, txtIpPart4};
            var parts = _ip.Split('.');
            for (var i = 0; i < parts.Length; i++)
            {
                _txtIpParts[i].Text = parts[i];
                _txtIpParts[i].TextChanged += IpBox_TextChanged;
                _txtIpParts[i].GotFocus += IpBox_GotFocus;
                _txtIpParts[i].BeforeKeyPress += IpBox_BeforeKeyPress;

                if (i < parts.Length - 1)
                    _txtIpParts[i].Tag = _txtIpParts[i + 1];

            }
        }

        private static void IpBox_GotFocus(object sender, EventArgs e)
        {
            var ipBox = (TextBox) sender;
            ipBox.SelectionStart = 0;
            ipBox.SelectionLength = ipBox.Text.Length;
        }

        private void IpBox_TextChanged(object sender, EventArgs e)
        {
            SetIp(GetIpAddress());
        }

        private void IpBox_BeforeKeyPress(object sender, KeyPressEventArgs e)
        {
            if(e.KeyChar == '.')
            {
                e.Handled = true;
                if (sender is NumberText txtIpPart && txtIpPart.Tag != null)
                {
                    ((NumberText)txtIpPart.Tag).Focus();
                }
            }
        }

        public string GetIpAddress()
        {
            return $"{txtIpPart1.Text}.{txtIpPart2.Text}.{txtIpPart3.Text}.{txtIpPart4.Text}";
        }
    }
}