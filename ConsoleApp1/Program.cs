using LabelReporting;
using System;
using System.Threading;
using System.Threading.Tasks;
using DevExpress.XtraReports.UI;
using System.ServiceModel;
using FPLabelPrintingWcfService;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            using (ServiceHost host = new ServiceHost(typeof(PrintSetService)))
            {
                host.Opened += Host_Opened;
                host.Open();
                Console.ReadKey();
            }
        }

        private static void Host_Opened(object sender, EventArgs e)
        {
            Console.WriteLine("服务已经启动，按任意键终止");
        }
    }
}