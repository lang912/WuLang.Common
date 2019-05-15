using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WuLang.Common
{
    /// <summary>
    /// 结果
    /// </summary>
    public class MResult
    {
        /// <summary>
        /// 错误代码 200 代表成功，其余错误
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Msg { get; set; }
    }

    /// <summary>
    /// 泛型类
    /// </summary>
    /// <typeparam name="T">类型</typeparam>
    public class MResult<T> : MResult
    {
        /// <summary>
        /// 数据区域
        /// </summary>
        public T Data { get; set; }
    }
}
