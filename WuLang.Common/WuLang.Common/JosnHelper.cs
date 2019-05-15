using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WuLang.Common
{
    /// <summary>
    /// 序列化帮助类
    /// </summary>
    public static class JosnHelper
    {
        /// <summary>
        /// 序列化
        /// </summary>
        /// <param name="obj">源数据</param>
        /// <param name="props">传入的属性数组</param>
        /// <param name="retain">true:表示props是需要保留的字段  false：表示props是要排除的字段</param>
        /// <returns></returns>
        public static string SerializeObject(this object obj, string[] props = null, bool retain = true)
        {
            try
            {
                JsonSerializerSettings settings = new JsonSerializerSettings()
                {
                    DateFormatString = "yyyy-MM-dd HH:mm:ss"
                };
                if (props != null)
                {
                    settings.ContractResolver = new LimitPropsContractResolver(props, retain);
                    return JsonConvert.SerializeObject(obj, Formatting.Indented, settings);
                }
                return JsonConvert.SerializeObject(obj, settings);
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        public static T DeserializeObject<T>(this string json)
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(json);
            }
            catch
            {
                return default(T);
            }
        }

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static object DeserializeObject(this string json)
        {
            try
            {
                return JsonConvert.DeserializeObject(json);
            }
            catch
            {
                return (object)null;
            }
        }


        class LimitPropsContractResolver : DefaultContractResolver
        {
            private readonly string[] props = null;

            private readonly bool retain;

            /// <summary>
            /// 构造函数
            /// </summary>
            /// <param name="props">传入的属性数组</param>
            /// <param name="retain">true:表示props是需要保留的字段  false：表示props是要排除的字段</param>
            public LimitPropsContractResolver(string[] props, bool retain = true)
            {
                //指定要序列化属性的清单
                this.props = props;

                this.retain = retain;
            }

            protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
            {
                IList<JsonProperty> list = base.CreateProperties(type, memberSerialization);

                //只序列化清单列出的属性
                if (retain)
                {
                    return list.Where(p => props.Contains(p.PropertyName, StringComparer.OrdinalIgnoreCase)).ToList();
                }
                //过滤掉清单列出的属性
                else
                {
                    return list.Where(p => !props.Contains(p.PropertyName, StringComparer.OrdinalIgnoreCase)).ToList();
                }
            }
        }
    }
}
