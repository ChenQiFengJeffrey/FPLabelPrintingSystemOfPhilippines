using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FPLabelData.DTO;
using LabelPrintDAL;

namespace MobileWcfService
{
    public class PhilippinesPDAService : IPhilippinesPDAService
    {
        public string UploadItemCodeAndSNList(ItemCodeSN_UploadDTO value)
        {
            return new ItemCodeSNBLL().UploadItemCodeAndSNList(value);
        }
    }
}
