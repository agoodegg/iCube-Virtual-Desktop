using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;
using mshtml;
using AxSHDocVw;
using SHDocVw;
using iCube.Properties;
using Microsoft.Win32;
using System.Security.Principal;

namespace iCube
{
    public partial class Form1 : Form
    {

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);
        [DllImport("user32.dll")]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImport("user32.dll")]
        private static extern bool SetCursorPos(int X, int Y);
        [DllImport("user32.dll")]
        private static extern void mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo);
        [DllImport("user32.dll")]
        private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, uint dwExtraInfo);
        [DllImport("user32.dll")]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndlnsertAfter, int X, int Y, int cx, int cy, uint Flags);
        //ShowWindow参数
        private const int SW_SHOWNORMAL = 1;
        private const int SW_RESTORE = 9;
        private const int SW_SHOWNOACTIVATE = 4;
        //SendMessage参数
        private const int WM_KEYDOWN = 0X100;
        private const int WM_KEYUP = 0X101;
        private const int WM_SYSCHAR = 0X106;
        private const int WM_SYSKEYUP = 0X105;
        private const int WM_SYSKEYDOWN = 0X104;
        private const int WM_CHAR = 0X102;
        private const int WM_SYSCOMMAND = 0x0112;
        private const int SC_MAXIMIZE = 0xF030;

        public string[] args = null;

        UserActivityHook actHook = new UserActivityHook();

        public string HomeUrl = "";

        public SHDocVw.InternetExplorer ie = null;

        public String password = "";

        public String filterUrl = "";

        public bool allowNewWnd = false;

        public bool allowNav = true;

        public bool isAutoRun = false;

        public bool isAutoRunMode = false;

        public Navigator nav = null;

        public Form1()
        {
            InitializeComponent();
            InitActHook();
        }

        public Form1(string[] args)
        {
            InitializeComponent();
            InitActHook();
            this.args = args;

            if (this.args != null && this.args.Length > 0 && this.args[0].Equals("--auto"))
            {
                this.isAutoRunMode = true;
                this.isAutoRun = true;
                Settings.Default.isAutorun = true;

                if (Settings.Default.address == null || Settings.Default.address.Trim().Equals(""))
                {
                    Settings.Default.address = "www.cib.com.cn";
                }
                if (Settings.Default.password == null || Settings.Default.password.Trim().Equals(""))
                {
                    Settings.Default.password = "administrator";
                }

                this.HomeUrl = Settings.Default.address;
                this.password = Settings.Default.password;
                this.allowNav = Settings.Default.hasNavBar;
                this.isAutoRun = Settings.Default.isAutorun;
                this.filterUrl = Settings.Default.allowSites;

                Start();
            }
        }

        private void InitActHook() {
            actHook.KeyDown += new KeyEventHandler(MyKeyDown);
            actHook.OnMouseActivity += new MouseEventHandler(MouseMoved);
        }

        public void MyKeyDown(object sender, KeyEventArgs e)
        {
            //Console.WriteLine("====================");
            //Console.WriteLine(e.Alt);
            //Console.WriteLine(e.Control);
            //Console.WriteLine(e.KeyData+":"+Keys.P);
            //Console.WriteLine(e.Shift);
            //Console.WriteLine("====================");
           //Console.WriteLine(e.KeyCode);
            if (e.Alt && e.Shift && e.Control && e.KeyCode == Keys.P)
            {
                e.Handled = true;
                PasswordDlg dlg = new PasswordDlg(this);
                dlg.ShowDialog();
            }
            else if ((Control.ModifierKeys & Keys.Control) == Keys.Control && (Control.ModifierKeys & Keys.Shift) == Keys.Shift && !((Control.ModifierKeys & Keys.Alt) == Keys.Alt) && (e.KeyCode == Keys.ShiftKey || e.KeyCode == Keys.ControlKey))
            {
                e.Handled = false;
            }

        }

        public void MouseMoved(object sender, MouseEventArgs e)
        {
            
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutDlg dlg = new AboutDlg();
            dlg.ShowDialog();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (this.Address.Text.Trim().Equals("") || this.passwordText.Text.Trim().Equals(""))
            {
                MessageBox.Show("必须输入网址及解锁密码", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!(this.passwordText.Text.Equals(this.repassword.Text)))
            {
                MessageBox.Show("两次输入的密码必须相同", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Settings.Default.address = this.Address.Text;
            Settings.Default.password = this.passwordText.Text;
            Settings.Default.allowSites = this.AllowSites.Text;
            Settings.Default.hasNavBar = this.AllowNavBar.Checked;
            Settings.Default.isAutorun = this.AutoRunCheckBox.Checked;

            Settings.Default.Save();

            if (this.AllowNavBar.Checked)
            {
                this.allowNav = true;
            }
            else
            {
                this.allowNav = false;
            }
            this.HomeUrl = this.Address.Text.Trim();
            this.filterUrl = this.AllowSites.Text.Trim();
            this.password = this.passwordText.Text;


            Start();

            this.Hide();

            MessageBox.Show("按下Ctrl+Shift+Alt+P可输入密码解锁", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void hideTaskBar() 
        {
            
            IntPtr rwl = Form1.FindWindow("Shell_TrayWnd", null);   ////获得任务栏句柄
            IntPtr rwl2 = FindWindow("Button", null);
            Form1.ShowWindow(rwl, 0);     //当nCmdShow=0：隐藏；=1：显示 
            Form1.ShowWindow(rwl2, 0);
        }

        private void showTaskBar()
        {
            IntPtr rwl = Form1.FindWindow("Shell_TrayWnd", null);   //
            IntPtr rwl2 = FindWindow("Button", null);
            Form1.ShowWindow(rwl, 1);     //当nCmdShow=0：隐藏；=1：显示 
            Form1.ShowWindow(rwl2, 1);
        }

        private void hideDesktop()
        {
            IntPtr rwl = Form1.FindWindow("ProgMan", null);   ////
            Form1.ShowWindow(rwl, 0);     //当nCmdShow=0：隐藏；=1：显示 
        }

        private void showDesktop()
        {
            IntPtr rwl = Form1.FindWindow("ProgMan", null);   ////
            Form1.ShowWindow(rwl, 9);     //当nCmdShow=0：隐藏；=1：显示 
        }

        private void Start() 
        {
            //NoWindowCall("taskkill.exe", " /f /im explorer.exe");
            NoWindowCall("taskkill.exe", " /f /im iexplore.exe");

            hideTaskBar();
            hideDesktop();

            actHook.started = true;
            OpenUrl(this.HomeUrl);

            if (this.allowNav)
            {
                nav = new Navigator(this);
                nav.SetDesktopLocation(0, 0);
                nav.Location = new Point(0, 0); 
                nav.Show();
            }

            //System.Diagnostics.Process.Start("iexplore.exe", "-k -nohome " + this.textBox1.Text);
            TaskmgrHide();

            System.Diagnostics.Process.Start("ctfmon.exe");
        }

        private void NoWindowCall(string command, string parameters)
        {
            System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo(command, parameters);
            psi.RedirectStandardOutput = true; //重定向输出。设置为TRUE，这样我们就可以接收返回结果了。
            psi.CreateNoWindow = true;   //不创建窗口，相当于后台运行
            psi.UseShellExecute = false; //不使用系统Shell来启动程序
            var proc = System.Diagnostics.Process.Start(psi);
            proc.WaitForExit();
        }

        public void TaskmgrHide()
        {
            Process[] proc = Process.GetProcesses();
            foreach (Process thisproc in proc)
            {
                if (thisproc.ProcessName.Equals("taskmgr"))
                {
                    thisproc.Kill();
                    break;
                }
            }
            Process pro = new Process();
            pro.StartInfo.WorkingDirectory = Environment.GetFolderPath(Environment.SpecialFolder.System);
            pro.StartInfo.FileName = "taskmgr.exe";
            pro.StartInfo.CreateNoWindow = true;
            pro.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            pro.Start();
        }

        public void UnHideTaskMgr()
        {
            Process[] proc = Process.GetProcesses();
            foreach (Process thisproc in proc)
            {
                if (thisproc.ProcessName.Equals("taskmgr"))
                {
                    thisproc.Kill();
                    break;
                }
            }
        }


        private void button2_Click(object sender, EventArgs e)
        {
            NoWindowCall("taskkill.exe", " /f /im iexplore.exe");
            //quitLockMode();
            this.Close();
        }

        public void quitLockMode()
        {
            /*
            Boolean lauched = false;
            Process[] proc = Process.GetProcesses();
            foreach (Process thisproc in proc)
            {
                if (thisproc.ProcessName.Equals("explorer"))
                {
                    lauched = true;
                    break;
                }
            }

            if (!lauched)
            {
                System.Diagnostics.Process.Start(@"C:\Windows\explorer.exe");
            }*/

            showTaskBar();
            showDesktop();

            actHook.started = false;
            try
            {
                if (ie != null)
                {
                    ie.Quit();
                }
            }
            catch
            {

            }
            try
            {
                UnHideTaskMgr();
            }
            catch
            {

            }
        }

        void ieBrowser_BeforeNewWindow(object sender, WebBrowserExtendedNavigatingEventArgs e)
        {
            e.Cancel = true;
            if (isValidUrl(e.Url)) 
            {
                ((ExtendedWebBrowser)sender).Navigate(e.Url);
            }
            
        }


        public void OpenUrl(String url) 
        {
            //object o = null;
            ie = new SHDocVw.InternetExplorerClass();
            if (this.allowNewWnd)
            {
                ie.Resizable = false;
                ie.FullScreen = false;
                
            }
            else
            {
                ie.FullScreen = true;
                ie.Resizable = false;
            }
            ie.AddressBar = false;
            
            ie.MenuBar = false;
            ie.ToolBar = 0;
            ie.Visible = true;

            SendMessage(new IntPtr(ie.HWND), WM_SYSCOMMAND, SC_MAXIMIZE, 0);

            ie.Navigate2(url);
            
            ie.BeforeNavigate2 += new SHDocVw.DWebBrowserEvents2_BeforeNavigate2EventHandler(beforeNavHandler);
            ie.NewWindow2 += new SHDocVw.DWebBrowserEvents2_NewWindow2EventHandler(wbEvents_NewWindow2);
            ie.NewWindow3 += new SHDocVw.DWebBrowserEvents2_NewWindow3EventHandler(wbEvents_NewWindow3);
            ie.NewProcess += new SHDocVw.DWebBrowserEvents2_NewProcessEventHandler(wbEvents_NewProcess);
            ie.DocumentComplete += new SHDocVw.DWebBrowserEvents2_DocumentCompleteEventHandler(wbEvents_DocumentComplete);
            ie.WindowClosing += new SHDocVw.DWebBrowserEvents2_WindowClosingEventHandler(wbEvents_WindowClosing);

        }

        protected  void beforeNavHandler(Object ob1, ref Object URL, ref Object Flags, ref Object TargetFrameName, ref Object PostData, ref Object Headers, ref bool Cancel) 
        {
            //Console.WriteLine("beforeNavHandler:" + URL);

            if (this.isValidUrl(URL.ToString()))
            {
                Cancel = false;
            }
            else {
                Cancel = true;
            }
        }

        
        protected  void wbEvents_NewWindow2(ref object sender, ref bool Cancel)
        {
            Cancel = true;
        }
        

        protected  void wbEvents_NewWindow3(ref object ppDisp, ref bool Cancel, uint dwFlags, string bstrUrlContext, string bstrUrl)
        {
            //Console.WriteLine("wbEvents_NewWindow3:" + bstrUrl);
            if (bstrUrl.Trim().Equals("http://www.cib.com.cn/netbank/cn/open.htm")) 
            {
                Cancel = false;
                ppDisp = new SHDocVw.InternetExplorerClass();
                return;
            }
            
            if (this.allowNewWnd)
            {
                Cancel = true;
                SHDocVw.InternetExplorer newWnd = new SHDocVw.InternetExplorerClass();
                newWnd.FullScreen = false;
                newWnd.AddressBar = false;
                newWnd.Resizable = false;
                newWnd.MenuBar = false;
                newWnd.ToolBar = 0;
                newWnd.Visible = true;

                //ppDisp = newWnd;

                //maximize
                SendMessage(new IntPtr(newWnd.HWND), WM_SYSCOMMAND, SC_MAXIMIZE, 0);

                newWnd.BeforeNavigate2 += new SHDocVw.DWebBrowserEvents2_BeforeNavigate2EventHandler(beforeNavHandler);
                newWnd.NewWindow2 += new SHDocVw.DWebBrowserEvents2_NewWindow2EventHandler(wbEvents_NewWindow2);
                newWnd.NewWindow3 += new SHDocVw.DWebBrowserEvents2_NewWindow3EventHandler(wbEvents_NewWindow3);
                newWnd.NewProcess += new SHDocVw.DWebBrowserEvents2_NewProcessEventHandler(wbEvents_NewProcess);
                newWnd.DocumentComplete += new SHDocVw.DWebBrowserEvents2_DocumentCompleteEventHandler(wbEvents_DocumentComplete);
                newWnd.WindowClosing += new SHDocVw.DWebBrowserEvents2_WindowClosingEventHandler(wbEvents_WindowClosing);

                newWnd.Navigate2(bstrUrl);
            }
            else
            {
                Cancel = true;
                if (isValidUrl(bstrUrl))
                {
                    ie.Navigate2(bstrUrl);
                }
            }
            
            
        }

        protected  void wbEvents_NewProcess(int lCauseFlag, object pWB2, ref bool Cancel) 
        {
            Cancel = true;
        }

        protected  void wbEvents_DocumentComplete(object pDisp, ref object URL)
        {
            mshtml.HTMLDocumentClass doc = (mshtml.HTMLDocumentClass)ie.Document;

            foreach (mshtml.IHTMLElement archor in doc.links)
            {
                archor.setAttribute("target", "_self");
            }


        }

        protected void wbEvents_WindowClosing(bool bIsChildWindow, ref bool Cancel)
        {
            Cancel = true;
        }


        public bool isValidUrl(string url) 
        {

            if (url == null || url.Trim().Equals("") || url.Trim().Equals("about: blank"))
            {
                return true;
            }

            if (this.filterUrl == null || this.filterUrl.Equals("")) 
            {

                return true;
                /*
                this.filterUrl = this.HomeUrl.ToLower();
                if (this.filterUrl.StartsWith("www."))
                {
                    this.filterUrl = this.filterUrl.Substring(3);
                }
                 * */
            }

            char [] splits = new char[1];
            splits[0] = ';';
            string[] filters = filterUrl.Split(splits);
            foreach (string f in filters)
            {
                string tmp = f.Replace("*", "");

                if(url.Contains(tmp))
                {
                    return true;
                }
                
            }
            return false;
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Address.Text = Settings.Default.address;
            this.passwordText.Text = Settings.Default.password;
            this.repassword.Text = Settings.Default.password;
            this.AllowSites.Text = Settings.Default.allowSites;
            this.AllowNavBar.Checked = Settings.Default.hasNavBar;
            this.AutoRunCheckBox.Checked = Settings.Default.isAutorun;
        }

        public void ShowWindow()
        {
            if (isAutoRunMode)
            {
                this.Hide();
            }
            else
            {
                this.Show();
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {

            Settings.Default.address = this.Address.Text;
            Settings.Default.password = this.passwordText.Text;
            Settings.Default.allowSites = this.AllowSites.Text;
            Settings.Default.hasNavBar = this.AllowNavBar.Checked;
            Settings.Default.isAutorun = this.AutoRunCheckBox.Checked;

            Settings.Default.Save();

            if (this.nav != null)
            {
                this.nav.Close();
                this.nav.Dispose();
            }
        }

        public static bool IsAdministrator()
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        private void AutoRunCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if(!IsAdministrator()){
                return;
            }
            if (this.AutoRunCheckBox.Checked)
            {
                string starupPath = Application.ExecutablePath;
                //class Micosoft.Win32.RegistryKey. 表示Window注册表中项级节点,此类是注册表装.
                RegistryKey loca = Registry.LocalMachine;

                RegistryKey run = loca.CreateSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run");

                try
                {
                    //SetValue:存储值的名称
                    run.SetValue("iCube", starupPath+" --auto");
                    loca.Close();
                }
                catch (Exception ee)
                {
                    MessageBox.Show(ee.Message.ToString(), "提 示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                string starupPath = Application.ExecutablePath;
                //class Micosoft.Win32.RegistryKey. 表示Window注册表中项级节点,此类是注册表装.
                RegistryKey loca = Registry.LocalMachine;
                RegistryKey run = loca.CreateSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run");

                try
                {
                    //SetValue:存储值的名称
                    run.SetValue("iCube", false);
                    loca.Close();
                }
                catch (Exception ee)
                {
                    MessageBox.Show(ee.Message.ToString(), "提 示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
