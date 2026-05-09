using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;


namespace GSP.Mes
{
    public class HttpHelper
    {
       public HttpHelper() { }
       
        /// <summary> 
        /// 创建GET方式的HTTP请求 
        /// </summary> 
        public string GetHttpResponse(string url, int timeout, string userAgent, CookieCollection cookies)
        {
            try
            {
                HttpWebRequest request = null;
                if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
                {
                    //对服务端证书进行有效性校验（非第三方权威机构颁发的证书，如自己生成的，不进行验证，这里返回true）
                    ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                    request = WebRequest.Create(url) as HttpWebRequest;
                    request.ProtocolVersion = HttpVersion.Version10;    //http版本，默认是1.1,这里设置为1.0
                }
                else
                {
                   request = WebRequest.Create(url) as HttpWebRequest;
                }
                request.Method = "GET";

                //设置代理UserAgent和超时
                request.UserAgent = userAgent;
                request.Timeout = timeout;
                if (cookies != null)
                {
                    request.CookieContainer = new CookieContainer();
                    request.CookieContainer.Add(cookies);
                }
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                return GetResponseString(response);
            }
            catch(Exception e)
            {
              throw e;                
            }
        }
        /// <summary> 
        /// 创建POST方式的HTTP请求 
        /// </summary> 
        public string PostHttpResponse(string url, IDictionary<string, string> parameters, int timeout, string userAgent, CookieCollection cookies)
        {

            HttpWebRequest request = null;
            //如果是发送HTTPS请求 
            if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
            {
                //ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                request = WebRequest.Create(url) as HttpWebRequest;
                //request.ProtocolVersion = HttpVersion.Version10;
            }
            else
            {
                request = WebRequest.Create(url) as HttpWebRequest;
            }
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";

            //设置代理UserAgent和超时
            //request.UserAgent = userAgent;
            request.Timeout = timeout;

            if (cookies != null)
            {
                request.CookieContainer = new CookieContainer();
                request.CookieContainer.Add(cookies);
            }
            //发送POST数据 
            if (!(parameters == null || parameters.Count == 0))
            {
                StringBuilder buffer = new StringBuilder();
                int i = 0;
                foreach (string key in parameters.Keys)
                {
                    if (i > 0)
                    {
                        buffer.AppendFormat("&{0}={1}", key, parameters[key]);
                    }
                    else
                    {
                        buffer.AppendFormat("{0}={1}", key, parameters[key]);
                        i++;
                    }
                }
                byte[] data = Encoding.ASCII.GetBytes(buffer.ToString());
                using (Stream stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }
            }
            string[] values = request.Headers.GetValues("Content-Type");
            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            return GetResponseString(response);

        }

        public string PostHttpResponse(string url, string Jsionstr, int timeout, string userAgent, CookieCollection cookies)
        {
            try
            {
                HttpWebRequest request = null;
                //如果是发送HTTPS请求 
                if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
                {
                    //ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                    request = WebRequest.Create(url) as HttpWebRequest;
                    //request.ProtocolVersion = HttpVersion.Version10;
                }
                else
                {
                    request = WebRequest.Create(url) as HttpWebRequest;
                }
                request.Method = "POST";
                request.Accept = "text/html, application/xhtml+xml, */*";
                request.ContentType ="application/json";

                //设置代理UserAgent和超时
                request.UserAgent = userAgent;
                request.Timeout = timeout;

                if (cookies != null)
                {
                    request.CookieContainer = new CookieContainer();
                    request.CookieContainer.Add(cookies);
                }
                //发送POST数据 
                byte[] data = Encoding.UTF8.GetBytes(Jsionstr);
               
                using (Stream stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }

                string[] values = request.Headers.GetValues("Content-Type");
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                return GetResponseString(response);
            }
            catch (Exception ex) { return ex.ToString(); }
        }
        /// <summary>
        /// 获取请求的数据
        /// </summary>
        private string GetResponseString(HttpWebResponse webresponse)
        {
            using (Stream s = webresponse.GetResponseStream())
            {
                StreamReader reader = new StreamReader(s, Encoding.UTF8);
                return reader.ReadToEnd();
            }
        }

        /// <summary>
        /// 验证证书
        /// </summary>
        private bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            if (errors == SslPolicyErrors.None)
                return true;
            return false;
        }
    }
}
