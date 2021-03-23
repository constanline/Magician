using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Magician.Common.CustomControl
{
    [Designer("System.Windows.Forms.Design.ParentControlDesigner, System.Design", typeof(IDesigner))]
    public partial class RadiusControl : UserControl
    {
        //public enum RadiusCornerType
        //{
        //    All,
        //    Left,
        //    Right,
        //    Top,
        //    Bottom,
        //    LeftTop,
        //    RightTop,
        //    LeftBottom,
        //    RightBottom
        //}

        //protected RadiusCornerType borderRadiusCorner = RadiusCornerType.All;

        //public RadiusCornerType BorderRadiusCorner
        //{
        //    get
        //    {
        //        return this.borderRadiusCorner;
        //    }
        //    set
        //    {
        //        this.borderRadiusCorner = value;
        //    }
        //}

        protected bool radiusLeftTop = true;
        [Description("左上角圆角"), Category("左上角圆角"), DefaultValue(true)]
        public bool RadiusLeftTop
        {
            get
            {
                return this.radiusLeftTop;
            }
            set
            {
                this.radiusLeftTop = value;
            }
        }

        protected bool radiusLeftBottom = true;
        [Description("左下角圆角"), Category("左下角圆角"), DefaultValue(true)]
        public bool RadiusLeftBottom
        {
            get
            {
                return this.radiusLeftBottom;
            }
            set
            {
                this.radiusLeftBottom = value;
            }
        }

        protected bool radiusRightTop = true;
        [Description("右上角圆角"), Category("右上角圆角"), DefaultValue(true)]
        public bool RadiusRightTop
        {
            get
            {
                return this.radiusRightTop;
            }
            set
            {
                this.radiusRightTop = value;
            }
        }
        
        protected bool radiusRightBottom = true;
        [Description("右下角圆角"), Category("右下角圆角"), DefaultValue(true)]
        public bool RadiusRightBottom
        {
            get
            {
                return this.radiusRightBottom;
            }
            set
            {
                this.radiusRightBottom = value;
            }
        }


        protected Color borderColor = Color.Black;

        [Description("边框颜色"), Category("边框颜色")]
        public Color BorderColor
        {
            get
            {
                return this.borderColor;
            }
            set
            {
                this.borderColor = value;
            }
        }

        bool allowExtraMargin = true;

        [Description("文本内容"), Category("文本内容")]
        public bool AllowExtraMargin
        {
            get
            {
                return allowExtraMargin;
            }
            set
            {
                allowExtraMargin = value;
            }
        }

        protected int borderThickness = 1;

        [Description("边框粗细"), Category("边框粗细")]
        public int BorderThickness
        {
            get
            {
                return this.borderThickness;
            }
            set
            {
                this.borderThickness = value;
            }
        }

        protected int borderRadius = 0;

        [Description("边角半径"), Category("边角半径")]
        public int BorderRadius
        {
            get
            {
                return this.borderRadius;
            }
            set
            {
                this.borderRadius = value;
            }
        }

        public RadiusControl()
        {
            InitializeComponent();
            this.SetStyle(ControlStyles.DoubleBuffer |
               ControlStyles.UserPaint |
               ControlStyles.AllPaintingInWmPaint |
               ControlStyles.SupportsTransparentBackColor,
               true);
            this.UpdateStyles();
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            if(Parent.BackgroundImage == null)
            {
                e.Graphics.Clear(Parent.BackColor);
            }
            else if (borderRadius > 0)
            {
                //Rectangle rect = new Rectangle(0, 0, this.Width - 1, this.Height - 1);
                //// 指定图形路径， 有一系列 直线/曲线 组成
                //GraphicsPath borderPath = new GraphicsPath();
                //borderPath.StartFigure();
                //Rectangle rectArc = new Rectangle(new Point(rect.X, rect.Y), new Size(2 * borderRadius, 2 * borderRadius));
                //borderPath.AddArc(rectArc, 180, 90);
                //borderPath.AddLine(new Point(rect.X + borderRadius, rect.Y), new Point(rect.X, rect.Y));
                //borderPath.AddLine(new Point(rect.X, rect.Y), new Point(rect.X, rect.Y + borderRadius));
                //borderPath.CloseFigure();
                //e.Graphics.FillPath(new SolidBrush(Color.Transparent), borderPath);
                //Color tmp = BackColor;
                //BackColor = Color.Transparent;
                //base.OnPaintBackground(e);
                //BackColor = tmp;
            }
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);
            Graphics g = pe.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.CompositingQuality = CompositingQuality.HighQuality;
            if (borderThickness <= 0)
                return;
            Pen pen = new Pen(borderColor, borderThickness);

            if (borderRadius <= 0)
            {
                g.DrawRectangle(pen, 0, 0, this.Width - 1, this.Height - 1);
                g.FillRectangle(new SolidBrush(BackColor), 0, 0, this.Width - 1, this.Height - 1);
                return;
            }

            // 要实现 圆角化的 矩形
            Rectangle rect;
            if (allowExtraMargin)
            {
                rect = new Rectangle(Margin.Left, Margin.Top, this.Width - 1 - Margin.Left - Margin.Right, this.Height - 1 - Margin.Top - Margin.Bottom);
            }
            else
            {
                rect = new Rectangle(0, 0, this.Width - 1, this.Height - 1);
            }
            // 指定图形路径， 有一系列 直线/曲线 组成
            GraphicsPath borderPath = new GraphicsPath();
            borderPath.StartFigure();
            //if(borderRadiusCorner == RadiusCornerType.All || borderRadiusCorner==RadiusCornerType.Left 
            //    || borderRadiusCorner == RadiusCornerType.Top||borderRadiusCorner == RadiusCornerType.LeftTop)
            //{

            //}
            if (radiusLeftTop)
            {
                borderPath.AddArc(new Rectangle(new Point(rect.X, rect.Y), new Size(2 * borderRadius, 2 * borderRadius)), 180, 90);
                borderPath.AddLine(new Point(rect.X + borderRadius, rect.Y), new Point(rect.Right - borderRadius, rect.Y));
            }
            else
            {
                borderPath.AddLine(new Point(rect.X, rect.Y + borderRadius), new Point(rect.X, rect.Y));
                borderPath.AddLine(new Point(rect.X, rect.Y), new Point(rect.Right - borderRadius, rect.Y));
            }
            if (radiusRightTop)
            {
                borderPath.AddArc(new Rectangle(new Point(rect.Right - 2 * borderRadius, rect.Y), new Size(2 * borderRadius, 2 * borderRadius)), 270, 90);
                borderPath.AddLine(new Point(rect.Right, rect.Y + borderRadius), new Point(rect.Right, rect.Bottom - borderRadius));
            }
            else
            {
                borderPath.AddLine(new Point(rect.Right - borderRadius, rect.Y), new Point(rect.Right, rect.Y));
                borderPath.AddLine(new Point(rect.Right, rect.Y), new Point(rect.Right, rect.Bottom - borderRadius));
            }
            if (radiusRightBottom)
            {
                borderPath.AddArc(new Rectangle(new Point(rect.Right - 2 * borderRadius, rect.Bottom - 2 * borderRadius), new Size(2 * borderRadius, 2 * borderRadius)), 0, 90);
                borderPath.AddLine(new Point(rect.Right - borderRadius, rect.Bottom), new Point(rect.X + borderRadius, rect.Bottom));
            }
            else
            {
                borderPath.AddLine(new Point(rect.Right, rect.Bottom - borderRadius), new Point(rect.Right, rect.Bottom));
                borderPath.AddLine(new Point(rect.Right, rect.Bottom), new Point(rect.X + borderRadius, rect.Bottom));
            }
            if (radiusLeftBottom)
            {
                borderPath.AddArc(new Rectangle(new Point(rect.X, rect.Bottom - 2 * borderRadius), new Size(2 * borderRadius, 2 * borderRadius)), 90, 90);
                borderPath.AddLine(new Point(rect.X, rect.Bottom - borderRadius), new Point(rect.X, rect.Y + borderRadius));
            }
            else
            {
                borderPath.AddLine(new Point(rect.X + borderRadius, rect.Bottom), new Point(rect.X, rect.Bottom));
                borderPath.AddLine(new Point(rect.X, rect.Bottom), new Point(rect.X, rect.Y + borderRadius));
            }
            borderPath.CloseFigure();
            g.DrawPath(pen, borderPath);
            g.FillPath(new SolidBrush(BackColor), borderPath);
        }
    }
}
