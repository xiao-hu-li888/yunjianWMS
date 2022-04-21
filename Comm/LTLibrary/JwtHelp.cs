using JWT;
using JWT.Algorithms;
using JWT.Exceptions;
using JWT.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LTLibrary
{
    public class JwtHelp
    {
        /// <summary>
        /// 私钥
        /// </summary>
        private const string secret = "SLK7S2321LWOTQSL99231LWEISXWT532AKXOETY";
        /// <summary>
        /// JWT加密返回token
        /// </summary>
        /// <param name="user">用户信息（非敏感信息）</param>
        /// <param name="ExpSeconds">过期时间（秒）</param>
        /// <returns>token</returns>
        public static string Encryption(UserInfo user,int ExpSeconds)
        {
            double exp = (DateTime.UtcNow.AddSeconds(ExpSeconds) - new DateTime(1970, 1, 1)).TotalSeconds;//10秒后过期
            var payload = new Dictionary<string, object>
            {
                {"UserGuid",user.UserGuid },
                {"UserName",user.UserName },
                {"exp",exp }//过期时间
            }; 
            IJwtAlgorithm algorithm = new HMACSHA256Algorithm();
            IJsonSerializer serializer = new JsonNetSerializer();
            IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
            IJwtEncoder encoder = new JwtEncoder(algorithm, serializer, urlEncoder);

            var token = encoder.Encode(payload, secret);
            return token;
        }
        /// <summary>
        /// 解密token
        /// </summary>
        /// <param name="token">token加密信息</param>
        /// <returns>返回UserInfo</returns>
        public static UserInfo Decrypt(string token)
        { 
            try
            {
                IJwtAlgorithm algorithm = new HMACSHA256Algorithm();
                IJsonSerializer serializer = new JsonNetSerializer();
                IDateTimeProvider provider = new UtcDateTimeProvider();
                IJwtValidator validator = new JwtValidator(serializer, provider);
                IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
                IJwtDecoder decoder = new JwtDecoder(serializer, validator, urlEncoder, algorithm); 
                var json = decoder.Decode(token, secret, verify: true);
                UserInfo user=(UserInfo)Newtonsoft.Json.JsonConvert.DeserializeObject(json, typeof(UserInfo));
                return user; 
            }
            catch (TokenExpiredException)
            {
              //  Console.WriteLine("Token 已经过期！"); 
            }
            catch (SignatureVerificationException)
            {
             //   Console.WriteLine("签名校验失败，数据可能被篡改！"); 
            }
            return null;
        }
    }
}
