using System;
using System.Collections.Generic;
using Chalkable.Common;
using Chalkable.Data.Master.DataAccess;
using Chalkable.Data.Master.Model;

namespace Chalkable.BusinessLogic.Services.Master
{
    public interface IDeveloperService
    {
        Developer GetDeveloperBySchool(Guid schoolId);
        Developer GetDeveloperById(Guid developerId);
        IList<Developer> GetDevelopers();
        Developer Add(string login, string password, string name, string webSite, Guid schoolId);
        Developer Edit(Guid developerId, string name, string email, string webSite);
    }
    public class DeveloperService : MasterServiceBase, IDeveloperService
    {

        public DeveloperService(IServiceLocatorMaster serviceLocator) : base(serviceLocator)
        {
        }

        //TODO: needs test 
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

        public IList<Developer> GetDevelopers()
        {
            using (var uow = Read())
            {
                return new DeveloperDataAccess(uow).GetAll();
            }
        }

        public Developer Edit(Guid developerId, string name, string email, string webSite)
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


        public Developer Add(string login, string password, string name, string webSite, Guid schoolId)
        {
            using (var uow = Update())
            {
                var user = ServiceLocator.UserService.CreateSchoolUser(login, password, schoolId, CoreRoles.DEVELOPER_ROLE.Name); // security here 
                var res = new Developer
                    {
                        Id = user.Id,
                        Name = name,
                        SchoolRef = schoolId,
                        WebSite = webSite,
                        User = user
                    };
                new DeveloperDataAccess(uow).Insert(res);
                uow.Commit();
                return res;
            }
        }
    }
}
