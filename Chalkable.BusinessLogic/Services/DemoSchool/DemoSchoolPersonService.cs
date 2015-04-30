using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool
{
    public class DemoSchoolPersonStorage : BaseDemoIntStorage<SchoolPerson>
    {
        public DemoSchoolPersonStorage()
            : base(null, true)
        {

        }
    }

    public class DemoSchoolPersonService : DemoSchoolServiceBase, ISchoolPersonService
    {
        private DemoSchoolPersonStorage SchoolPersonStorage { get; set; }
        public DemoSchoolPersonService(IServiceLocatorSchool serviceLocator)
            : base(serviceLocator)
        {
            SchoolPersonStorage = new DemoSchoolPersonStorage();
        }

        public IList<SchoolPerson> GetAll()
        {
            return SchoolPersonStorage.GetAll();
        }

        public SchoolPerson GetSchoolPerson(int personId, int? schoolLocalId, int? roleId)
        {
            var persons = SchoolPersonStorage.GetData().Where(x => x.Value.PersonRef == personId && x.Value.SchoolRef == schoolLocalId);
            if (roleId.HasValue)
                persons = persons.Where(x => x.Value.RoleRef == roleId);
            return persons.Select(x => x.Value).First();
        }

        public bool Exists(int personId, int roleRef, int? schoolRef)
        {
            return
                SchoolPersonStorage.GetData().Count(
                    x => x.Value.RoleRef == roleRef && x.Value.PersonRef == personId && x.Value.SchoolRef == schoolRef) == 1;
        }

        public int GetRoleId(int personId, int schoolRef)
        {
            return
                 SchoolPersonStorage.GetData().Where(x => x.Value.PersonRef == personId && x.Value.SchoolRef == schoolRef)
                    .Select(x => x.Value.RoleRef).First();
        }

        public void AddSchoolPerson(SchoolPerson schoolPerson)
        {
            SchoolPersonStorage.Add(schoolPerson);
        }
    }
}