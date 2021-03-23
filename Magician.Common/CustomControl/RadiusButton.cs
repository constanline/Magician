using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Magician.Common.CustomControl
{
    [DefaultEvent("Click")]
    public partial class RadiusButton : RadiusControl
    {
        Label label = new Label();

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Bindable(true)]
        [Description("文本内容"), Category("文本内容")]
        public override string Text
        {
            get
            {
                return label.Text;
            }
            set
            {
                label.Text = value;
            }
        }

        public RadiusButton() : base()
        {
            label.AutoSize = false;
            label.TextAlign = ContentAlignment.MiddleCenter;
            label.Click += Label_Click;
            this.Controls.Add(label);
        }

        private void Label_Click(object sender, EventArgs e)
        {
            base.OnClick(e);
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            base.OnPaintBackground(e);
            label.BackColor = BackColor;
            label.ForeColor = ForeColor;
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            label.Location = new Point(borderThickness + borderRadius + Padding.Left, borderThickness + Padding.Top);
            label.Size = new Size(
                Width - borderThickness * 2 - borderRadius * 2 - Padding.Left - Padding.Right,
                Height - borderThickness - Padding.Top - Padding.Bottom);
        }
    }
}
