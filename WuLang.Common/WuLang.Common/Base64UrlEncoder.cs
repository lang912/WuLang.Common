using System;
using System.Text;

namespace WuLang.Common
{
    /// <summary>
    /// jwt的base64编码
    /// </summary>
    public static class Base64UrlEncoder
    {
        private static char base64PadCharacter = '=';
        private static string doubleBase64PadCharacter = "==";
        private static char base64Character62 = '+';
        private static char base64Character63 = '/';
        private static char base64UrlCharacter62 = '-';
        private static char _base64UrlCharacter63 = '_';

        /// <summary>
        /// 加码
        /// </summary>
        /// <param name="arg">待编码字符串</param>
        /// <returns>结果</returns>
        public static string Encode(string arg)
        {
            if (arg == null)
            {
                throw new ArgumentNullException();
            }

            return Encode(Encoding.UTF8.GetBytes(arg));
        }

        /// <summary>
        /// 编码
        /// </summary>
        /// <param name="inArray">待编码数组</param>
        /// <returns>结果</returns>
        public static string Encode(byte[] inArray)
        {
            if (inArray == null)
            {
                throw new ArgumentNullException();
            }

            string s = Convert.ToBase64String(inArray, 0, inArray.Length);
            s = s.Split(base64PadCharacter)[0]; // 去掉所有等号
            s = s.Replace(base64Character62, base64UrlCharacter62);  // 替换加号为减号
            s = s.Replace(base64Character63, _base64UrlCharacter63);  // 替换斜线为下划线
            return s;
        }

        /// <summary>
        /// 解码
        /// </summary>
        /// <param name="arg">参数</param>
        /// <returns>结果</returns>
        public static string Decode(string arg)
        {
            return Encoding.UTF8.GetString(DecodeBytes(arg));
        }

        /// <summary>
        /// 解码
        /// </summary>
        /// <param name="str">参数</param>
        /// <returns>结果</returns>
        public static byte[] DecodeBytes(string str)
        {
            if (str == null)
            {
                throw new ArgumentNullException();
            }

            // 替换之前的斜线和减号
            str = str.Replace(base64UrlCharacter62, base64Character62);
            str = str.Replace(_base64UrlCharacter63, base64Character63);

            switch (str.Length % 4)
            {
                case 0:
                    break;
                case 2:
                    str += doubleBase64PadCharacter;
                    break;
                case 3:
                    str += base64PadCharacter;
                    break;
                default:
                    throw new FormatException("输入字符串格式有误");
            }

            return Convert.FromBase64String(str);
        }
    }
}
