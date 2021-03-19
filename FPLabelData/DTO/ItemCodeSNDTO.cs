using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPLabelData.DTO
{
    public class ItemCodeSNDTO
    {
        public int Oid { get; set; }
        /// <summary>
        /// 原材料物料号
        /// </summary>
        public string ItemCode { get; set; }
        /// <summary>
        /// SN
        /// </summary>
        public string SerivalNum { get; set; }
        /// <summary>
        /// 扫描日期
        /// </summary>
        public DateTime CreateDate { get; set; }

    }
}
