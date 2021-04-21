using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ExtractInventoryTool.ToolForm
{
    public class LoadingHelper
    {
        #region 相关变量定义
        /// <summary>
        /// 定义委托进行窗口关闭
        /// </summary>
        private delegate void CloseDelegate();
        private static MarqueeProgressBar loadingForm;
        private static readonly Object syncLock = new Object();  //加锁使用
        #endregion

        private LoadingHelper()
        {

        }

        /// <summary>
        /// 显示loading框
        /// </summary>
        public static void ShowLoadingScreen(string message="")
        {
            // Make sure it is only launched once.
            if (loadingForm != null)
                return;
            Thread thread = new Thread(LoadingHelper.ShowForm);
            thread.IsBackground = true;
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start(message);

        }

        /// <summary>
        /// 显示窗口
        /// </summary>
        private static void ShowForm(object obj)
        {
            if (loadingForm != null)
            {
                loadingForm.closeOrder();
                loadingForm = null;
            }
            string message= Convert.ToString(obj);
            loadingForm = new MarqueeProgressBar(message);
            loadingForm.TopLevel = true;
            //loadingForm.TopMost = true;
            loadingForm.ShowDialog();
        }

        /// <summary>
        /// 关闭窗口
        /// </summary>
        public static void CloseForm()
        {
            Thread.Sleep(50); //可能到这里线程还未起来，所以进行延时，可以确保线程起来，彻底关闭窗口
            if (loadingForm != null)
            {
                lock (syncLock)
                {
                    Thread.Sleep(50);
                    if (loadingForm != null)
                    {
                        Thread.Sleep(50);  //通过三次延时，确保可以彻底关闭窗口
                        loadingForm.Invoke(new CloseDelegate(LoadingHelper.CloseFormInternal));
                    }
                }
            }
        }

        /// <summary>
        /// 关闭窗口，委托中使用
        /// </summary>
        private static void CloseFormInternal()
        {

            loadingForm.closeOrder();
            loadingForm = null;

        }

    }
}
