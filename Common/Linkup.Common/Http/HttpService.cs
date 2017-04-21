using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using System.Net;
using System.Web;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using System.Security.Cryptography.X509Certificates;

namespace Linkup.Common
{
    public class HttpService
    {
        private static HttpService _instance;
        public static HttpService Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new HttpService();
                return _instance;
            }
            set { _instance = value; }
        }

        private HttpService()
        {

        }

        public HttpRequestResult Request(HttpRequestArgs args)
        {
            HttpWebResponse response = null;
            Stream receiveStream = null;
            StreamReader readStream = null;

            MemoryStream postStream = new MemoryStream();

            HttpRequestResult result = new HttpRequestResult();

            string uri = args.Url;

            if (String.IsNullOrEmpty(uri))
            {
                result.Exception = new Exception("没有要请求的API URI信息");
                return result;
            }

            try
            {
                HttpWebRequest request;

                request = (HttpWebRequest)WebRequest.Create(uri);
                request.Method = args.Method;

                //判断是否使用证书
                if (args.UseCertificate)
                {
                    X509Certificate2 cert = new X509Certificate2(args.CertificatePath, args.CertificatePassword);
                    request.ClientCertificates.Add(cert);
                }

                //直接POST字符串内容，一般是JSON直接发到WEB API上
                if (String.IsNullOrEmpty(args.Content) == false)
                {
                    StreamWriter writer = new StreamWriter(postStream);
                    writer.Write(args.Content);
                    writer.Flush();                   
                }
                
                if (postStream.Length > 0)
                {
                    using (Stream stream = request.GetRequestStream())
                    {
                        postStream.WriteTo(stream);
                    }
                }

                response = (HttpWebResponse)request.GetResponse();

                receiveStream = response.GetResponseStream();
                Encoding encode = System.Text.Encoding.GetEncoding("utf-8");
                readStream = new StreamReader(receiveStream, encode);

                result.Content = readStream.ReadToEnd();
            }
            catch (Exception ex)
            {
                Debug.Assert(false, ex.Message);
                result.Exception = ex;
            }
            finally
            {
                if (response != null)
                    response.Close();
                if (receiveStream != null)
                    receiveStream.Close();
                if (readStream != null)
                    readStream.Close();
                if (postStream != null)
                    postStream.Close();
            }

            return result;
        }
    }
}
