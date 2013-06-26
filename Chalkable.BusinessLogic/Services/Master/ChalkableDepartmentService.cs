using System.Collections.Generic;
using Chalkable.Data.Master.DataAccess;
using Chalkable.Data.Master.Model;

namespace Chalkable.BusinessLogic.Services.Master
{
    public interface IChalkableDepartmentService
    {
        IList<ChalkableDepartment> GetChalkableDepartments();
    }

    public class ChalkableDepartmentService : MasterServiceBase, IChalkableDepartmentService
    {
        public ChalkableDepartmentService(IServiceLocatorMaster serviceLocator) : base(serviceLocator)
        {
        }

        public IList<ChalkableDepartment> GetChalkableDepartments()
        {
            using (var uow = Read())
            {
                return new ChalkableDepartmentDataAccess(uow).GetAll();
            }
        }
    }
}