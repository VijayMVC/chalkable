using Chalkable.Data.Common;

namespace Chalkable.BusinessLogic.Services
{
    public interface IDbService
    {
        UnitOfWork GetUowForRead();
        UnitOfWork GetUowForUpdate();
    }

    public class DbService : IDbService
    {
        private string connectionString;
        public DbService(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public UnitOfWork GetUowForRead()
        {
            return new UnitOfWork(connectionString, false);
        }

        public UnitOfWork GetUowForUpdate()
        {
            return new UnitOfWork(connectionString, true);
        }
    }
}