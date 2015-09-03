using System;
using StackExchange.Redis;

namespace Chalkable.Common
{
    public class GlobalCache
    {
        private static readonly ConnectionMultiplexer Connection =
            ConnectionMultiplexer.Connect(Settings.RedisCacheConnectionString);

        private static IDatabase Cache
        {
            get { return Connection.GetDatabase(); }
        }

        private static string Get(string sessionKey, string subKey)
        {
            return Cache.StringGet(sessionKey + ':' + subKey);
        }

        private static void Set(string sessionKey, string subKey, string value, TimeSpan? expiery = null)
        {
            Cache.StringSet(sessionKey + ':' + subKey, value, expiery);
        }

        private static void Delete(string sessionKey, string subKey)
        {
            Cache.KeyDelete(sessionKey + ':' + subKey);
        }

        public static string GetAuth(string sessionKey)
        {
            return Get(sessionKey, "Auth");
        }

        public static void SetAuth(string sessionKey, string token, TimeSpan expiery)
        {
            Set(sessionKey, "Auth", token, expiery);
        }

        public static string GetClaims(string sessionKey)
        {
            return Get(sessionKey, "Claims");
        }

        public static void SetClaims(string sessionKey, string token, TimeSpan expiery)
        {
            Set(sessionKey, "Claims", token, expiery);
        }

        public static string GetAccessToken(string sessionKey)
        {
            return Get(sessionKey, "AccessToken");
        }

        public static void SetAccessToken(string sessionKey, string token, TimeSpan expiery)
        {
            Set(sessionKey, "AccessToken", token, expiery);
        }

        public static string GetSisToken(string sessionKey)
        {
            return Get(sessionKey, "SisToken");
        }

        public static void SetSisToken(string sessionKey, string token, TimeSpan expiery)
        {
            Set(sessionKey, "SisToken", token, expiery);
        }

        public static void CleanSession(string sessionKey)
        {
            Delete(sessionKey, "Auth");
            Delete(sessionKey, "Claims");
            Delete(sessionKey, "AccessToken");
            Delete(sessionKey, "SisToken");
        }
    }
}