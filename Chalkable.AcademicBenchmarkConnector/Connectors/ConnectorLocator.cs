using System;
using System.Security.Cryptography;
using System.Text;
using Chalkable.Common;

namespace Chalkable.AcademicBenchmarkConnector.Connectors
{
    public interface IConnectorLocator
    {
        IStandardsConnector StandardsConnector { get; }

        string ApiRoot { get; }
        string AuthSignarute { get; }
        long AuthExpires { get; }
    }

    public class ConnectorLocator : IConnectorLocator
    {

        public IStandardsConnector StandardsConnector { get; private set; }

        public string ApiRoot { get; }
        public string AuthSignarute { get; private set; } 
        public long AuthExpires { get; private set; }


        public ConnectorLocator()
        {
            ApiRoot = Settings.AcademicBenchmarkApiUrl;
            InitAuthParams();
            InitConnectors();
        }

        private void InitConnectors()
        {
            StandardsConnector = new StandardsConnector(this);
        }

        private void InitAuthParams()
        {
            var userID = ""; // Partner defined. May be an empty string.
            // Seconds since epoch. Example is 24 hours.
            AuthExpires = (long)Math.Floor(
              (DateTime.UtcNow.AddHours(24) - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds
            );
            var message = $"{AuthExpires}\n{userID}";
            var keyBytes = Encoding.UTF8.GetBytes(Settings.AcademicBenchmarkPartnerKey);
            var messageBytes = Encoding.UTF8.GetBytes(message);
            using (var hmac = new HMACSHA256(keyBytes))
            {
                AuthSignarute = Convert.ToBase64String(hmac.ComputeHash(messageBytes));
            }
        }
    }
}
