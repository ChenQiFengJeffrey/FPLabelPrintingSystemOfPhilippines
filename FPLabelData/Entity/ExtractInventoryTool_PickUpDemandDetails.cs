using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPLabelData.Entity
{
    public class ExtractInventoryTool_PickUpDemandDetails
    {
        public int Oid { get; set; }
        public string SupplierCode { get; set; }
        public string Supplier { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public int SysInventory { get; set; }
        public int InTransit { get; set; }
        public int HUB { get; set; }
        public int Total { get; set; }
        public DateTime ProductionDate { get; set; }
        public int DailyUsage { get; set; }
    }
}
