using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPLabelData.DTO
{
    public class ItemCodeSN_UploadDTO
    {
        public string RoNumber { get; set; }
        public string FinishedProductNum { get; set; }
        public string ItemCode { get; set; }
        public List<ItemCodeSNDTO> SNList { get; set; }
    }
}
