using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPLabelData.Entity
{
    public class ExtractInventoryTool_BOMImportRule
    {
        /// <summary>
        /// sheet的index
        /// </summary>
        public int St { get; set; }
        /// <summary>
        /// 物料号起始行
        /// </summary>
        public int MR { get; set; }
        /// <summary>
        /// 物料号起始列
        /// </summary>
        public int MC { get; set; }
        /// <summary>
        /// 供应商代码起始行
        /// </summary>
        public int SR { get; set; }
        /// <summary>
        /// 供应商代码起始列
        /// </summary>
        public int SC { get; set; }
        /// <summary>
        /// 车型代码起始行
        /// </summary>
        public int VR { get; set; }
        /// <summary>
        /// 车型代码起始列
        /// </summary>
        public int VC { get; set; }
    }
}
