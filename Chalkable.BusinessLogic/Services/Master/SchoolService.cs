using System;
using System.Collections.Generic;
using Chalkable.BusinessLogic.Model;
using Chalkable.BusinessLogic.Security;
using Chalkable.Data.Common;
using Chalkable.Data.Master.DataAccess;
using Chalkable.Data.Master.Model;

namespace Chalkable.BusinessLogic.Services.Master
{
    public interface ISchoolService
    {
        Data.Master.Model.School Create(Guid districtId, string name, IList<UserInfo> principals);
        District Create(string name);
    }

    public class SchoolService : MasterServiceBase, ISchoolService
    {
        public SchoolService(IServiceLocatorMaster serviceLocator) : base(serviceLocator)
        {
        }

        public District Create(string name)
        {
            using (var uow = Update())
            {
                var da = new DistrictDataAccess(uow);
                var res = new District {Id = new Guid(), Name = name};
                da.Create(res);
                uow.Commit();
                return res;
            }
        }

        public Data.Master.Model.School Create(Guid districtId, string name, IList<UserInfo> principals)
        {
            Data.Master.Model.School school;
            using (var uow = Update())
            {
                var server = FindServer(uow);
                school = new Data.Master.Model.School
                {
                    DistrictRef = districtId,
                    Id = Guid.NewGuid(),
                    Name = name,
                    ServerUrl = server
                };
                var da = new SchoolDataAccess(uow);
                da.Create(school);
                //TODO: create school db
                uow.Commit();
            }

            var schoolSl = ServiceLocator.SchoolServiceLocator(school.Id, school.Name, school.ServerUrl);

            foreach (var principal in principals)
            {
                schoolSl.PersonService.Add(principal.Login, principal.Password, principal.FirstName, principal.LastName, CoreRoles.ADMIN_GRADE_ROLE.Name, principal.Gender, principal.Salutation, principal.BirthDate);
            }

            throw new System.NotImplementedException();
        }

        private static string FindServer(UnitOfWork uow)
        {
            var da = new SchoolDataAccess(uow);
            var serverLoading = da.CalcServersLoading();
            string s = null;
            int cnt = int.MaxValue;
            foreach (var sl in serverLoading)
            {
                if (sl.Value >= cnt) continue;
                cnt = sl.Value;
                s = sl.Key;
            }
            if (s == null)
                throw new NullReferenceException();
            return s;
        }
    }
}