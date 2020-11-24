using Microsoft.CSharp;
using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Services.Description;
using System.Xml;

namespace LabelPrintDAL
{
    public static class WebServiceHelper
    {
        /// <summary>
        /// 动态调用WebService
        /// </summary>
        /// <param name="url">WebService地址</param>
        /// <param name="methodname">方法名(模块名)</param>
        /// <param name="args">参数列表,无参数为null</param>
        /// <returns>object</returns>
        public static object InvokeWebService(string url, string methodname, object[] args)
        {
            return InvokeWebService(url, null, methodname, args);
        }
        /// <summary>
        /// 动态调用WebService
        /// </summary>
        /// <param name="url">WebService地址</param>
        /// <param name="classname">类名</param>
        /// <param name="methodname">方法名(模块名)</param>
        /// <param name="args">参数列表</param>
        /// <returns>object</returns>
        public static object InvokeWebService(string url, string classname, string methodname, object[] args)
        {
            string @namespace = "WebService.webservice";
            if (classname == null || classname == "")
            {
                classname = WebServiceHelper.GetClassName(url);
            }
            Pivots.ConfigurationManager.Instance.LoadConfig();
            string agencyname = Pivots.ConfigurationManager.Instance.ConfigDocument.SelectSingleNode("//agencyname").InnerText;
            string agencypassword = Pivots.ConfigurationManager.Instance.ConfigDocument.SelectSingleNode("//agencypassword").InnerText;
            string agencyterritory = Pivots.ConfigurationManager.Instance.ConfigDocument.SelectSingleNode("//agencyterritory").InnerText;
            string agencysite = Pivots.ConfigurationManager.Instance.ConfigDocument.SelectSingleNode("//agencysite").InnerText;
            string agencyport = Pivots.ConfigurationManager.Instance.ConfigDocument.SelectSingleNode("//agencyport").InnerText;
            string agencyurl = "";
            if (!string.IsNullOrWhiteSpace(agencysite) && !string.IsNullOrWhiteSpace(agencyport))
                agencyurl = agencysite + ":" + agencyport;
            Pivots.ConfigurationManager.Instance.LoadConfig();

            WebClient wc = new WebClient();

            if (!string.IsNullOrWhiteSpace(agencyurl))
            {
                WebProxy myProxy = new WebProxy(agencyurl, true);
                if (string.IsNullOrWhiteSpace(agencyterritory))
                    myProxy.Credentials = new System.Net.NetworkCredential(agencyname, agencypassword);
                else
                    myProxy.Credentials = new System.Net.NetworkCredential(agencyname, agencypassword, agencyterritory);
                wc.Proxy = myProxy;
                //wc.BaseAddress = url;
                //wc.Proxy = WebRequest.DefaultWebProxy;
                ////wc.Credentials = System.Net.CredentialCache.DefaultCredentials;
                ////wc.Proxy.Credentials = System.Net.CredentialCache.DefaultCredentials;
                //wc.Credentials = new NetworkCredential("user", "password");
                //wc.Proxy.Credentials = new NetworkCredential("user", "password");
            }
            Stream stream = wc.OpenRead(url + "?WSDL");   //获取服务描述语言(WSDL)
            //XmlTextReader reader = new XmlTextReader(url + "?WSDL");
            ServiceDescription sd = ServiceDescription.Read(stream);    //通过直接从 Stream实例加载 XML 来初始化ServiceDescription类的实例。
            ServiceDescriptionImporter sdi = new ServiceDescriptionImporter();

            sdi.AddServiceDescription(sd, "", "");
            CodeNamespace cn = new CodeNamespace(@namespace);  //CodeNamespace表示命名空间声明。
                                                               //生成客户端代理类代码
            CodeCompileUnit ccu = new CodeCompileUnit();
            ccu.Namespaces.Add(cn);
            sdi.Import(cn, ccu);
            CSharpCodeProvider csc = new CSharpCodeProvider();

            ICodeCompiler icc = csc.CreateCompiler();//取得C#程式码编译器的执行个体

