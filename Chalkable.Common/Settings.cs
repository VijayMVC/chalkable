using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chalkable.Common
{
    public static class Settings
    {
        public static string MasterConnectionString
        {
            get
            {
                var connectionString = ConfigurationManager.ConnectionStrings["ChalkableMaster"].ConnectionString;
                return connectionString;
            }
        }

        public static string SchoolConnectionStringTemplate
        {
            get
            {
                var connectionString = ConfigurationManager.ConnectionStrings["ChalkableSchool"].ConnectionString;
                return connectionString;
            }
        }
    }
}
