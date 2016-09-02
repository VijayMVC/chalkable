using System;
using System.Security.Cryptography;
using System.Text;
using Chalkable.Common;

namespace Chalkable.AcademicBenchmarkConnector.Connectors
{
    public interface IConnectorLocator
    {
        IStandardsConnector StandardsConnector { get; }
        ITopicsConnector TopicsConnector { get; }
        ISyncConnector SyncConnector { get; }
        string ApiRoot { get; }
        AuthContext AuthContext { get; }
    }

    public class ConnectorLocator : IConnectorLocator
    {
        public IStandardsConnector StandardsConnector { get; private set; }
        public ITopicsConnector TopicsConnector { get; private set; }
        public ISyncConnector SyncConnector { get; private set; }
        public string ApiRoot => Settings.AcademicBenchmarkApiUrl;

        private AuthContext _authContext;
        public AuthContext AuthContext
        {
            get
            {
                _authContext = _authContext ?? AuthContext.GenerateAuthContext();
                return _authContext;
            }
        }

        public ConnectorLocator()
        {
            InitConnectors();
        }
        private void InitConnectors()
        {
            StandardsConnector = new StandardsConnector(this);
            TopicsConnector = new TopicsConnector(this);
            SyncConnector = new SyncConnector(this);
        }
    }

    public class AuthContext
    {
        public string AuthSignarute { get; private set; }
        public long AuthExpires { get; private set; }

        public static AuthContext GenerateAuthContext()
        {
            var expires = GenerateAuthExpires();
            return new AuthContext
            {
                AuthExpires = expires,
                AuthSignarute = GenerateAuthSignature(expires)
            };
        }
        private static long GenerateAuthExpires()
        {
            return (long)Math.Floor((DateTime.UtcNow.AddHours(24) - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds);
        }

        private static string GenerateAuthSignature(long authExpires)
        {
            var userID = ""; // Partner defined. May be an empty string.
            var message = $"{authExpires}\n{userID}";
            var keyBytes = Encoding.UTF8.GetBytes(Settings.AcademicBenchmarkPartnerKey);
            var messageBytes = Encoding.UTF8.GetBytes(message);
            using (var hmac = new HMACSHA256(keyBytes))
            {
                return Convert.ToBase64String(hmac.ComputeHash(messageBytes));
            }
        }
    }
}
