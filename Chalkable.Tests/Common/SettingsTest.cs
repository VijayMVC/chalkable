using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
    }
}
