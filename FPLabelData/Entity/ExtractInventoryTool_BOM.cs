using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPLabelData.Entity
{
    public class ExtractInventoryTool_BOM
    {
        public int Oid { get; set; }
        /// <summary>
        /// 车辆代码
        /// </summary>
        public string VehicleModelCode { get; set; }
        /// <summary>
        /// 物料
        /// </summary>
        public int Material { get; set; }
        /// <summary>
        /// 物料名称
        /// </summary>
        public string Material_Name { get; set; }
        public string Material_Code { get; set; }
        public string Material_SupplierName { get; set; }
        public string Material_SupplierCode { get; set; }
        public string Client_Name { get; set; }
        public string Client_Code { get; set; }
        /// <summary>
        /// 单位用量
        /// </summary>
        public int UnitUsage { get; set; }
        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdateTime { get; set; }
        /// <summary>
        /// 唯一码
        /// </summary>
        public string UniqueCode { get; set; }

    }
}
