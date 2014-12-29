using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Services.DemoSchool.Common;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoSchoolPersonStorage:BaseDemoIntStorage<SchoolPerson>
    {
        public DemoSchoolPersonStorage(DemoStorage storage) : base(storage, null, true)
        {

        }

        public SchoolPerson GetSchoolPerson(int personId, int? schoolLocalId, int? roleId)
        {
            var persons = data.Where(x => x.Value.PersonRef == personId && x.Value.SchoolRef == schoolLocalId);
            if (roleId.HasValue)
                persons = data.Where(x => x.Value.RoleRef == roleId);
            return persons.Select(x => x.Value).First();
        }

        public bool Exists(int personId, int roleRef, int? schoolRef)
        {
            return
                data.Count(
                    x => x.Value.RoleRef == roleRef && x.Value.PersonRef == personId && x.Value.SchoolRef == schoolRef) ==
                1;
        }

        public int GetRoleId(int personId, int schoolRef)
        {
            return
                data.Where(x => x.Value.PersonRef == personId && x.Value.SchoolRef == schoolRef)
                    .Select(x => x.Value.RoleRef).First();
        }

        public new void Delete(IList<SchoolPerson> schoolPersons)
        {
            foreach (var schoolPerson in schoolPersons)
            {
                var item =
                    data.First(x => x.Value.PersonRef == schoolPerson.PersonRef && x.Value.RoleRef == schoolPerson.RoleRef &&
                                    x.Value.SchoolRef == schoolPerson.SchoolRef);

                Delete(item.Key);
            }
        }
    }
}
