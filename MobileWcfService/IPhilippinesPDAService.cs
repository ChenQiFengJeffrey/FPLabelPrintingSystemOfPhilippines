using FPLabelData.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace MobileWcfService
{
    [ServiceContract]
    public interface IPhilippinesPDAService
    {
        [OperationContract]
        string UploadItemCodeAndSNList(ItemCodeSN_UploadDTO value);
    }
}
