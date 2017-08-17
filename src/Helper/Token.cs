/**
 * Created by w on 2017/7/6.
 */
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace W.WebApi.Helper
{
    /// <summary>
    /// 令牌
    /// </summary>
    public class Token
    {
        public const string HEADER_AUTHORIZATION = "Authorization";
        public const int TOKEN_EXPIRE_INTERVAL = 8;
        /// <summary>
        /// 编号
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 用户Id
        /// </summary>
        public string userId { get; set; }
        /// <summary>
        /// 过期时间
        /// </summary>
        public long expireTime { get; set; }


        public static Token create(string userId)
        {
            IList<Token> cacheToken = Cache.instance().get(Cache.CACHE_KEY_TOKEN).data as IList<Token>;
            Token token = null;
            if (cacheToken.Where(a => a.userId == userId).Count() <= 0)
            {
                token = new Token()
                {
                    id = Guid.NewGuid().ToString("N"),
                    userId = userId,
                    expireTime = TimeHelper.ConvertToTimeStamp(DateTime.Now.AddHours(TOKEN_EXPIRE_INTERVAL))
                };
                cacheToken.Add(token);
            }
            else
            {
                token = cacheToken.Single(a => a.userId == userId);
                token.expireTime = TimeHelper.ConvertToTimeStamp(DateTime.Now.AddHours(TOKEN_EXPIRE_INTERVAL));
            }
            return token;
        }

        public static Token get(string tokenId)
        {
            IList<Token> cacheToken = Cache.instance().get(Cache.CACHE_KEY_TOKEN).data as IList<Token>;
            Token token = null;
            if (cacheToken.Where(a => a.id == tokenId).Count() > 0)
            {
                token = cacheToken.Single(a => a.id == tokenId);
                //todo:此处检查token是否过期，最好是做一个job检查全部token，这里可能回积压很多垃圾数据
                //判断是否过期
                if (TimeHelper.ConvertToDateTime(token.expireTime) < DateTime.Now)
                {
                    cacheToken.Remove(token);
                    Cache.instance().update(Cache.CACHE_KEY_TOKEN);
                    //会删除get之后的会话
                    //Cache.instance().update(Cache.CACHE_KEY_TOKEN, cacheToken);
                    return null;
                }
            }
            return token;
        }

        public static Token get(HttpRequest request)
        {
            Token token;
            try
            {
                IHeaderDictionary headers = request.Headers;
                string authorization = headers[HEADER_AUTHORIZATION].ToArray()[0];
                string[] authorizations = Encoding.UTF8.GetString(Convert.FromBase64String(authorization)).Split('|');
                var tokenId = authorizations[2];
                token = get(tokenId);
            }
            catch (Exception)
            {
                return null;
            }
            return token;
        }

        public static Result check(HttpRequest request)
        {
            try
            {
                IHeaderDictionary headers = request.Headers;
                if (!headers.ContainsKey(HEADER_AUTHORIZATION))
                {
                    return Result.FAILURE("签名校验失败[没有找到签名信息]");
                }
                string authorization = headers[HEADER_AUTHORIZATION].ToArray()[0];
                string[] authorizations = Encoding.UTF8.GetString(Convert.FromBase64String(authorization)).Split('|');

                if (authorizations.Length != 5)
                    return Result.FAILURE("签名校验失败[格式错误]");

                string appId = authorizations[0]
                    , timespan = authorizations[1]
                    , tokenId = authorizations[2]
                    , nonce = authorizations[3]
                    , sign = authorizations[4];

                //todo:可根据timespan/nonce 判断是否过期
                //return Result.FAILURE("签名校验失败[签名过期]");

                //todo:未校验appId

                //sign格式：{appId}|{timespan}|{tokenId}|{nonce}
                //todo:sign应该先3ds加密再MD5
                string signLocal = Crypto.md5($"{appId}|{timespan}|{tokenId}|{nonce}");
                if (signLocal != sign)
                {
                    return Result.FAILURE("签名校验失败[签名错误]");
                }

                if (request.Path.Value.ToLower() != "/api/token")
                {
                    //todo:访问权限校验
                    Token token = get(tokenId);
                    if (token == null)
                        return Result.FAILURE("签名校验失败[token无效或已过期]");
                    //todo:查询用户
                    //if (new UserInfo().GetUsers().Where(a => a.Id.ToString() == token.userId).Count() <= 0)
                    //{
                    //    return Result.FAILURE("签名校验失败[没有找到会话用户]");
                    //}
                }

                return new Result(true, "验证完成");
            }
            catch (Exception)
            {
                return Result.FAILURE("签名校验失败[解析时发生错误]");
            }
        }
    }
}
