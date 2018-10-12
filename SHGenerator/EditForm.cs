using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;

namespace SHGenerator
{
    public partial class EditForm : DevExpress.XtraEditors.XtraForm
    {
        public EditForm()
        {
            InitializeComponent();
        }

        public void SetText(string text)
        {
            this.richEditControl1.Text = text;
        }
    }
}