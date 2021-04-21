using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtractInventoryTool.ColumnNameConfig
{
    public class BOMConfig
    {
        public string Oid = "Oid";
        public string VehicleModelCode = "车型代码";//车型代码
        public string Material = "Material";
        public string Material_Name = "物料";
        public string Material_Code = "物料号";
        public string Material_SupplierName = "供应商";
        public string Material_SupplierCode = "供应商代码";
        public string Client_Name = "客户";
        public string Client_Code = "客户代码";
        public string UnitUsage = "单位用量";//单位用量
        public string UpdateTime = "修改时间";//修改时间
    }
}
