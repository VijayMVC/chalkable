using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Services.DemoSchool.Storage;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool
{
    public class DemoInfractionStorage : BaseDemoIntStorage<Infraction>
    {
        public DemoInfractionStorage()
            : base(x => x.Id, true)
        {
        }

        public IList<Infraction> GetAll(bool onlyActive)
        {
            var infractions = data.Select(x => x.Value);
            if (onlyActive)
                infractions = infractions.Where(x => x.IsActive);
            return infractions.ToList();
        }
    }

    public class DemoInfractionService : DemoSchoolServiceBase, IInfractionService
    {
        private DemoInfractionStorage InfractionStorage { get; set; }
        public DemoInfractionService(IServiceLocatorSchool serviceLocator)
            : base(serviceLocator)
        {
            InfractionStorage = new DemoInfractionStorage();
        }

        public void AddList(IList<Infraction> infractions)
        {
            InfractionStorage.Add(infractions);
        }

        public void EditList(IList<Infraction> infractions)
        {
            throw new NotImplementedException();
        }

        public void DeleteList(IList<Infraction> infractions)
        {
            throw new NotImplementedException();
        }

        public IList<Infraction> GetDisciplineTypes()
        {
            return InfractionStorage.GetAll();
        }

        public void Add(IList<Infraction> infractions)
        {
            InfractionStorage.Add(infractions);
        }

        public void Edit(IList<Infraction> infractions)
        {
            InfractionStorage.Update(infractions);
        }

        public void Delete(IList<Infraction> infractions)
        {
            throw new NotImplementedException();
        }

        public IList<Infraction> GetInfractions()
        {
            throw new NotImplementedException();
        }
    }
}
