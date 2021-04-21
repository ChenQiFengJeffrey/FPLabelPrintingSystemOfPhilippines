using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPLabelData.Entity
{
    public class ExtractInventoryTool_MaterialExtension: ExtractInventoryTool_Material
    {
        public string ClientName { get; set;}
        public string ClientCode { get; set; }
        /// <summary>
        /// 前端下拉选项卡显示用，代码+名称
        /// </summary>
        public string NameCode {
            get {
                return Code + Name;
            }
        }
    }
}
