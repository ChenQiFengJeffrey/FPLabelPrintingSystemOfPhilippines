using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPLabelData.Entity
{
    public class ExtractInventoryTool_ProductionPlan
    {
        public int Oid { get; set; }
        /// <summary>
        /// 车辆代码
        /// </summary>
        public string VehicleModelCode { get; set; }
        /// <summary>
        /// 客户
        /// </summary>
        public int Client { get; set; }
        /// <summary>
        /// 生产日期
        /// </summary>
        public DateTime ProductionDate { get; set; }
        /// <summary>
        /// 台数
        /// </summary>
        public int UnitNum { get; set; }
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
