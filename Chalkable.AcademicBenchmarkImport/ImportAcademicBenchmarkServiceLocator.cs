using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.BusinessLogic.Services;
using Chalkable.BusinessLogic.Services.AcademicBenchmark;
using Chalkable.Common;

namespace Chalkable.AcademicBenchmarkImport
{
    public class ImportAcademicBenchmarkServiceLocator : AcademicBenchmarkServiceLocator
    {
        public ImportAcademicBenchmarkServiceLocator(UserContext context, string connectionString = null) : base(context, connectionString)
        {
            DbService = new ImportDbService(string.IsNullOrWhiteSpace(connectionString) ? Settings.AcademicBenchmarkDbConnectionString : connectionString);
        }
    }
}
