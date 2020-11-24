using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPLabelPrintingSystemOfPhilippines.ColumnNameConfig
{
    public class FinishedProductLabelDTO
    {
        public string ID { get; set; }

        public string Barcode { get; set; }

        public List<SN> SNList { get; set; }

        public List<Package> Package { get; set; }

        public List<GoodSet> goodList { get; set; }
    }
}
