using System;
using System.Collections.Generic;
using Chalkable.Common.Exceptions;
using Chalkable.Data.Master.DataAccess;
using Chalkable.Data.Master.Model;

namespace Chalkable.BusinessLogic.Services.Master
{
    public interface IDeveloperService
    {
        Developer GetDeveloperByDictrict(Guid districtId);
        Developer GetDeveloperById(Guid developerId);
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

        //TODO: needs test 
        public Developer GetDeveloperByDictrict(Guid districtId)
        {
            using (var uow = Read())
            {
                return new DeveloperDataAccess(uow).GetDeveloper(districtId);
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

        public Developer Edit(Guid developerId, string name, string email, string webSite, string paypalLogin)
        {
            using (var uow = Update())
            {
                var da = new DeveloperDataAccess(uow);
                var developer = da.GetById(developerId);
                ServiceLocator.UserService.ChangeUserLogin(developerId, email); // security here 
                developer.Name = name;
                developer.WebSite = webSite;
                developer.PayPalLogin = paypalLogin;
                da.Update(developer);
                uow.Commit();
                return developer;
            }
        }


        public Developer Add(string login, string password, string name, string webSite, string paypalLogin)
        {
            using (var uow = Update())
            {
                var user = ServiceLocator.UserService.CreateDeveloperUser(login, password); // security here 
                var res = new Developer
                    {
                        Id = user.Id,
                        Name = name,
                        WebSite = webSite,
                        User = user,
                        DistrictRef = user.Id,
                        PayPalLogin = paypalLogin
                    };
                new DeveloperDataAccess(uow).Insert(res);
                uow.Commit();
                return res;
            }
        }


        public Developer ChangePayPalLogin(Guid developerId, string paypalLogin)
        {
            if(Context.DeveloperId != developerId)
                throw new ChalkableSecurityException();
            using (var uow = Update())
            {
                var da = new DeveloperDataAccess(uow);
                var developer = da.GetById(developerId);
                developer.PayPalLogin = paypalLogin;
                da.Update(developer);
                uow.Commit();
                return developer;
            }

        }
    }
}
