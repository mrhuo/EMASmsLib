using RestSharp;
using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Security.Cryptography;
using System.Text;

namespace EMASmsLib
{
    /// <summary>
    /// EMASms 短信插件
    /// </summary>
    public class EMASms
    {
        /// <summary>
        /// 配置错误信息模板
        /// </summary>
        private const string configErrorMsg = "使用 CreateWithAppSettings() 初始化 EMASms 时，app.config|web.config/appSettings 下缺少配置[{0}]";
        /// <summary>
        /// 一些内部字段
        /// </summary>
        private string accountSid, ecpId, appId, token, sendUrl;

        /// <summary>
        /// 私有构造方法
        /// </summary>
        /// <param name="accountSid">accountSid</param>
        /// <param name="ecpId">ecpId</param>
        /// <param name="appId">appId</param>
        /// <param name="token">token</param>
        private EMASms(
            string accountSid,
            string ecpId,
            string appId,
            string token)
        {
            this.accountSid = accountSid;
            this.ecpId = ecpId;
            this.appId = appId;
            this.token = token;
            this.sendUrl = $"https://naas.ecplive.cn/api/exec/Account/{accountSid}/ema/sendSms";
        }

        /// <summary>
        /// 使用 app.config 或 web.config 中的配置初始化 EMASms
        /// </summary>
        /// <returns></returns>
        public static EMASms CreateWithAppSettings()
        {
            var accountSid = GetConfigSetting("accountSid");
            var ecpId = GetConfigSetting("ecpId");
            var appId = GetConfigSetting("appId");
            var token = GetConfigSetting("token");
            return CreateWithParam(accountSid, ecpId, appId, token);
        }

        /// <summary>
        /// 手动使用参数初始化 EMASms
        /// </summary>
        /// <param name="accountSid"></param>
        /// <param name="ecpId"></param>
        /// <param name="appId"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static EMASms CreateWithParam(
            string accountSid,
            string ecpId,
            string appId,
            string token)
        {
            if (string.IsNullOrWhiteSpace(accountSid))
            {
                throw new ArgumentNullException(accountSid);
            }
            if (string.IsNullOrWhiteSpace(ecpId))
            {
                throw new ArgumentNullException(ecpId);
            }
            if (string.IsNullOrWhiteSpace(appId))
            {
                throw new ArgumentNullException(appId);
            }
            if (string.IsNullOrWhiteSpace(token))
            {
                throw new ArgumentNullException(token);
            }
            return new EMASms(accountSid, ecpId, appId, token);
        }

        /// <summary>
        /// 从 app.config 或 web.config 中读取配置
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private static string GetConfigSetting(string key)
        {
            var value = ConfigurationManager.AppSettings[key];
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new Exception(string.Format(configErrorMsg, key));
            }
            return value;
        }

        /// <summary>
        /// 发送短信给用户
        /// </summary>
        /// <param name="phone"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public string Send(string phone, string content)
        {
            var ts = GetTs();

            var client = new RestClient(sendUrl);
            var request = new RestRequest(Method.POST);
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("content-type", "application/x-www-form-urlencoded");
            request.AddHeader("accountsid", accountSid);
            request.AddHeader("apiid", appId);
            request.AddHeader("sign", GetSign(ts));
            request.AddHeader("ts", $"{ts}");
            request.AddParameter("ecpId", ecpId);
            request.AddParameter("destinationPhone", phone);
            request.AddParameter("message", content);
            request.AddParameter("smsStatusUrl", "");
            request.AddParameter("appId", appId);

            IRestResponse response = client.Execute(request);
            if (response != null)
            {
                return response.Content;
            }
            return null;
        }

        /// <summary>
        /// 获取 ts 参数（自1970年1月1日开始的毫秒数）
        /// </summary>
        /// <returns></returns>
        public long GetTs()
        {
            return (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000;
        }

        /// <summary>
        /// 生成签名：md5(accountSid+token+ts)
        /// </summary>
        /// <param name="ts"></param>
        /// <returns></returns>
        public string GetSign(long ts)
        {
            return GetMd5(accountSid + token + ts);
        }

        /// <summary>
        /// 获取字符串对应的 MD5
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public string GetMd5(string str)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] fromData = Encoding.UTF8.GetBytes(str);
            byte[] targetData = md5.ComputeHash(fromData);
            string byte2String = null;

            for (int i = 0; i < targetData.Length; i++)
            {
                byte2String += targetData[i].ToString("x2");
            }
            return byte2String;
        }
    }
}
