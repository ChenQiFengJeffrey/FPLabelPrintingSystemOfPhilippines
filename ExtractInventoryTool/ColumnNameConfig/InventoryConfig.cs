using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtractInventoryTool.ColumnNameConfig
{
    public class InventoryConfig
    {
        public string Oid = "Oid";
        public string Material = "Material";
        public string MaterialCode = "物料号";
        public string MaterialName = "物料";
        public string SupplierCode = "供应商代码";
        public string SupplierName = "供应商名称";
        public string SysInventory = "系统库存";
        public string Min = "Min";
        public string Max = "Max";
        public string HUB = "HUB库存";
        public string InTransit = "在途库存";
        public string Total = "合计库存";
    }
}
