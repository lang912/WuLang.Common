using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace WuLang.Common
{
    public class JWTUtility
    {
        /// <summary>
        /// 生成jwt
        /// </summary>
        /// <param name="dic">jwt存储的用户信息</param>
        /// <param name="exp">过期时间</param>
        /// <returns>结果</returns>
        public static string CreateJWT(Dictionary<string, object> dic, DateTime exp)
        {
            string header = CreateHeader();
            string playLoad = CreatePlayLoad(exp, dic);
            string signature = Signature(header, playLoad, string.Empty, exp);
            return $"{header}.{playLoad}.{signature}";
        }

        /// <summary>
        /// 根据playload生成jwt
        /// </summary>
        /// <param name="playLoad">消息体</param>
        /// <param name="expr">过期时间</param>
        /// <returns>结果</returns>
        public static string CreateJWT(string playLoad, string userID, DateTime expr)
        {
            string header = CreateHeader();
            string signature = Signature(header, playLoad, userID, expr);
            return $"{header}.{playLoad}.{signature}";
        }

        /// <summary>
        /// 构建jwt的header
        /// </summary>
        /// <returns>结果</returns>
        private static string CreateHeader()
        {
            Dictionary<string, object> header = new Dictionary<string, object>();
            header.Add("typ", "JWT");
            header.Add("alg", "md5");
            string jsonHeader = header.SerializeObject();
            return Base64UrlEncoder.Encode(Encoding.UTF8.GetBytes(jsonHeader));
        }

        /// <summary>
        /// 构建payload
        /// </summary>
        /// <param name="expre">过期时间</param>
        /// <param name="user">用户信息</param>
        /// <returns>结果</returns>
        private static string CreatePlayLoad(DateTime expre, Dictionary<string, object> dic)
        {
            Dictionary<string, object> playLoad = new Dictionary<string, object>();
            foreach (var iem in dic)
            {
                playLoad.Add(iem.Key, iem.Value);
            }

            playLoad.Add("exp", expre);
            playLoad.Add("iat", DateTime.Now);
            return Base64UrlEncoder.Encode(Encoding.UTF8.GetBytes(dic.SerializeObject()));
        }

        /// <summary>
        /// 生成签名
        /// </summary>
        /// <param name="header">header</param>
        /// <param name="playLoad">主体</param>
        /// <param name="userID">用户编号</param>
        /// <param name="exp">有效期</param>
        /// <returns>结果</returns>
        public static string Signature(string header, string playLoad, string userID, DateTime exp)
        {
            string signKey = MD5.Create(DateTime.Now.ToString("yyyyMMddHHmmssffff")).ToString();
            //RedisHelper.SetParmRedis<string>($"jwt{userID}", signKey, exp, "redisUserConfiguration");
            string signContent = $"{header}.{playLoad}{signKey}";
            return MD5.Create(signContent).ToString();
        }

        /// <summary>
        /// 验证jwt
        /// </summary>
        /// <param name="authorization">授权信息</param>
        /// <returns>结果</returns>
        public static MResult CheckSign(string authorization)
        {
            Dictionary<string, object> dic = GetJwtPlayload(authorization);
            string id = GetJwtContent(authorization, "id");
            string header = GetJwtHeader(authorization);
            string strContent = GetStrPlayload(authorization);
            string signKey = string.Empty;// RedisHelper.GetParmRedis<string>($"jwt{id}", "redisUserConfiguration");
            string signContent = $"{header}.{strContent}{signKey}";
            string oriSign = GetSign(authorization);
            string sign = MD5.Create(signContent).ToString();
            if (string.Equals(oriSign, sign, StringComparison.CurrentCultureIgnoreCase))
            {
                return new MResult() { Code = 200 };
            }

            return new MResult() { Msg = "登录已过期,请重新登录" };
        }

        /// <summary>
        /// 延迟jwt有效期
        /// </summary>
        /// <param name="jwt"></param>
        /// <param name="effectiveDate">有效期</param>
        /// <returns>新的jwt</returns>
        public static MResult DelayJWTExpTime(string jwt, DateTime effectiveDate)
        {
            var checkSign = CheckSign(jwt);
            if (checkSign.Code == 200)
            {
                return new MResult() { Msg = checkSign.Msg };
            }

            Dictionary<string, object> playLoad = GetJwtPlayload(jwt);
            if (playLoad == null || !playLoad.ContainsKey("exp"))
            {
                return new MResult() { Msg = "参数有误，exp不存在" };
            }

            if (!playLoad.ContainsKey("id"))
            {
                return new MResult() { Msg = "参数有误，id不存在" };
            }

            playLoad["exp"] = effectiveDate;
            string newPayLoad = Base64UrlEncoder.Encode(Encoding.UTF8.GetBytes(playLoad.SerializeObject()));
            return new MResult() { Code = 200, Msg = CreateJWT(newPayLoad, playLoad["id"].ToString(), effectiveDate) };
        }

        /// <summary>
        /// 禁用jwt
        /// </summary>
        /// <param name="jwt">header中的jwt</param>
        /// <returns></returns>
        public static MResult DisableJWT(string jwt)
        {
            var checkSign = CheckSign(jwt);
            if (checkSign.Code != 200)
            {
                return new MResult() { Msg = checkSign.Msg };
            }

            string userID = GetJwtContent(jwt, "id");
            //RedisHelper.Delete($"jwt{userID}", "redisUserConfiguration");
            return new MResult() { Code = 200 };
        }

        /// <summary>
        /// 校验token有效性
        /// </summary>
        /// <param name="jwt">jwt内容</param>
        /// <returns>结果</returns>
        public static bool CheckExpr(string jwt)
        {
            try
            {
                string strExpr = GetJwtContent(jwt, "exp");
                if (DateTime.Now > Convert.ToDateTime(strExpr))
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                ////LogService.WriteLog(e, CheckExpr);
                return false;
            }

            return true;
        }

        /// <summary>
        /// 获取授权信息节点
        /// </summary>
        /// <param name="jwt">jwt数据</param>
        /// <returns>结果</returns>
        public static Dictionary<string, object> GetJwtPlayload(string jwt)
        {
            if (string.IsNullOrWhiteSpace(jwt))
            {
                throw new ArgumentNullException();
            }

            string[] jwtArr = jwt.Split('.');
            if (jwtArr == null || jwtArr.Length != 3)
            {
                throw new ArgumentNullException("授权信息缺失");
            }

            Dictionary<string, object> playLoad = Base64UrlEncoder.Decode(jwtArr[1]).DeserializeObject<Dictionary<string, object>>();
            return playLoad;
        }

        /// <summary>
        /// 获取jwt内容
        /// </summary>
        /// <param name="jwt">内容</param>
        /// <param name="nodeName">节点名字</param>
        /// <returns>结果</returns>
        public static string GetJwtContent(string jwt, string nodeName)
        {
            Dictionary<string, object> playLoad = GetJwtPlayload(jwt);
            if (playLoad.ContainsKey(nodeName))
            {
                return playLoad[nodeName].ToString();
            }

            throw new ArgumentException($"节点{nodeName}不存在");
        }

        /// <summary>
        /// 获取jwtheader
        /// </summary>
        /// <param name="jwt">jwt</param>
        /// <returns>结果</returns>
        public static string GetJwtHeader(string jwt)
        {
            if (string.IsNullOrWhiteSpace(jwt))
            {
                throw new ArgumentNullException();
            }

            string[] jwtArr = jwt.Split('.');
            if (jwtArr == null || jwtArr.Length != 3)
            {
                throw new ArgumentNullException("授权信息缺失");
            }

            return jwtArr[0];
        }

        /// <summary>
        /// 获取jwtheader
        /// </summary>
        /// <param name="jwt">jwt</param>
        /// <returns>结果</returns>
        public static string GetStrPlayload(string jwt)
        {
            if (string.IsNullOrWhiteSpace(jwt))
            {
                throw new ArgumentNullException();
            }

            string[] jwtArr = jwt.Split('.');
            if (jwtArr == null || jwtArr.Length != 3)
            {
                throw new ArgumentNullException("授权信息缺失");
            }

            return jwtArr[1];
        }

        /// <summary>
        /// 获取jwtheader
        /// </summary>
        /// <param name="jwt">jwt</param>
        /// <returns>结果</returns>
        public static string GetSign(string jwt)
        {
            if (string.IsNullOrWhiteSpace(jwt))
            {
                throw new ArgumentNullException();
            }

            string[] jwtArr = jwt.Split('.');
            if (jwtArr == null || jwtArr.Length != 3)
            {
                throw new ArgumentNullException("授权信息缺失");
            }

            return jwtArr[2];
        }
    }
}
