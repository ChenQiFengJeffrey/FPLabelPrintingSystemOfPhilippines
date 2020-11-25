using FPLabelData;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace FPLabelPrintingWcfService
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码和配置文件中的接口名“IPrintSetService”。
    [ServiceContract]
    public interface IPrintSetService
    {
        [OperationContract]
        DataTable GetPrintSetByFPNum(string finishedProductNum);

        [OperationContract]
        DataTable GetGoodSetByFPNum(string finishedProductNum);

        [OperationContract]
        DataTable GetRoSetByFPNum(string finishedProductNum);

        [OperationContract]
        void InsertLabelRecord(List<FinishedProductLabelDTO> dtoList);
    }
}
