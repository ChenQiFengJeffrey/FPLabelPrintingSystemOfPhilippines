using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPLabelData.Entity
{
    public class ExtractInventoryTool_ProductionPlanImportRule
    {
        /// <summary>
        /// sheet的index
        /// </summary>
        public int St { get; set; }
        /// <summary>
        /// 车型代码起始行
        /// </summary>
        public int VR { get; set; }
        /// <summary>
        /// 车型代码起始列
        /// </summary>
        public int VC { get; set; }
        /// <summary>
        /// 台数起始行
        /// </summary>
        public int UR { get; set; }
        /// <summary>
        /// 台数起始列
        /// </summary>
        public int UC { get; set; }
        /// <summary>
        /// 日期起始行
        /// </summary>
        public int DR { get; set; }
    }
}
