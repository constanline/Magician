using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Magician.Common.CustomControl
{
    public partial class RadiusTextBox : RadiusControl
    {
        readonly TextBox textBox = new TextBox();
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Bindable(true)]
        [Description("文本内容"), Category("文本内容")]
        public override string Text
        {
            get
            {
                return textBox.Text;
            }
            set
            {
                textBox.Text = value;
            }
        }

        public override Color ForeColor
        {
            get
            {
                return textBox.ForeColor;
            }

            set
            {
                textBox.ForeColor = value;
            }
        }

        [Description("密码字符"), Category("密码字符")]
        public Char PasswordChar
        {
            get
            {
                return this.textBox.PasswordChar;
            }
            set
            {
                this.textBox.PasswordChar = value;
            }
        }

        public RadiusTextBox() : base()
        {
            textBox.BorderStyle = BorderStyle.None;
            this.Controls.Add(textBox);
        }

        protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
        {
            height = textBox.Height + borderThickness * 2 + 2 + this.Padding.Top + this.Padding.Bottom;
            base.SetBoundsCore(x, y, width, height, specified);
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            base.OnPaintBackground(e);
            if (Color.Transparent.ToArgb() == BackColor.ToArgb())
                textBox.BackColor = Color.White;
            else
                textBox.BackColor = BackColor;
            textBox.ForeColor = ForeColor;
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            int y = Height - textBox.Height - borderThickness - Padding.Top;
            textBox.Location = new Point(borderThickness + borderRadius + Padding.Left, y);
            textBox.Size = new Size(
                Width - borderThickness * 2 - borderRadius * 2 - Padding.Left - Padding.Right,
                Height - borderThickness - Padding.Top - Padding.Bottom);
        }
    }
}
