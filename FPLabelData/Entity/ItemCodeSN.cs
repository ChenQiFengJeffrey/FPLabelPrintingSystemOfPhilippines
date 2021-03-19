using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPLabelData.Entity
{
        public class ItemCodeSN
        {
            public int Oid { get; set; }
            /// <summary>
            /// 原材料物料号
            /// </summary>
            public string ItemCode { get; set; }
            /// <summary>
            /// 原材料名称
            /// </summary>
            public string ItemName { get; set; }
            /// <summary>
            /// 成品物料号
            /// </summary>
            public string FinishedProductNum { get; set; }
            /// <summary>
            /// SN
            /// </summary>
            public string SerivalNum { get; set; }
            /// <summary>
            /// RO
            /// </summary>
            public string RoNumber { get; set; }
            /// <summary>
            /// 打印日期
            /// </summary>
            public DateTime PrintDate { get; set; }
            /// <summary>
            /// 打印日期
            /// </summary>
            public DateTime CreateDate { get; set; }
            /// <summary>
            /// mainitem
            /// </summary>
            public string MainItem { get; set; }
            /// <summary>
            /// status
            /// </summary>
            public string Status { get; set; }
        }
}
