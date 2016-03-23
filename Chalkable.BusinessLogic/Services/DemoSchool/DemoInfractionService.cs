using System;
using System.Collections.Generic;
using System.Linq;
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
            InfractionStorage.Delete(infractions);
        }

        public IList<Infraction> GetInfractions(bool activeOnly = false, bool onlyVisibleInClassRoom = false)
        {
            var res = InfractionStorage.GetAll().AsQueryable();
            if(activeOnly)
                res = res.Where(x=>x.IsActive);
            if (onlyVisibleInClassRoom)
                res = res.Where(x => x.VisibleInClassroom);
            return res.ToList();
        }
    }
}
