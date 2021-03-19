using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using FPLabelData.DTO;
using LabelPrintDAL;
using Pivots;
using Pivots.BLL;
using Pivots.Commons;
using Pivots.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace MobileDeviceWebAPI.Controllers
{
    public class InBoundController : ApiController 
    {
        [HttpPost]
        public HttpActionResult<ItemCodeSN_UploadDTO> UploadItemCodeAndSNList([FromBody]string value)
        {
            string appPath = HttpRuntime.AppDomainAppPath;
            HttpActionResult<ItemCodeSN_UploadDTO> result = Newtonsoft.Json.JsonConvert.DeserializeObject<HttpActionResult<ItemCodeSN_UploadDTO>>(value);
            string resultMessage = new ItemCodeSNBLL().UploadItemCodeAndSNList(result.Data);
            if (string.IsNullOrEmpty(resultMessage))
            {
                result.Success = true;
            }
            else
            {
                result.Success = false;
                result.Message = resultMessage;
            }
            return result;
        }

    }
}