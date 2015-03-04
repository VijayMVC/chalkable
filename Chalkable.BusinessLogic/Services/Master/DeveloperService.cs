using System;
using System.Collections.Generic;
using Chalkable.BusinessLogic.Security;
using Chalkable.Data.Master.DataAccess;
using Chalkable.Data.Master.Model;

namespace Chalkable.BusinessLogic.Services.Master
{
    public interface IDeveloperService
    {
        Developer GetDeveloperByDictrict(Guid districtId);
        Developer GetById(Guid developerId);
        IList<Developer> GetDevelopers();
        Developer Add(string login, string password, string name, string webSite, string paypalLogin);
        Developer Edit(Guid developerId, string name, string email, string webSite, string paypalLogin);
        Developer ChangePayPalLogin(Guid developerId, string paypalLogin);
    }
    public class DeveloperService : MasterServiceBase, IDeveloperService
    {

        public DeveloperService(IServiceLocatorMaster serviceLocator) : base(serviceLocator)
        {
        }
        
        public Developer GetDeveloperByDictrict(Guid districtId)
        {
            return DoRead(u => new DeveloperDataAccess(u).GetDeveloper(districtId));
        }

        public Developer GetById(Guid developerId)
        {
            return DoRead(u => new DeveloperDataAccess(u).GetById(developerId));
        }

        public IList<Developer> GetDevelopers()
        {
            return DoRead(u => new DeveloperDataAccess(u).GetAll());
        }

        public Developer Edit(Guid developerId, string name, string email, string webSite, string paypalLogin)
        {
            BaseSecurity.EnsureSysAdminOrCurrentUser(developerId, Context);

            var user = ServiceLocator.UserService.GetById(developerId);
            user.Login = email;
            var developer = GetById(developerId);
            developer.Name = name;
            developer.WebSite = webSite;
            developer.PayPalLogin = paypalLogin;
            DoUpdate(u =>
            {
                new DeveloperDataAccess(u).Update(developer);
                new UserDataAccess(u).Update(user);
            });
            return developer;
        }


        public Developer Add(string login, string password, string name, string webSite, string paypalLogin)
        {
            var user = ServiceLocator.UserService.CreateDeveloperUser(login, password);
            var res = new Developer
            {
                Id = user.Id,
                Name = name,
                WebSite = webSite,
                User = user,
                DistrictRef = user.Id,
                PayPalLogin = paypalLogin
            };
            DoUpdate(u => new DeveloperDataAccess(u).Insert(res));
            return res;
        }


        public Developer ChangePayPalLogin(Guid developerId, string paypalLogin)
        {
            BaseSecurity.EnsureSysAdminOrCurrentUser(developerId, Context);
            var developer = GetById(developerId);
            developer.PayPalLogin = paypalLogin;
            DoUpdate(u=>new DeveloperDataAccess(u).Update(developer));
            return developer;
        }
    }
}
