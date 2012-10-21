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
    public partial class Navigator : Form
    {
        private Form1 parent;
        public Navigator(Form1 form)
        {
            this.ShowInTaskbar = false;
            this.parent = form;
            InitializeComponent();

            
        }

        private void HomeBtn_Click(object sender, EventArgs e)
        {
            try
            {
                parent.ie.Navigate2(parent.HomeUrl);
            }
            catch
            {
                parent.OpenUrl(parent.HomeUrl);
            }
            
        }



        private void BackBtn_Click(object sender, EventArgs e)
        {
            
            try
            {
                parent.ie.GoBack();
            }
            catch
            {

            }
        }

        private void ForwardBtn_Click(object sender, EventArgs e)
        {
            
            try
            {
                parent.ie.GoForward();
            }
            catch
            {

            }
        }

        private void Navigator_Load(object sender, EventArgs e)
        {

        }

        private void Navigator_MouseHover(object sender, EventArgs e)
        {
            this.Opacity=0.6;
        }

        private void Navigator_MouseLeave(object sender, EventArgs e)
        {
            this.Opacity = 0.3;
        }

        private void HomeBtn_MouseHover(object sender, EventArgs e)
        {
            this.Opacity = 0.6;
        }

        private void BackBtn_MouseHover(object sender, EventArgs e)
        {
            this.Opacity = 0.6;
        }

        private void ForwardBtn_MouseHover(object sender, EventArgs e)
        {
            this.Opacity = 0.6;
        }

    }
}
