using System;
using System.Windows.Forms;
using CCWin;

namespace Magician.Common.CustomControl
{
    public partial class MaskForm : CCSkinMain
    {
        private readonly Form _form;

        public MaskForm(Form form)
        {
            InitializeComponent();
            _form = form;
        }

        private void MaskForm_Load(object sender, EventArgs e)
        {
            DialogResult = _form.ShowDialog();
        }
    }
}