﻿using System.Collections.Generic;
using Chalkable.BusinessLogic.Security;
using Chalkable.Common.Exceptions;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;
using Chalkable.Data.School.Model.Sis;

namespace Chalkable.BusinessLogic.Services.School
{
    public interface ISchoolPersonService
    {
        IList<SchoolPerson> GetAll();
    }

    public class SchoolPersonService : SchoolServiceBase, ISchoolPersonService
    {
        public SchoolPersonService(IServiceLocatorSchool serviceLocator)
            : base(serviceLocator)
        {
        }

        public IList<SchoolPerson> GetAll()
        {
            if (!BaseSecurity.IsDistrictAdmin(Context))
                throw new ChalkableSecurityException();
            using (var uow = Read())
            {
                return (new SchoolPersonDataAccess(uow)).GetAll();
            }
        }
    }
}