using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using Newtonsoft.Json;

namespace Magician.Common.Util
{
    public class HttpContentType
    {
        public const string FORM = "application/x-www-form-urlencoded";
        public const string TEXT = "text/plain";
        public const string JSON = "application/json";
        public const string JAVASCRIPT = "application/javascript";
        public const string XML = "application/xml";
        public const string TEXT_XML = "text/xml";
        public const string HTML = "text/html";
    }

    public class HttpMethod
    {
        public const string GET = "GET";
        public const string POST = "POST";
        public const string PUT = "PUT";
        public const string DELETE = "DELETE";
    }

    public class HttpUtil
    {
        private static int _singleUploadLength = 4096;

        private static int _uploadBandwidth = int.MaxValue;

        private static int _downloadBandwidth = int.MaxValue;

        public int SingleUploadLength
        {
            get { return _singleUploadLength; }
            set { _singleUploadLength = value < 1024 ? 1024 : value; }
        }

        /// <summary>
        /// 上传带宽，单位为kb
        /// </summary>
        public int UploadBandwidth
        {
            get { return _uploadBandwidth; }
            set { _uploadBandwidth = value <= 0 ? int.MaxValue : value; }
        }

        /// <summary>
        /// 下载带宽，单位为kb
        /// </summary>
        public int DownloadBandwidth
        {
            get { return _downloadBandwidth; }
            set { _downloadBandwidth = value <= 0 ? int.MaxValue : value; }
        }

        public static byte[] GetRequest(string url, Dictionary<string, string> dicHeader, out string msg,
            bool isDownload = false, Action<int> cbDownStep = null)
        {
            return SendRequest(HttpMethod.GET, url, null, dicHeader, out msg, null, isDownload, null, cbDownStep);
        }

        public static byte[] DelRequest(string url, Dictionary<string, string> dicHeader, out string msg)
        {
            return SendRequest(HttpMethod.DELETE, url, null, dicHeader, out msg);
        }

        public static byte[] PostRequest(string url, Dictionary<string, object> dicData,
            Dictionary<string, string> dicHeader, out string msg, string filePath = null, string contentType = null,
            Action<int, long, long> cbUpStep = null)
        {
            return SendRequest(HttpMethod.POST, url, dicData, dicHeader, out msg, filePath, false, null, null,
                cbUpStep);
        }

        public static byte[] PutRequest(string url, Dictionary<string, object> dicData,
            Dictionary<string, string> dicHeader, out string msg, string filePath = null, string contentType = null,
            Action<int, long, long> cbUpStep = null)
        {
            return SendRequest(HttpMethod.PUT, url, dicData, dicHeader, out msg, filePath, false, null, null,
                cbUpStep);
        }

        private static Dictionary<string, object> ParsePostData(object postData)
        {
            Dictionary<string, object> dicData;
            if (!(postData is Dictionary<string, object>))
            {
                dicData = JsonConvert.DeserializeObject<Dictionary<string, object>>(
                    JsonConvert.SerializeObject(postData));
            }
            else
            {
                dicData = (Dictionary<string, object>) postData;
            }

            return dicData;
        }

        public static byte[] SendRequest(string method, string url, object postData, Dictionary<string, string> dicHeader, 
            out string msg, string filePath = null, bool isDownload = false, string contentType = null, Action<int> cbDownStep = null, 
            Action<int, long, long> cbUpStep = null, CookieContainer cc = null, Encoding encoding = null)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);

            if (cc != null)
            {
                request.CookieContainer = cc;
            }

            foreach (var key in dicHeader.Keys)
            {
                request.Headers.Add(key, dicHeader[key]);
            }

            if ((method == HttpMethod.GET || method == HttpMethod.DELETE) &&
                (!string.IsNullOrEmpty(filePath) || postData != null))
            {
                throw new Exception(string.Format("使用{0}方法，但存在表单数据", method));
            }
            request.Method = method;
            if (contentType == null)
            {
                contentType = HttpContentType.JSON;
            }
            request.ContentType = contentType;

            if (encoding == null)
            {
                encoding = StringUtil.DefaultEncoding;
            }

