using System;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using System.ComponentModel;

namespace iCube
{
    partial class Navigator
    {
        private const int WM_SYSCOMMAND = 0x0112;
        private const int SC_MOVE = 0xF010;
        private const int HTCAPTION = 0x0002; 

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.HomeBtn = new System.Windows.Forms.Button();
            this.BackBtn = new System.Windows.Forms.Button();
            this.ForwardBtn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // HomeBtn
            // 
            this.HomeBtn.BackgroundImage = global::iCube.Properties.Resources.home;
            this.HomeBtn.Location = new System.Drawing.Point(2, 2);
            this.HomeBtn.Name = "HomeBtn";
            this.HomeBtn.Size = new System.Drawing.Size(36, 36);
            this.HomeBtn.TabIndex = 0;
            this.HomeBtn.UseVisualStyleBackColor = true;
            this.HomeBtn.Click += new System.EventHandler(this.HomeBtn_Click);
            this.HomeBtn.MouseHover += new System.EventHandler(this.HomeBtn_MouseHover);
            // 
            // BackBtn
            // 
            this.BackBtn.BackgroundImage = global::iCube.Properties.Resources.left_arrow;
            this.BackBtn.Location = new System.Drawing.Point(44, 2);
            this.BackBtn.Name = "BackBtn";
            this.BackBtn.Size = new System.Drawing.Size(36, 36);
            this.BackBtn.TabIndex = 1;
            this.BackBtn.UseVisualStyleBackColor = true;
            this.BackBtn.Click += new System.EventHandler(this.BackBtn_Click);
            this.BackBtn.MouseHover += new System.EventHandler(this.BackBtn_MouseHover);
            // 
            // ForwardBtn
            // 
            this.ForwardBtn.BackgroundImage = global::iCube.Properties.Resources.right_arrow;
            this.ForwardBtn.Location = new System.Drawing.Point(86, 2);
            this.ForwardBtn.Name = "ForwardBtn";
            this.ForwardBtn.Size = new System.Drawing.Size(36, 36);
            this.ForwardBtn.TabIndex = 2;
            this.ForwardBtn.UseVisualStyleBackColor = true;
            this.ForwardBtn.Click += new System.EventHandler(this.ForwardBtn_Click);
            this.ForwardBtn.MouseHover += new System.EventHandler(this.ForwardBtn_MouseHover);
            // 
            // Navigator
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ClientSize = new System.Drawing.Size(124, 40);
            this.ControlBox = false;
            this.Controls.Add(this.ForwardBtn);
            this.Controls.Add(this.BackBtn);
            this.Controls.Add(this.HomeBtn);
            this.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Navigator";
            this.Opacity = 0.3D;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Form2";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.Navigator_Load);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Form1_MouseDown);
            this.MouseLeave += new System.EventHandler(this.Navigator_MouseLeave);
            this.MouseHover += new System.EventHandler(this.Navigator_MouseHover);
            this.ResumeLayout(false);

        }

        #endregion

        #region   
        [DllImport("user32.dll ")]
        public static extern bool ReleaseCapture();
        [DllImport("user32.dll ")]
        public static extern bool SendMessage(IntPtr hwnd, int wMsg, int wParam, int lParam);
        private void Form1_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, WM_SYSCOMMAND, SC_MOVE + HTCAPTION, 0);
        }
        #endregion


        private System.Windows.Forms.Button HomeBtn;
        private System.Windows.Forms.Button BackBtn;
        private System.Windows.Forms.Button ForwardBtn;
    }
}