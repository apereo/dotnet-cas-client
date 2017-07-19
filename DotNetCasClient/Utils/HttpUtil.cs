/*
 * Licensed to Apereo under one or more contributor license
 * agreements. See the NOTICE file distributed with this work
 * for additional information regarding copyright ownership.
 * Apereo licenses this file to you under the Apache License,
 * Version 2.0 (the "License"); you may not use this file
 * except in compliance with the License. You may obtain a
 * copy of the License at:
 * 
 * http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on
 * an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied. See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */
using System.IO;
using System.Net;
using System.Text;

namespace DotNetCasClient.Utils
{
    /// <summary>
    /// A helper utility class to facilitate outbound HTTP GET and POST request
    /// </summary>
    /// <author>Scott Holodak</author>
    internal static class HttpUtil
    {
        /// <summary>
        /// Executes an HTTP GET request against the Url specified, returning the 
        /// entire response body in string form.
        /// </summary>
        /// <param name="url">The URL to request</param>
        /// <param name="requireHttp200">
        /// Boolean indicating whether or not to return 
        /// null if the repsonse status code is not 200 (OK).
        /// </param>
        /// <returns>
        /// The response body or null if the response status is required to 
        /// be 200 (OK) but is not
        /// </returns>
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

        /// <summary>
        /// Executes an HTTP POST against the Url specified with the supplied post data, 
        /// returning the entire response body in string form.
        /// </summary>
        /// <param name="url">The URL to post to</param>
        /// <param name="postData">The x-www-form-urlencoded data to post to the URL</param>
        /// <param name="requireHttp200">
        /// Boolean indicating whether or not to return 
        /// null if the repsonse status code is not 200 (OK).
        /// </param>
        /// <returns>
        /// The response body or null if the response status is required to 
        /// be 200 (OK) but is not
        /// </returns>
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