            //设定编译器的参数
            CompilerParameters cplist = new CompilerParameters();//创建编译器的参数实例
            cplist.GenerateExecutable = false;
            cplist.GenerateInMemory = true;
            cplist.ReferencedAssemblies.Add("System.dll");
            cplist.ReferencedAssemblies.Add("System.XML.dll");
            cplist.ReferencedAssemblies.Add("System.Web.Services.dll");
            cplist.ReferencedAssemblies.Add("System.Data.dll");
            //编译代理类
            CompilerResults cr = icc.CompileAssemblyFromDom(cplist, ccu);
            if (true == cr.Errors.HasErrors)
            {
                System.Text.StringBuilder sb = new StringBuilder();
                foreach (CompilerError ce in cr.Errors)
                {
                    sb.Append(ce.ToString());
                    sb.Append(System.Environment.NewLine);
                }
                throw new Exception(sb.ToString());
            }

            //生成代理实例,并调用方法
            System.Reflection.Assembly assembly = cr.CompiledAssembly;
            Type t = assembly.GetType(@namespace + "." + classname, true, true);
            object obj = Activator.CreateInstance(t);
            System.Reflection.MethodInfo mi = t.GetMethod(methodname);//MethodInfo 的实例可以通过调用GetMethods或者Type对象或派生自Type的对象的GetMethod方法来获取，还可以通过调用表示泛型方法定义的 MethodInfo 的MakeGenericMethod方法来获取。
            return mi.Invoke(obj, args);

        }


        private static string GetClassName(string url)
        {
            //假如URL为"http://localhost/InvokeService/Service1.asmx"
            //最终的返回值为 Service1
            string[] parts = url.Split('/');
            string[] pps = parts[parts.Length - 1].Split('.');
            return pps[0];
        }


        /// <summary>
        /// HTTPPost方法调用数据接口
        /// </summary>
        /// <param name="url">调用的URL地址</param>
        /// <param name="parameter">传递参数</param>
        /// <param name="charset">字符码</param>
        /// <returns></returns>
        public static string HttpPostWebService(string url, string methodname, Dictionary<string, string> parameter, Encoding charset)
        {
            Pivots.ConfigurationManager.Instance.LoadConfig();
            string agencyname = Pivots.ConfigurationManager.Instance.ConfigDocument.SelectSingleNode("//agencyname").InnerText;
            string agencypassword = Pivots.ConfigurationManager.Instance.ConfigDocument.SelectSingleNode("//agencypassword").InnerText;
            string agencyterritory = Pivots.ConfigurationManager.Instance.ConfigDocument.SelectSingleNode("//agencyterritory").InnerText;
            string agencysite = Pivots.ConfigurationManager.Instance.ConfigDocument.SelectSingleNode("//agencysite").InnerText;
            string agencyport = Pivots.ConfigurationManager.Instance.ConfigDocument.SelectSingleNode("//agencyport").InnerText;

            WebProxy myProxy = null;
            Pivots.ConfigurationManager.Instance.LoadConfig();
            if (!string.IsNullOrWhiteSpace(agencysite) && !string.IsNullOrWhiteSpace(agencyport))
            {
                myProxy = new WebProxy(agencysite, Convert.ToInt32(agencyport));
                if (string.IsNullOrWhiteSpace(agencyterritory))
                    myProxy.Credentials = new System.Net.NetworkCredential(agencyname, agencypassword);
                else
                    myProxy.Credentials = new System.Net.NetworkCredential(agencyname, agencypassword, agencyterritory);
                //myProxy.BypassProxyOnLocal = false;
            }
            string param = string.Empty;
            if (!(parameter == null || parameter.Count == 0))
            {
                StringBuilder buffer = new StringBuilder();
                int i = 0;
                foreach (string key in parameter.Keys)
                {
                    if (i > 0)
                    {
                        buffer.AppendFormat("&{0}={1}", key, parameter[key]);
                    }
                    else
                    {
                        buffer.AppendFormat("{0}={1}", key, parameter[key]);
                    }
                    i++;
                }
                param = buffer.ToString();
            }
            return HttpPostWebService(url, methodname, parameter, charset, myProxy);
            //return HttpPostWebService(url, methodname, param);
        }


