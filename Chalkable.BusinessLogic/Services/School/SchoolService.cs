﻿using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Security;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.School.DataAccess;

namespace Chalkable.BusinessLogic.Services.School
{
    public interface ISchoolService
    {
        void Add(Data.School.Model.School school);
        void Add(IList<Data.School.Model.School> schools);
        void Delete(IList<int> ids);
        IList<Data.School.Model.School> GetSchools();
    }

    public class SchoolService : SchoolServiceBase, ISchoolService
    {
        public SchoolService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }

        public void Add(Data.School.Model.School school)
        {
            Add(new List<Data.School.Model.School> {school});
        }

        public IList<Data.School.Model.School> GetSchools()
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();
            using (var uow = Read())
            {
                var da = new SchoolDataAccess(uow);
                return da.GetAll();
            }
        }


        public void Add(IList<Data.School.Model.School> schools)
        {
            if (Context.Role.Id != CoreRoles.SUPER_ADMIN_ROLE.Id)
                throw new ChalkableSecurityException();
            if (!Context.DistrictId.HasValue)
                throw new UnassignedUserException();
            using (var uow = Update())
            {
                var da = new SchoolDataAccess(uow);
                da.Insert(schools);
                uow.Commit();
            }
            ServiceLocator.ServiceLocatorMaster.SchoolService.Add(schools.Select(x=> new Data.Master.Model.School
                {
                    Id = Guid.NewGuid(),
                    DistrictRef = Context.DistrictId.Value,
                    LocalId = x.Id,
                    Name = x.Name
                }).ToList());
        }

        public void Delete(IList<int> ids)
        {
            
            //using (var uow = Update())
            //{
            //    new SchoolDataAccess(uow).Delete(ids);
            //    uow.Commit();
            //}
            //ServiceLocator.ServiceLocatorMaster.SchoolService.
            throw new NotImplementedException();
        }
    }
}