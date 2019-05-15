using System;

namespace WuLang.Common
{
    /// <summary>
    /// keyid生成类
    /// </summary>
    public class KeyIDHelper
    {
        /// <summary>
        /// 锁
        /// </summary>
        public static readonly object obj = new object();

        /// <summary>
        /// 生成文件名字
        /// </summary>
        /// <returns>结果</returns>
        public static string Generator()
        {
            lock (obj)
            {
                string dateStr = DateTime.Now.ToString("yyMMddHHmmssffff");
                string random = new Random(Guid.NewGuid().GetHashCode()).Next(0, 999999).ToString().PadLeft(6, '0');
                return dateStr + random;
            }
        }

        /// <summary>
        /// 生成主键
        /// </summary>
        /// <param name="date">订单号</param>
        /// <returns>结果</returns>
        public static string Generator(string date)
        {
            string keyID = Generator();
            string dateStr = keyID.Substring(6);
            keyID = date.Substring(0, 6) + dateStr;
            return keyID;
        }
    }
}