            if (!string.IsNullOrEmpty(filePath))
            {
                var boundary = "----------" + DateTime.Now.Ticks.ToString("x");
                request.ContentType = "multipart/form-data; boundary=" + boundary;
                var sb = new StringBuilder();
                if (postData != null)
                {
                    var dicData = ParsePostData(postData);

                    if (dicData != null)
                    {
                        //拼接非文件表单控件  
                        foreach (var key in dicData.Keys)
                        {
                            sb.Append("--" + boundary);
                            sb.Append("\r\n");
                            sb.Append("Content-Disposition: form-data; name=\"" + key + "\"");
                            sb.Append("\r\n\r\n");
                            sb.Append(dicData[key]);
                            sb.Append("\r\n");
                        }
                    }
                }


                //拼接文件控件  
                sb.Append("--" + boundary);
                sb.Append("\r\n");
                sb.Append("Content-Disposition: form-data; name=\"file\"; filename=\"" + FileUtil.GetFileNameNoPath(filePath) + "\"");
                sb.Append("\r\n");
                sb.Append("Content-Type: application/octet-stream");
                sb.Append("\r\n\r\n");

                var postHeaderBytes = encoding.GetBytes(sb.ToString());

                var postFooterBytes = encoding.GetBytes("\r\n--" + boundary + "--\r\n");

                var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);

                request.ContentLength = postHeaderBytes.Length + fs.Length + postFooterBytes.Length;
 
                var singleBufferLength = _singleUploadLength;
                var buffer = new byte[singleBufferLength];

                //已上传的字节数  
                var offset = 0;
                
                var postStream = request.GetRequestStream();
                postStream.Write(postHeaderBytes, 0, postHeaderBytes.Length);

                var size = fs.Read(buffer, 0, singleBufferLength);
                var lastRecordTime = DateTime.Now;
                var lastRecordSize = 0;
                //发送请求头部消息  
                while (size > 0)
                {
                    postStream.Write(buffer, 0, size);
                    offset += size;

                    lastRecordSize += size;
                    var nowTime = DateTime.Now;
                    if ((nowTime - lastRecordTime).TotalMilliseconds >= 1000)
                    {
                        cbUpStep?.Invoke(lastRecordSize, offset, fs.Length);
                        lastRecordSize = 0;
                        lastRecordTime = nowTime;
                    }

                    if (lastRecordSize / 1024 >= _uploadBandwidth)
                    {
                        var sleepMs = 1000 - Convert.ToInt32((nowTime - lastRecordTime).TotalMilliseconds);
                        if (sleepMs > 0)
                        {
                            Thread.Sleep(sleepMs);
                        }
                    }

                    size = fs.Read(buffer, 0, singleBufferLength);
                }

                fs.Close();

                postStream.Write(postFooterBytes, 0, postFooterBytes.Length);
                postStream.Close();
            }
            else if (postData != null)
            {
                string strPostData;
                if (contentType == HttpContentType.JSON)
                {
                    strPostData = JsonConvert.SerializeObject(postData);
                }
                else
                {
                    var dicData = ParsePostData(postData);
                    var sb = new StringBuilder();
                    foreach (var key in dicData.Keys)
                    {
                        sb.Append("&").Append(key).Append("=").Append(dicData[key]);
                    }

                    strPostData = sb.Length > 0 ? sb.Remove(0, 1).ToString() : string.Empty;
                }

                var postStream = request.GetRequestStream();
                var buffer = encoding.GetBytes(strPostData);
                postStream.Write(buffer, 0, buffer.Length);
                postStream.Close();
            }

            try
            {
                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    if (cc != null)
                    {
                        foreach (Cookie cookie in response.Cookies)
                        {
                            cc.Add(cookie);
                        }
                    }

                    byte[] buffer = null;
                    using (var responseStream = response.GetResponseStream())
                    {
                        if (responseStream != null)
                        {
                            buffer = new byte[response.ContentLength];
                            var readSize = 0;
                            while (readSize < response.ContentLength)
                            {
                                readSize += responseStream.Read(buffer, readSize,
                                    (int)response.ContentLength - readSize);
                                cbDownStep?.Invoke(Convert.ToInt32((long)readSize * 1000 / response.ContentLength));
                            }
                        }
                    }

                    msg = string.Empty;
                    return buffer;
                }
            }
            catch (WebException e)
            {
                msg = string.Empty;
                using (var responseStream = e.Response.GetResponseStream())
                {
                    if (e.Response.ContentLength > 0 && responseStream != null)
                    {
                        var buffer = new byte[e.Response.ContentLength];
                        responseStream.Read(buffer, 0, buffer.Length);
                        msg = encoding.GetString(buffer);
                    }
                }

                msg = string.Format("StatusCode:{0};Message:{1}", e.Status, msg);
                return null;
            }
        }
    }
}