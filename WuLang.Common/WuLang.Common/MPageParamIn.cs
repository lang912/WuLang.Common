using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WuLang.Common
{
    /// <summary>
    /// 分页请求参数
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MPageParamIn<T>
    {
        /// <summary>
        /// 当前页
        /// </summary>
        public int PageNo { get; set; }

        /// <summary>
        /// 每页行数
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// 条件集
        /// </summary>
        public T Params { get; set; }
    }
}
