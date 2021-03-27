
namespace TestApplication
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.ipBox1 = new Magician.Common.CustomControl.IpBox();
            this.numberText1 = new Magician.Common.CustomControl.NumberText();
            this.SuspendLayout();
            // 
            // ipBox1
            // 
            this.ipBox1.BackColor = System.Drawing.Color.White;
            this.ipBox1.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ipBox1.Location = new System.Drawing.Point(114, 126);
            this.ipBox1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.ipBox1.Name = "ipBox1";
            this.ipBox1.Size = new System.Drawing.Size(161, 32);
            this.ipBox1.TabIndex = 2;
            // 
            // numberText1
            // 
            this.numberText1.BackColor = System.Drawing.Color.Transparent;
            this.numberText1.DecimalPlaces = 0;
            this.numberText1.DownBack = null;
            this.numberText1.Icon = null;
            this.numberText1.IconIsButton = false;
            this.numberText1.IconMouseState = CCWin.SkinClass.ControlState.Normal;
            this.numberText1.IsPasswordChat = '\0';
            this.numberText1.IsSystemPasswordChar = false;
            this.numberText1.Lines = new string[0];
            this.numberText1.Location = new System.Drawing.Point(214, 35);
            this.numberText1.Margin = new System.Windows.Forms.Padding(0);
            this.numberText1.MaxLength = 0;
            this.numberText1.MaxValue = 100;
            this.numberText1.MinimumSize = new System.Drawing.Size(28, 28);
            this.numberText1.MinValue = 0;
            this.numberText1.MouseBack = null;
            this.numberText1.MouseState = CCWin.SkinClass.ControlState.Normal;
            this.numberText1.Multiline = false;
            this.numberText1.Name = "numberText1";
            this.numberText1.NormlBack = null;
            this.numberText1.Padding = new System.Windows.Forms.Padding(5);
            this.numberText1.ReadOnly = false;
            this.numberText1.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.numberText1.ShowErrorProvider = true;
            this.numberText1.Size = new System.Drawing.Size(185, 28);
            // 
            // 
            // 
            this.numberText1.SkinTxt.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.numberText1.SkinTxt.Dock = System.Windows.Forms.DockStyle.Fill;
            this.numberText1.SkinTxt.Font = new System.Drawing.Font("微软雅黑", 9.75F);
            this.numberText1.SkinTxt.Location = new System.Drawing.Point(5, 5);
            this.numberText1.SkinTxt.MaxLength = 255;
            this.numberText1.SkinTxt.Name = "BaseText";
            this.numberText1.SkinTxt.Size = new System.Drawing.Size(175, 18);
            this.numberText1.SkinTxt.TabIndex = 0;
            this.numberText1.SkinTxt.WaterColor = System.Drawing.Color.FromArgb(((int)(((byte)(127)))), ((int)(((byte)(127)))), ((int)(((byte)(127)))));
            this.numberText1.SkinTxt.WaterText = "";
            this.numberText1.TabIndex = 3;
            this.numberText1.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.numberText1.WaterColor = System.Drawing.Color.FromArgb(((int)(((byte)(127)))), ((int)(((byte)(127)))), ((int)(((byte)(127)))));
            this.numberText1.WaterText = "";
            this.numberText1.WordWrap = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.numberText1);
            this.Controls.Add(this.ipBox1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion
        private Magician.Common.CustomControl.IpBox ipBox1;
        private Magician.Common.CustomControl.NumberText numberText1;
    }
}

