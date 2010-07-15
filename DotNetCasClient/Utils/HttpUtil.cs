using System.IO;
using System.Net;
using System.Text;

namespace DotNetCasClient.Utils
{
    public static class HttpUtil
    {
        internal static string PerformHttpGet(string url, bool requireHttp200)
        {
            string responseBody = null;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                if (!requireHttp200 || response.StatusCode == HttpStatusCode.OK)
                {
                    using (Stream responseStream = response.GetResponseStream()) 
                    {
                        if (responseStream != null)
                        {
                            using (StreamReader responseReader = new StreamReader(responseStream))
                            {
                                responseBody = responseReader.ReadToEnd();
                            }
                        }
                    }
                }
            }

            return responseBody;
        }

        internal static string PerformHttpPost(string url, string postData, bool requireHttp200)
        {
            string responseBody = null;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = Encoding.UTF8.GetByteCount(postData);

            using (StreamWriter requestWriter = new StreamWriter(request.GetRequestStream()))
            {
                requestWriter.Write(postData);
            }

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                using (Stream responseStream = response.GetResponseStream())
                {
                    if (responseStream != null)
                    {
                        using (StreamReader responseReader = new StreamReader(responseStream))
                        {
                            responseBody = responseReader.ReadToEnd();
                        }
                    }
                }
            }

            return responseBody;
        }
    }
}
