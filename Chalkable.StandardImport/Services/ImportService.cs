using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.BusinessLogic.Services.Master;
using Chalkable.StandardImport.Models;

namespace Chalkable.StandardImport.Services
{
    public abstract class ImportService
    {
        protected IServiceLocatorMaster ServiceLocatorMaster { get; private set; }
        public CsvContainer CsvContainer { get; protected set; }

        public ImportService(string connectionString)
        {
            ServiceLocatorMaster = new StandardImportServiceLocatorMaster(connectionString);
        }

        public abstract void Import(byte[] data);
    }
}
