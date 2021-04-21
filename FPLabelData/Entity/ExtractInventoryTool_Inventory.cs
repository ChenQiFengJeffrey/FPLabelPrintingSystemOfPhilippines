using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPLabelData.Entity
{
    public class ExtractInventoryTool_Inventory
    {
        public int Oid { get; set; }
        /// <summary>
        /// 物料
        /// </summary>
        public int Material { get; set; }
        /// <summary>
        /// 系统库存
        /// </summary>
        public int SysInventory { get; set; }
        /// <summary>
        /// Min
        /// </summary>
        public int Min { get; set; }
        /// <summary>
        /// Max
        /// </summary>
        public int Max { get; set; }
        /// <summary>
        /// HUB库存
        /// </summary>
        public int HUB { get; set; }
        /// <summary>
        /// 在途库存
        /// </summary>
        public int InTransit { get; set; }
        /// <summary>
        /// 合计库存
        /// </summary>
        public int Total { get; set; }
    }
}
