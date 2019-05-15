using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace WuLang.Common
{
    /// <summary>
    /// http请求帮助类
    /// </summary>
    public class APIClient
    {
        /// <summary>
        /// get请求
        /// </summary>
        /// <typeparam name="T">返回值类型</typeparam>
        /// <param name="url">请求url</param>
        /// <param name="methodName">方法名字，记录日志</param>
        /// <returns>结果</returns>
        public static T Get<T>(string url, string methodName = "Get")
        {

            DateTime startTime = DateTime.Now;
            string result = string.Empty;
            try
            {
                if (url.StartsWith("https"))
                {
                    System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;
                }

                HttpClient client = new HttpClient();
                client.Timeout = new TimeSpan(0, 1, 0);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage responseMessage = client.GetAsync(url).Result;
                responseMessage.EnsureSuccessStatusCode();
                result = responseMessage.Content.ReadAsStringAsync().Result;
                if (typeof(T) == typeof(string))
                {
                    return (T)Convert.ChangeType(result, typeof(T));
                }

                return JosnHelper.DeserializeObject<T>(result);
            }
            finally
            {
                WriteLog(url, methodName, startTime, string.Empty, result);
            }
        }

        /// <summary>
        /// 带header的get请求
        /// </summary>
        /// <typeparam name="T">返回值类型</typeparam>
        /// <param name="url">请求url</param>
        /// <param name="headers">header</param>
        /// <param name="methodName">方法名字，记录日志</param>
        /// <returns>结果</returns>
        public static T GetWithHeader<T>(string url, Dictionary<string, string> headers, string methodName = "Get")
        {

            DateTime startTime = DateTime.Now;
            string result = string.Empty;
            try
            {
                if (url.StartsWith("https"))
                {
                    System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;
                }

                HttpClient client = new HttpClient();
                client.Timeout = new TimeSpan(0, 1, 0);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                if (headers != null && headers.Count > 0)
                {
                    foreach (var item in headers.Keys)
                    {
                        client.DefaultRequestHeaders.Add(item, headers[item]);
                    }
                }
                HttpResponseMessage responseMessage = client.GetAsync(url).Result;
                responseMessage.EnsureSuccessStatusCode();
                result = responseMessage.Content.ReadAsStringAsync().Result;
                if (typeof(T) == typeof(string))
                {
                    return (T)Convert.ChangeType(result, typeof(T));
                }

                return JosnHelper.DeserializeObject<T>(result);
            }
            finally
            {
                WriteLog(url, methodName, startTime, JosnHelper.SerializeObject(headers), result);
            }
        }

        /// <summary>
        /// jsonpost请求
        /// </summary>
        /// <typeparam name="T">返回结果</typeparam>
        /// <param name="url">请求地址</param>
        /// <param name="json">入参</param>
        /// <param name="methodName">方法名，记录日志</param>
        /// <param name="headers">请求头</param>
        /// <returns>结果</returns>
        public static T PostJsonWithHeader<T>(string url, string json, Dictionary<string, string> headers, string methodName = "PostJson")
        {
            DateTime startTime = DateTime.Now;
            string result = string.Empty;
            try
            {
                if (url.StartsWith("https"))
                {
                    System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;
                }
                HttpClient client = new HttpClient();
                client.Timeout = new TimeSpan(0, 1, 0);
                HttpContent httpContent = new StringContent(json);
                httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                if (headers != null && headers.Count > 0)
                {
                    foreach (var item in headers.Keys)
                    {
                        client.DefaultRequestHeaders.Add(item, headers[item]);
                    }
                }

                HttpResponseMessage responseMessage = client.PostAsync(url, httpContent).Result;
                responseMessage.EnsureSuccessStatusCode();
                result = responseMessage.Content.ReadAsStringAsync().Result;
                if (typeof(T) == typeof(string))
                {
                    return (T)Convert.ChangeType(result, typeof(T));
                }
                return JosnHelper.DeserializeObject<T>(result);
            }
            finally
            {
                WriteLog(url, methodName, startTime, $"{json}{Environment.NewLine}{JosnHelper.SerializeObject(headers)}", result);
            }
        }

        /// <summary>
        /// jsonpost请求
        /// </summary>
        /// <typeparam name="T">返回结果</typeparam>
        /// <param name="url">请求地址</param>
        /// <param name="json">入参</param>
        /// <param name="methodName">方法名，记录日志</param>
        /// <returns>结果</returns>
        public static T PostJson<T>(string url, string json, string methodName = "PostJson")
        {
            DateTime startTime = DateTime.Now;
            string result = string.Empty;
            try
            {
                if (url.StartsWith("https"))
                {
                    System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;
                }
                HttpClient client = new HttpClient();
                client.Timeout = new TimeSpan(0, 1, 0);
                HttpContent httpContent = new StringContent(json);
                httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                HttpResponseMessage responseMessage = client.PostAsync(url, httpContent).Result;
                responseMessage.EnsureSuccessStatusCode();
                result = responseMessage.Content.ReadAsStringAsync().Result;
                if (typeof(T) == typeof(string))
                {
                    return (T)Convert.ChangeType(result, typeof(T));
                }
                return JosnHelper.DeserializeObject<T>(result);
            }
            finally
            {
                WriteLog(url, methodName, startTime, json, result);
            }

        }

        /// <summary>
        /// 以text/json提交
        /// </summary>
        /// <typeparam name="T">返回类型</typeparam>
        /// <param name="url">请求地址</param>
        /// <param name="json">入参</param>
        /// <param name="methodName">方法名，记录日志</param>
        /// <returns>结果</returns>
        public static T PostText<T>(string url, string json, string methodName = "PostText")
        {
            DateTime startTime = DateTime.Now;
            string result = string.Empty;
            try
            {
                if (url.StartsWith("https"))
                {
                    System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;
                }
                HttpClient client = new HttpClient();
                client.Timeout = new TimeSpan(0, 1, 0);
                HttpContent httpContent = new StringContent(json);
                httpContent.Headers.ContentType = new MediaTypeHeaderValue("text/json");
                HttpResponseMessage responseMessage = client.PostAsync(url, httpContent).Result;
                responseMessage.EnsureSuccessStatusCode();
                result = responseMessage.Content.ReadAsStringAsync().Result;
                if (typeof(T) == typeof(string))
                {
                    return (T)Convert.ChangeType(result, typeof(T));
                }
                return JosnHelper.DeserializeObject<T>(result);
            }
            finally
            {
                WriteLog(url, methodName, startTime, json, result);
            }
        }

        /// <summary>
        /// 以text/json提交
        /// </summary>
        /// <typeparam name="T">返回类型</typeparam>
        /// <param name="url">请求地址</param>
        /// <param name="json">入参</param>
        /// <param name="methodName">方法名，记录日志</param>
        /// <returns>结果</returns>
        public static T PostForm<T>(string url, string json, string methodName = "PostForm")
        {
            DateTime startTime = DateTime.Now;
            string result = string.Empty;
            try
            {
                if (url.StartsWith("https"))
                {
                    System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;
                }
                HttpClient client = new HttpClient();
                client.Timeout = new TimeSpan(0, 1, 0);
                HttpContent httpContent = new StringContent(json);
                httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
                HttpResponseMessage responseMessage = client.PostAsync(url, httpContent).Result;
                responseMessage.EnsureSuccessStatusCode();
                result = responseMessage.Content.ReadAsStringAsync().Result;
                if (typeof(T) == typeof(string))
                {
                    return (T)Convert.ChangeType(result, typeof(T));
                }
                return JosnHelper.DeserializeObject<T>(result);
            }
            finally
            {
                WriteLog(url, methodName, startTime, json, result);
            }
        }

        /// <summary>
        /// 记录日志,需要引入NLOG，没有的话无法记录
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="method">方法名字</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="paramaters">请求参数</param>
        /// <param name="result">结果</param>
        private static void WriteLog(string url, string method, DateTime startTime, string paramaters, string result)
        {
            try
            {
                StringBuilder build = new StringBuilder();
                build.Append($"{Environment.NewLine}");
                build.Append($"RequestUri:{url}{Environment.NewLine}");
                build.Append($"ActionName:{method}{Environment.NewLine}");
                build.Append($"EnterTime:{startTime.ToString("yyyy-MM-dd HH:mm:ss")}{Environment.NewLine}");
                build.Append($"CostTime:{(DateTime.Now.Subtract(startTime)).TotalMilliseconds}{Environment.NewLine}");
                build.Append($"Paramaters:{paramaters}{Environment.NewLine}");
                build.Append($"Result:{result}{Environment.NewLine}");
                build.Append($"{Environment.NewLine}{Environment.NewLine}{Environment.NewLine}");
                NLog.LogManager.GetCurrentClassLogger().Info(build.ToString());
            }
            catch (Exception)
            {

            }
        }
    }
}
