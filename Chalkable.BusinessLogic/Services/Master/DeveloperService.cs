using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Data.Master.DataAccess;
using Chalkable.Data.Master.Model;

namespace Chalkable.BusinessLogic.Services.Master
{
    public interface IDeveloperService
    {
        Developer GetDeveloperBySchool(Guid schoolId);
        Developer GetDeveloperById(Guid developerId);
        Developer EditDeveloper(Guid developerId, string name, string email, string webSite);
    }
    public class DeveloperService : MasterServiceBase, IDeveloperService
    {
        public DeveloperService(IServiceLocatorMaster serviceLocator) : base(serviceLocator)
        {
        }

        public Developer GetDeveloperBySchool(Guid schoolId)
        {
            using (var uow = Read())
            {
                return new DeveloperDataAccess(uow).GetDeveloper(schoolId);
            }
        }

        public Developer GetDeveloperById(Guid developerId)
        {
            using (var uow = Read())
            {
                return new DeveloperDataAccess(uow).GetById(developerId);
            }
        }

        public Developer EditDeveloper(Guid developerId, string name, string email, string webSite)
        {
            using (var uow = Update())
            {
                var da = new DeveloperDataAccess(uow);
                var developer = da.GetById(developerId);
                ServiceLocator.UserService.ChangeUserLogin(developerId, email); // security here 
                developer.Name = name;
                developer.WebSite = webSite;
                da.Update(developer);
                uow.Commit();
                return developer;
            }
        }
    }
}
