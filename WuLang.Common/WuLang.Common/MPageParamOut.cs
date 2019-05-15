using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WuLang.Common
{
    /// <summary>
    /// 分页结果
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PageParamOut<T> : MResult
    {
        /// <summary>
        /// 总数据量
        /// </summary>
        public int Total { get; set; }

        /// <summary>
        /// 当前页数据量
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// 当前页编号
        /// </summary>
        public int PageNo { get; set; }

        /// <summary>
        /// 数据
        /// </summary>
        public IList<T> Rows { get; set; }
    }
}
