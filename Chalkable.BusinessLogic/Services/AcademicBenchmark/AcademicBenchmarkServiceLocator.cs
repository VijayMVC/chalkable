using Chalkable.AcademicBenchmarkConnector.Connectors;
using Chalkable.Common;

namespace Chalkable.BusinessLogic.Services.AcademicBenchmark
{
    public interface IAcademicBenchmarkServiceLocator
    {
        UserContext Context { get; }
        IStandardService StandardService { get; }
        IDbService DbService { get; }
    }

    public class AcademicBenchmarkServiceLocator : IAcademicBenchmarkServiceLocator
    {
        public UserContext Context { get; protected set; }
        public IStandardService StandardService { get; }
        public IDbService DbService { get; protected set; }

        private AcademicBenchmarkServiceLocator(UserContext context)
        {
            Context = context;
            DbService = new DbService(Settings.AcademicBenchmarkDbConnectionString);
            StandardService = new StandardService(this);
        }
    }
}
