using System;
using System.Text;
using Chalkable.API.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Chalkable.Common
{
    public static class AttachmentSecurityTools
    {
        private const string Key1 = "ioNr3Bd9cd1C7sd5G135CSkLUJ3U8Z5K";
        private const string Key2 = "JLE2m645o137hZ1JU08hz2UNIDI6LIX9";
        private const string Key3 = "58zk9HB0Jy9IcUh86YLMx9748rVz7057";

        private static string ComputeSignature(Guid districtId, int schoolId, Guid userId, int attachmentId, long ts)
        {
            var s = $"{Key1}--{districtId.ToString().ToLowerInvariant()}-{schoolId}-{userId.ToString().ToLowerInvariant()}-{attachmentId}--{Key2}--{ts}--{Key3}";
            return HashHelper.HexOfCumputedHash(s);
        }

        public static string ComputeRequestStr(Guid districtId, int schoolId, Guid userId, int attachmentId)
        {
            var ts = DateTime.UtcNow.Ticks;
            var o = new JObject
            {
                ["districtId"] = districtId.ToString(),
                ["schoolId"] = schoolId,
                ["userId"] = userId.ToString(),
                ["attachmentId"] = attachmentId,
                ["ts"] = ts,
                ["signature"] = ComputeSignature(districtId, schoolId, userId, attachmentId, ts)
            };

            var coded = JsonConvert.SerializeObject(o);

            return Convert.ToBase64String(Encoding.UTF8.GetBytes(coded));
        }

        public static bool TryParseAndVerifyRequestStr(string r, out Guid districtId, out int schoolId, out Guid userId,
            out int attachmentId)
        {
            districtId = Guid.Empty;
            schoolId = -1;
            userId = Guid.Empty;
            attachmentId = -1;

            var decoded = Encoding.UTF8.GetString(Convert.FromBase64String(r));

            var json = JsonConvert.DeserializeObject<JObject>(decoded);

            JToken jtoken;

            if (!json.TryGetValue("attachmentId", StringComparison.InvariantCultureIgnoreCase, out jtoken)) return false;
            attachmentId = jtoken.Value<int>();

            if (!json.TryGetValue("districtId", StringComparison.InvariantCultureIgnoreCase, out jtoken)) return false;
            if (!Guid.TryParse(jtoken.Value<string>(), out districtId)) return false;

            if (!json.TryGetValue("schoolId", StringComparison.InvariantCultureIgnoreCase, out jtoken)) return false;
            schoolId = jtoken.Value<int>();

            if (!json.TryGetValue("userId", StringComparison.InvariantCultureIgnoreCase, out jtoken)) return false;
            if (!Guid.TryParse(jtoken.Value<string>(), out userId)) return false;

            if (!json.TryGetValue("ts", StringComparison.InvariantCultureIgnoreCase, out jtoken)) return false;
            var timestamp = jtoken.Value<long>();

            if (!json.TryGetValue("signature", StringComparison.InvariantCultureIgnoreCase, out jtoken)) return false;
            var signature = json["signature"].Value<string>();


            var computed = ComputeSignature(districtId, schoolId, userId, attachmentId, timestamp);

            return string.Compare(signature, computed, StringComparison.InvariantCultureIgnoreCase) == 0;
        }
    }
}