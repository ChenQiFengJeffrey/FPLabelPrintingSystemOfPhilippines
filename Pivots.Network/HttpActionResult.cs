using Pivots.Commons.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pivots.Network
{
    public class HttpActionResult<T> : IHttpActionResult<T>
    {
        public T Data { get; set; }
        public String DriveID { get; set; }
        public int UserID { get; set; }
        public Boolean Success { get; set; }
        public String Message { get; set; }

        private Dictionary<String, String> _extraParams = new Dictionary<String, String>();

        public Dictionary<String, String> ExtraParams
        {
            get { return _extraParams; }
            set { _extraParams = new Dictionary<String, String>(); }
        }

        public string this[String key]
        {
            //实现索引器的get方法
            get
            {
                return _extraParams[key];
            }

            //实现索引器的set方法
            set
            {
                _extraParams[key] = value;
            }
        }
    }
}
