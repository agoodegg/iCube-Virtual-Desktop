using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace iCube
{
    class HideOnStartupApplicationContext : ApplicationContext
    {
        public HideOnStartupApplicationContext(Form mainForm)
        {
            mainForm.Closed += delegate(object sender, EventArgs e)
            {
                Application.Exit();
            };
        }
    }
}
