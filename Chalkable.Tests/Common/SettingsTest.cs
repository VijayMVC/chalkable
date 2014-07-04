using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Common;
using NUnit.Framework;

namespace Chalkable.Tests.Common
{
    [TestFixture]
    public class SettingsTest
    {
        [Test]
        public void Test()
        {
            Debug.WriteLine(Settings.MasterConnectionString);
            Debug.WriteLine(Settings.SchoolConnectionStringTemplate);
            Debug.WriteLine(Settings.Configuration.SchoolTemplateDataBase);
            Debug.WriteLine(Settings.Configuration.Servers.AllKeys.JoinString(",")); 
        }

        [Test]
        public void PasswordTest()
        {
            string password = "5MQVuZf$xbKbn7NYsG78";
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = new MD5CryptoServiceProvider().ComputeHash(bytes);
            var b64 = Convert.ToBase64String(hash);
            Debug.WriteLine(b64);
        }
    }
}
