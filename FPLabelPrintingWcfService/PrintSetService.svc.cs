using LabelPrintDAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using FPLabelData;

namespace FPLabelPrintingWcfService
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码、svc 和配置文件中的类名“PrintSetService”。
    // 注意: 为了启动 WCF 测试客户端以测试此服务，请在解决方案资源管理器中选择 PrintSetService.svc 或 PrintSetService.svc.cs，然后开始调试。
    public class PrintSetService : IPrintSetService
    {
        /// <summary>
        /// 获取打印配置
        /// </summary>
        /// <param name="finishedProductNum"></param>
        /// <returns></returns>
        public DataTable GetPrintSetByFPNum(string finishedProductNum)
        {
            PrintSetBLL psbll = new PrintSetBLL();
            return psbll.GetPrintSetByFPNum(finishedProductNum);
        }
        /// <summary>
        /// 获取成品配置
        /// </summary>
        /// <param name="finishedProductNum"></param>
        /// <returns></returns>
        public DataTable GetGoodSetByFPNum(string finishedProductNum)
        {
            GoodSetBLL gsbll = new GoodSetBLL();
            return gsbll.GetGoodSetByFPNum(finishedProductNum);
        }

        public void InsertLabelRecord(List<FinishedProductLabelDTO> dtoList)
        {
            LabelRecordBLL bll = new LabelRecordBLL();
            bll.InsertLabelRecord(dtoList);
            return;
        }
    }
}
