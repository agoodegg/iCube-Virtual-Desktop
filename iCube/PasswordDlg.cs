using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace iCube
{
    public partial class PasswordDlg : Form
    {
        private Form1 parent;

        public PasswordDlg(Form1 parent)
        {
            this.parent = parent;
            InitializeComponent();
        }

        private void okBtn_Click(object sender, EventArgs e)
        {
            if (this.textBox1.Text.Trim().Equals(parent.password))
            {
                this.parent.Show();
                this.parent.quitLockMode();
                this.Close();
            }
            else
            {
                this.Close();
                MessageBox.Show("密码不正确", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            
        }
    }
}
