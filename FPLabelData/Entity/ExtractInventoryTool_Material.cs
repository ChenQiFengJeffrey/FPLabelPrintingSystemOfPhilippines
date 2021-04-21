using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPLabelData.Entity
{
    public class ExtractInventoryTool_Material
    {
        public int Oid { get; set; }
        /// <summary>
        /// 客户名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 代码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 供应商
        /// </summary>
        public string Supplier { get; set; }

        /// <summary>
        /// 供应商代码
        /// </summary>
        public string SupplierCode { get; set; }

        /// <summary>
        /// 客户
        /// </summary>
        public int Client { get; set; }

        /// <summary>
        /// 唯一码
        /// </summary>
        public string UniqueCode { get; set; }
    }
}
