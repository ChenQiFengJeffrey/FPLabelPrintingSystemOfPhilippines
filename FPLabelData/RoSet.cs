using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPLabelData
{
    public class RoSet
    {
        public int Oid { get; set; }
        public int FinishedProductNum { get; set; }//成品料号
        public string RoNumber { get; set; }//RO号(订单追溯码)
    }
}
