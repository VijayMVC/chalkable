﻿using System.Collections.Generic;
using Chalkable.BusinessLogic.Security;
using Chalkable.BusinessLogic.Services.DemoSchool.Storage;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Common.Exceptions;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool
{
    public class DemoInfractionService : DemoSchoolServiceBase, IInfractionService
    {
        public DemoInfractionService(IServiceLocatorSchool serviceLocator, DemoStorage storage)
            : base(serviceLocator, storage)
        {
        }

        public void AddList(IList<Infraction> infractions)
        {
            if(!BaseSecurity.IsSysAdmin(Context))
                throw new ChalkableSecurityException();
            Storage.InfractionStorage.Add(infractions);
        }

        public void EditList(IList<Infraction> infractions)
        {
            if (!BaseSecurity.IsSysAdmin(Context))
                throw new ChalkableSecurityException();

            Storage.InfractionStorage.Update(infractions);
        }

        public void DeleteList(IList<int> ids)
        {
            if (!BaseSecurity.IsSysAdmin(Context))
                throw new ChalkableSecurityException();

            Storage.InfractionStorage.Delete(ids);
        }

        public IList<Infraction> GetDisciplineTypes(bool onlyActive = true)
        {
            return Storage.InfractionStorage.GetAll(onlyActive);
        }

        public void Add(IList<Infraction> infractions)
        {
            Storage.InfractionStorage.Add(infractions);
        }

        public void Edit(IList<Infraction> infractions)
        {
            Storage.InfractionStorage.Update(infractions);
        }

        public void Delete(IList<int> ids)
        {
            Storage.InfractionStorage.Delete(ids);
        }

        public IList<Infraction> GetInfractions(bool onlyActive = true)
        {
            return Storage.InfractionStorage.GetAll(onlyActive);
        }
    }
}
