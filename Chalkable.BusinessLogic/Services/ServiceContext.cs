using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.BusinessLogic.Security;
using Chalkable.Data.Master.Model;

namespace Chalkable.BusinessLogic.Services
{
    public class ServiceContext
    {
        public string ConnectionString { get; private set; }
        public Guid UserId { get; private set; }
        public string UserName { get; private set; }
        public CoreRole Role { get; private set; }

        public ServiceContext(string connectionString, Guid id, string name, CoreRole role)
        {
            ConnectionString = connectionString;
            UserId = id;
            UserName = name;
            Role = role;
        }
    }
}
