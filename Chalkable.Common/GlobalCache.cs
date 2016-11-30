using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace Chalkable.Common
{
    public class GlobalCache
    {
        private static ConnectionMultiplexer[] _connections;

        public static void InitGlobalCache(string connectionString, int numConnections = 1)
        {
            _connections = new ConnectionMultiplexer[numConnections];

            for (var i = 0; i < numConnections; ++i)
                _connections[i] = ConnectionMultiplexer.Connect(connectionString);
        }

        private static ConnectionMultiplexer GetOneWithMinOps()
        {
            var connection = _connections[0];

            for (var i = 1; i < _connections.Length; ++i)
            {
                if (connection.OperationCount > _connections[i].OperationCount)
                    connection = _connections[i];
            }

#if DEBUG
            Debug.WriteLine($"Using Redis.ConnectionMultiplexer {connection.ClientName} ops={connection.OperationCount}, tmo={connection.TimeoutMilliseconds}");
#endif

            return connection;
        }

        private static IDatabase Cache => GetOneWithMinOps().GetDatabase();

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

        public static void UpdateExpiryUserInfo(string sessionKey, TimeSpan expiry)
        {
            Cache.KeyExpire(sessionKey, DateTime.Now.Add(expiry));
        }
    }
}