        /// <summary>
        /// HTTPPost方法调用数据接口
        /// </summary>
        /// <param name="url">调用的URL地址</param>
        /// <param name="parameter">传递参数</param>
        /// <param name="charset">字符码</param>
        /// <param name="webProxy">代理对象</param>
        public static string HttpPostWebService(string url, string methodname, Dictionary<string, string> parameter, Encoding charset, WebProxy webProxy)
        {
            try
            {
                //创建WebRequest调用对象
                HttpWebRequest webRequest = WebRequest.Create(url + "/" + methodname) as HttpWebRequest;
                if (webProxy != null)
                {
                    //设置通过代理完成调用
                    webRequest.Proxy = webProxy;
                    //webRequest.Credentials = webProxy.Credentials;
                }
                //数据编码为键值对
                webRequest.Method = "POST";
                webRequest.ContentType = "application/x-www-form-urlencoded";
                webRequest.Timeout = 600000;//定义请求时间
                //webRequest.UseDefaultCredentials = true;
                //写入参数内容
                if (!(parameter == null || parameter.Count == 0))
                {
                    StringBuilder buffer = new StringBuilder();
                    int i = 0;
                    foreach (string key in parameter.Keys)
                    {
                        if (i > 0)
                        {
                            buffer.AppendFormat("&{0}={1}", key, parameter[key]);
                        }
                        else
                        {
                            buffer.AppendFormat("{0}={1}", key, parameter[key]);
                        }
                        i++;
                    }
                    byte[] data = charset.GetBytes(buffer.ToString());
                    webRequest.ContentLength = data.Length;
                    using (Stream stream = webRequest.GetRequestStream())
                    {
                        stream.Write(data, 0, data.Length);
                    }
                }
                //调用返回内容
                string ReturnVal = string.Empty;
                //发起调用操作
                using (WebResponse response = webRequest.GetResponse())
                {
                    //响应数据流
                    Stream stream = response.GetResponseStream();

                    #region 这种方式读取到的是一个返回的结果字符串
                    XmlTextReader Reader = new XmlTextReader(stream);
                    Reader.MoveToContent();
                    ReturnVal = Reader.ReadInnerXml();
                    #endregion

                    #region 这种方式读取到的是一个Xml格式的字符串
                    ////以UTF-8编码转换为StreamReader
                    //StreamReader reader = new StreamReader(stream, Encoding.GetEncoding("UTF-8"));
                    ////读取至结束
                    //ReturnVal = reader.ReadToEnd();
                    #endregion

                    response.Dispose();
                    response.Close();

                    Reader.Dispose();
                    Reader.Close();

                    stream.Dispose();
                    stream.Close();
                }
                return ReturnVal;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            finally
            {
                GC.Collect();
            }
        }


        public static string HttpPostWebService(string url, string method, string param)
        {
            string result = string.Empty;

            Stream writer = null;
            HttpWebRequest request = null;
            HttpWebResponse response = null;


            byte[] bytes = Encoding.UTF8.GetBytes(param);

            request = (HttpWebRequest)WebRequest.Create(url + "/" + method);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = bytes.Length;

            try
            {
                writer = request.GetRequestStream();  //获取用于写入请求数据的Stream对象
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            writer.Write(bytes, 0, bytes.Length);  //把参数数据写入请求数据流
            writer.Close();

            try
            {
                response = (HttpWebResponse)request.GetResponse();  //获得响应
            }
            catch (WebException ex)
            {
                return ex.Message;
            }

            #region 这种方式读取到的是一个返回的结果字符串
            Stream stream = response.GetResponseStream();  //获取响应流
            XmlTextReader Reader = new XmlTextReader(stream);
            Reader.MoveToContent();
            result = Reader.ReadInnerXml();
            #endregion

            #region 这种方式读取到的是一个Xml格式的字符串
            //StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
            //result = reader.ReadToEnd();
            #endregion

            response.Dispose();
            response.Close();

            //reader.Close();
            //reader.Dispose();

            Reader.Dispose();
            Reader.Close();

            stream.Dispose();
            stream.Close();

            return result;
        }

    }
}
