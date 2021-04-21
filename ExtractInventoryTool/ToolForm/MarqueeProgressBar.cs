using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ExtractInventoryTool.ToolForm
{
    public partial class MarqueeProgressBar : Form
    {
        public MarqueeProgressBar()
        {
            InitializeComponent();
        }
        public MarqueeProgressBar(string message)
        {
            InitializeComponent();
            if (!string.IsNullOrEmpty(message))
            {
                label1.Text = message;
            }
        }

        public delegate void SetUISomeInfo();

        /// <summary>
        /// 关闭命令
        /// </summary>
        public void closeOrder()
        {
            if (this.InvokeRequired)
            {
                //这里利用委托进行窗体的操作，避免跨线程调用时抛异常，后面给出具体定义
                SetUISomeInfo UIinfo = new SetUISomeInfo(new Action(() =>
                {
                    while (!this.IsHandleCreated)
                    {
                        ;
                    }
                    if (this.IsDisposed)
                        return;
                    if (!this.IsDisposed)
                    {
                        this.Dispose();
                    }

                }));
                this.Invoke(UIinfo);
            }
            else
            {
                if (this.IsDisposed)
                    return;
                if (!this.IsDisposed)
                {
                    this.Dispose();
                }
            }
        }

        private void MarqueeProgressBar_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (!this.IsDisposed)
            {
                this.Dispose(true);
            }
        }
    }
}
