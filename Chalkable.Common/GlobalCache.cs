using System;
using System.Collections.Generic;
using Newtonsoft.Json;
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

        public static void CleanSession(string sessionKey)
        {
            try
            {
                Cache.KeyDeleteAsync(sessionKey);
            }
            catch (Exception)
            {
                //ignored
            }
        }

        public class UserInfo
        {
            public string Auth { get; set; }
            public string SisToken { get; set; }
            public string Claims { get; set; }
        }

        public static UserInfo GetUserInfo(string sessionKey)
        {
            var value = Cache.StringGet(sessionKey);
            return string.IsNullOrWhiteSpace(value) ? null : JsonConvert.DeserializeObject<UserInfo>(value);
        }

        public static void SetUserInfo(string sessionKey, UserInfo userInfo, TimeSpan? expiry)
        {
            Cache.StringSet(sessionKey, JsonConvert.SerializeObject(userInfo), expiry);
        }
    }
}