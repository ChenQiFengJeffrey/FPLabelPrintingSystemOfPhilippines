using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPLabelData.Entity
{
    public class ExtractInventoryTool_Client
    {
        public int Oid { get; set; }
        /// <summary>
        /// 客户名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 代码/唯一码
        /// </summary>
        public string UniqueCode { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 车型代码规则
        /// </summary>
        public string RegexRule { get; set; }
    }
}
