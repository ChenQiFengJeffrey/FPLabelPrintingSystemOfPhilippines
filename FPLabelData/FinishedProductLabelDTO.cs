using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPLabelData
{
    public class FinishedProductLabelDTO
    {
        public string FinishedProductNum { get; set; }
        public string Oid { get; set; }
        public string ID { get; set; }
        public bool A { get; set; }
        public bool B { get; set; }
        public bool C { get; set; }
        public bool D { get; set; }
        public bool E { get; set; }
        public bool F { get; set; }
        public bool G { get; set; }
        public bool H { get; set; }
        public bool I { get; set; }
        public bool J { get; set; }
        public bool K { get; set; }
        public bool L { get; set; }
        public bool M { get; set; }
        public bool HOME { get; set; }
        public bool SME { get; set; }
        public bool MSI { get; set; }
        public bool FTTH { get; set; }
        public bool MSIVOICEONLY { get; set; }
        public bool COPPERDATAONLY { get; set; }
        public bool FTTHDATAONLY { get; set; }
        public bool FTTHNONWIFI { get; set; }
        public bool FTTHNONWIFIDATAONLY { get; set; }
        public string ONU { get; set; }
        public string VVDSL { get; set; }
        public string TELSET { get; set; }
        public string BIZBOX { get; set; }
        public string Barcode { get; set; }
        public List<GoodSet> GoodList { get; set; }
        public string WorkStation { get; set; }
        public string RoNumber { get; set; }
    }
}
