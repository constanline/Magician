using System;
using System.Drawing;
using System.Windows.Forms;

namespace Magician.Common.CustomControl
{
    public partial class MessageForm : MagicianForm
    {
        public Action<DialogResult> CbCancel;
        public Action<DialogResult> CbOk;

        public MessageForm(string title) : this(title, string.Empty)
        {
        }

        public MessageForm(string title, string message) : this(title, message, MessageBoxButtons.OK)
        {
        }

        public MessageForm(string title, string message, MessageBoxButtons buttons)
        {
            InitializeComponent();
            Text = title;
            if (buttons == MessageBoxButtons.OK || buttons == MessageBoxButtons.OKCancel)
            {
                var pnlButton = new Panel {Dock = DockStyle.Bottom, Size = new Size(284, 44)};
                
                var btnOk = new Button
                {
                    BackColor = Color.FromArgb(109, 216, 208),
                    ForeColor = Color.White,
                    Font = new Font("宋体", 10.5F),
                    Size = new Size(80, 30),
                    Text = this.GetLanguageString("btnOk")
                };
                btnOk.Click += BtnOK_Click;
                pnlButton.Controls.Add(btnOk);
                if (buttons == MessageBoxButtons.OK)
                {
                    btnOk.Location = new Point(91, 7);
                }
                else
                {
                    btnOk.Location = new Point(60, 7);

                    var btnCancel = new Button();
                    btnOk.BackColor = Color.FromArgb(109, 216, 208);
                    btnOk.ForeColor = Color.White;
                    btnCancel.Font = new Font("宋体", 10.5F);
                    btnCancel.Location = new Point(173, 7);
                    btnCancel.Size = new Size(80, 30);
                    btnCancel.Text = this.GetLanguageString("btnCancel");
                    btnCancel.Click += BtnCancel_Click;
                    pnlButton.Controls.Add(btnCancel);
                }

                Controls.Add(pnlButton);
            }

            var lblMessage = new Label
            {
                Font = new Font("宋体", 10.5F),
                ForeColor = Color.FromArgb(168, 168, 168),
                Text = message,
                Dock = DockStyle.Fill,
                AutoSize = false,
                Padding = new Padding(10)
            };
            Controls.Add(lblMessage);
            lblMessage.BringToFront();
        }

        public sealed override string Text
        {
            get { return base.Text; }
            set { base.Text = value; }
        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            CbOk?.Invoke(DialogResult.OK);
            DialogResult = DialogResult.OK;
            Close();
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void DialogForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (DialogResult == DialogResult.None)
            {
                CbCancel?.Invoke(DialogResult.Cancel);
                DialogResult = DialogResult.Cancel;
            }
        }
    }
}