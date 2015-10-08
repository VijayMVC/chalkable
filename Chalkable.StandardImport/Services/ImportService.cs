using Chalkable.BusinessLogic.Services.Master;
using Chalkable.StandardImport.Models;

namespace Chalkable.StandardImport.Services
{
    public abstract class ImportService
    {
        protected IServiceLocatorMaster ServiceLocatorMaster { get; private set; }
        public CsvContainer CsvContainer { get; protected set; }

        protected ImportService(string connectionString)
        {
            ServiceLocatorMaster = new StandardImportServiceLocatorMaster(connectionString);
        }

        public abstract void Import(byte[] data);
    }
}
